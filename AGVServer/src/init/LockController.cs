namespace AGV.locked {
	public class LockController {
		private object lockTask = new object();  //任务线程锁
		private object lockShedule = new object();  //调度线程锁
		private object lockForkLift = new object();  //PLC线程锁  读写操作不同同时进行
		private object lockData = new object();  //数据锁 不能同时操作数据

		private static LockController lockController = new LockController();

		public static LockController getLockController() {
			if (lockController == null) {
				lockController = new LockController();
			}
			return lockController;
		}

		/// <summary>
		/// 获取任务线程锁
		/// </summary>
		/// <returns></returns>
		public object getLockTask() {
			return lockTask;
		}

		/// <summary>
		/// 获取调度锁
		/// </summary>
		/// <returns></returns>
		public object getLockShedule() {
			return lockShedule;
		}

		public object getLockForkLift() {
			return lockForkLift;
		}

		public object getLockData() {
			return lockData;
		}

	}
}
