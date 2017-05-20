using AGV.bean;
using AGV.dao;
using AGV.forklift;
using AGV.init;
using AGV.task;
using AGV.taskexe;
using AGV.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace AGV.command {

	/// <summary>
	/// 描述任务从用户下达到发送AGV执行前的逻辑
	/// </summary>
	public class CommandService:ICommandService {

		private static CommandService commandService = null;

		private string latestMsgFromClient = "";

		public void setLatestMsgFromClient(string receiveStr) {
			this.latestMsgFromClient = receiveStr;
		}

		public static ICommandService getInstance() {
			if (commandService == null) {
				commandService = new CommandService();
			}
			return commandService;
		}

		public void resolveTaskCommand() {
			try {
				while (true) {
					if (TaskexeService.getInstance().isSystemRunning()) {
						sendCommand();
					}
					Thread.Sleep(1000);
				}
			} catch (Exception) {
				AGVLog.WriteInfo("循环器异常退出！",new StackFrame(true));
			}
		}

		public void resolveSYSCtrlCommand() {
			try {
				while (true) {
					if (!TaskexeService.getInstance().isSystemRunning()) {
						sendPauseCommand();
					} else {
						sendContinueCommand();
					}
					Thread.Sleep(1000);
				}
			} catch (Exception) {
				AGVLog.WriteInfo("循环器异常退出！",new StackFrame(true));
			}
		}

		public void sendCommand() {
			while (TaskexeService.getInstance().hasNextTaskexe()) {
				TaskexeBean taskexeBean = TaskexeService.getInstance().getAndRemoveNextTaskexeBean();
				string cmd = "cmd=set task by name;name=" + taskexeBean.getTaskid() + ".xml";
				SingleTask singleTask = AGVCacheData.getSingleTaskByID(taskexeBean.getTaskid());

				sendContinueCommand();

				Thread.Sleep(1000);

				int i = -2;
				while (string.IsNullOrEmpty(latestMsgFromClient) || latestMsgFromClient.IndexOf("task_isfinished=") < 0 || !"0".Equals(latestMsgFromClient.Substring(latestMsgFromClient.IndexOf("task_isfinished=") + "task_isfinished=".Length,1))) {
					if ("1".Equals(singleTask.getAllocOpType())) {
						send(cmd);
						Thread.Sleep(3000);
						if (!AGVEngine.getInstance().isAgvReady()) {
							latestMsgFromClient = "task_isfinished=" + (i++) + ";";
						}
					}
				}
				while (true) {
					if ("Over".Equals(TaskexeDao.getDao().selectTaskexeByUuid(taskexeBean.getUuid()).getOpflag())) {
						break;
					}
					Thread.Sleep(1000);
				}
				Thread.Sleep(1000);
			}
		}

		public void sendPauseCommand() {
			string cmd = "cmd=pause;pauseStat=" + 1 + ";";
			send(cmd);
		}

		public void sendContinueCommand() {
			string cmd = "cmd=pause;pauseStat=" + 0 + ";";
			send(cmd);
		}

		private void send(string cmd) {
			AGVLog.WriteInfo("向AGV下达命令: " + cmd,new StackFrame(true));
			if (AGVEngine.getInstance().isAgvReady()) {
				ForkLiftWrappersService.getInstance().getForkLiftByNunber(1).getAGVSocketClient().SendMessage(cmd);
			} else {
				AGVLog.WriteSendInfo("向AGV下达命令: " + cmd,new StackFrame(true));
			}
		}
	}
}
