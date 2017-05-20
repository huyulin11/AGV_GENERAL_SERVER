using AGV.bean;
using AGV.forklift;
using System.Collections.Generic;

namespace AGV.taskexe {

	/// <summary>
	/// 描述任务从用户下达到发送AGV执行前的逻辑
	/// </summary>
	public interface ITaskexeService {
		bool isSystemRunning();

		/// <summary>
		/// 系统是否需要使用此任务调度器
		/// </summary>
		bool isNeed();

		Queue<TaskexeBean> getTaskexeTaskList();

		/// <summary>
		/// 获取下一个bean，将该bean对应的实体状态修改为Send（如果他的初始状态为New的话），
		/// 并从队列中剔除掉
		/// </summary>
		TaskexeBean getAndRemoveNextTaskexeBean();

		bool hasNextTaskexe();

		void start();
	}
}
