using AGV.bean;
using AGV.dao;
using AGV.forklift;
using AGV.init;
using AGV.tools;
using AGV.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace AGV.taskexe {

	/// <summary>
	/// 描述任务从用户下达到发送AGV执行前的逻辑
	/// </summary>
	public class TaskexeService : ITaskexeService {

		private static TaskexeService taskexeService = null;
		private Queue<TaskexeBean> taskexeBeanQueue = new Queue<TaskexeBean>();

		private bool need = true;//系统是否需要使用到此任务调度器

		public static ITaskexeService getInstance() {
			if (taskexeService == null) {
				taskexeService = new TaskexeService();
			}
			return taskexeService;
		}

		public bool isNeed() {
			return need;
		}

		public bool isSystemRunning() {
			List<TaskexeBean> taskexeBeanList = TaskexeDao.getDao().getTaskexeStartOrPauseList();
			if (taskexeBeanList == null || taskexeBeanList.Count <= 0) {
				return true;
			}
			return "sysContinue".Equals(taskexeBeanList[0].getTaskid());
		}

		public Queue<TaskexeBean> getTaskexeTaskList() {

			if (!(taskexeBeanQueue == null || taskexeBeanQueue.Count <= 0)) {
				return taskexeBeanQueue;
			}

			List<TaskexeBean> taskexeBeanList = TaskexeDao.getDao().getTaskexeTaskList();
			foreach (TaskexeBean taskexeBean in taskexeBeanList) {
				taskexeBeanQueue.Enqueue(taskexeBean);
			}

			return taskexeBeanQueue;
		}

		public TaskexeBean removedNext() {
			TaskexeBean taskexeBean = getTaskexeTaskList().Dequeue();
			TaskexeDao.getDao().deleteTaskexeBean(taskexeBean);
			return taskexeBean;
		}

		public void sendCommand(TaskexeBean taskexeBean) {
			string cmd = "cmd=set task by name;name=" + taskexeBean.getTaskid() + ";";
			AGVLog.WriteError("向AGV下达命令: " + cmd, new StackFrame(true));
			Console.WriteLine("向AGV下达命令: " + cmd);
			if (AGVEngine.getInstance().isAgvReady()) {
				ForkLiftWrappersService.getInstance().getForkLiftByNunber(1).getAGVSocketClient().SendMessage(cmd);
			} else {
				CommandDao.getDao().InsertCommand(taskexeBean.getTaskid());
			}
		}

		public bool isCommandDone() {
			List<CommandBean> commandBeanList = CommandDao.getDao().selectLatestCommand();

			if (commandBeanList == null || commandBeanList.Count <= 0) {
				return false;
			}

			return "Over".Equals(commandBeanList[0].getOpflag());
		}

		public bool hasNext() {
			return !(getTaskexeTaskList() == null || getTaskexeTaskList().Count <= 0);
		}

		public void resolveTask2Command() {
			while (true) {
				while (hasNext()) {
					TaskexeBean taskexeBean = removedNext();
					sendCommand(taskexeBean);
					Thread.Sleep(1000);
					while (!isCommandDone()) {
						Console.WriteLine("当前任务尚未完成" );
						Thread.Sleep(5000);
					}
				}
				Thread.Sleep(100);
			}
		}

		public void start() {
			ThreadFactory.newBackgroudThread(new ThreadStart(resolveTask2Command)).Start();
		}
	}
}
