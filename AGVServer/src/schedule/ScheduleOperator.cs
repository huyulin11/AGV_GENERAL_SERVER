using AGV.task;

namespace AGV.schedule {
	public interface ScheduleOperator {
		
		/// <summary>
		/// 开启调度任务，检测数据库待执行的任务，将任务有效率
		/// </summary>
		void startShedule();

		bool getScheduleFlag();

		/// <summary>
		/// 用于当前处于上货或下货阶段
		/// </summary>
		void setDownDeliverPeriod(bool ddp);

		/// <summary>
		/// 获取当前是处于上货还是下货阶段
		/// </summary>
		/// <returns> true 表示当前处于上货阶段 false 表示当前处于下货阶段</returns>
		bool getDownDeliverPeriod();
	}
}
