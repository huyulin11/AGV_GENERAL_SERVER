using System;
using System.Drawing;
using System.Windows.Forms;
using AGV.forklift;
using AGV.util;

namespace AGV.form {
	//用于显示单车的信息，包括运行状态、电池电量
	public partial class PauseCtrlPanel : Panel
    {
        public PauseCtrlPanel()
        {
            InitializeComponent();
        }

        public void initPanel(ForkLiftWrapper fl)
        {
            this.forklift = fl;

            forkNumberLabel.Text = fl.getForkLift().forklift_number.ToString() + "号车";


            if (fl.getPauseStr().Equals("暂停"))
            {
                pauseCtrlButton.Text = "启动";
            }
            else
            {
                pauseCtrlButton.Text = fl.getPauseStr();
            }

            forkNumberLabel.Location = new Point(10, 20);
            forkNumberLabel.Size = new Size(60, 30);

            pauseCtrlButton.Location = new Point(80, 10);
            pauseCtrlButton.Size = new Size(60, 30);

            if (fl.getPauseStr().Equals("运行")) //不支持运行的时候设置暂停
            {
                pauseCtrlButton.Enabled = false;
            }

            pauseCtrlButton.Click += pauseCtroButton_Click;
            this.Controls.Add(forkNumberLabel);
            this.Controls.Add(pauseCtrlButton);
        }


        public void updatePanel()
        {
            if (forklift.getPauseStr().Equals("暂停"))
            {
                pauseCtrlButton.Text = "启动";
            }
            else
            {
                pauseCtrlButton.Text = forklift.getPauseStr();
            }

            if (forklift.getPauseStr().Equals("运行")) //不支持运行的时候设置暂停
            {
                pauseCtrlButton.Enabled = false;
            }
            else
            {
                pauseCtrlButton.Enabled = true;
            }
        }

        /// <summary>
        /// 点击后，注意查看主界面车子是否启动，如果没有启动，可以进来再次点击启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pauseCtroButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if(forklift.getPauseStr().Equals("暂停"))
            {
                AGVUtil.setForkCtrl(forklift, 0);
                forklift.getForkLift().shedulePause = 0;
                forklift.getPosition().calcPositionArea();
                button.Text = "运行";
                button.Enabled = false;
            }
        }

        private ForkLiftWrapper forklift;
        private Label forkNumberLabel = new Label();
        private Button pauseCtrlButton = new Button();
    }
}
