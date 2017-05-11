using AGV.dao;
using AGV.forklift;
using AGV.locked;
using AGV.tools;
using System;
using System.Threading;

public enum eMSG_STAT { MSG_OK, MSG_IMCOMPLETE, MSG_ERR }; //接收消息格式

namespace AGV.schedule {
	public class ScheduleProduction : ScheduleOperator {
		private bool scheduleFlag = true;
		
		private bool downDeliverPeriod = false;  //当前是否处于上货的状态 上货状态的时候 不发送下货任务

		private bool need = false;//系统是否需要使用到任务调度

		public bool getScheduleFlag() {
			return scheduleFlag;
		}

		public bool isNeed() {
			return need;
		}
		
		public ScheduleProduction() {
			initEnv();
		}

		private bool initEnv() {
			bool envOK = false;

			lock (LockController.getLockController().getLockTask()) {
				ForkLiftWrappersService.getInstance().connectForks();
			}

			Thread.Sleep(100);
			while (true) {
				foreach (ForkLiftWrapper fl in AGVCacheData.getForkLiftWrapperList()) {
					if (fl.getForkLift().isUsed == 1) {
						if (fl.getPosition().getPx() == 0 || fl.getPosition().getPy() == 0) {
							Console.WriteLine("Wait for Fork " + fl.getForkLift().id + " to update position");
							//continue;
						}
					}
				}
				break;
			}
			return envOK;
		}

		/// <summary>
		/// 开启调度任务，检测数据库待执行的任务，将任务有效率
		/// </summary>
		public void startShedule() {
			ThreadFactory.newBackgroudThread(
				new ThreadStart(ScheduleProductionTask.getScheduleTask().sheduleTask)
				).Start();
			ThreadFactory.newBackgroudThread(
				new ThreadStart(ScheduleProductionInstruction.getScheduleInstruction().scheduleInstruction)
				).Start();
		}

		/// <summary>
		/// 用于当前处于上货或下货阶段
		/// </summary>
		public void setDownDeliverPeriod(bool ddp) {
			downDeliverPeriod = ddp;
		}

		/// <summary>
		/// 获取当前是处于上货还是下货阶段
		/// <returns> true 表示当前处于上货阶段 false 表示当前处于下货阶段</returns>
		/// </summary>
		public bool getDownDeliverPeriod() {
			return this.downDeliverPeriod;
		}
	}
}
