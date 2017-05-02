using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using AGV.util;
using AGV.init;
using AGV.forklift;
using AGV.tools;
using System.Windows.Forms;

namespace AGV.socket {

	public class AGVSocketClient {
		private TcpClient tcpClient = null;
		private bool connectStatus = false;  //连接状态
		private string stx = ((char)2).ToString();
		private string etx = ((char)3).ToString();
		private bool isConnectThread = false;   //重连线程是否开启
		private byte readTimeOutTimes = 0;  //读取消息超时次数
		private bool isRecvMsgFlag = false;  //是否开启数据接收处理线程
		public delegate void handleRecvMessageCallback(int fID, byte[] buffer, int length);  //消息处理回调函数
		public delegate void handleReconnectCallback(ForkLiftWrapper forkLiftWrapper, bool status);  //重连回调函数
		public object clientLock = new object();

		private handleRecvMessageCallback hrmCallback = null;
		private handleReconnectCallback hrctCallback = null;

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
			while (tcpClient == null) {
				Console.WriteLine("client is null wait 1 second");
				AGVLog.WriteWarn("client is null, wait 1 second", new StackFrame(true));
				Thread.Sleep(1);
			}

			return tcpClient;
		}

		private void setTcpClient( ) {
			tcpClient.ReceiveTimeout = AGVConstant.TCPCONNECT_REVOUT;
			tcpClient.SendTimeout = AGVConstant.TCPCONNECT_SENDOUT;
			connectStatus = true;
		}

		public void TcpConnect(string ip, int port) {
			try {
				if (tcpClient == null) {
					(tcpClient = new TcpClient()).Connect(ip, port);
					setTcpClient();

					AGVLog.WriteInfo("connect ip: " + ip + " port: " + port + "succee", new StackFrame(true));
					Console.WriteLine("connect ip: " + ip + " port: " + port + "succee");
				}
			} catch (Exception ex) {
				AGVLog.WriteError("Connect ip: " + ip + " port: " + port + " fail" + ex.Message, new StackFrame(true));
				Console.WriteLine("Connect ip: " + ip + " port: " + port + " fail");

				Closeclient();
			}
		}
		/// <summary>
		/// 重新与车子建立连接
		/// </summary>
		public void reConnect() {
			Console.WriteLine("ConnectStatus:   " + connectStatus);
			AGVLog.WriteInfo("ConnectStatus: " + connectStatus, new StackFrame(true));

			while (forkLiftWrapper.getForkLift().isUsed == 1 && !connectStatus) {
				isConnectThread = true; //设置重连标志true
				Thread.Sleep(5000);  //5秒钟重连一次

				Console.WriteLine("start to reconnect");
				AGVLog.WriteWarn("start to reconnect", new StackFrame(true));

				TcpConnect(forkLiftWrapper.getForkLift().ip, forkLiftWrapper.getForkLift().port);

				if (connectStatus == true) {
					AGVLog.WriteInfo("reconnect ip: " + forkLiftWrapper.getForkLift().ip + " :"+ forkLiftWrapper.getForkLift().port + "success", new StackFrame(true));
					Console.WriteLine("reconnect ip: " + forkLiftWrapper.getForkLift().ip + ":" +forkLiftWrapper.getForkLift().port + "success");
					AGVLog.WriteInfo("restart recv thread", new StackFrame(true));

					//hrctCallback?.Invoke(forkLiftWrapper, true);
					if (hrctCallback != null) {
						hrctCallback(forkLiftWrapper, true);
					}
				}
			}
			isConnectThread = false; //重连成功

		}

		/// <summary>
		/// 消息处理回调函数注册
		/// </summary>
		public void registerRecvMessageCallback(handleRecvMessageCallback hrmCallback) {
			this.hrmCallback = hrmCallback;
		}

		/// <summary>
		/// 注册重连回调函数
		/// </summary>
		public void registerReconnectCallback(handleReconnectCallback hrctCallback) {
			this.hrctCallback = hrctCallback;
		}

		public void startRecvMsg() {
			this.isRecvMsgFlag = true;  //设置接收标志
			ThreadFactory.newBackgroudThread(new ParameterizedThreadStart(receive)).Start();
		}

		/// <summary>
		/// 结束数据接收处理线程
		/// </summary>
		public void setRecvFlag(bool status) {
			this.isRecvMsgFlag = status;
		}

		private void receive(object fl) {
			//ForkLiftWrapper forklift = new ForkLiftWrapper();
			//forklift.setForkLift((ForkLiftItem)fl);
			byte[] buffer = new byte[512];
			Socket msock;
			TcpClient vClient = null;
			Console.WriteLine("receive ConnectStatus: " + connectStatus);
			if (connectStatus == false) //检查连接状态
				return;

			while (isRecvMsgFlag) {
				try {
					vClient = getTcpClient();
					msock = tcpClient.Client;
					Array.Clear(buffer, 0, buffer.Length);
					tcpClient.GetStream();
					
					int bytes = msock.Receive(buffer);

					readTimeOutTimes = 0; //读取超时次数清零
					if (hrmCallback != null) {
						hrmCallback(forkLiftWrapper.getForkLift().id, buffer, bytes);
					}
				} catch (SocketException ex) {
					if (ex.ErrorCode == 10060 && readTimeOutTimes < 10) //超时次数超过10次，关闭socket进行重连
					{
						AGVLog.WriteWarn("read msg timeout", new StackFrame(true));
						Console.WriteLine("read msg timeout");
						readTimeOutTimes++;
						continue;
					}
					AGVLog.WriteError("读取消息错误" + ex.ErrorCode, new StackFrame(true));
					Console.WriteLine("recv msg client close" + ex.ErrorCode);
					Closeclient();
				}
			}
		}

		public void SendMessage(string sendMessage) {
			Socket msock;
			Console.WriteLine("SendMessage ConnectStatus " + connectStatus);
			if (tcpClient == null || connectStatus == false) //检查连接状态
			{
				Exception ex = new Exception("connect err");
				throw (ex);
			}

			msock = tcpClient.Client;
			try {
				byte[] data = new byte[128];
				data = Encoding.ASCII.GetBytes(sendMessage);
				msock.Send(data);

			} catch (Exception se) {
				AGVLog.WriteError("发送消息错误" + se.Message, new StackFrame(true));
				Console.WriteLine("send message error" + se.Message);
				Closeclient();
			}
		}

		public void Sendbuffer(byte[] buffer) {
			if (tcpClient == null || connectStatus == false) //检查连接状态
				return;

			Socket msock;
			try {
				msock = tcpClient.Client;
				msock.Send(buffer);
			} catch (Exception ex) {
				AGVLog.WriteError("发送消息错误" + ex.Message, new StackFrame(true));
				Console.WriteLine("send message error");
				Closeclient();
			}
		}

		/// <summary>
		/// 开启重连的线程
		/// </summary>
		private void startReconnectThread() {
			ThreadFactory.newBackgroudThread(new ThreadStart(reConnect)).Start();
		}

		public void Closeclient() {
			try {
				connectStatus = false;
				if (tcpClient != null) {
					AGVLog.WriteInfo("关闭socket", new StackFrame(true));
					tcpClient.Client.Close();
					tcpClient.Close();
					tcpClient = null;
					if (isConnectThread == false && forkLiftWrapper.getForkLift().isUsed == 1)  //connect断开后开启与车子重连
					{
						DialogResult dr = MessageBox.Show("是否重新连接SOCKET", "", MessageBoxButtons.YesNo);
						if (dr == DialogResult.No) {
							Console.WriteLine(" cancelItemClick cancel ");
							return;
						} else {
							startReconnectThread();
						}
						//startReconnectThread();
					}

					if (forkLiftWrapper.getForkLift().isUsed == 1) {
						if (hrctCallback != null) {
							hrctCallback(forkLiftWrapper, true);
						}
					}

				}
			} catch (Exception ex) {
				AGVLog.WriteError("关闭socket错误" + ex.Message, new StackFrame(true));
				Console.WriteLine("close socket fail");
			}
			Console.WriteLine("client is null now");
			AGVLog.WriteWarn("client is null now", new StackFrame(true));
		}
	}
}
