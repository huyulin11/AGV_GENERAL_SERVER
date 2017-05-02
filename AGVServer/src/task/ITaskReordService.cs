using System.Collections.Generic;

namespace AGV.task {

	/// <summary>
	/// 描述车子的集合及其管理逻辑
	/// </summary>
	public interface ITaskReordService {

		/// <summary>
		/// 添加任务记录
		/// </summary>
		void addTaskRecord(TASKSTAT_T stat, SingleTask st);

		/// <summary>
		/// 添加任务记录，包括两部1、更新taskRecordList 2、更新数据库
		/// </summary>
		void addTaskRecord(TaskRecord tr);

		/// <summary>
		/// 根据相关条件移除任务 包括更新任务列表 删除数据库
		/// </summary>
		void removeTaskRecord(SingleTask st, TASKSTAT_T taskRecordStat);

		void topTaskRecord(TaskRecord tr);

		/// <summary>
		///获取状态不为执行完毕的所有任务
		/// </summary>
		List<TaskRecord> getTaskRecordList();

		/// <summary>
		///获取状态为缓存的所有任务
		/// </summary>
		List<TaskRecord> getReadySendTaskRecordList();


		/// <summary>
		///获取制定任务类型的所有任务
		/// </summary>
		List<TaskRecord> getTaskRecordList(int singleTaskID);

		/// <summary>
		///获取制定任务类型的、且状态为缓存的所有任务
		/// </summary>
		List<TaskRecord> getReadySendTaskRecordList(int singleTaskID);

		void deleteAllTaskRecord();
	}
}
