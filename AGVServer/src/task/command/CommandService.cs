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
				AGVLog.WriteSendInfo("循环器异常退出！",new StackFrame(true));
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
				AGVLog.WriteSendInfo("循环器异常退出！",new StackFrame(true));
			}
		}

		public void sendCommand() {
			AGVLog.WriteSendInfo("开始处理！", new StackFrame(true));
			TaskexeBean taskexeBean = TaskexeService.getInstance().getNextTaskexeBean();
			if (taskexeBean == null) {
				return;
			}
			string cmd = "cmd=set task by name;name=" + taskexeBean.getTaskid() + ".xml";
			SingleTask singleTask = AGVCacheData.getSingleTaskByID(taskexeBean.getTaskid());

			sendContinueCommand();

			Thread.Sleep(1000);

			int i = -2;

			if ("1".Equals(singleTask.getAllocOpType())) {
				while (string.IsNullOrEmpty(latestMsgFromClient) || latestMsgFromClient.IndexOf("task_isfinished=") < 0 || !"0".Equals(latestMsgFromClient.Substring(latestMsgFromClient.IndexOf("task_isfinished=") + "task_isfinished=".Length, 1))) {
					send(cmd);
					AGVLog.WriteSendInfo("发送命令:"+ cmd, new StackFrame(true));
					Thread.Sleep(3000);
					if (!AGVEngine.getInstance().isAgvReady()) {
						latestMsgFromClient = "task_isfinished=" + (i++) + ";";
					}
				}
			}

			while (true) {
				AGVLog.WriteSendInfo("判断任务"+ taskexeBean.getTaskid() + "是否Over！", new StackFrame(true));
				if ("Over".Equals(TaskexeDao.getDao().selectTaskexeByUuid(taskexeBean.getUuid()).getOpflag())) {
					break;
				}
				Thread.Sleep(5000);
			}
			Thread.Sleep(1000);
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
			if (AGVEngine.getInstance().isAgvReady()) {
				while (true) {
					if (ForkLiftWrappersService.getInstance().getForkLiftByNunber(1).getAGVSocketClient().SendMessage(cmd)) {
						break;
					}
					Thread.Sleep(5000);
				}
			} else {
				AGVLog.WriteSendInfo("向AGV下达命令: " + cmd,new StackFrame(true));
			}
		}
	}
}
