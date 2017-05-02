using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using AGV.schedule;
using AGV.task;
using AGV.init;
using AGV.forklift;
using AGV.message;
using AGV.dao;
using AGV.util;
using AGV.tools;
using AGV.elevator;
using AGV.sys;
using AGV.locked;

namespace AGV.form {
	public partial class MainFrm : Form {
		ContextMenu addTasktMenu = new ContextMenu();
		ContextMenu cancelTasktMenu = new ContextMenu();
		private TaskButton selectedButton = null;  //被点击的button
		private bool isUpdateFrm = false;
		private bool isStop = false;
		List<AGVPanel> agvPanelList = new List<AGVPanel>();
		bool lowpowerBattery = false; //低电量，需要做相应的提示
		public MainFrm() {
			InitializeComponent();
			initMainFrm();
		}

		private void initAgvPanel() {
			int tmp = 0;
			foreach (ForkLiftWrapper fl in AGVCacheData.getForkLiftWrapperList()) {
				AGVPanel ap_s = new AGVPanel();
				Console.WriteLine(" fl id " + fl.getForkLift().id);
				ap_s.initPanel(fl);
				ap_s.Location = new Point(780 + tmp, 40);
				ap_s.Size = new Size(120, 200);
				this.agvPanel.Controls.Add(ap_s);
				tmp += 300;

				ap_s.AutoSize = true;
				ap_s.Anchor = AnchorStyles.Right & AnchorStyles.Left;
				Console.WriteLine(" init agv panel number = " + fl.getForkLift().forklift_number);
				agvPanelList.Add(ap_s);
			}
		}

		private void initUserPanel() {
			User user = AGVEngine.getInstance().getCurrentUser();
			if (user == null) {
				this.userLabel.Text = "无";
			} else {
				this.userLabel.Text = user.userName;
			}
		}

		private void initPanel() {
			foreach (SingleTask st in singleTaskList) {
				TaskButton button = new TaskButton();
				if (st.taskType == TASKTYPE_T.TASK_TYPE_UP_DILIVERY || st.taskType == TASKTYPE_T.TASK_TYPE_UP_PICK) {
					if (st.taskType == TASKTYPE_T.TASK_TYPE_UP_DILIVERY)
						button.MouseDown += taskButtonMouseDown;  //卸货1、2界面不能发送，需要到配置中单独发送
					button.Name = st.taskName;
					button.Text = st.taskText;
					Console.WriteLine(" button name = " + button.Text);
					upButtonList.Add(button);
					upPanel.Controls.Add(button);
				} else if (st.taskType == TASKTYPE_T.TASK_TYPE_DOWN_DILIVERY || st.taskType == TASKTYPE_T.TASK_TYPE_DOWN_PICK) {
					button.Name = st.taskName;
					button.Text = st.taskText;
					downButtonList.Add(button);
					downPanel.Controls.Add(button);
				}

				button.setSingleTask(st);
				buttonStHash.Add(st.taskID, button);
			}

			addAllTaskRecordButton.Location = new Point(1150, 70);
			addAllTaskRecordButton.Size = new Size(120, 40);
			addAllTaskRecordButton.Click += addAllTaskRecordButton_Click;
			upPanel.Controls.Add(addAllTaskRecordButton);

			this.systemPauseButton.Name = "pause";
			this.systemPauseButton.Text = "系统暂停";
			systemPauseButton.Location = new Point(90, 100);
			systemPauseButton.Size = new Size(120, 60);
			systemPauseButton.Click += systemPauseButton_Click;
			systemPauseButton.BackColor = Color.DarkGray;
			systemPauseButton.Font = new Font("SimSun", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			controlPanel.Controls.Add(systemPauseButton);
			initUserPanel();  //最底下的panel
			initAgvPanel(); //初始化单车信息的Panel
		}

		private void updatePanel() {
			foreach (SingleTask st in singleTaskList) {
				TaskButton button = (TaskButton)buttonStHash[st.taskID];
				if (st.taskStat == TASKSTAT_T.TASK_READY_SEND || st.taskStat == TASKSTAT_T.TASK_END) //待可以发送状态，可以取消该任务
				{
					button.BackColor = Color.LightGray;
				} else if (st.taskStat == TASKSTAT_T.TASK_SEND || st.taskStat == TASKSTAT_T.TASK_SEND_SUCCESS) {
					button.BackColor = Color.Green;
				}
			}
		}

		private bool checkDoubleTaskRecord(int taskID)  //检查是是否缓存了相对应的偶数任务,偶数不是指ID，是指任务号
		{
			bool exist = false;
			List<TaskRecord> tsList = TaskReordService.getInstance().getReadySendTaskRecordList(taskID);
			if (tsList.Count > 0) {
				exist = true;
			}

			return exist;
		}

		private void palletItemClick(object sender, EventArgs e) {
			MenuItem item = (MenuItem)sender;
			Console.WriteLine("select button name = " + selectedButton.Name);
			TaskRecord tr = (TaskRecord)selectedButton.getBindValue();
			if (tr == null) {
				int number = 0;
				try {
					if (selectedButton.st.taskText.Length == 2) {
						number = int.Parse(selectedButton.st.taskText.Substring(0, 1));
					} else if (selectedButton.st.taskText.Length == 3) {
						number = int.Parse(selectedButton.st.taskText.Substring(0, 2));
					}

				} catch (Exception ex) {
					Console.WriteLine("parse task nunber err");
				}

				if (number % 2 == 1) //当前添加的是奇数任务，1，3，5，7，9，11号任务
				{
					bool exist = checkDoubleTaskRecord(selectedButton.st.taskID + 1);
					if (exist) {
						DialogResult dr;
						dr = MessageBox.Show(number + 1 + "号任务已经缓存，该任务不能被添加", "任务添加提示", MessageBoxButtons.OK);  //有偶数号任务被添加时，前面的奇数号任务不能被添加，否则AGV会撞到前面的货

						if (dr == DialogResult.OK) {
							Console.WriteLine(" palletItemClick invalid ");
							return;
						}
					}
				}
				selectedButton.BackColor = Color.LightGray;
				tr = new TaskRecord();
				tr.singleTask = selectedButton.st;
				tr.taskRecordName = selectedButton.st.taskName;
				tr.taskRecordStat = TASKSTAT_T.TASK_READY_SEND;
				TaskReordService.getInstance().addTaskRecord(tr);
				selectedButton.bindValue(tr);
			}
		}

		private void cancelItemClick(object sender, EventArgs e) {
			MenuItem item = (MenuItem)sender;
			TaskRecord tr = (TaskRecord)selectedButton.getBindValue();
			if (tr != null && tr.taskRecordStat == TASKSTAT_T.TASK_READY_SEND) {
				int number = 0;
				try {
					if (selectedButton.st.taskText.Length == 2) {
						number = int.Parse(selectedButton.st.taskText.Substring(0, 1));
					} else if (selectedButton.st.taskText.Length == 3) {
						number = int.Parse(selectedButton.st.taskText.Substring(0, 2));
					}
				} catch (Exception ex) {
					Console.WriteLine("parse task nunber err");
				}

				if (number % 2 == 1) //当前添加的是奇数任务，1，3，5，7，9，11号任务
				{
					bool exist = checkDoubleTaskRecord(selectedButton.st.taskID + 1);
					if (exist) {
						DialogResult dr;
						dr = MessageBox.Show(number + 1 + "号任务已经缓存，确定要取消该任务?", "任务添加提示", MessageBoxButtons.YesNo);  //有偶数号任务被添加时，取消前面的任务，如果前面的货没有被移走，AGV会撞到前面的货

						if (dr == DialogResult.No) {
							Console.WriteLine(" cancelItemClick cancel ");
							return;
						}
					}
				}
				selectedButton.BackColor = Color.White;
				TaskReordService.getInstance().removeTaskRecord(tr.singleTask, TASKSTAT_T.TASK_READY_SEND);
				selectedButton.bindValue(null);
			}
			/*
				if (item.Name.Equals("cancelTaskItem"))
				{

					st = (SingleTask)this.SingleTaskDTG.CurrentCell.Value;
				}
				if (st != null)
				{
					st.taskStat = TASKSTAT_T.TASK_NOT_ASSIGN;
					this.SingleTaskDTG.CurrentCell.Style.BackColor = Color.White;
					AGVInitialize.getInitialize().getSchedule().removeTaskRecord(st, TASKSTAT_T.TASK_READY_SEND);
				}*/
		}

		/// <summary>
		/// 检测是否缓存了该货物前面的任务，如果有，需要取消前面一个任务再置顶，否则会出现前面的货物没叉玩，直接叉了后面的货物
		/// </summary>
		/// <param name="taskRecord"></param>
		/// <returns></returns>
		private bool checkTopFunc(TaskRecord taskRecord) {
			bool ret = true;
			Console.WriteLine("taskRecord.singleTask.taskID = " + taskRecord.singleTask.taskID);
			if (taskRecord.singleTask.taskID % 2 == 1) //表示里面一排货物, 注意是taskID
			{
				List<TaskRecord> trList = TaskReordService.getInstance().getTaskRecordList(taskRecord.singleTask.taskID - 1);
				foreach (TaskRecord tr in trList) {
					if (tr.taskRecordStat == TASKSTAT_T.TASK_READY_SEND) {
						MessageBox.Show("需要先取消前面货物的任务", "置顶提示", MessageBoxButtons.OK);
						ret = false;
					}
				}
			}

			return ret;

		}

		/// <summary>
		/// 将该任务的优先级提到最高 设置该任务的优先级，默认查询任务的时候将会参考该任务的有先级来查询
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void topItemClick(object sender, EventArgs e) {
			MenuItem item = (MenuItem)sender;
			TaskRecord tr = (TaskRecord)selectedButton.getBindValue();
			Console.WriteLine(" tr is nulll " + tr == null);
			try {
				if (checkTopFunc(tr)) {
					selectedButton.BackColor = Color.Gray;
					TaskReordService.getInstance().topTaskRecord(tr);
				}
			} catch (Exception ex) {
				Console.WriteLine(ex.ToString());
			}
		}

		private void addAllTaskRecordButton_Click(object sender, EventArgs e) {
			List<TaskRecord> listTaskRecords = TaskReordService.getInstance().getReadySendTaskRecordList();
			if (listTaskRecords.Count > 0) {
				DialogResult dr;
				dr = MessageBox.Show("有缓存任务，禁止一键添加", "提示", MessageBoxButtons.OK);
			} else {
				AGVUtil.addAllTaskRecord();
				updateFrm();
			}
		}

		public void initMainFrm() {
			MenuItem addItem = new MenuItem("添加");
			addItem.Name = "addPalletItem";
			addItem.Name = "addPalletItem";
			addItem.Click += new EventHandler(palletItemClick);
			addTasktMenu.MenuItems.Add(addItem);

			MenuItem cancelItem = new MenuItem("取消");
			cancelItem.Name = "cancelTaskItem";
			cancelItem.Click += new EventHandler(cancelItemClick);

			cancelTasktMenu.MenuItems.Add(cancelItem);

			MenuItem toplItem = new MenuItem("置顶");  //将该任务的优先级提到最高
			toplItem.Name = "topTaskItem";
			toplItem.Click += new EventHandler(topItemClick);
			cancelTasktMenu.MenuItems.Add(toplItem);



			initPanel();
			updatePanel();
			foreach (DictionaryEntry de in buttonStHash) {
				int id = (int)de.Key;
				TaskButton button = (TaskButton)de.Value;
				switch (id) {
					case 1:
						// 上货
						button.Location = new System.Drawing.Point(291, 65);
						button.Size = new System.Drawing.Size(96, 29);
						break;

					case 2:
						//卸货A
						button.Location = new System.Drawing.Point(498, 65);
						button.Size = new System.Drawing.Size(96, 29);
						break;

					case 3:
						//卸货B
						button.Location = new System.Drawing.Point(699, 65);
						button.Size = new System.Drawing.Size(96, 29);
						break;
					case 4:
						// 1号
						button.Location = new System.Drawing.Point(109, 50);
						button.Size = new System.Drawing.Size(96, 29);
						break;

					case 5:
						//2号
						button.Location = new System.Drawing.Point(109, 100);
						button.Size = new System.Drawing.Size(96, 29);
						break;

					case 6:
						//3号
						button.Location = new System.Drawing.Point(239, 50);
						button.Size = new System.Drawing.Size(96, 29);
						break;

					case 7:
						// 4号
						button.Location = new System.Drawing.Point(239, 100);
						button.Size = new System.Drawing.Size(96, 29);
						break;

					case 8:
						//5号
						button.Location = new System.Drawing.Point(389, 50);
						button.Size = new System.Drawing.Size(96, 29);
						break;

					case 9:
						//6号
						button.Location = new System.Drawing.Point(389, 100);
						button.Size = new System.Drawing.Size(96, 29);


						break;
					case 16:
						//卸货6号
						button.Location = new System.Drawing.Point(989, 50);
						button.Size = new System.Drawing.Size(96, 29);
						break;

					case 10:
						//7号
						button.Location = new System.Drawing.Point(539, 50);
						button.Size = new System.Drawing.Size(96, 29);
						break;

					case 11:
						//8号
						button.Location = new System.Drawing.Point(539, 100);
						button.Size = new System.Drawing.Size(96, 29);
						break;

					case 12:
						//9号
						button.Location = new System.Drawing.Point(689, 50);
						button.Size = new System.Drawing.Size(96, 29);
						break;

					case 13:
						// 10号
						button.Location = new System.Drawing.Point(689, 100);
						button.Size = new System.Drawing.Size(96, 29);
						break;
					case 14:
						//11号
						button.Location = new System.Drawing.Point(839, 50);
						button.Size = new System.Drawing.Size(96, 29);
						break;
					case 17:
						//卸货12
						button.Location = new System.Drawing.Point(989, 100);
						button.Size = new System.Drawing.Size(96, 29);
						break;
					case 15:
						//12号
						button.Location = new System.Drawing.Point(839, 100);
						button.Size = new System.Drawing.Size(96, 29);
						break;
				}

			}
			startUpdateThread();
			//this.Update();
		}

		private void taskButtonMouseDown(object sender, MouseEventArgs e) {
			TaskButton button = (TaskButton)sender;
			if (e.Button == MouseButtons.Right) {
				selectedButton = button;
				Console.WriteLine(" select button name = " + selectedButton.Name);
				//string sql = "select * from taskrecord, singletask where taskRecordStat in (1, 2, 3) and singleTask = singletask.id and singletask.taskName = '" +  button.Name + "'";
				TaskRecord tr = (TaskRecord)button.getBindValue();
				if (tr == null) {
					addTasktMenu.Show(button, new Point(e.X, e.Y));
				} else if (tr.taskRecordStat == TASKSTAT_T.TASK_READY_SEND)   //该状态可以取消任务
				  {
					cancelTasktMenu.Show(button, new Point(e.X, e.Y));
				}
			}
		}

		private TaskRecord lookTaskRecordByTaskId(int taskID)  //获取taskID对应的任务，如果任务列表中没有，则返回空
		{
			List<TaskRecord> trList = TaskReordService.getInstance().getTaskRecordList();
			foreach (TaskRecord tr in trList) {
				if (tr.singleTask != null && tr.singleTask.taskID == taskID) {
					return tr;
				}
			}

			return null;
		}

		delegate void setFrmEnableCallBack(bool enabled);
		public void setFrmEnable(bool enabled) {
			if (this.InvokeRequired) {
				setFrmEnableCallBack stcb = new setFrmEnableCallBack(setFrmEnable);
				this.Invoke(stcb, new object[] { enabled });
			} else {
				this.Enabled = enabled;
			}
		}

		delegate void setFrmEnableExcludeControPanelCallBack(bool enabled);
		public void setFrmEnableExcludeControPanel(bool enabled) {
			if (this.InvokeRequired) {
				setFrmEnableExcludeControPanelCallBack stcb = new setFrmEnableExcludeControPanelCallBack(setFrmEnableExcludeControPanel);
				this.Invoke(stcb, new object[] { enabled });
			} else {
				this.upPanel.Enabled = enabled;
				this.downPanel.Enabled = enabled;
				if (enabled) {
					this.systemPauseButton.Name = "pause";
					this.systemPauseButton.Text = "系统暂停";
				} else {
					this.systemPauseButton.Name = "start";
					this.systemPauseButton.Text = "系统启动";
				}
			}
		}

		delegate void setWindowStateCallBack(FormWindowState windowState);
		public void setWindowState(FormWindowState windowState) {
			if (this.InvokeRequired) {
				setWindowStateCallBack stcb = new setWindowStateCallBack(setWindowState);
				this.Invoke(stcb, new object[] { windowState });
			} else {
				this.WindowState = windowState;
			}
		}

		public void updateFrm()  //更新主界面
		{
			Console.WriteLine(" frm was set true");
			this.isUpdateFrm = true;
		}

		private void _updateAgvPanel() {
			AGVMessage message = new AGVMessage();
			foreach (AGVPanel ap_s in agvPanelList) {
				ap_s.updatePanel();
				if (ap_s.getForkLiftWrapper().getForkLift().isUsed == 1 && ap_s.getForkLiftWrapper().getBatteryInfo().isBatteryLowpower()) {
					message.setMessageType(AGVMessageHandler_TYPE_T.AGVMessageHandler_LOWPOWER);
					message.setMessageStr(ap_s.getForkLiftWrapper().getForkLift().forklift_number + "号车电量低于20%, 请等待已发送的任务完成，然后更换电池，切勿立即关闭程序");
					lowpowerBattery = true;
				}
			}

			if (lowpowerBattery) {
				AGVMessageHandler.getMessageHandler().setMessage(message);
			}
		}

		private void _updateSystemPause() {
			bool systemPause = AGVSystem.getSystem().getSystemPause();

			if (systemPause && this.systemPauseButton.Name.Equals("pause")) {
				setFrmEnableExcludeControPanel(false);
			} else if (!systemPause && this.systemPauseButton.Name.Equals("start")) {
				setFrmEnableExcludeControPanel(true);
			}
		}
		private void _updateFrm() {
			while (!isStop) {
				_updateAgvPanel();
				_updateSystemPause(); //如果设置系统暂停后，界面需要更新，用户不能再操作
				if (!isUpdateFrm) {
					Thread.Sleep(1000);
					continue;
				}
				lock (LockController.getLockController().getLockTask()) {

					foreach (DictionaryEntry de in buttonStHash) {
						int taskID = (int)de.Key;
						TaskButton tb = (TaskButton)de.Value;
						//Console.WriteLine(" look task id = " + taskID);
						TaskRecord tr = lookTaskRecordByTaskId(taskID);

						tb.bindValue(tr);
						if (tr != null) {
							if (tr.taskRecordStat == TASKSTAT_T.TASK_READY_SEND) {
								tb.BackColor = Color.LightGray;
							} else if (tr.taskRecordStat == TASKSTAT_T.TASK_SEND || tr.taskRecordStat == TASKSTAT_T.TASK_SEND_SUCCESS) {
								tb.BackColor = Color.LightGreen;
							}

						} else {
							tb.BackColor = Color.White; //没有任务记录表示，任务执行完成或没有发送任务，颜色改为白色
						}
					}
				}
				isUpdateFrm = false;
			}


		}

		public void startUpdateThread() {
			isUpdateFrm = true;  //第一次启动时，更新一次界面
			ThreadFactory.newBackgroudThread(new ThreadStart(_updateFrm)).Start();
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e) {

		}

		private void label1_Click(object sender, EventArgs e) {

		}

		private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {

		}

		private void MainFrm_Load(object sender, EventArgs e) {

		}

		private void label3_Click(object sender, EventArgs e) {

		}

		private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e) {

		}

		private void systemPauseButton_Click(object sender, EventArgs e) {
			Button button = (Button)sender;
			if (button.Name.Equals("pause")) {
				AGVSystem.getSystem().setPause(SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_SYSTEM_WITH_START);
				setFrmEnableExcludeControPanel(false);

			} else if (button.Name.Equals("start")) {
				AGVSystem.getSystem().setPause(SHEDULE_PAUSE_TYPE_T.SHEDULE_PAUSE_TYPE_MIN);
				setFrmEnableExcludeControPanel(true);
			}
		}

		private void mainForm_Closing(Object sender, FormClosingEventArgs e) {
			List<TaskRecord> trList = TaskReordService.getInstance().getTaskRecordList();
			DialogResult dr = System.Windows.Forms.DialogResult.No;
			if (trList.Count > 0) {
				dr = MessageBox.Show("当前有任务没完成，确认退出?", "退出提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
			} else if (ElevatorFactory.getElevator().getOutCommand() != elevator.COMMAND_FROME2S.LIFT_OUT_COMMAND_MIN) {
				dr = MessageBox.Show("升降机上有任务，确认退出？", "退出提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
			} else {
				dr = MessageBox.Show("确认退出？", "退出提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2);
			}

			if (dr == System.Windows.Forms.DialogResult.OK) {
				this.Dispose();
				System.Environment.Exit(0);
			} else if (dr == System.Windows.Forms.DialogResult.Cancel) {
				e.Cancel = true;
			}
		}

		private void ForkliftUseConfigureToolStripMenuItem_Click(object sender, EventArgs e) {
			AGVConfigureForm acForm = FormController.getFormController().getAGVConfigureForm();
			acForm.ShowDialog();
		}

		private void ForkliftManualCtrlToolStripMenuItem_Click(object sender, EventArgs e) {
			AGVManualCtrlForm amcForm = FormController.getFormController().getAGVManualCtrlForm();
			amcForm.updateAGVMCForm();
			amcForm.ShowDialog();
		}

	}
}
