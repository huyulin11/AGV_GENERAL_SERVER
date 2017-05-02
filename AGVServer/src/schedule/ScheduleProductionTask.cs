using AGV.dao;
using AGV.elevator;
using AGV.forklift;
using AGV.init;
using AGV.locked;
using AGV.message;
using AGV.sys;
using AGV.task;
using AGV.tools;
using AGV.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace AGV.schedule {
	public class ScheduleProductionTask {
		private static ScheduleProductionTask scheduleProductionTask = null;

		private List<TaskRecord> taskRecordList = new List<TaskRecord>();  //存储要处理的任务的列表
		private TaskRecord upTaskRecord = null; //上料任务仅允许一个上料任务

		private int upPicSingleTaskkUsed = 1; //用于轮流获取楼上取货任务, 默认从14.xml开始卸货

		private int downPicSingleTaskkUsed = 0; //用于轮流获取楼上取货任务

		private List<SingleTask> upPickSingleTaskList = AGVCacheData.getUpPickSingleTaskList();
		private List<SingleTask> downPickSingleTaskList = AGVCacheData.getDownPickSingleTaskList();

		private ScheduleProductionTask() {
		}

		public static ScheduleProductionTask getScheduleTask() {
			if (scheduleProductionTask == null) {
				scheduleProductionTask = new ScheduleProductionTask();
			}
			return scheduleProductionTask;
		}

		public void sheduleTask() {
			ForkLiftWrapper tmpForkLiftWrapper = null;
			SingleTask tmpSingleTask = null;
			int upRecordStep = 0;  //没有上货， 该值大于0的时候，表示上货还没有结束，可能存在升降机正在运送，车子当前没有任务
			int downRecordStep = 0;  //没有下货，该值大于0的时候， 表示下货没有结束
			int result = -1;

			///***开始处理任务的循环语句***///
			while (ScheduleFactory.getSchedule().getScheduleFlag()) {

				Thread.Sleep(2000);
				if (AGVSystem.getSystem().getSystemPause()) {  //系统暂停后，调度程序不执行
					Console.WriteLine("system pause");
					continue;
				}

				//Console.WriteLine(" shedule Task");
				lock (LockController.getLockController().getLockTask()) {
					taskRecordList = TaskReordService.getInstance().getTaskRecordList();
					upTaskRecord = checkUpTaskRecord();
				}
				//if (upTaskRecord != null)
				//  Console.WriteLine(" upTaskRecord name = " + upTaskRecord.taskRecordName);

				if (ScheduleFactory.getSchedule().getDownDeliverPeriod()) //当前处于上货阶段，有的话控制升降机上升
				{
					//读取升降机上料信号
					if (ElevatorFactory.getElevator().getOutCommand() == COMMAND_FROME2S.LIFT_OUT_COMMAND_UP)  //检测到楼下有货，发送指令到升降机运货到楼上
					{
						ElevatorFactory.getElevator().setDataCommand(COMMAND_FROMS2E.LIFT_IN_COMMAND_UP);
						while (ElevatorFactory.getElevator().getOutCommand() != COMMAND_FROME2S.LIFT_OUT_COMMAND_DOWN)  //等待升降机送货到楼上
						{
							Console.WriteLine("wait lifter up goods");
							Thread.Sleep(100);
						}

						if (upRecordStep > 0) //升降机将货运到楼上,保证upRecordStep不小于0
						{
							upRecordStep--;
							AGVLog.WriteError("上货期间 升降机将货运送到楼上: step = " + upRecordStep, new StackFrame(true));
						}
					}

					if (ElevatorFactory.getElevator().getOutCommand() == COMMAND_FROME2S.LIFT_OUT_COMMAND_DOWN
						|| ElevatorFactory.getElevator().getOutCommand() == COMMAND_FROME2S.LIFT_OUT_COMMAND_UP_DOWN) //上货期间 楼上楼下都有货， 楼上的车子需要继续运行
					{
						lock (LockController.getLockController().getLockForkLift()) {
							//运动到楼上后，发送指令到楼上AGV，把货取走
							tmpForkLiftWrapper = getSheduleForkLift();
							if (tmpForkLiftWrapper != null) {
								tmpSingleTask = getUpPickSingleTaskOnTurn();
								TaskRecord tr_tmp = new TaskRecord();
								tr_tmp.singleTask = tmpSingleTask;
								tr_tmp.taskRecordName = tmpSingleTask.taskName;
								TaskReordService.getInstance().addTaskRecord(tr_tmp);
								tmpForkLiftWrapper.sendTask(tr_tmp); //发送任务
								upPicSingleTaskkUsed++; //用于下次切换卸货点
							} else {
								Console.WriteLine(" 楼上没有可用的车去卸货");
								AGVLog.WriteError(" 楼上没有可用的车去卸货", new StackFrame(true));
							}
						}
						if (tmpForkLiftWrapper != null) {

							while (ElevatorFactory.getElevator().getOutCommand() == COMMAND_FROME2S.LIFT_OUT_COMMAND_DOWN
								|| ElevatorFactory.getElevator().getOutCommand() == COMMAND_FROME2S.LIFT_OUT_COMMAND_UP_DOWN)  //等待楼上货物被取走,如果车的状态回到idle,说明任务发送失败
							{
								if (tmpForkLiftWrapper.getForkLift().taskStep == TASK_STEP.TASK_IDLE) {
									break;
								}
								Console.WriteLine("wait lifter goods to be pick");
								Thread.Sleep(500);
							}

							if (upRecordStep > 0) //升降机将货运到楼上,保证upRecordStep不小于0
							{
								upRecordStep--;
								AGVLog.WriteError("上货期间 楼上货物被取走: step = " + upRecordStep, new StackFrame(true));
							}
						}
					}


					if (checkDownDeliverPeriodOver(upRecordStep))  //检测上料任务有没有结束 条件1：没有上料任务缓存  条件2：所有车子空闲 条件3：升降机上没有货物
					{
						ScheduleFactory.getSchedule().setDownDeliverPeriod ( false);
					}

					downRecordStep = 0; //下料信号置0
					Console.WriteLine("上料阶段");
				}

				if (upTaskRecord != null) {
					if (!ScheduleFactory.getSchedule().getDownDeliverPeriod() && !checkUpDeliverPeriodOver(downRecordStep)) //检查下货任务有没有结束，升降机从楼上到楼下流程走完，没有正在执行的下货任务
						{
						Console.WriteLine("当前下货任务没有执行完成，执行完后再开始执行上货任务");

					} else if (upTaskRecord.taskRecordStat == TASKSTAT_T.TASK_READY_SEND) {
						tmpForkLiftWrapper = ForkLiftWrappersService.getInstance().getForkLiftByNunber(3);  //获取楼下三号车

						if (tmpForkLiftWrapper.getForkLift().taskStep != TASK_STEP.TASK_IDLE) {
							Console.WriteLine("上料任务正在执行，等待上料任务执行完成");
							continue;
						}

						if (ElevatorFactory.getElevator().getOutCommand() != COMMAND_FROME2S.LIFT_OUT_COMMAND_MIN) //只要升降机上有货或有异常都不发送上货任务，否则容易造成楼上楼下都要货
						{
							Console.WriteLine(" 升降机楼下有货，不发送上货任务");
						} else {
							lock (LockController.getLockController().getLockForkLift())  //锁住车的状态
							{
								result = tmpForkLiftWrapper.sendTask(upTaskRecord); //发送任务
								ScheduleFactory.getSchedule().setDownDeliverPeriod(true);//上货任务发送后，才进入上料阶段

								if (result == 0) //发送成功 才正式进入上货阶段
								{
									if (upRecordStep <= 2) //避免上货 step被加得太多，不能进入下货阶段
									{
										upRecordStep += 2;
										AGVLog.WriteError("上货期间 发送任务: step = " + upRecordStep, new StackFrame(true));
									}
								}
							}
						}
					}
				}

				//检测升降机2楼有货物，发送指令将升降机送货到楼下
				//检测升降机1楼有货物，调度1楼AGV送货
				//读取升降机上料信号
				if (!ScheduleFactory.getSchedule().getDownDeliverPeriod()) {
					Console.WriteLine(" 下料阶段");
					upRecordStep = 0; //上料信号置0
					if (ElevatorFactory.getElevator().getOutCommand() == COMMAND_FROME2S.LIFT_OUT_COMMAND_DOWN)  //检测到楼上有货，发送指令到升降机运货到楼下
					{
						int times_tmp = 0;
						ElevatorFactory.getElevator().setDataCommand(COMMAND_FROMS2E.LIFT_IN_COMMAND_DOWN);
						while (ElevatorFactory.getElevator().getOutCommand() == COMMAND_FROME2S.LIFT_OUT_COMMAND_DOWN && times_tmp < 60)  //等待升降机送货到楼下
						{
							Console.WriteLine("wait lifter down goods"); //光电感应大概10S可以结束
							times_tmp++;
							Thread.Sleep(1000);
						}

						if (times_tmp < 60) {
							if (downRecordStep > 0) //升降机将货运到楼上,保证upRecordStep不小于0
							{
								downRecordStep--;
								AGVLog.WriteError("下货期间 楼上货物送到楼下: step = " + downRecordStep, new StackFrame(true));
							}
						}

						times_tmp = 0;
					}

					if (ElevatorFactory.getElevator().getOutCommand() == COMMAND_FROME2S.LIFT_OUT_COMMAND_UP ||
							ElevatorFactory.getElevator().getOutCommand() == COMMAND_FROME2S.LIFT_OUT_COMMAND_UP_DOWN) //检测到楼下有货，通知AGV来取货
					{
						TaskRecord tr_tmp = new TaskRecord();
						tmpForkLiftWrapper = ForkLiftWrappersService.getInstance().getForkLiftByNunber(3);
						tmpSingleTask = getDownPickSingleTaskOnTurn();
						tr_tmp.singleTask = tmpSingleTask;
						tr_tmp.taskRecordName = tmpSingleTask.taskName;
						if (tmpForkLiftWrapper != null && tmpForkLiftWrapper.getForkLift().taskStep == TASK_STEP.TASK_IDLE) {
							TaskReordService.getInstance().addTaskRecord(tr_tmp); //发货后，才确认添加该记录
							result = tmpForkLiftWrapper.sendTask(tr_tmp);
							if (result == 0) //任务发送成功
							{
								downPicSingleTaskkUsed++; //用于下次切换卸货点
								if (downRecordStep > 0) //升降机将货运到楼上,保证upRecordStep不小于0
								{
									downRecordStep--;
									AGVLog.WriteError("下货期间 楼下货物被取走: step = " + downRecordStep, new StackFrame(true));
								}
							} else {
								TaskReordService.getInstance().removeTaskRecord(tr_tmp.singleTask, tr_tmp.taskRecordStat);  //如果任务没发送成功，删除该条记录
							}
						}

					}

					lock (LockController.getLockController().getLockTask()) {
						foreach (TaskRecord tr in taskRecordList) {
							lock (LockController.getLockController().getLockForkLift()) {
								if (tr.taskRecordStat == TASKSTAT_T.TASK_READY_SEND) {
									tmpForkLiftWrapper = null;
									if (tr.singleTask.taskType == TASKTYPE_T.TASK_TYPE_DOWN_PICK)
										tmpForkLiftWrapper = ForkLiftWrappersService.getInstance().getForkLiftByNunber(3);
									else if (tr.singleTask.taskType == TASKTYPE_T.TASK_TYPE_UP_DILIVERY)
										tmpForkLiftWrapper = getSheduleForkLift();  //有任务执行的时候，才考虑检查车子状态
																					//if (fl.getForkLift().taskStep == TASK_STEP.TASK_IDLE && fl.finishStatus == 1)  //检查车子的状态，向空闲的车子发送任务,如果发送失败，后面会检测发送状态，
																					//并将该任务状态改成待发重新发送
									if (tmpForkLiftWrapper != null && tmpForkLiftWrapper.getForkLift().taskStep == TASK_STEP.TASK_IDLE) {
										result = tmpForkLiftWrapper.sendTask(tr); //发送任务
										if (result == -1) //任务没有发送成功会中断本次循环，防止发送任务到后面的车
										{
											break;
										}

										if (tr.singleTask.taskType == TASKTYPE_T.TASK_TYPE_UP_DILIVERY && downRecordStep < 4) //发送的是楼上送货，并且送货发送次数小于2次
										{
											downRecordStep += 2;
											AGVLog.WriteError("下货期间 发送任务: step = " + downRecordStep, new StackFrame(true));
										}

									}
								}
							}
						}
					}
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

		private TaskRecord checkUpTaskRecord() {
			foreach (TaskRecord tr in taskRecordList) {
				if (tr.singleTask.taskType == TASKTYPE_T.TASK_TYPE_DOWN_DILIVERY)  //上料任务
				{
					Console.WriteLine(" check up record = " + tr.taskRecordID + " tr stat = " + tr.taskRecordStat);

					return tr;
				}
			}

			return null;
		}


		private bool checkDownDeliverPeriodOver(int upStep) {
			foreach (ForkLiftWrapper fl in AGVCacheData.getForkLiftWrapperList()) {
				if (fl.getForkLift().isUsed == 1 && fl.getForkLift().taskStep != TASK_STEP.TASK_IDLE) //如果有车在运行
				{
					return false;
				}
			}

			if (ElevatorFactory.getElevator().getOutCommand() != 0) //升降机上有任务, 表示上料没有结束
			{
				return false;
			}

			if (upStep > 0) {
				Console.WriteLine("当前处于上货 " + upStep + " 阶段");
				AGVLog.WriteError("当前处于上货 " + upStep + " 阶段", new StackFrame(true));
				return false;
			}

			return true;
		}

		/// <summary>
		/// 检测楼上往楼下送货任务有没有结束
		/// </summary>
		/// <param name="upStep"></param>
		/// <returns></returns>
		private bool checkUpDeliverPeriodOver(int step) {
			foreach (ForkLiftWrapper fl in AGVCacheData.getForkLiftWrapperList()) {
				if (fl.getForkLift().isUsed == 1 && fl.getForkLift().taskStep != TASK_STEP.TASK_IDLE) //如果有车在运行
				{
					Console.WriteLine("fl " + fl.getForkLift().id + "is run");
					return false;
				}
			}

			if (ElevatorFactory.getElevator().getOutCommand() != 0) //升降机上有任务, 表示上料没有结束
			{
				Console.WriteLine("升降机上有货11");
				return false;
			}

			if (step > 0) {
				Console.WriteLine("当前处于下货 " + step + " 阶段");
				AGVLog.WriteError("当前处于下货 " + step + " 阶段", new StackFrame(true));
				return false;
			}

			return true;
		}

		private ForkLiftWrapper getSheduleForkLift()  //如果两辆AGV都空闲，必须选择前面一辆AGV，否则后面一辆AGV一直不能走
		{
			ForkLiftWrapper fl_1 = null;
			ForkLiftWrapper fl_2 = null;  //另一辆车
			ForkLiftWrapper forklift = null;
			int freeForkCount = 0; //空闲车辆总数
			foreach (ForkLiftWrapper fl in AGVCacheData.getForkLiftWrapperList()) {
				if (fl.getForkLift().forklift_number == 3)  //只考虑楼上的车子
					continue;

				if (fl_1 == null) {
					fl_1 = fl;
				} else {
					fl_2 = fl;
				}

				if (fl.getForkLift().isUsed == 1 && fl.getForkLift().taskStep == TASK_STEP.TASK_IDLE && fl.getForkLift().finishStatus == 1)  //如果有车子同时满足要求，选择使用优先级较高的车
				{
					fl.getPosition().updateStartPosition();  //车子执行完任务回到起始位置 这时候的起始位置才是有效的
					freeForkCount++;
					forklift = fl;                   //可用的叉车
				}
			}

			if (freeForkCount == 1) {
				if (forklift.getForkLift().id == fl_1.getForkLift().id) {
					if (!check_start_state(forklift, fl_2)) {
						return null;
					}
				} else if (forklift.getForkLift().id == fl_2.getForkLift().id) {
					if (!check_start_state(forklift, fl_1)) {
						return null;
					}
				}

			} else if (freeForkCount == 2) {
				forklift = getHighLevel_ForkLiftf(fl_1, fl_2);  //有车可选的时候，使用优先级高的车
			}

			return forklift;
		}

		/// <summary>
		/// 获取优先选择的单车
		/// </summary>
		/// <param name="fl_1"></param>
		/// <param name="fl_2"></param>
		/// <returns></returns>
		private ForkLiftWrapper getHighLevel_ForkLiftf(ForkLiftWrapper fl_1, ForkLiftWrapper fl_2) {
			if (fl_1.getPosition().getStartPx() < fl_2.getPosition().getStartPx())
				return fl_1;
			else
				return fl_2;
		}

		/// <summary>
		/// 根据顺序获取楼上取货任务
		/// </summary>
		private SingleTask getUpPickSingleTaskOnTurn() {
			int tmp = upPicSingleTaskkUsed % 2;
			int count = upPickSingleTaskList.Count;
			SingleTask st = null;
			if (tmp < count) {
				st = upPickSingleTaskList[tmp];
			}

			Console.WriteLine(" get up st = " + st.taskID + " taskName = " + st.taskName);
			return st; //轮流获取single task的值
		}

		/// <summary>
		/// 根据顺序获取楼上取货任务
		/// </summary>
		/// <returns></returns>
		private SingleTask getDownPickSingleTaskOnTurn() {
			int tmp = downPicSingleTaskkUsed % 2;
			int count = downPickSingleTaskList.Count;
			SingleTask st = null;

			if (tmp < count) {
				st = downPickSingleTaskList[tmp];
			}

			return st; //轮流获取single task的值
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="needStart">需要启动的车</param>
		/// <param name="work"></param>
		/// <returns></returns>
		private bool check_start_state(ForkLiftWrapper needStart, ForkLiftWrapper work) {
			if (work.getPosition().getArea() == 1 && work.getForkLift().isUsed == 1) //只有下货阶段可以提前启动
			{
				if (!ScheduleFactory.getSchedule().getDownDeliverPeriod() && work.getPosition().getPx() - needStart.getPosition().getPx() > 500) {
					return true;
				}

				///上货的时候直接不给启动
				Console.WriteLine("叉车 " + work.getForkLift().id + "在区域1，不能启动" + " 距离 " + (work.getPosition().getPx() - needStart.getPosition().getPx()));
				AGVLog.WriteInfo("叉车 " + work.getForkLift().id + "在区域1，不能启动" + " 距离 " + (work.getPosition().getPx() - needStart.getPosition().getPx()), new StackFrame(true));
				return false;
			}

			return true;

		}

		/// <summary>
		/// 检测是否有待发送任务
		/// </summary>
		/// <returns></returns>
		public bool checkReadySendTaskRecord() {
			foreach (TaskRecord tr in taskRecordList) {
				if (tr.taskRecordStat == TASKSTAT_T.TASK_READY_SEND)  //上料任务
				{
					Console.WriteLine(" there is task ready send");

					return true;
				}
			}

			return false;
		}

	}
}
