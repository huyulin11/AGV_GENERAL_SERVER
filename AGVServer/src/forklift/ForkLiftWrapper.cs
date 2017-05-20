
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

namespace AGV.forklift {
	public class ForkLiftWrapper  //描述一个车子
	{
		private BatteryInfo batteryInfo = new BatteryInfo(); //车子电池信息

		private ForkLiftItem forkLift = null;

		private AGVSocketClient tcpClient = null;  //与服务器建立的TCP连接

		private Position position = new Position();

		public ForkLiftWrapper() {
			forkLift = new ForkLiftItem();
		}

		public ForkLiftWrapper(ForkLiftItem forkLift) {
			this.forkLift = forkLift;
		}

		public ForkLiftItem getForkLift() {
			return forkLift;
		}

		public void setForkLift(ForkLiftItem forkLift) {
			this.forkLift = forkLift;
		}

		public Position getPosition() {
			return position;
		}

		public void setPosition(Position position) {
			this.position = position;
		}

		public void setPosition(int x, int y) {
			if (position == null)
				position = new Position();
			position.setPx(x);
			position.setPy(y);
		}

		public AGVSocketClient getAGVSocketClient() {
			return tcpClient;
		}

		public void setAGVSocketClient(AGVSocketClient tcpClient) {
			this.tcpClient = tcpClient;
			tcpClient.setForkLiftWrapper(this);
		}

		public BatteryInfo getBatteryInfo() {
			return batteryInfo;
		}

		public string getPauseStr() {
			return forkLift.pauseStat == 1 ? "暂停" : "运行";
		}

		public void updateAlarm(int alarm) {
			Console.WriteLine(forkLift.forklift_number + "号车 alarm = " + alarm +
				"gAlarm =" + forkLift.gAlarm);
			if (alarm == 0) {
				forkLift.gAlarm++;
			} else if (alarm == 1) {
				forkLift.gAlarm = 0;
			}
			Console.WriteLine(forkLift.forklift_number + "号车 alarm = " + alarm + "gAlarm =" + forkLift.gAlarm);

			if (forkLift.gAlarm > AGVConstant.AGVALARM_TIME)  //防撞信号 检测超过12次，弹出报警提示
			{
				string msg = forkLift.forklift_number + "触发防撞，暂停所有AGV";
				AGVLog.WriteError(msg,new StackFrame(true));
				AGVMessage message = new AGVMessage();
				message.setMessageType(AGVMessageHandler_TYPE_T.AGVMessageHandler_AGV_ALARM);
				message.setMessageStr(msg);
				TaskexeDao.getDao().InsertTaskexePause(msg);
				forkLift.gAlarm = 0;
				AGVMessageHandler.getMessageHandler().setMessage(message);
			}
		}

		/// <summary>
		/// 发送任务
		/// </summary>
		public int sendTask(TaskRecord tr) {
			int result = 0;
			Console.WriteLine("ready to send task: " + tr.singleTask.taskName + "forklist stat:" + getForkLift().taskStep + "forklift finished:" + getForkLift().finishStatus);

			string cmd = "cmd=set task by name;name=" + tr.taskRecordName; //发送命令格式，如果有多个对应值用;隔开，如果后面没有命令了，不需要再加;号
			Console.WriteLine("send msg :" + cmd + "to " + getForkLift().forklift_number);

			lock (getAGVSocketClient().clientLock) {
				try {
					getAGVSocketClient().SendMessage(cmd);  //确保发送成功

					tr.taskRecordStat = TASKSTAT_T.TASK_SEND;
					tr.singleTask.taskStat = TASKSTAT_T.TASK_SEND;
					FormController.getFormController().getMainFrm().updateFrm(); //设置更新界面
					tr.forkLiftWrapper = this;
					getForkLift().taskStep = TASK_STEP.TASK_SENDED;
					getForkLift().currentTask = tr.singleTask.taskText;
					DBDao.getDao().UpdateTaskRecord(tr);
					DBDao.getDao().updateForkLift(this);  //更新车子状态
				} catch (Exception ex) {
					Console.WriteLine(ex.ToString());
					AGVLog.WriteError("发送" + tr.singleTask.taskText +
						" 任务到" + getForkLift().forklift_number + "号车 失败",
						new StackFrame(true));
					result = -1;
				}
				AGVLog.WriteError("发送" + tr.singleTask.taskText +
					" 任务到" + getForkLift().forklift_number + "号车 成功",
					new StackFrame(true));
				return result;
			}
		}
	}
}
