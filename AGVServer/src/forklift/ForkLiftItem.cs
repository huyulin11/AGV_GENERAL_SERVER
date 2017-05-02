public enum TASK_STEP { TASK_IDLE, TASK_SENDED, TASK_EXCUTE, TASK_END }; //车子任务执行状态，解决车子接收任务与下一刻反馈报文的不同步

namespace AGV.forklift {
	public class ForkLiftItem  //描述一个车子
	{
		public int id;  //ID
		public int forklift_number;  //车子编号 唯一
		public string ip;  //车子IP
		public int port; //车子监听数据端口号
		public string currentTask = null; //当前任务 默认没有任务
		public int currentStep = 0;  //当前进去到哪一步，每一个任务都分成几个step，需要保证前面几个step是一样的

		/// <summary>
		/// idle 空闲 sended  任务刚发送 excute 任务刚执行 end  任务执行结束
		/// </summary>
		public TASK_STEP taskStep = TASK_STEP.TASK_IDLE;
		public int waitTimes = 0;
		public static int WAIT_FEEDBACK_TIME = 3; //等待报文3次反馈确认
		public int finishStatus = 1; //任务是否结束 默认没有任务

		public int shedulePause = 0; //是否暂停, 不包括系统暂停和暂停楼上所有车辆的指令，比如该车辆是运行状态，发送系统暂停或暂停楼上车辆后，该值仍然为0
		public int pauseStat = 0;
		public int gAlarm = 0;  //AGV防撞信号


		public int isUsed = 1;  //车子默认使用两辆
		public ForkLiftItem() {
		}

		public ForkLiftItem(int id, int forklift_number, string ip, int port, string currentTask = null, int finishStatus = 1) {
			this.id = id;
			this.forklift_number = forklift_number;
			this.ip = ip;
			this.port = port;
			this.currentTask = currentTask;
			this.finishStatus = finishStatus;
		}

	}
}
