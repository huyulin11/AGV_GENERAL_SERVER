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

		TaskexeBean removedNext();

		void sendCommand(TaskexeBean taskexeBean);

		bool isCommandDone();

		bool hasNext();

		void resolveTask2Command();

		void start();
	}
}
