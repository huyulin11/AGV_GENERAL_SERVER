using AGV.bean;
using AGV.init;
using AGV.locked;
using AGV.task;
using System.Collections.Generic;

namespace AGV.dao {
	class TaskexeDao {
		private static TaskexeDao dao = null;

		private string selectTaskexeStartOrPauseSql = "SELECT  `uuid`,  `time`,  taskid,  opflag FROM  agv.taskexe_s2c_sop where delflag=0 order by `time` desc limit 0,1 ";

		private string selectTaskexeTaskSql = "SELECT  `uuid`,  `time`,  taskid,  opflag FROM  agv.taskexe_s2c_task where delflag=0 order by  `time` limit 0,10  ";

		private TaskexeDao() { }

		public static TaskexeDao getDao() {
			if (dao == null) {
				dao = new TaskexeDao();
			}
			return dao;
		}

		public List<TaskexeBean> getTaskexeTaskList() {
			return DBDao.getDao().SelectTaskexeBySql(selectTaskexeTaskSql);
		}

		public List<TaskexeBean> getTaskexeStartOrPauseList() {
			return DBDao.getDao().SelectTaskexeBySql(selectTaskexeStartOrPauseSql);
		}

		public void deleteTaskexeBean(TaskexeBean taskexeBean) {
			string sql = " update agv.taskexe_s2c_task set delflag = 1, opflag = 'Over' where `uuid` = '" + taskexeBean.getUuid()+"'";
			DBDao.getDao().DeleteWithSql(sql);
		}

	}
}