using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using AGV.util;
using AGV.socket;
using AGV.message;
using AGV.schedule;
using AGV.form;

namespace AGV.init {
	class AGVMain {
		protected static void testTask() {

		}

		[DllImport("user32.dll")]
		public static extern bool MessageBeep(uint uType);
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main() {
			AGVLog agvLog = new AGVLog();
			//List<SingleTask> sList = new List<SingleTask>();
			agvLog.initAGVLog(); //初始化log

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			//FormController.getFormController().getLoginFrm().ShowDialog();

			//agvTask.startConnectForkLift(); //开始与车子建立连接
			//agvTask.startPLCRecv(); //开始采集PLC数据
			//agvTask.startToHandleTaskList(); //开始处理任务发送
			AGVEngine.getInstance().agvInit();
			//AGVSocketServer.getSocketServer().StartAccept();

			//ScheduleFactory.getSchedule().startShedule();

			//AGVMessageHandler.getMessageHandler().StartHandleMessage();
			//int i = 0;
			//while(i < 1)
			//{
			//i++;
			//AGVInitialize.getInitialize().getAGVElevatorOperator().setDataCommand(LIFT_IN_COMMAND_T.LIFT_IN_COMMAND_UP);
			//}
			//FormController.getFormController().getMainFrm().ShowDialog();
			//Application.Run(AGVInitialize.getInitialize().getMainFrm());

			//AGVTcpClient tcpClient = new AGVTcpClient();

			//Thread.Sleep(1000);


			//tcpClient.SendMessage("cmd=set task by name;name=abing;");

			//connectDB();
			//AGVInitialize init = AGVInitialize.getInitialize();
			//init.getAllForkLifts(false);

			//

			//while (!isExit) { Thread.Sleep(5); };

		}
	}
}
