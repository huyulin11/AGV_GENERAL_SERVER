using System;
using System.Collections.Generic;

using System.Windows.Forms;
using System.Diagnostics;

using System.Runtime.InteropServices;
using AGV.util;
using AGV.schedule;
using AGV.dao;
using AGV.task;
using AGV.forklift;
using AGV.elevator;
using AGV.message;
using AGV.form;
using AGV.socket;

namespace AGV.init {

	/// <summary>
	/// 初始化单例，控制系统运行
	/// </summary>
	public class AGVEngine {
		#region 导入设备打开接口 JG_OpenUSBAlarmLamp
		/// <summary>
		/// 功能：打开设备
		/// 说明：如果当前设备在自检过程中(上电时发生)，该命令无效，返回值为0
		/// </summary>
		/// <param name="ulDVID">IN,保留参数，代入0</param>
		/// <returns>1打开成功，0打开失败</returns>
		[DllImport("jg_usbAlarmLamp.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 JG_OpenUSBAlarmLamp(Int32 ulDVID);
		#endregion

		private static AGVEngine engine = null;
		public static string AGVCONFIG_PATH = "config/AGVConfig.xml";
		
		private User currentUser = null;
		
		private bool useUsbAlarm = true;  //默认使用usb报警灯，如果打开usb报警灯失败，则切换到电脑声音报警

		public String env_err_type_text(ENV_ERR_TYPE err) {
			string err_text = "";
			switch (err) {
				case ENV_ERR_TYPE.ENV_ERR_OK:
					err_text = "OK";
					break;
				case ENV_ERR_TYPE.ENV_LIFT_COM_ERR:
					err_text = "请检查升降机串口";
					break;
				case ENV_ERR_TYPE.ENV_CACHE_TASKRECORD_WARNING:
					err_text = "检测到有未完成的任务，是否保存";
					break;
				case ENV_ERR_TYPE.ENV_CACHE_UPTASKRECORD_WARNING:
					err_text = "检测到有未完成的上货，是否保存";
					break;
				default:
					Console.WriteLine("没有找到对应的错误");
					break;
			}

			return err_text;
		}

		public static AGVEngine getInstance() {
			if (engine == null) {
				engine = new AGVEngine();
			}
			return engine;
		}

		public bool getUseUsbAlarm() {
			return useUsbAlarm;
		}

		public void setCurrentUser(User user) {
			this.currentUser = user;
		}

		public User getCurrentUser() {
			return currentUser;
		}
		
		/// <summary>
		/// 可能有缓存任务，刚启动时，车子的状态需要设置
		/// </summary>
		private void setForkliftStateFirst() {
			foreach (ForkLiftWrapper fl in AGVCacheData.getForkLiftWrapperList()) {
				List<TaskRecord> taskRecordList = TaskReordService.getInstance().getTaskRecordList();
				foreach (TaskRecord tr in taskRecordList) {
					if (tr.taskRecordStat == TASKSTAT_T.TASK_SEND || tr.taskRecordStat == TASKSTAT_T.TASK_SEND_SUCCESS) {
						if (tr.forkLiftWrapper != null && tr.forkLiftWrapper.getForkLift().forklift_number == fl.getForkLift().forklift_number) {
							fl.getForkLift().taskStep = TASK_STEP.TASK_EXCUTE;
						}
					}
				}
			}
		}

		private AGVEngine() { }

		/// <summary>
		/// 开机判断有没有缓存的上货任务，如果有，设置当前阶段为上货
		/// </summary>
		/// <param name="trList"></param>
		/// <returns></returns>
		private bool checkCurrentPeriod(List<TaskRecord> trList) {
			foreach (TaskRecord tr in trList) {
				if (tr.singleTask.taskType == TASKTYPE_T.TASK_TYPE_UP_PICK || tr.singleTask.taskType == TASKTYPE_T.TASK_TYPE_DOWN_DILIVERY)  //上料任务
				{
					return true;
				}
			}

			return false;
		}

		private ENV_ERR_TYPE checkCacheTaskRecoreds() {
			ENV_ERR_TYPE err = ENV_ERR_TYPE.ENV_ERR_OK;
			List<TaskRecord> taskRecordList = new List<TaskRecord>();
			taskRecordList = TaskReordService.getInstance().getTaskRecordList();
			if (checkCurrentPeriod(taskRecordList)) {
				err = ENV_ERR_TYPE.ENV_CACHE_UPTASKRECORD_WARNING;
				return err;
			}

			if (taskRecordList.Count > 0) {
				err = ENV_ERR_TYPE.ENV_CACHE_TASKRECORD_WARNING;
			}

			return err;
		}

		/// <summary>
		///检查升降串口，必要条件，升降机串口不能用，主程序不能运行
		/// </summary>
		private ENV_ERR_TYPE checkRunning() {
			ENV_ERR_TYPE err = ENV_ERR_TYPE.ENV_ERR_OK;
			if (ElevatorFactory.getElevator().getStat() == false) {
				err = ENV_ERR_TYPE.ENV_LIFT_COM_ERR;
				return err;
			}

			err = checkCacheTaskRecoreds();

			///检查3号车是否可用，备选条件，如果不可用，是否存在其它上货方式

			///检测USB报警灯，如果USB报警灯有问题，切换到电脑声音报警
			try {
				if (JG_OpenUSBAlarmLamp(0) == 0) {
					MessageBox.Show("打开报警灯失败，切换到电脑声音报警");
					useUsbAlarm = false;
				}
			} catch (Exception ex) {
				MessageBox.Show("打开报警灯失败，切换到电脑声音报警");
				useUsbAlarm = false;
			}
			return err;
		}

		private void handleCheckRunning(ENV_ERR_TYPE err) {
			if (err == ENV_ERR_TYPE.ENV_LIFT_COM_ERR) {
				DialogResult dr;
				dr = MessageBox.Show(env_err_type_text(err), "错误提示", MessageBoxButtons.OK);

				if (dr == DialogResult.OK) {
					Console.WriteLine(" exit ");
					Environment.Exit(0);
				}
			} else if (err == ENV_ERR_TYPE.ENV_CACHE_TASKRECORD_WARNING) {
				DialogResult dr;
				dr = MessageBox.Show(env_err_type_text(err), "检测到缓存任务", MessageBoxButtons.YesNo);

				if (dr == DialogResult.Yes) {
					Console.WriteLine(" do nothing ");
				} else if (dr == DialogResult.No) {
					TaskReordService.getInstance().deleteAllTaskRecord();
				}
			} else if (err == ENV_ERR_TYPE.ENV_CACHE_UPTASKRECORD_WARNING) {
				DialogResult dr;
				dr = MessageBox.Show(env_err_type_text(err), "缓存任务", MessageBoxButtons.YesNo);

				if (dr == DialogResult.Yes) {
					ScheduleFactory.getSchedule().setDownDeliverPeriod(true);  //设置当前处于上货阶段
				} else if (dr == DialogResult.No) {
					TaskReordService.getInstance().deleteAllTaskRecord();
				}
			}
		}
		public void agvInit() {

			FormController.getFormController().getLoginFrm().ShowDialog();

			AGVLog.WriteInfo("程序启动", new StackFrame(true));
			setForkliftStateFirst();

			handleCheckRunning(checkRunning());

			ElevatorFactory.getElevator().startReadSerialPortThread();
			AGVSocketServer.getSocketServer().StartAccept();
			ScheduleFactory.getSchedule().startShedule();
			AGVMessageHandler.getMessageHandler().StartHandleMessage();

			FormController.getFormController().getMainFrm().ShowDialog();
		}
	}
}
