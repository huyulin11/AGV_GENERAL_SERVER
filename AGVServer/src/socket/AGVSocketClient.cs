using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using AGV.util;
using AGV.init;
using AGV.forklift;
using AGV.tools;
using AGV.dao;
using System.Windows.Forms;
using AGV.taskexe;
using AGV.command;
using System.IO;

namespace AGV.socket {

	public class AGVSocketClient {
		private TcpClient tcpClient = null;
		private string stx = ((char)2).ToString();
		private string lastMsgAboutSend = "";
		private string etx = ((char)3).ToString();
		private byte readTimeOutTimes = 0;  //读取消息超时次数
		public delegate void handleRecvMessageCallback(int fID, byte[] buffer, int length);  //消息处理回调函数
		public object clientLock = new object();

		private handleRecvMessageCallback receiveMsgCallback = null;

		private ForkLiftWrapper forkLiftWrapper = null;

		public AGVSocketClient(ForkLiftWrapper forkLiftWrapper) {
			this.forkLiftWrapper = forkLiftWrapper;
		}

		public AGVSocketClient() {
		}

		public void setForkLiftWrapper(ForkLiftWrapper forkLiftWrapper) {
			this.forkLiftWrapper = forkLiftWrapper;
		}

		public ForkLiftWrapper getForkLiftWrapper() {
			return this.forkLiftWrapper;
		}

		private TcpClient getTcpClient() {
			return getTcpConnect(forkLiftWrapper.getForkLift().ip, forkLiftWrapper.getForkLift().port);
		}

		private void initTcpClient() {
			tcpClient.ReceiveTimeout = AGVConstant.TCPCONNECT_REVOUT;
			tcpClient.SendTimeout = AGVConstant.TCPCONNECT_SENDOUT;
		}

		private TcpClient getTcpConnect(string ip, int port) {
			try {
				if (tcpClient == null) {
					(tcpClient = new TcpClient()).Connect(ip, port);
					initTcpClient();
					lastMsgAboutSend = "连接到AGV" + "成功！" + "(ip:" + ip + ",port:" + port + ")";
					AGVLog.WriteConnectInfo(lastMsgAboutSend, new StackFrame(true));
				}
				return tcpClient;
			} catch (Exception ex) {
				lastMsgAboutSend = "连接到 ip: " + ip + " port: " + port + " 失败" + ex.Message + "5秒后重新获取连接";
				AGVLog.WriteConnectInfo(lastMsgAboutSend, new StackFrame(true));
				Closeclient();
				return getTcpConnect(ip, port);
			}
		}

		public void registerRecvMessageCallback(handleRecvMessageCallback hrmCallback) {
			this.receiveMsgCallback = hrmCallback;
		}

		public void startRecvMsg() {
			ThreadFactory.newThread(new ThreadStart(receive)).Start();
		}

		private void receive() {
			while (true) {
				try {
					byte[] buffer = new byte[512];
					Socket msock;
					msock = getTcpClient().Client;
					Array.Clear(buffer, 0, buffer.Length);
					getTcpClient().GetStream();

					int bytes = msock.Receive(buffer);
					string receiveStr = Encoding.ASCII.GetString(buffer).Trim();

					readTimeOutTimes = 0; //读取超时次数清零
					if (receiveMsgCallback != null) {
						receiveMsgCallback(forkLiftWrapper.getForkLift().id, buffer, bytes);
					}
					if (!"正常接收AGV消息".Equals(lastMsgAboutSend)) {
						lastMsgAboutSend = "正常接收AGV消息";
						AGVLog.WriteConnectInfo(lastMsgAboutSend, new StackFrame(true));
					}
				} catch (SocketException ex) {
					if (ex.ErrorCode == 10060 && readTimeOutTimes < 10) //超时次数超过10次，关闭socket进行重连
					{
						lastMsgAboutSend = "读取消息超时" + "，系统稍后将重新连接AGV";
						AGVLog.WriteConnectInfo(lastMsgAboutSend, new StackFrame(true));
						readTimeOutTimes++;
						continue;
					}
					lastMsgAboutSend = "读取消息错误，系统稍后将重新连接AGV";
					AGVLog.WriteConnectInfo(lastMsgAboutSend, new StackFrame(true));
					Closeclient();
				} catch (IOException ex) {
					lastMsgAboutSend = "读取消息错误，系统稍后将重新连接AGV";
					AGVLog.WriteConnectInfo(lastMsgAboutSend, new StackFrame(true));
					Closeclient();
				} catch (Exception ex) {
					lastMsgAboutSend = "读取消息错误，系统稍后将重新连接AGV";
					AGVLog.WriteConnectInfo(lastMsgAboutSend, new StackFrame(true));
					Closeclient();
				}
			}
		}

		private string lastMessage;

		public bool SendMessage(string sendMessage) {
			if (!sendMessage.Equals(lastMessage)) {
				AGVLog.WriteSendInfo(sendMessage, new StackFrame(true));
			} else {
			}
			lastMessage = sendMessage;

			Socket msock;
			try {
				msock = getTcpClient().Client;
				byte[] data = Encoding.ASCII.GetBytes(sendMessage);
				DBDao.getDao().InsertConnectMsg(sendMessage, "SendMessage");
				msock.Send(data);
				if (!"发送消息成功".Equals(lastMsgAboutSend)) {
					lastMsgAboutSend = "发送消息成功";
					AGVLog.WriteConnectInfo(lastMsgAboutSend, new StackFrame(true));
				}
				return true;
			} catch (Exception se) {
				lastMsgAboutSend = "发送消息错误" + se.Message + "，系统稍后将重新连接AGV";
				AGVLog.WriteConnectInfo(lastMsgAboutSend, new StackFrame(true));
				Closeclient();
				return false;
			}
		}

		private void Closeclient() {
			try {
				if (tcpClient != null) {
					tcpClient.Client.Close();
					tcpClient.Close();
					tcpClient = null;
				}
				Thread.Sleep(5000);
			} catch (Exception ex) {
				lastMsgAboutSend = "关闭socket错误" + ex.Message + "，系统稍后将重新连接AGV";
				AGVLog.WriteConnectInfo(lastMsgAboutSend, new StackFrame(true));
			}
		}
	}
}
