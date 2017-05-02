using System;
using AGV.dao;
using AGV.forklift;

namespace AGV.task {
	public enum TASKSTAT_T {
		TASK_NOT_ASSIGN = 0,
		TASK_READY_SEND = 1,  //缓存的任务
		TASK_SEND, //任务已经发送，但是不确定是否发送成功
		TASK_SEND_SUCCESS,  //任务已经发送成功
		TASK_END //任务执行完毕，将被删除
	};

	public enum TASKLEVEL_T {
		TASK_LVL_DEFAULT = 0,  //一般叉获任务，优先级最低
		TASK_LVL_BACK, //回原点任务
		TASK_LVL_CHARGER,  //充电任务，优先级最高
		TASK_LVL_MAX
	};

	/// <summary>
	/// 描述单个任务
	/// </summary>
	public class TaskRecord {
		public int taskRecordID;  //任务ID，总共14个任务
		public ForkLiftWrapper forkLiftWrapper;
		public SingleTask singleTask;
		public string taskRecordName;  //任务名称 taskID.xml
		public TASKSTAT_T taskRecordStat = TASKSTAT_T.TASK_READY_SEND;
		//public int waitTimes = 0; //任务发送后，报文可能没有及时反馈，等待次数超过3次，视为发送失败，修改车子状态，后面重新发送
		public int taskLevel = 0; //任务级别
		public int taskRecordExcuteMinute = 0; //任务执行完成需要的时间
		public DateTime updateTime;  //发送成功时候的时间

		public TaskRecord(TASKSTAT_T taskRecordStat, SingleTask singleTask, ForkLiftWrapper forkLiftWrapper = null) {
			this.forkLiftWrapper = forkLiftWrapper;
			this.taskRecordStat = taskRecordStat;
			this.singleTask = singleTask;
			this.taskRecordName = singleTask.taskName;
		}

		public void setSingleTaskByTaskName(string taskName) {
			SingleTask st = DBDao.getDao().SelectSingleTaskByName(taskName);
			Console.WriteLine("TaskRecord taskName = " + taskName);
			if (st != null)
				this.singleTask = st;
			else {
				Console.WriteLine("TaskRecord Set Name error");
			}
		}

		public TaskRecord() {
		}
	}
}
