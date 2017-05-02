using System.Threading;

namespace AGV.tools {
	public class ThreadFactory {

		public static Thread newThread(ThreadStart ts) {
			return new Thread(ts);
		}

		public static Thread newThread(ParameterizedThreadStart ts) {
			return new Thread(ts);
		}

		public static Thread newBackgroudThread(ThreadStart ts) {
			Thread newBackgroudThread;
			newBackgroudThread = new Thread(ts);
			newBackgroudThread.IsBackground = true;
			return newBackgroudThread;
		}

		public static Thread newBackgroudThread(ParameterizedThreadStart ts) {
			Thread newBackgroudThread;
			newBackgroudThread = new Thread(ts);
			newBackgroudThread.IsBackground = true;
			return newBackgroudThread;
		}
	};
}
