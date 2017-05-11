using AGV.dao;
using System.Collections.Generic;

namespace AGV.task {

	/// <summary>
	/// 描述车子的集合及其管理逻辑
	/// </summary>
	public class TaskReordService : ITaskReordService {
		private static TaskReordService taskReordService = null;
		
		public static ITaskReordService getInstance() {
			if (taskReordService == null) {
				taskReordService = new TaskReordService();
			}
			return taskReordService;
		}

		/// <summary>
		/// 添加任务记录
		/// </summary>
		public void addTaskRecord(TASKSTAT_T stat, SingleTask st) {
			/*TaskRecord tr = new TaskRecord();
            tr.taskRecordStat = taskRecordStat;
            tr.singleTask = st;
            tr.taskRecordName = st.taskName;*/
			DBDao.getDao().InsertTaskRecord(stat, st);
		}

		/// <summary>
		/// 添加任务记录，包括两部1、更新taskRecordList 2、更新数据库
		/// </summary>
		public void addTaskRecord(TaskRecord tr) {
			DBDao.getDao().InsertTaskRecord(tr);
		}

		/// <summary>
		/// 根据相关条件移除任务 包括更新任务列表 删除数据库
		/// </summary>
		public void removeTaskRecord(SingleTask st, TASKSTAT_T taskRecordStat) {
			DBDao.getDao().RemoveTaskRecord(st, taskRecordStat);
		}

		public void topTaskRecord(TaskRecord tr) {
			int count = 0;
			count = DBDao.getDao().selectMaxBySql("select max(taskLevel) from taskrecord");  //查询所有被置过顶的任务
			tr.taskLevel = count + 1;

			DBDao.getDao().UpdateTaskRecord(tr);
		}

		/// <summary>
		///获取状态不为执行完毕的所有任务
		/// </summary>
		public List<TaskRecord> getTaskRecordList() {
			return TaskrecordDao.getDao().getTaskRecordList();
		}

		/// <summary>
		///获取状态为缓存的所有任务
		/// </summary>
		public List<TaskRecord> getReadySendTaskRecordList() {
			return TaskrecordDao.getDao().getReadySendTaskRecordList();
		}


		/// <summary>
		///获取制定任务类型的所有任务
		/// </summary>
		public List<TaskRecord> getTaskRecordList(int singleTaskID) {
			return TaskrecordDao.getDao().getTaskRecordList(singleTaskID);
		}

		/// <summary>
		///获取制定任务类型的、且状态为缓存的所有任务
		/// </summary>
		public List<TaskRecord> getReadySendTaskRecordList(int singleTaskID) {
			return TaskrecordDao.getDao().getReadySendTaskRecordList(singleTaskID);
		}

		public void deleteAllTaskRecord() {
			TaskrecordDao.getDao().deleteAllTaskRecord();
		}
	}
}
