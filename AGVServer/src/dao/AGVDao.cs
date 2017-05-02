using AGV.task;
using System.Collections.Generic;

namespace AGV.dao {

	public class AGVDao {

		private static AGVDao dao = null;

		private string selectFromTaskrecordStr = "select * from taskrecord where 1=1 and ";
		private string orderByTaskrecordStr = " order by taskRecordStat desc, taskLevel desc";

		private AGVDao() {
		}

		public static AGVDao getDao() {
			if (dao == null) {
				dao = new AGVDao();
			}
			return dao;
		}

		/// <summary>
		///获取taskrecord数据集
		/// </summary>
		private List<TaskRecord> selectTaskRecordList(string whereCause) {
			string sql = selectFromTaskrecordStr + (string.IsNullOrEmpty(whereCause) ? " 1=1 " : whereCause) + orderByTaskrecordStr;
			List<TaskRecord> taskRecordList = DBDao.getDao().SelectTaskRecordBySql(sql);
			return taskRecordList;
		}

		/// <summary>
		///获取状态不为执行完毕的所有任务
		/// </summary>
		public List<TaskRecord> getTaskRecordList() {
			return selectTaskRecordList("taskRecordStat != 4");
		}

		/// <summary>
		///获取状态为缓存的所有任务
		/// </summary>
		public List<TaskRecord> getReadySendTaskRecordList() {
			return selectTaskRecordList("taskRecordStat = 1");
		}


		/// <summary>
		///获取制定任务类型的所有任务
		/// </summary>
		public List<TaskRecord> getTaskRecordList(int singleTaskID) {
			return selectTaskRecordList("singleTask =" + singleTaskID);
		}

		/// <summary>
		///获取制定任务类型的、且状态为缓存的所有任务
		/// </summary>
		public List<TaskRecord> getReadySendTaskRecordList(int singleTaskID) {
			return selectTaskRecordList("singleTask =" + singleTaskID + " and taskRecordStat = 1 ");
		}

		public void deleteAllTaskRecord() {
			string sql = "truncate table taskrecord";
			DBDao.getDao().DeleteWithSql(sql);
		}

	}
}