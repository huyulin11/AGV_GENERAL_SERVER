using AGV.bean;
using AGV.dao;
using AGV.forklift;
using AGV.init;
using AGV.tools;
using AGV.util;
using AGV.task;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using AGV.command;

namespace AGV.taskexe {

	/// <summary>
	/// 描述任务从用户下达到发送AGV执行前的逻辑
	/// </summary>
	public class TaskexeService : ITaskexeService {
		private Thread thread1 = null;

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

		public TaskexeBean getNextTaskexeBean() {
			List<TaskexeBean> taskexeBeanList = TaskexeDao.getDao().getTaskexeTaskList();
			if (taskexeBeanList == null || taskexeBeanList.Count <= 0) {
				return null;
			}
			TaskexeBean taskexeBean = taskexeBeanList[0];
			AGVLog.WriteSendInfo("下一个执行的任务ID：" + taskexeBean.getTaskid(), new StackFrame(true));
			if (!"Send".Equals(taskexeBean.getOpflag())) {
				TaskexeDao.getDao().sendTaskexeBean(taskexeBean);
			}
			return taskexeBean;
		}

		public TaskexeBean getAndRemoveNextTaskexeBean() {
			TaskexeBean taskexeBean = getTaskexeTaskList().Dequeue();
			if (!"Send".Equals(taskexeBean.getOpflag())) {
				TaskexeDao.getDao().sendTaskexeBean(taskexeBean);
			}
			return taskexeBean;
		}

		public bool hasNextTaskexe() {
			return !(getTaskexeTaskList() == null || getTaskexeTaskList().Count <= 0);
		}

		public void start() {
			Thread.CurrentThread.Name = "主要线程";
			thread1 = ThreadFactory.newThread(new ThreadStart(CommandService.getInstance().resolveTaskCommand));
			thread1.Name = "任务解析线程";
			thread1.Start();
			Thread thread2 = ThreadFactory.newThread(new ThreadStart(CommandService.getInstance().resolveSYSCtrlCommand));
			thread2.Start();
			Thread thread3 = ThreadFactory.newThread(new ThreadStart(listenThread1));
			thread3.Start();
		}

		public void listenThread1() {
			while (true) {
				if (thread1 == null) {
					return;
				} else {
					AGVLog.WriteThreadInfo(thread1.Name + "的执行状态为：" + thread1.ThreadState + ",其托管线程id为"
						+ thread1.ManagedThreadId, new StackFrame(true));
				}
				Thread.Sleep(2000);
			}
		}
	}
}
