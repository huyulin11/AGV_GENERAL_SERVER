using AGV.init;
using AGV.locked;
using AGV.task;
using System.Collections.Generic;

namespace AGV.dao {
	class CommandQueueDao {
		private static CommandQueueDao dao = null;

		private CommandQueueDao() { }

		public static CommandQueueDao getDao() {
			if (dao == null) {
				dao = new CommandQueueDao();
			}
			return dao;
		}

		public List<SingleTask> getDownPickSingleTask() {
			List<SingleTask> sList = DBDao.getDao().SelectSingleTaskList();
			List<SingleTask> downList = new List<SingleTask>();
			foreach (SingleTask st in sList) {
				if (st.taskType == TASKTYPE_T.TASK_TYPE_DOWN_PICK) {
					downList.Add(st);
				}
			}
			return downList;
		}

		public SingleTask getSingleTaskByID(int singleTaskID) {
			SingleTask singleTask = null;
			foreach (SingleTask st in AGVCacheData.getSingleTaskList()) {
				if (st.taskID == singleTaskID)
					singleTask = st;
			}
			return singleTask;
		}

		public SingleTask getSingleTaskByTaskName(string taskName) {
			SingleTask singleTask = null;
			foreach (SingleTask st in AGVCacheData.getSingleTaskList()) {
				if (st.taskName.StartsWith(taskName))
					singleTask = st;
			}
			return singleTask;
		}

	}
}