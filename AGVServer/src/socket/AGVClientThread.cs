using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using AGV.schedule;
using AGV.task;
using AGV.init;
using AGV.tools;
using AGV.forklift;
using AGV.util;
using System.Diagnostics;
using AGV.dao;
using AGV.message;
using AGV.sys;

namespace AGV.socket {
	public class AGVClientThread {
		private Socket serverSocket = null;
		private Socket clientSocket = null;

		private ManualResetEvent manualResetEvent = new ManualResetEvent(false);

		private bool clientFuncOK = false;
		private bool serverFuncOK = false;

		private static AGVClientThread currentClientThread = null;

		int i;

		public static AGVClientThread getSocketClientThread(Socket serverSocket) {
			if (currentClientThread == null) {
				currentClientThread = new AGVClientThread(serverSocket);
			}
			return currentClientThread;
		}

		private AGVClientThread(Socket serverSocket) {
			this.serverSocket = serverSocket;
		}

		private void checkClientSocket() {
			if (clientSocket == null) {
				throw new Exception("clientSocket未初始化，无法使用");
			}
		}

		public void listen() {
			try {
				Console.WriteLine("wait connection");
				clientSocket = serverSocket.Accept();
				Console.WriteLine("accept");

				manualResetEvent.Reset();
				ThreadFactory.newThread(new ThreadStart(ClientService)).Start();
				ThreadFactory.newThread(new ThreadStart(ServerService)).Start();
				manualResetEvent.WaitOne();
				clearSocket();

			} catch (Exception ex) {
				Console.WriteLine(ex.ToString());
			}
		}

		public int Send(byte[] byteData) {
			checkClientSocket();
			return clientSocket.Send(byteData);
		}

		private void handleRecordTask(string taskName, string cmd) {
			SingleTask st = SingleTaskDao.getDao().getSingleTaskByTaskName(taskName);
			if (cmd.Equals("add")) {
				st.taskStat = TASKSTAT_T.TASK_SEND;
				TaskReordService.getInstance().addTaskRecord(TASKSTAT_T.TASK_READY_SEND, st);
			} else if (cmd.Equals("remove")) {
				TaskReordService.getInstance().removeTaskRecord(st, TASKSTAT_T.TASK_READY_SEND);
			}
		}

		private void handleMessage(String content) {
			Console.WriteLine("Content : " + content);
			int pos_c = -1;
			string cmd = null;
			pos_c = content.IndexOf("cmd=");
			if (pos_c != -1) {
				cmd = content.Substring(pos_c + 4);
				Console.WriteLine("cmd = " + cmd);

				if (cmd.StartsWith("add_recordTask"))  //添加任务
				{
					pos_c = cmd.IndexOf("param=");
					if (pos_c != -1) {
						string taskName = cmd.Substring(pos_c + 6);
						handleRecordTask(taskName, "add");
						Console.WriteLine("taskName = " + taskName);
					}
				} else if (cmd.StartsWith("setSystemPause"))  //移除任务
				  {
					pos_c = cmd.IndexOf("param=");
					if (pos_c != -1) {
						string tmp = cmd.Substring(pos_c + 6);
						Console.WriteLine(" pauseStat = " + tmp);
						if (tmp.Equals("0")) {
							AGVSystem.getSystem().setPause(SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_TYPE_MIN);
						} else if (tmp.Equals("1")) {
							AGVSystem.getSystem().setPause(SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_SYSTEM_WITH_START);
						}
					}
				} else if (cmd.StartsWith("updateDownTask")) {
					//AGVInitialize.getInitialize().getSchedule().updateDownPickSingleTask();
				}
			}
		}

		private void ClientService() {
			checkClientSocket();
			string data = null;
			byte[] bytes = new byte[4096];
			Console.WriteLine("new user");
			try {
				Console.WriteLine("read to receive");
				while ((i = clientSocket.Receive(bytes)) != 0) {
					if (i < 0) {
						break;
					}

					Console.WriteLine(i);
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    DBDao.getDao().InsertConnectMsg(data, "ClientService");
					if (data.IndexOf("<AGV>") > -1) {
						handleMessage(data);
					}
				}
				Thread.Sleep(10);
			} catch (Exception ex) {
				Console.WriteLine(ex.ToString());
			}
			clientFuncOK = true;
			turnToMainThread();
		}

		private void ServerService() {
			try {
				checkClientSocket();
				while (true) {
					Console.WriteLine("read to send");
					ForkLiftWrapper forklift = AGVCacheData.getForkLiftByID(3);
					StringBuilder sb = new StringBuilder();
					sb.Append("battery_soc=");
					sb.Append(forklift.getBatteryInfo().getBatterySoc() + ";");

					sb.Append("agvMessage=");
					sb.Append((int)AGVMessageHandler.getMessageHandler().getMessage().getMessageType());

					Console.WriteLine(" send data = " + sb.ToString());
					AGVLog.WriteError(" send data = " + sb.ToString(), new StackFrame(true));
                    byte[] byteData = Encoding.ASCII.GetBytes(sb.ToString());
                    DBDao.getDao().InsertConnectMsg(sb.ToString(), "ServerService");
					clientSocket.Send(byteData);
					Thread.Sleep(10000);
				}
			} catch (Exception ex) {
				Console.WriteLine(ex.ToString());
			}
			serverFuncOK = true;
			turnToMainThread();
		}

		private void turnToMainThread() {
			if (clientFuncOK && serverFuncOK) {
				manualResetEvent.Set();
				Console.WriteLine("结束线程调用");
			}
		}

		private void clearSocket() {
			if (clientSocket != null) {
				clientSocket.Close();
				clientSocket = null;
				clientFuncOK = false;
				serverFuncOK = false;
			}
		}
	}
}
