using AGV.dao;
using AGV.forklift;
using AGV.schedule;
using AGV.socket;
using AGV.tools;
using AGV.util;
using System.Diagnostics;
using System.Threading;

namespace AGV.sys {

	/// <summary>
	/// 描述单个任务
	/// </summary>
	public class AGVSystem {
		private static AGVSystem system = null;

		private SHEDULE_PAUSE_TYPE_T lastPause = SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_TYPE_MIN;  //上一个暂停标志，用于对比pause，作比较
		private SHEDULE_PAUSE_TYPE_T currentPause = SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_TYPE_MIN;
		
		private bool lowpowerShedule = false;  //当前有AGV处于低电池状态

		private AGVSystem() { }

		public void setLowpowerShedule(bool flag) {
			this.lowpowerShedule = flag;
		}

		public static AGVSystem getSystem() {
			if (system == null) {
				system = new AGVSystem();
			}
			return system;
		}

		public void setLastPause(SHEDULE_PAUSE_TYPE_T lastPause) {
			this.lastPause = lastPause;
		}

		public SHEDULE_PAUSE_TYPE_T getLastPause() {
			return this.lastPause;
		}

		public void setCurrentPause(SHEDULE_PAUSE_TYPE_T currentPause) {
			this.currentPause = currentPause;
		}

		public SHEDULE_PAUSE_TYPE_T getCurrentPause() {
			return this.currentPause;
		}

		private void shedulePause()  //用于系统暂停时，检测暂停是否发送成功，如果没有发送成功，则一直向该车发送暂停
		{
			while (currentPause > SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_TYPE_MIN && currentPause < SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_UP_MAX) {
				foreach (ForkLiftWrapper fl in AGVCacheData.getForkLiftWrapperList()) {
					if (currentPause == SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_UP_WITH_START || currentPause == SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_UP_WITHOUT_START) //楼上楼下都有货时，暂停楼上的车，露楼下的车不用检测20160929 破凉
					{
						if (fl.getForkLift().forklift_number == 3) {
							continue;
						}
					}
					if (fl.getPauseStr().Equals("运行"))  //如果该车返回的pauseStat没有被设置成1，则向该车发送暂停
					{
						AGVUtil.setForkCtrlWithPrompt(fl, 1);
					}
				}

				Thread.Sleep(30000);
			}
		}

		public void setPause(SHEDULE_PAUSE_TYPE_T spt) {
			this.currentPause = spt;

			AGVLog.WriteInfo(spt.ToString() + "was set ", new StackFrame(true));
			///同步楼下的客户端
			if (currentPause == SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_SYSTEM_WITH_START) {
				AGVSocketServer.getSocketServer().sendDataMessage("systemPause=1");
			} else if (currentPause == SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_TYPE_MIN && lastPause == SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_SYSTEM_WITH_START) {
				AGVSocketServer.getSocketServer().sendDataMessage("systemPause=0");
			}

			if (this.currentPause > SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_TYPE_MIN && this.currentPause < SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_UP_MAX) {
				AGVLog.WriteInfo("启动暂停检测线程", new StackFrame(true));

				ThreadFactory.newBackgroudThread(new ThreadStart(shedulePause)).Start();
			}
		}

		public bool getSystemPause() {
			if (currentPause == SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_SYSTEM_WITH_START || currentPause == SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_SYSTEM_WITHOUT_START) {
				return true;
			}
			return false;
		}

	}
}
