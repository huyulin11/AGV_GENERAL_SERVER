using AGV.dao;
using AGV.elevator;
using AGV.forklift;
using AGV.init;
using AGV.locked;
using AGV.message;
using AGV.sys;
using AGV.util;
using System;
using System.Diagnostics;
using System.Threading;

namespace AGV.schedule {
	public class ScheduleProductionInstruction {
		private static ScheduleProductionInstruction scheduleProductionInstruction = null;

		private ScheduleProductionInstruction() {
		}

		public static ScheduleProductionInstruction getScheduleInstruction() {
			if (scheduleProductionInstruction == null) {
				scheduleProductionInstruction = new ScheduleProductionInstruction();
			}
			return scheduleProductionInstruction;
		}

		public void scheduleInstruction() {
			while (ScheduleFactory.getSchedule().getScheduleFlag()) {
				Thread.Sleep(500);
				sheduleLift();

				if (AGVSystem.getSystem().getSystemPause()) {
					if (AGVSystem.getSystem().getLastPause() != SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_SYSTEM_WITH_START && AGVSystem.getSystem().getLastPause() != SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_SYSTEM_WITHOUT_START) //避免多次设置
					{
						foreach (ForkLiftWrapper fl in AGVCacheData.getForkLiftWrapperList()) {
							if (fl.getForkLift().shedulePause == 0) {
								AGVUtil.setForkCtrl(fl, 1);  //向不是暂停的车发送暂停指令
							}
						}
					}

					AGVSystem.getSystem().setLastPause(AGVSystem.getSystem().getCurrentPause());
					continue; //系统暂停后不需要调度
				} else if (AGVSystem.getSystem().getCurrentPause() == SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_UP_WITH_START || AGVSystem.getSystem().getCurrentPause() == SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_UP_WITHOUT_START) //暂停楼上的车，有时候卸货不及时
				{
					if (AGVSystem.getSystem().getLastPause() != SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_UP_WITH_START && AGVSystem.getSystem().getLastPause() != SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_UP_WITHOUT_START) //避免多次设置
					{

						foreach (ForkLiftWrapper fl in AGVCacheData.getForkLiftWrapperList()) {
							if (fl.getForkLift().forklift_number != 3 && fl.getForkLift().shedulePause == 0)  //只调度楼上的车
							{
								AGVUtil.setForkCtrl(fl, 1);
							}
						}
					}

					AGVSystem.getSystem().setLastPause(AGVSystem.getSystem().getCurrentPause());
					continue;  //楼上的车子被暂停后，不需要调度
				} else if (AGVSystem.getSystem().getCurrentPause() == SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_DOWN_WITH_START || AGVSystem.getSystem().getCurrentPause() == SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_DOWN_WITHOUT_START) {
					if (AGVSystem.getSystem().getLastPause() != SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_DOWN_WITH_START && AGVSystem.getSystem().getLastPause() != SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_DOWN_WITHOUT_START) //避免多次设置
					{
						ForkLiftWrapper fl = ForkLiftWrappersService.getInstance().getForkLiftByNunber(3);
						AGVUtil.setForkCtrl(fl, 1);
					}

					AGVSystem.getSystem().setLastPause(AGVSystem.getSystem().getCurrentPause());
				} else {
					if (AGVSystem.getSystem().getLastPause() == SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_SYSTEM_WITH_START) {
						foreach (ForkLiftWrapper fl in AGVCacheData.getForkLiftWrapperList()) {
							if (fl.getForkLift().shedulePause == 0) {
								AGVUtil.setForkCtrl(fl, 0);  //之前不是暂停的车，发送启动指令
							}
						}
					}

					if (AGVSystem.getSystem().getLastPause() == SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_UP_WITH_START) {
						foreach (ForkLiftWrapper fl in AGVCacheData.getForkLiftWrapperList()) {
							if (fl.getForkLift().forklift_number != 3 && fl.getForkLift().shedulePause == 0) {
								AGVUtil.setForkCtrl(fl, 0);  //之前不是暂停的车，发送启动指令
							}
						}
					}

					if (AGVSystem.getSystem().getLastPause() == SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_DOWN_WITH_START) {
						ForkLiftWrapper fl = ForkLiftWrappersService.getInstance().getForkLiftByNunber(3);
						AGVUtil.setForkCtrl(fl, 0);
					}

					AGVSystem.getSystem().setLastPause(AGVSystem.getSystem().getCurrentPause());
				}

				lock (LockController.getLockController().getLockForkLift())  //加锁，避免车的状态不一致
				{
					_sheduleRunning();
				}
			}
		}

		private void sheduleLift() {
			AGVMessage message = new AGVMessage();
			if (ElevatorFactory.getElevator().getOutCommand() == COMMAND_FROME2S.LIFT_OUT_COMMAND_UP_DOWN)  //楼上楼下都有货
			{
				AGVLog.WriteWarn("升降机楼上楼下都有货，楼上的车被暂停", new StackFrame(true));
				message.setMessageType(AGVMessageHandler_TYPE_T.AGVMEASAGE_LIFT_UPDOWN);
				message.setMessageStr("升降机楼上楼下都有货，楼上的车被暂停");

				AGVMessageHandler.getMessageHandler().setMessage(message);  //发送消息
			} else if (ElevatorFactory.getElevator().getOutCommand() > COMMAND_FROME2S.LIFT_OUT_COMMAND_UP_DOWN)  //升降机 卡货
			{
				AGVLog.WriteWarn("升降机卡货，系统被暂停", new StackFrame(true));
				message.setMessageType(AGVMessageHandler_TYPE_T.AGVMessageHandler_LIFT_BUG);
				message.setMessageStr("升降机卡货，系统被暂停");

				AGVMessageHandler.getMessageHandler().setMessage(message);  //发送消息
			}
		}

		private void _sheduleRunning() {
			ForkLiftWrapper fl_1 = null;
			ForkLiftWrapper fl_2 = null;
			SHEDULE_TYPE_T shedule_type = SHEDULE_TYPE_T.SHEDULE_TYPE_MIN;
			foreach (ForkLiftWrapper fl in AGVCacheData.getForkLiftWrapperList()) {
				if (fl.getForkLift().forklift_number != 3 && fl.getForkLift().isUsed == 1 && fl.getForkLift().taskStep != TASK_STEP.TASK_IDLE)  //调度没有在使用的车 车子任务没有完成，只有当两辆车同时使用时，才调度
				{
					if (fl_1 != null)
						fl_2 = fl;
					else
						fl_1 = fl;
				}
			}

			if (fl_1 != null && fl_2 != null)  //两车同时运行时才需要调度
			{
				shedule_type = getForkSheduleType(fl_1);

				if (shedule_type == SHEDULE_TYPE_T.SHEDULE_TYPE_1TO2) {
					if (fl_2.getForkLift().shedulePause == 0 && fl_2.getPosition().getArea() == 2)  //检测到另一辆车在区域2运行，需要暂停刚进入区域2的车
					{
						if (fl_1.getForkLift().shedulePause == 0) {
							AGVUtil.setForkCtrl(fl_1, 1);
							fl_1.getForkLift().shedulePause = 1;
						}
					} else //否则该车正常进入区域2，考虑到之前可能被暂停，没有车在区域2后，该车将被启动
					  {
						fl_1.getPosition().setArea(2);
						if (fl_1.getForkLift().shedulePause == 1) {
							//向1车发送启动
							AGVUtil.setForkCtrl(fl_1, 0);
							fl_1.getForkLift().shedulePause = 0;
						}
					}
				} else if (shedule_type == SHEDULE_TYPE_T.SHEDULE_TYPE_2TO3) {
					if (fl_2.getForkLift().shedulePause == 0 && fl_2.getPosition().getArea() == 3) {
						if (fl_1.getForkLift().shedulePause == 0) {
							AGVUtil.setForkCtrl(fl_1, 1);
							fl_1.getForkLift().shedulePause = 1;
						}
					} else {
						fl_1.getPosition().setArea(3);
						if (fl_1.getForkLift().shedulePause == 1) {
							//向1车发送启动
							AGVUtil.setForkCtrl(fl_1, 0);
							fl_1.getForkLift().shedulePause = 0;
						}
					}
				} else if (shedule_type == SHEDULE_TYPE_T.SHEDULE_TYPE_3TO2) {
					if (fl_2.getForkLift().shedulePause == 0 && fl_2.getPosition().getArea() == 2) {
						if (fl_1.getForkLift().shedulePause == 0) {
							AGVUtil.setForkCtrl(fl_1, 1);
							fl_1.getForkLift().shedulePause = 1;
						}
					} else {
						fl_1.getPosition().setArea(2);
						if (fl_1.getForkLift().shedulePause == 1) {
							//向1车发送启动
							AGVUtil.setForkCtrl(fl_1, 0);
							fl_1.getForkLift().shedulePause = 0;
						}
					}
				} else if (shedule_type == SHEDULE_TYPE_T.SHEDULE_TYPE_2TO1) {
					if (fl_2.getForkLift().shedulePause == 0 && fl_2.getPosition().getArea() == 1) {
						if (fl_1.getForkLift().shedulePause == 0) {
							AGVUtil.setForkCtrl(fl_1, 1);
							fl_1.getForkLift().shedulePause = 1;
						}
					} else {
						fl_1.getPosition().setArea(1);
						if (fl_1.getForkLift().shedulePause == 1) {
							//向1车发送启动
							AGVUtil.setForkCtrl(fl_1, 0);
							fl_1.getForkLift().shedulePause = 0;
						}
					}
				}

				shedule_type = getForkSheduleType(fl_2);

				if (shedule_type == SHEDULE_TYPE_T.SHEDULE_TYPE_1TO2) {
					if (fl_1.getForkLift().shedulePause == 0 && fl_1.getPosition().getArea() == 2)  //检测到另一辆车在区域2运行，需要暂停刚进入区域2的车
					{
						if (fl_2.getForkLift().shedulePause == 0) {
							AGVUtil.setForkCtrl(fl_2, 1);
							fl_2.getForkLift().shedulePause = 1;
						}
					} else //否则该车正常进入区域2，考虑到之前可能被暂停，没有车在区域2后，该车将被启动
					  {
						fl_2.getPosition().setArea(2);
						if (fl_2.getForkLift().shedulePause == 1) {
							//向1车发送启动
							AGVUtil.setForkCtrl(fl_2, 0);
							fl_2.getForkLift().shedulePause = 0;
						}
					}
				} else if (shedule_type == SHEDULE_TYPE_T.SHEDULE_TYPE_2TO3) {
					if (fl_1.getForkLift().shedulePause == 0 && fl_1.getPosition().getArea() == 3) {
						if (fl_2.getForkLift().shedulePause == 0) {
							AGVUtil.setForkCtrl(fl_2, 1);
							fl_2.getForkLift().shedulePause = 1;
						}
					} else {
						fl_2.getPosition().setArea(3);
						if (fl_2.getForkLift().shedulePause == 1) {
							//向1车发送启动
							AGVUtil.setForkCtrl(fl_2, 0);
							fl_2.getForkLift().shedulePause = 0;
						}
					}
				} else if (shedule_type == SHEDULE_TYPE_T.SHEDULE_TYPE_3TO2) {
					if (fl_1.getForkLift().shedulePause == 0 && fl_1.getPosition().getArea() == 2) {
						if (fl_2.getForkLift().shedulePause == 0) {
							AGVUtil.setForkCtrl(fl_2, 1);
							fl_2.getForkLift().shedulePause = 1;
						}
					} else {
						fl_2.getPosition().setArea(2);
						if (fl_2.getForkLift().shedulePause == 1) {
							//向1车发送启动
							AGVUtil.setForkCtrl(fl_2, 0);
							fl_2.getForkLift().shedulePause = 0;
						}
					}
				} else if (shedule_type == SHEDULE_TYPE_T.SHEDULE_TYPE_2TO1) {
					if (fl_1.getForkLift().shedulePause == 0 && fl_1.getPosition().getArea() == 1) {
						if (fl_2.getForkLift().shedulePause == 0) {
							AGVUtil.setForkCtrl(fl_2, 1);
							fl_2.getForkLift().shedulePause = 1;
						}
					} else {
						fl_2.getPosition().setArea(1);
						if (fl_2.getForkLift().shedulePause == 1) {
							//向1车发送启动
							AGVUtil.setForkCtrl(fl_2, 0);
							fl_2.getForkLift().shedulePause = 0;
						}
					}
				}

				checkPausePosition(fl_1);
				checkPausePosition(fl_2);
			} else if (fl_1 != null && fl_2 == null) {
				shedule_type = getForkSheduleType(fl_1);
				if (shedule_type == SHEDULE_TYPE_T.SHEDULE_TYPE_1TO2) {
					fl_1.getPosition().setArea(2);
				} else if (shedule_type == SHEDULE_TYPE_T.SHEDULE_TYPE_2TO3) {
					fl_1.getPosition().setArea(3);
				} else if (shedule_type == SHEDULE_TYPE_T.SHEDULE_TYPE_3TO2) {
					fl_1.getPosition().setArea(2);
				} else if (shedule_type == SHEDULE_TYPE_T.SHEDULE_TYPE_2TO1) {
					fl_1.getPosition().setArea(1);
				}

				if (fl_1.getForkLift().shedulePause == 1) //如果车子被暂停，启动该车
				{
					AGVUtil.setForkCtrl(fl_1, 0);
					fl_1.getForkLift().shedulePause = 0;
				}
			}
		}

		private void checkPausePosition(ForkLiftWrapper fl) {
			if (fl.getForkLift().shedulePause == 1) {

				if (fl.getPosition().getArea() == 3 && fl.getPosition().getPx() > AGVConstant.BORDER_X_3 + AGVConstant.BORDER_X_3_DEVIATION_PLUS)  //从区域3进入区域2的时候， 如果没有暂停成功或暂停慢了，重新启动车子，并报警
				{
					Console.WriteLine(fl.getForkLift().forklift_number + "号车 pause position = " + fl.getPosition().getPx());

					fl.getPosition().setArea(2);
					fl.getForkLift().shedulePause = 0;

					AGVMessage message = new AGVMessage();
					message.setMessageType(AGVMessageHandler_TYPE_T.AGVMessageHandler_SENDPAUSE_ERR);
					message.setMessageStr("第二分界线 检测中断错误");

					AGVLog.WriteError("第二分界线 检测中断错误", new StackFrame(true));
					AGVMessageHandler.getMessageHandler().setMessage(message);
				} else if (fl.getPosition().getArea() == 2 && fl.getPosition().getPx() > AGVConstant.BORDER_X_2 + AGVConstant.BORDER_X_2_DEVIATION_PLUS)  //从区域2进入区域1的时候， 如果没有暂停成功或暂停慢了，重新启动车子，并报警
				{
					Console.WriteLine(fl.getForkLift().forklift_number + "号车 pause position = " + fl.getPosition().getPx());

					fl.getPosition().setArea(1);
					fl.getForkLift().shedulePause = 0;

					AGVMessage message = new AGVMessage();
					message.setMessageType(AGVMessageHandler_TYPE_T.AGVMessageHandler_SENDPAUSE_ERR);
					message.setMessageStr("第一分界线 检测中断错误");

					AGVLog.WriteError("第一分界线 检测中断错误", new StackFrame(true));
					AGVMessageHandler.getMessageHandler().setMessage(message);
				} else if (fl.getPosition().getArea() == 2 && fl.getPosition().getPx() < AGVConstant.BORDER_X_3 - AGVConstant.BORDER_X_3_DEVIATION_PLUS) //从区域2进入区域3的时候，暂停没成功或暂停慢了， 报警，需要手动启动
				{
					Console.WriteLine(fl.getForkLift().forklift_number + "号车 pause position = " + fl.getPosition().getPx());

					fl.getPosition().setArea(3);
					fl.getForkLift().shedulePause = 0;

					AGVMessage message = new AGVMessage();
					message.setMessageType(AGVMessageHandler_TYPE_T.AGVMessageHandler_SENDPAUSE_ERR);
					message.setMessageStr("第二分界线 检测中断错误");

					AGVLog.WriteError("第二分界线 检测中断错误", new StackFrame(true));
					AGVMessageHandler.getMessageHandler().setMessage(message);
				} else if (fl.getPosition().getArea() == 1 && fl.getPosition().getPx() < AGVConstant.BORDER_X_2 - AGVConstant.BORDER_X_2_DEVIATION_PLUS) //从区域2进入区域3的时候，暂停没成功或暂停慢了， 报警，需要手动启动
				{
					Console.WriteLine(fl.getForkLift().forklift_number + "号车 pause position = " + fl.getPosition().getPx());

					fl.getPosition().setArea(2);
					fl.getForkLift().shedulePause = 0;

					AGVMessage message = new AGVMessage();
					message.setMessageType(AGVMessageHandler_TYPE_T.AGVMessageHandler_SENDPAUSE_ERR);
					message.setMessageStr("第二分界线 检测中断错误");

					AGVLog.WriteError("第一分界线 检测中断错误", new StackFrame(true));
					AGVMessageHandler.getMessageHandler().setMessage(message);
				}
			}
		}

		private SHEDULE_TYPE_T getForkSheduleType(ForkLiftWrapper fl) {
			if (fl.getPosition().getArea() == 1 && fl.getPosition().getPx() < AGVConstant.BORDER_X_2 - AGVConstant.BORDER_X_DEVIARION) {
				Console.WriteLine(" check " + fl.getForkLift().forklift_number + "号车 从区域1进入2");
				return SHEDULE_TYPE_T.SHEDULE_TYPE_1TO2;
			} else if (fl.getPosition().getArea() == 2 && fl.getPosition().getPx() < AGVConstant.BORDER_X_3 - AGVConstant.BORDER_X_DEVIARION) {
				Console.WriteLine(" check " + fl.getForkLift().forklift_number + "号车 从区域2进入3");
				return SHEDULE_TYPE_T.SHEDULE_TYPE_2TO3;
			} else if (fl.getPosition().getArea() == 3 && fl.getPosition().getPx() > AGVConstant.BORDER_X_3 + AGVConstant.BORDER_X_DEVIARION) {
				Console.WriteLine(" check " + fl.getForkLift().forklift_number + "号车 从区域3进入2");
				return SHEDULE_TYPE_T.SHEDULE_TYPE_3TO2;
			} else if (fl.getPosition().getArea() == 2 && fl.getPosition().getPx() > AGVConstant.BORDER_X_2 + AGVConstant.BORDER_X_DEVIARION) {
				Console.WriteLine(" check " + fl.getForkLift().forklift_number + "号车 从区域2进入1");
				return SHEDULE_TYPE_T.SHEDULE_TYPE_2TO1;
			}

			return SHEDULE_TYPE_T.SHEDULE_TYPE_MIN;
		}

	}
}
