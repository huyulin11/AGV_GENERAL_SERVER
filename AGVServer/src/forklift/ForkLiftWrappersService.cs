
using AGV.power;
using AGV.socket;
using AGV.init;
using AGV.util;
using AGV.message;
using System;
using System.Diagnostics;
using AGV.task;
using AGV.dao;
using AGV.form;
using System.Collections.Generic;
using System.Text;
using AGV.locked;

namespace AGV.forklift {

	/// <summary>
	/// 解析获取叉车发来发来的数据包，得到叉车的运行状态
	/// </summary>
	public class ForkLiftWrappersService : IForkLiftWrappersService {
		private static ForkLiftWrappersService forkLiftWrappersService = null;

		private List<ForkLiftWrapper> forkLiftWrapperList = AGVCacheData.getForkLiftWrapperList();
		private string lastMsg = string.Empty;
		private int setCtrlTimes = 0;
		private static int pauseSetTime_f1 = 0;
		private static int pauseSetTime_f2 = 0;

		public static IForkLiftWrappersService getInstance() {
			if (forkLiftWrappersService == null) {
				forkLiftWrappersService = new ForkLiftWrappersService();
			}
			return forkLiftWrappersService;
		}

		/// <summary>
		/// 与叉车连接
		/// </summary>
		public void connectForks() {
			foreach (ForkLiftWrapper fl in forkLiftWrapperList) {
				try {
					fl.setAGVSocketClient(new AGVSocketClient());
					fl.getAGVSocketClient().registerRecvMessageCallback(handleForkLiftMsg);
					fl.getAGVSocketClient().registerReconnectCallback(flReconnect);
					fl.getAGVSocketClient().TcpConnect(fl.getForkLift().ip, fl.getForkLift().port);
					fl.getAGVSocketClient().startRecvMsg();   //开始接收数据
				} catch (Exception ex) {
					fl.getAGVSocketClient().setRecvFlag(true);  //设置重启线程标志
					AGVLog.WriteError("connect ip: " + fl.getForkLift().ip + "port: " + fl.getForkLift().port + "fail", new StackFrame(true));
				}
			}
		}

		/// <summary>
		/// 根据编号，查找对应的单车
		/// </summary>
		public ForkLiftWrapper getForkLiftByNunber(int number) {
			foreach (ForkLiftWrapper fl in forkLiftWrapperList) {
				if (fl.getForkLift().forklift_number == number)
					return fl;
			}

			return null;
		}

		private void flReconnect(ForkLiftWrapper forkLiftWrapper, bool status) {
			forkLiftWrapper.getAGVSocketClient().setRecvFlag(status);
			if (status)  //如果接收成功
			{
				forkLiftWrapper.getAGVSocketClient().startRecvMsg();  //重新启动接收程序线程
			} else {
				AGVLog.WriteError(forkLiftWrapper.getForkLift().forklift_number + "号车 连接错误", new StackFrame(true));
				AGVMessage message = new AGVMessage();
				message.setMessageType(AGVMessageHandler_TYPE_T.AGVMessageHandler_NET_ERR);
				message.setMessageStr(forkLiftWrapper.getForkLift().forklift_number + "号车 连接错误");

				AGVMessageHandler.getMessageHandler().setMessage(message);
			}

		}
		/*处理车子反馈报文 msg格式cmd=position;battery=%d;error=%d;x=%d;y=%d;a=%f;z=%d;
        speed=%d;task=%s;veer_angle=%f;
        task_step=%d;task_isfinished=%d;task_error=%d;walk_path_id=%d */
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id">表示车子</param>
		/// <param name="msg"></param>
		/// <returns></returns>
		private void handleForkLiftMsg(int id, byte[] buffer, int length) {
			int pos = -1;
			int pos_e = -1;
			int pos_t = -1;
			string taskName = "";
			int x = 0; //车子横坐标
			int y = 0; //车子纵坐标
			int pause_stat = -1;  //默认是错误状态
			int battery_soc = -1;
			int finish_stat = -1;
			int gAlarm = 1;  //AGV防撞信号 默认1 表示没有报警

			string msg = parseForkLiftMsg(buffer, length);
			if (string.IsNullOrEmpty(msg)) {
				AGVLog.WriteError("msg is null", new StackFrame(true));
				return;
			}

			if (msg.Equals(lastMsg)) {
				//Console.WriteLine(" same msg, do not handle");
				return;
			}
			//Console.WriteLine("msg = " + msg);
			//解析taskName
			try {
				//if (id == 2)
				// AGVLog.WriteError(msg, new StackFrame(true));

				pos_t = msg.IndexOf("task=");

				if (pos_t != -1) {
					pos_e = msg.Substring(pos_t, msg.Length - pos_t).IndexOf(";");
					if (pos_e != -1) {
						taskName = msg.Substring(pos_t + 5, pos_e - 5);
						//AGVLog.WriteInfo("forklift taskName = " + taskName, new StackFrame(true));
						//Console.WriteLine("forklift taskName = " + taskName);
					}
				}

				if (string.IsNullOrEmpty(taskName)) {
					//AGVLog.WriteError("forklift taskName is null", new StackFrame(true));
					//Console.WriteLine("msg format err: taskName is null");
					//return ;  //主要判断车的finished状态
				}

				//解析坐标位置 x,y
				pos_t = msg.IndexOf(";x=");
				if (pos_t != -1) {
					pos_e = msg.Substring(pos_t + 1, msg.Length - pos_t - 1).IndexOf(";");
					if (pos_e != -1) {
						// Console.WriteLine("x = " + msg.Substring(pos_t + 3, pos_e - 2) + " id = " + id);
						x = int.Parse(msg.Substring(pos_t + 3, pos_e - 2));
					}
				}

				pos_t = msg.IndexOf(";y=");
				if (pos_t != -1) {
					pos_e = msg.Substring(pos_t + 1, msg.Length - pos_t - 1).IndexOf(";");
					if (pos_e != -1) {
						// Console.WriteLine("y = " + msg.Substring(pos_t + 3, pos_e - 2));
						y = int.Parse(msg.Substring(pos_t + 3, pos_e - 2));
					}
				}

				pos_t = msg.IndexOf("pause_stat=");
				if (pos_t != -1) {
					pos_e = msg.Substring(pos_t, msg.Length - pos_t).IndexOf(";");
					pause_stat = int.Parse(msg.Substring(pos_t + 11, pos_e - 11));
				}

				pos_t = msg.IndexOf("gAlarm=");
				if (pos_t != -1) {
					pos_e = msg.Substring(pos_t, msg.Length - pos_t).IndexOf(";");
					gAlarm = int.Parse(msg.Substring(pos_t + 7, pos_e - 7));
				}

				pos_t = msg.IndexOf("battery=");
				if (pos_t != -1) //获取电池数据
				{
					pos_e = msg.Substring(pos_t, msg.Length - pos_t).IndexOf(";");
					battery_soc = int.Parse(msg.Substring(pos_t + 8, pos_e - 8));
					//Console.WriteLine("battery = " + battery_soc);
				}

				pos = msg.IndexOf("task_isfinished=");
				finish_stat = Convert.ToInt16(msg[pos + 16]) - 48;  //转的对象是单个字符 0会转成48
			} catch (Exception ex) {
				Console.WriteLine(ex.ToString());
				Console.WriteLine("接收 数据异常");
				return;
			}

			if (!checkForkLiftMsg(battery_soc, pause_stat, finish_stat, gAlarm)) {
				//Console.WriteLine("接收数据异常");
				return;
			}

			lock (LockController.getLockController().getLockTask()) {
				if (pos != -1) //成功匹配到状态
				{
					foreach (ForkLiftWrapper fl in forkLiftWrapperList) {
						if (fl.getForkLift().id == id) {
							if (x != 0 && y != 0) {
								fl.setPosition(x, y);
							}

							if (id == 1 && pauseSetTime_f1 == 0) {
								fl.getForkLift().shedulePause = pause_stat;  //只在启动的时候设置一次
								pauseSetTime_f1 = 1;
								fl.getPosition().calcPositionArea();
							} else if (id == 2 && pauseSetTime_f2 == 0) {
								fl.getForkLift().shedulePause = pause_stat;  //只在启动的时候设置一次
								pauseSetTime_f2 = 1;
								fl.getPosition().calcPositionArea();
							}

							fl.getForkLift().pauseStat = pause_stat;
							if (pause_stat >= 0)
								checkForkliftPauseStat(fl, pause_stat);

							fl.getForkLift().finishStatus = finish_stat;
							fl.getBatteryInfo().setBatterySoc(battery_soc);
							fl.updateAlarm(gAlarm);

							AGVLog.WriteInfo("forklift id " + id + "taskName = " + taskName + "forklift stat = " + fl.getForkLift().finishStatus, new StackFrame(true));
							{
								bool stat = checkTaskSendStat(fl, taskName);

								if (stat == false) {
									Console.WriteLine("任务列表中不能匹配正确状态的任务");
									AGVLog.WriteError("任务列表中不能匹配正确状态的任务", new StackFrame(true));
								}
							}
						}
					}
				}
			}
			return;
		}

		/// <summary>
		/// 解析车子反馈报文
		/// </summary>
		/// <param name="buffer">第一个字节为0 最后一个字节为0xff，如果不满足此格式，丢弃该报文</param>
		/// <returns>解析得到的字符串</returns>
		private string parseForkLiftMsg(byte[] buffer, int length) {
			if (buffer.Length == 0) {
				AGVLog.WriteError("forklift buffer length is 0", new StackFrame(true));
				Console.WriteLine("forklift buffer length is 0");
				return null;
			}

			// if (buffer[0] != 0 || buffer[length - 1] != 0xff)  //第一个byte为0 最后一个为0xff
			//{
			//   AGVLog.WriteError("forklift buffer is not match buffer[0] = " + Convert.ToInt16(buffer[0]) + "buffer[length - 1] = " + Convert.ToInt16(buffer[length - 1]), new StackFrame(true));
			// return null;
			//}

			return Encoding.ASCII.GetString(buffer, 1, buffer.Length - 2);
		}

		/// <summary>
		/// 解析车子反馈报文
		/// </summary>
		/// <param name="buffer">第一个字节为0 最后一个字节为0xff，如果不满足此格式，丢弃该报文</param>
		/// <returns>解析得到的字符串</returns>
		private bool checkForkLiftMsg(int battery_soc, int pause_stat, int finished_stat, int gAlarm) {
			if (battery_soc < 0 || pause_stat < 0 || finished_stat < 0 || gAlarm < 0)
				return false;

			return true;
		}

		private void checkForkliftPauseStat(ForkLiftWrapper fl, int pause_stat) {
			if (fl.getForkLift().shedulePause != pause_stat)  //车的暂停状态与返回值不一样，说明没有发送成功
			{
				setCtrlTimes++;
				if (setCtrlTimes > 10) {
					//setForkCtrl(fl, fl.isPaused);  //重新发送车子暂停状态 
					//报警，需要人工处理
					setCtrlTimes = 0;
					AGVMessage message = new AGVMessage();
					message.setMessageType(AGVMessageHandler_TYPE_T.AGVMessageHandler_SENDPAUSE_ERR);
					message.setMessageStr("检测中断错误");

					//AGVInitialize.getInitialize().getAGVMessageHandler().setMessage(message);
				}
			}
		}

		/// <summary>
		/// 检查任务状态
		/// </summary>
		/// <param name="fl">车子实例</param>
		/// <param name="taskName"></param>
		/// <returns></returns>
		private bool checkTaskSendStat(ForkLiftWrapper fl, string taskName) {
			bool stat = true;
			//Console.WriteLine(" fl id " + fl.getForkLift().id + " finishStatus = " + fl.finishStatus);
			if (fl.getForkLift().finishStatus == 1) //车子状态空闲有两种可能1：车子任务执行完成 2：任务在执行中 报文没有及时反馈
			{
				foreach (TaskRecord tr in TaskReordService.getInstance().getTaskRecordList()) {
					//if (tr.forkLift != null && tr.taskRecordName.Equals(taskName))
					if (tr.forkLiftWrapper != null && tr.forkLiftWrapper.getForkLift().id == fl.getForkLift().id)  //可能存在任务没发送成功，反馈的taskName与现在taskrecord的名称不一样
					{
						if (tr.taskRecordStat == TASKSTAT_T.TASK_SEND) {
							if (fl.getForkLift().waitTimes > 15) //等待次数超过15次，后面重新发送该任务
							{
								Console.WriteLine("send task: " + taskName + "to " + fl.getForkLift().forklift_number + "fail");
								fl.getForkLift().waitTimes = 0;
								tr.forkLiftWrapper = null;
								fl.getForkLift().taskStep = TASK_STEP.TASK_IDLE;  //车子状态改为空闲
								fl.getForkLift().currentTask = "";
								DBDao.getDao().updateForkLift(fl);  //赋值空字符串
								if (tr.singleTask.taskType == TASKTYPE_T.TASK_TYPE_UP_PICK) {

									DBDao.getDao().RemoveTaskRecord(tr);
									AGVLog.WriteWarn("forklift number: " + fl.getForkLift().forklift_number + " taskName: " + taskName + " 发送失败 移除任务", new StackFrame(true));
								} else {
									tr.taskRecordStat = TASKSTAT_T.TASK_READY_SEND; //改变任务的状态，后面重新发送
									tr.singleTask.taskStat = TASKSTAT_T.TASK_READY_SEND;
									DBDao.getDao().UpdateTaskRecord(tr);
									AGVLog.WriteWarn("forklift number: " + fl.getForkLift().forklift_number + " taskName: " + taskName + " 发送失败 更新任务状态，等待重新发送", new StackFrame(true));
								}
								FormController.getFormController().getMainFrm().updateFrm(); //设置更新界面
							} else {
								fl.getForkLift().waitTimes++;
								Console.WriteLine("fl: " + fl.getForkLift().forklift_number + "taskName: " + taskName + "waittimes: " + fl.getForkLift().waitTimes);
								AGVLog.WriteWarn("forklift number: " + fl.getForkLift().forklift_number + " taskName: " + taskName + " waittimes: " + fl.getForkLift().waitTimes, new StackFrame(true));
							}
							break;
						} else if (tr.taskRecordStat == TASKSTAT_T.TASK_SEND_SUCCESS) //确保没有重复进入，否则会插入多条备份记录
						  {
							Console.WriteLine("task: " + taskName + "in " + fl.getForkLift().forklift_number + "finished");
							AGVLog.WriteInfo("taskName: " + taskName + "in " + fl.getForkLift().forklift_number + " finished", new StackFrame(true));
							DBDao.getDao().RemoveTaskRecord(tr);  //移除record是3的记录
							DBDao.getDao().InsertTaskRecordBak(tr);
							tr.singleTask.taskStat = TASKSTAT_T.TASK_END;
							tr.taskRecordStat = TASKSTAT_T.TASK_END;
							FormController.getFormController().getMainFrm().updateFrm(); //设置更新界面 //设置更新界面
							fl.getForkLift().taskStep = TASK_STEP.TASK_IDLE;
							fl.getForkLift().currentTask = "";
							DBDao.getDao().updateForkLift(fl);
							break;
						} else if (tr.taskRecordStat == TASKSTAT_T.TASK_END) {
							//break;  继续任务状态没及时删除,继续循环
						}

						break; //每次只匹配一条记录，可能存在两条记录，对应singleTask一样，一条正在运行，一条待发送，适应一键添加功能
					}

				}
			} else if (fl.getForkLift().finishStatus == 0) {
				//bool storeTask = true; //是否需要缓存该任务
				foreach (TaskRecord tr in TaskReordService.getInstance().getTaskRecordList()) {
					//Console.WriteLine(" tr stat = " + tr.taskRecordStat + " taskName = " + tr.taskRecordName);
					if (tr.forkLiftWrapper != null && tr.forkLiftWrapper.getForkLift().id == fl.getForkLift().id) //任务列表中匹配到非待发送的该任务则不缓存  
					{
						if (tr.taskRecordStat == TASKSTAT_T.TASK_SEND) {
							tr.singleTask.taskStat = TASKSTAT_T.TASK_SEND_SUCCESS;
							tr.taskRecordStat = TASKSTAT_T.TASK_SEND_SUCCESS;
							DBDao.getDao().UpdateTaskRecord(tr);
							stat = true;
						} else if (tr.taskRecordStat == TASKSTAT_T.TASK_SEND_SUCCESS && tr.forkLiftWrapper.getForkLift().id == fl.getForkLift().id) {
						}

						fl.getForkLift().waitTimes = 0; //发送任务，等待确认是否发送成功
						fl.getForkLift().taskStep = TASK_STEP.TASK_EXCUTE;
						fl.getForkLift().currentTask = tr.singleTask.taskText;
						break; //每次只匹配一条记录，可能存在两条记录，对应singleTask一样，一条正在运行，一条待发送，适应一键添加功能
					}
				}
				/*
								if (storeTask)  //系统启动后，车子可能正在执行任务起来，将正在执行的任务缓存
								{
									TaskRecord tr = new TaskRecord();
									tr.taskRecordName = taskName;
									tr.forkLift = fl;
									tr.taskRecordStat = TASKSTAT_T.TASK_SEND_SUCCESS; //状态已经发送成功
									tr.setSingleTaskByTaskName(AGVUtil.parseTaskRecordName(taskName));
									fl.getForkLift().taskStep = TASK_STEP.TASK_EXCUTE;
									addTs = tr;
									//AGVInitialize.getInitialize().getMainFrm().updateCurrentTask(st.taskName); //更新界面上的当前任务
									Console.WriteLine("store task: " + tr.taskRecordName + "taskNumber:" + taskRecordList.Count);
									AGVLog.WriteInfo("store task: " + tr.taskRecordName + "at boot task count: " + taskRecordList.Count, new StackFrame(true));
								}
				*/
			} else {
				Console.WriteLine("fork status err");
				AGVLog.WriteError("fork lift staus: " + fl.getForkLift().finishStatus + "err", new StackFrame(true));
			}
			return stat;
		}


	}
}
