using AGV.bean;
using AGV.init;
using AGV.locked;
using AGV.task;
using AGV.util;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AGV.dao {
	class CommandDao {
		private static CommandDao dao = null;
		
		private CommandDao() {
		}

		public static CommandDao getDao() {
			if (dao == null) {
				dao = new CommandDao();
			}
			return dao;
		}

		/// <summary>
		/// 插入任务记录
		/// </summary>
		public void InsertCommand(string taskid) {
			string sql = "insert into command_queue_s2c (uuid,taskid,opflag) values (uuid(),'" + taskid + "','" + "New" + "') ";
			try {
				lock (DBDao.getDao().getLockDB()) {
					MySqlDataReader dataReader = DBDao.getDao().execNonQuery(sql);
					dataReader.Close();
				}
			} catch (Exception ex) {
				Console.WriteLine(ex.ToString());
			}
		}

		public List<CommandBean> selectLatestCommand() {
			string query = "select * from command_queue_s2c order by time desc limit 0,1 ";
			List<CommandBean> list = new List<CommandBean>();
			AGVLog.WriteInfo("SelectForkList sql = " + query, new StackFrame(true));
			try {
				lock (DBDao.getDao().getLockDB()) {
					MySqlDataReader dataReader = DBDao.getDao().execQuery(query);
					while (dataReader.Read()) {
						CommandBean command = new CommandBean();
						command.setUuid(dataReader["uuid"] + "");
						command.setTime(dataReader["time"] + "");
						command.setTaskid(dataReader["taskid"] + "");
						command.setOpflag(dataReader["opflag"] + "");
						list.Add(command);
					}
					dataReader.Close();
				}
			} catch (Exception ex) {
				Console.WriteLine(" sql err " + ex.ToString());
			}
			return list;
		}

	}
}