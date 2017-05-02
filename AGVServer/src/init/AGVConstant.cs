namespace AGV.init {
	public class AGVConstant {
		public const byte AGVTASK_MAX = 14; //最多14个任务
		public const int readPLCInterval = 2000;   //2秒钟读一次 时间太长会导致连续拍两次按钮检测不到
		public const int handleTaskInterval = 1000; //任务处理时间间隔

		public const int TCPCONNECT_REVOUT = 15000;
		public const int TCPCONNECT_SENDOUT = 10000;
		public const int AGVALARM_TIME = 30;  //检测到防撞信号超过12次，报警

		public const int BORDER_X_DEVIARION = 100;
		public const int BORDER_X_1 = 40200;
		public const int BORDER_X_2 = 28500; //区域1 和 区域2 的分界点
		public const int BORDER_X_3 = 1200;  //区域2 和区域3 的分界点

		public const int BORDER_X_2_DEVIATION_PLUS = 1500;  //区域3的正误差，如果车子由3进入2，正常应该停在BORDER_X_2处，超出1200+1500就会报警
		public const int BORDER_X_3_DEVIATION_PLUS = 2000;  //区域3的正误差，如果车子由2进入1，正常应该停在BORDER_X_3处，超出BORDER_X_3+BORDER_X_3_DEVIATION_PLUS就会报警
		public const int BORDER_Y_1 = -100;
		public const int BORDER_Y_2 = 12740;
		public const int BORDER_Y_3 = 1000;
	}
}
