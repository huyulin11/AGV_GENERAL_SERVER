using System.Collections.Generic;
using AGV.dao;
using AGV.init;
using AGV.forklift;
using AGV.task;
using AGV.locked;

namespace AGV.dao {
	/// <summary>
	/// 程序中广泛用到的基础数据，且变化频率很低的话，缓存在此
	/// </summary>
	public class AGVCacheData {
		private static List<User> userList = null;
		private static List<ForkLiftWrapper> forkLiftWrapperList = null;
		private static List<SingleTask> singleTaskList = null;  //缓存所有将发送或正在处理的任务
		private static List<SingleTask> upPickSingleTaskList = null;
		private static List<SingleTask> downPickSingleTaskList = null;

		public static List<User> getUserList() {
			if (userList == null) {
				userList = DBDao.getDao().SelectUserList();
			}
			return userList;
		}

		/// <summary>
		/// 车子查询后，不能重新查询，整个运行周期只维护一个Forklift列表
		/// </summary>
		/// <returns></returns>
		public static List<ForkLiftWrapper> getForkLiftWrapperList() {
			if (forkLiftWrapperList == null) {
				forkLiftWrapperList = DBDao.getDao().getForkLiftWrapperList();
			}
			return forkLiftWrapperList;
		}

		public static ForkLiftWrapper getForkLiftByID(int forkLiftID) {
			ForkLiftWrapper forkLift = null;
			foreach (ForkLiftWrapper fl in getForkLiftWrapperList()) {
				if (fl.getForkLift().id == forkLiftID)
					forkLift = fl;
			}
			return forkLift;
		}

		public static SingleTask getSingleTaskByID(int id) {
			SingleTask singleTask = null;
			foreach (SingleTask s in getSingleTaskList()) {
				if (s.taskID == id)
					singleTask = s;
			}
			return singleTask;
		}

		public static SingleTask getSingleTaskByID(string id) {
			return getSingleTaskByID(int.Parse(id));
		}

		public static List<SingleTask> getSingleTaskList() {//获取供选择任务列表
			lock (LockController.getLockController().getLockData()) {
				if (singleTaskList == null) {
					upPickSingleTaskList = new List<SingleTask>();
					downPickSingleTaskList = new List<SingleTask>();
					singleTaskList = DBDao.getDao().SelectSingleTaskList();
					foreach (SingleTask st in singleTaskList) {
						if (st.taskType == TASKTYPE_T.TASK_TYPE_UP_PICK) {
							upPickSingleTaskList.Add(st);  //总共只有两个楼上取货任务
						} else if (st.taskType == TASKTYPE_T.TASK_TYPE_DOWN_PICK) {
							downPickSingleTaskList.Add(st);
						}
					}
				}
			}
			return singleTaskList;
		}

		public static List<SingleTask> getUpPickSingleTaskList() {//获取供选择任务列表
			lock (LockController.getLockController().getLockData()) {
				if (singleTaskList == null || upPickSingleTaskList == null || downPickSingleTaskList == null) {
					getSingleTaskList();
				}
			}
			return upPickSingleTaskList;
		}


		public static List<SingleTask> getDownPickSingleTaskList() {//获取供选择任务列表
			lock (LockController.getLockController().getLockData()) {
				if (singleTaskList == null || upPickSingleTaskList == null || downPickSingleTaskList == null) {
					getSingleTaskList();
				}
			}
			return downPickSingleTaskList;
		}

	}
}
