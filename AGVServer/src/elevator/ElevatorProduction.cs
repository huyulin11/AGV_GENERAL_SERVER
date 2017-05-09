using System;

using Microsoft.VisualBasic.Devices;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using AGV.init;
using AGV.message;
using AGV.tools;
using AGV.util;

namespace AGV.elevator {
	/// <summary>
	/// 串口控制升降机
	/// 发送数据格式 一个字，两个字节{0x1, 0x0}, 读取能读到6个字节，前两个返回的是控制字节，后面四个是反馈字节，后面反馈字节中有一个控制指令
	/// </summary>
	public class ElevatorProduction : ElevatorOperator {
		private SerialPort sp = new SerialPort();
		private bool isStop = false;
		private bool stat = false;
		private static byte[] dataCommand = new byte[2];  //升降机控制指令, 0x6上料 0x5下料

		private static byte[] UP_COMMAND = { 0x1, 0x0 };  //表示上料指令
		private static byte[] DOWN_COMMAND = { 0x2, 0x0 };  //表示下料指令

		private byte[] common = { 0x0, 0x0, 0x0, 0x0 }; //每次读数据，需要先写一个数据, 写0表示清除之前的命令
		private COMMAND_FROME2S outCommand = 0; //输出命令

		public ElevatorProduction() {
			initSerialPort();
		}

		private void getCHPort() {
			foreach (String portName in SerialPort.GetPortNames()) {
				Console.WriteLine("portName = " + portName);
				sp.PortName = portName;
			}
		}

		private void setSerialPort() {
			sp.BaudRate = 9600;
			sp.DataBits = 7;
			sp.StopBits = StopBits.One;
			sp.Parity = Parity.None;
			sp.ReadTimeout = 1000;
		}

		public ElevatorOperator initSerialPort() {
			getCHPort();
			try {
				if (sp.PortName.Equals("COM2")) {
					throw new Exception("没有找到升降机串口，请检查");
				}
				setSerialPort();
				if (!sp.IsOpen) {
					sp.Open();
				}
				stat = true;
			} catch (Exception ex) {
                stat = true;  //串口异常
				Console.WriteLine(ex.ToString());
			}

			return this;
		}

		public void setDataCommand(COMMAND_FROMS2E command) {
			if (command == COMMAND_FROMS2E.LIFT_IN_COMMAND_DOWN) {
				dataCommand[0] = DOWN_COMMAND[0];
				dataCommand[1] = DOWN_COMMAND[1];
			} else if (command == COMMAND_FROMS2E.LIFT_IN_COMMAND_UP) {
				dataCommand[0] = UP_COMMAND[0];
				dataCommand[1] = UP_COMMAND[1];
			}
		}

		/// <summary>
		/// 解析从升降机发送到服务端的指令
		/// </summary>
		private COMMAND_FROME2S readCommandFromE2S(byte[] response) {
			COMMAND_FROME2S outCommand = COMMAND_FROME2S.LIFT_OUT_COMMAND_MIN;
			int i = 0;
			for (i = 0; i < response.Length; i++) {
				if ((COMMAND_FROME2S)response[i] < COMMAND_FROME2S.LIFT_OUT_COMMAND_MAX 
					&& (COMMAND_FROME2S)response[i] > COMMAND_FROME2S.LIFT_OUT_COMMAND_MIN) {
					outCommand = (COMMAND_FROME2S)response[i];
				}
			}
			if (outCommand != COMMAND_FROME2S.LIFT_OUT_COMMAND_MIN) {
			}
			return outCommand;	
		}

		public COMMAND_FROME2S getOutCommand() {
			return outCommand;
		}

		private void handleLiftComException() {
			AGVMessage message = new AGVMessage();
			message.setMessageType(AGVMessageHandler_TYPE_T.AGVMessageHandler_LIFT_COM);
			message.setMessageStr("升降机PLC端口异常，请检查，当前处于系统暂停");
			AGVMessageHandler.getMessageHandler().setMessage(message);  //发送消息
			isStop = true; //结束串口读取线程
		}
		/// <summary>
		/// 一个命令尝试发送三次，如果三次都没有收到响应，则不继续发送
		///</summary>
		/// <param name="command"></param>
		private void sendCommand(byte[] command) {
			int count = 0;
			int j = 0;
			int times = 0;
			if (command[0] == 0 && command[1] == 0)  //结束符0x32, 否则不是有效命令
			{
				Console.WriteLine("command err");
				return;
			}
			sp.Write(command, 0, 2);   //总共发送1个字节 

			Thread.Sleep(100);
			Console.WriteLine("write byte " + command[0] + "    " + command[1]);
			while ((count = sp.BytesToRead) != 6 && times < 30) //固定升降机返回字节，否则表示升降机出现错误
			{
				times++;
				//Console.WriteLine("wait to read bytes"); //等待反馈
				Thread.Sleep(100);
			}
			times = 0;
			if (count != 6) {
				Console.WriteLine(" count = " + count);
			}
			//while(sendTimes > 0)
			int i = 0; //读取三次，看是否写成功，如果写成功能读到响应
			count = sp.BytesToRead;

			byte[] response = new byte[count];
			try {

				sp.Read(response, 0, count);
			} catch (System.TimeoutException te) {
				Thread.Sleep(10);
			}

			for (j = 0; j < count; j++) {
				Console.WriteLine("write check response[" + j + "]" + " = " + response[j]);
				if (j > 0 && response[j] == command[0]) {
					goto end;
				}
			}

			Thread.Sleep(100);

			Console.WriteLine("send fail");
			return;
		end:
			Console.WriteLine("send success");
		}

		private void handleDataSerialPort() {
			try {
				int count;
				byte[] response = null;
				sp.Write(common, 0, 4);
				while (!isStop) {
					byte[] readBuffer = new byte[1];
					count = sp.BytesToRead;
					if (count != 8) {
						Thread.Sleep(1000);
						continue;
					}

					response = new byte[count];
					sp.Read(response, 0, count);

					outCommand = readCommandFromE2S(response);

					//如果读取到升降机异常， 不再向升降机发送命令
					if (outCommand > COMMAND_FROME2S.LIFT_OUT_COMMAND_UP_DOWN) {
						AGVLog.WriteInfo("升降机异常 " + outCommand, new StackFrame(true));
						continue;
					}
					if (dataCommand[0] > 0) {   //清空数据
						sendCommand(dataCommand);
						dataCommand[0] = 0;
						dataCommand[1] = 0;
					}
					Thread.Sleep(200);
				}
			} catch (Exception ex) {
				Console.WriteLine(ex.ToString());
				handleLiftComException();
			}
		}

		public void startReadSerialPortThread() {
			isStop = false;
			ThreadFactory.newThread(new ThreadStart(handleDataSerialPort)).Start();
		}

		public void reStart() {
			initSerialPort();
			startReadSerialPortThread();
		}

		public bool getStat(){
			return this.stat;
		}
	}
}
