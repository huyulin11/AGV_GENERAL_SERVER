using System.Drawing;
using System.Windows.Forms;
using AGV.forklift;

namespace AGV.form {
	//用于显示单车的信息，包括运行状态、电池电量
	public partial class AGVPanel : Panel
    {
        public Panel infoPanel = new Panel();
        public AGVPanel()
        {
            InitializeComponent();
        }

        public void initPanel(ForkLiftWrapper fl)
        {
            numberLabel.Text = fl.getForkLift().forklift_number.ToString();
            stateLabel.Text = "任务状态";
            batteryLabel.Text = "电池电量";
            pauseLabel.Text = "暂停状态";
            batteryLabel_c.Text = "";

            numberLabel.Location = new Point(70, 0);
            numberLabel.Size = new Size(20, 20);
            stateLabel_c.Text = "";
            stateLabel.Location = new System.Drawing.Point(10, 25);
            stateLabel.Size = new System.Drawing.Size(60, 29);


            stateLabel_c.Location = new System.Drawing.Point(85, 25);
            stateLabel_c.Size = new System.Drawing.Size(40, 29);

            batteryLabel.Location = new System.Drawing.Point(10, 65);
            batteryLabel.Size = new System.Drawing.Size(60, 29);


            batteryLabel_c.Location = new System.Drawing.Point(85, 65);
            batteryLabel_c.Size = new System.Drawing.Size(40, 29);

            pauseLabel.Location = new System.Drawing.Point(10, 105);
            pauseLabel.Size = new System.Drawing.Size(60, 29);

            pauseLabel_c.Location = new System.Drawing.Point(85, 105);
            pauseLabel_c.Size = new System.Drawing.Size(40, 29);

            this.fl = fl;
            this.infoPanel.Location = new Point(0, 30);
            this.infoPanel.Size = new Size(150, 140);
            this.infoPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Name = "panel_a" + fl.getForkLift().forklift_number;
            this.infoPanel.Controls.Add(stateLabel);
            this.infoPanel.Controls.Add(stateLabel_c);
            this.infoPanel.Controls.Add(batteryLabel);
            this.infoPanel.Controls.Add(batteryLabel_c);

            this.infoPanel.Controls.Add(pauseLabel);
            this.infoPanel.Controls.Add(pauseLabel_c);

            this.Controls.Add(numberLabel);

            this.Controls.Add(infoPanel);

        }

        delegate void setLabelCallBack(Label label, string text);
        public void setLabel(Label label, string text)
        {
            if (this.InvokeRequired)
            {
                setLabelCallBack stcb = new setLabelCallBack(setLabel);
                this.Invoke(stcb, new object[] { label, text });
            }
            else
            {
                label.Text = text;
            }
        }

        public void setForkLift(ForkLiftWrapper fl)
        {
            this.fl = fl;
        }

        public ForkLiftWrapper getForkLiftWrapper()
        {
            return fl;
        }

        public void updatePanel()
        {
            if (fl.getForkLift().taskStep == TASK_STEP.TASK_IDLE || fl.getForkLift().taskStep == TASK_STEP.TASK_END)
            {
                setLabel(stateLabel_c, "空闲");
            } else if ( fl.getForkLift().taskStep == TASK_STEP.TASK_SENDED || fl.getForkLift().taskStep == TASK_STEP.TASK_EXCUTE)
            {
                setLabel(stateLabel_c, fl.getForkLift().currentTask);
            }


            setLabel(batteryLabel_c, fl.getBatteryInfo().getBatterySoc().ToString());

            setLabel(pauseLabel_c, fl.getPauseStr());
            //batteryLabel_c.Text = fl.getBatterySoc().ToString();
            //Console.WriteLine("fl number " + fl.getForkLift().forklift_number  + "   battery soc = " + fl.getBatterySoc().ToString());
            
        }

        private ForkLiftWrapper fl;
        private Label stateLabel = new Label();
        private Label stateLabel_c = new Label();
        private Label batteryLabel = new Label();
        private Label batteryLabel_c = new Label();
        private Label pauseLabel = new Label();
        private Label pauseLabel_c = new Label();
        private Label numberLabel = new Label();

    }
}
