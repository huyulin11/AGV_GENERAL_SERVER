using AGV.forklift;

namespace AGV.task {
	public enum TASKTYPE_T {
		TASK_TYPE_DEFAULT = 0,
		TASK_TYPE_DOWN_DILIVERY,  //楼下送货
		TASK_TYPE_DOWN_PICK,  //楼下取货
		TASK_TYPE_UP_DILIVERY,  //楼上送货
		TASK_TYPE_UP_PICK,  //楼上取货
		TASK_TYPE_MAX
	}

	/// <summary>
	/// 描述单个任务
	/// </summary>
	public class SingleTask {
		public int taskID;  //任务ID
		public ForkLiftWrapper forkLiftWrapper;  //任务发送对应的车  状态为1的时候车对应空
		public string taskName;  //任务名称 taskID.xml
		public string taskText;  //界面上显示的名称
		public TASKSTAT_T taskStat = TASKSTAT_T.TASK_NOT_ASSIGN;
		public int waitTimes = 0; //任务发送后，报文可能没有及时反馈，等待次数超过3次，视为发送失败，修改车子状态，后面重新发送
		public bool taskUsed = false; //该任务是否被使用
		public TASKTYPE_T taskType = TASKTYPE_T.TASK_TYPE_DEFAULT; //任务类型 1 楼下上货任务 2 楼下卸货任务 3 楼上下货任务 4 楼上卸货任务
		private string allocOpType;  //对应货位的操作类型，1为上货，2为下货
		private	 string allocid;  //对应货位的ID

		public override string ToString()  //重写ToSting 使显示的时候只显示taskName
		{
			return taskText;
		}

		public string getAllocOpType() {
			return allocOpType;
		}

		public void setAllocOpType(string allocOpType) {
			this.allocOpType = allocOpType;
		}


		public string getAllocid() {
			return allocid;
		}

		public void setAllocid(string allocid) {
			this.allocid = allocid;
		}
		public SingleTask() {
		}

	}
}
