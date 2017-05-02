using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using AGV.tools;

namespace AGV.socket {
	public class AGVSocketServer {
		private static AGVSocketServer socketServer = null;
		private Socket serverSocket = null;
	
		private AGVSocketServer() {
			serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			serverSocket.Bind(new IPEndPoint(IPAddress.Any, 11000));
			serverSocket.Listen(100);
		}

		public static AGVSocketServer getSocketServer() {
			if (socketServer == null) {
				socketServer = new AGVSocketServer();
			}
			return socketServer;
		}

		public void StartAccept() {
			ThreadFactory.newBackgroudThread(new ThreadStart(Listening)).Start();
		}

		private void Listening() {
			byte[] bytes = new byte[1024];
			try {
				ManualResetEvent allDone = new ManualResetEvent(false);
				while (true) {
					allDone.Reset();
					ThreadFactory.newThread(new ThreadStart(TcpListen)).Start();
					allDone.WaitOne();
				}
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
			}
			Console.WriteLine("\nPress ENTER to continue...");
			Console.Read();
		}

		private void TcpListen() {
			while (true) {
				AGVClientThread.getSocketClientThread(serverSocket).listen();
			}
		}

		public void sendDataMessage(string dataMessage) {
			try {
				AGVClientThread.getSocketClientThread(serverSocket)
					.Send(Encoding.ASCII.GetBytes(dataMessage));
			} catch (Exception ex) {
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
