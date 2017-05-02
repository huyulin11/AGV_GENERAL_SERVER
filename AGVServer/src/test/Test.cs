using System;
using System.Threading;
namespace ThreadingTester {
	class ThreadClass {
		public static void trmain() {
			for (int x = 0; x < 10; x++) {
				Thread.Sleep(1000);
				Console.WriteLine(x);
			}
		}
		static void Main(string[] args) {
			Thread thrd1 = new Thread(new ThreadStart(trmain));
			thrd1.Start();
			for (int x = 0; x < 10; x++) {
				Thread.Sleep(900);
				Console.WriteLine("Main    :" + x);
			}
		}
	}
}