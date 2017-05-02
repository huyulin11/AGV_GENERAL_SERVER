using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using AGV.task;
using AGV.init;
using AGV.dao;

namespace AGV.form {
	partial class MainFrm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        //private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        /*
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        */

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.AGVConfigureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.使用说明ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.upPanel = new System.Windows.Forms.Panel();
            this.upLabel = new System.Windows.Forms.Label();
            this.downPanel = new System.Windows.Forms.Panel();
            this.downLabel = new System.Windows.Forms.Label();
            this.userPanel = new System.Windows.Forms.Panel();
            this.userLabel = new System.Windows.Forms.Label();
            this.userPictureBox = new System.Windows.Forms.PictureBox();
            this.agvPanel = new System.Windows.Forms.Panel();
            this.agvLabel = new System.Windows.Forms.Label();
            this.systemPauseButton = new System.Windows.Forms.Button();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.addAllTaskRecordButton = new System.Windows.Forms.Button();
            this.ForkliftUseConfigureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ForkliftManualCtrlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.upPanel.SuspendLayout();
            this.downPanel.SuspendLayout();
            this.userPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.userPictureBox)).BeginInit();
            this.agvPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AGVConfigureToolStripMenuItem,
            this.使用说明ToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // AGVConfigureToolStripMenuItem
            // 
            this.AGVConfigureToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ForkliftUseConfigureToolStripMenuItem,
            this.ForkliftManualCtrlToolStripMenuItem});
            this.AGVConfigureToolStripMenuItem.Name = "AGVConfigureToolStripMenuItem";
            resources.ApplyResources(this.AGVConfigureToolStripMenuItem, "AGVConfigureToolStripMenuItem");
            ForkliftUseConfigureToolStripMenuItem.Click += new System.EventHandler(this.ForkliftUseConfigureToolStripMenuItem_Click);
            ForkliftManualCtrlToolStripMenuItem.Click += new System.EventHandler(this.ForkliftManualCtrlToolStripMenuItem_Click);

            // 
            // 使用说明ToolStripMenuItem
            // 
            this.使用说明ToolStripMenuItem.Name = "使用说明ToolStripMenuItem";
            resources.ApplyResources(this.使用说明ToolStripMenuItem, "使用说明ToolStripMenuItem");
            // 
            // upPanel
            // 
            resources.ApplyResources(this.upPanel, "upPanel");
            this.upPanel.BackColor = System.Drawing.SystemColors.Control;
            this.upPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.upPanel.Controls.Add(this.upLabel);
            this.upPanel.Name = "upPanel";
            // 
            // upLabel
            // 
            resources.ApplyResources(this.upLabel, "upLabel");
            this.upLabel.Name = "upLabel";
            // 
            // downPanel
            // 
            resources.ApplyResources(this.downPanel, "downPanel");
            this.downPanel.BackColor = System.Drawing.SystemColors.Control;
            this.downPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.downPanel.Controls.Add(this.downLabel);
            this.downPanel.Name = "downPanel";
            // 
            // downLabel
            // 
            resources.ApplyResources(this.downLabel, "downLabel");
            this.downLabel.Name = "downLabel";
            // 
            // userPanel
            // 
            this.userPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.userPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.userPanel.Controls.Add(this.userLabel);
            this.userPanel.Controls.Add(this.userPictureBox);
            resources.ApplyResources(this.userPanel, "userPanel");
            this.userPanel.Name = "userPanel";
            // 
            // userLabel
            // 
            resources.ApplyResources(this.userLabel, "userLabel");
            this.userLabel.Name = "userLabel";
            // 
            // userPictureBox
            // 
            resources.ApplyResources(this.userPictureBox, "userPictureBox");
            this.userPictureBox.Image = global::AGV服务端.Properties.Resources.user;
            this.userPictureBox.Name = "userPictureBox";
            this.userPictureBox.TabStop = false;
            // 
            // agvPanel
            // 
            resources.ApplyResources(this.agvPanel, "agvPanel");
            this.agvPanel.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.agvPanel.Controls.Add(this.agvLabel);
            this.agvPanel.Name = "agvPanel";
            // 
            // agvLabel
            // 
            resources.ApplyResources(this.agvLabel, "agvLabel");
            this.agvLabel.Name = "agvLabel";
            // 
            // systemPauseButton
            // 
            resources.ApplyResources(this.systemPauseButton, "systemPauseButton");
            this.systemPauseButton.Name = "systemPauseButton";
            // 
            // controlPanel
            // 
            resources.ApplyResources(this.controlPanel, "controlPanel");
            this.controlPanel.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.controlPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.controlPanel.Name = "controlPanel";
            // 
            // addAllTaskRecordButton
            // 
            resources.ApplyResources(this.addAllTaskRecordButton, "addAllTaskRecordButton");
            this.addAllTaskRecordButton.Name = "addAllTaskRecordButton";
            // 
            // 使用配置ToolStripMenuItem
            // 
            this.ForkliftUseConfigureToolStripMenuItem.Name = "使用配置ToolStripMenuItem";
            resources.ApplyResources(this.ForkliftUseConfigureToolStripMenuItem, "使用配置ToolStripMenuItem");
            // 
            // 手动控制ToolStripMenuItem
            // 
            this.ForkliftManualCtrlToolStripMenuItem.Name = "手动控制ToolStripMenuItem";
            resources.ApplyResources(this.ForkliftManualCtrlToolStripMenuItem, "手动控制ToolStripMenuItem");
            // 
            // MainFrm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.controlPanel);
            this.Controls.Add(this.agvPanel);
            this.Controls.Add(this.userPanel);
            this.Controls.Add(this.upPanel);
            this.Controls.Add(this.downPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainFrm";

            this.WindowState = FormWindowState.Maximized;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Load += new System.EventHandler(this.MainFrm_Load);
            this.FormClosing += new FormClosingEventHandler(mainForm_Closing);

            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.upPanel.ResumeLayout(false);
            this.downPanel.ResumeLayout(false);
            this.userPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.userPictureBox)).EndInit();
            this.agvPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.MenuStrip menuStrip1;
        private Panel upPanel;
        private Panel downPanel;
        private System.Windows.Forms.ToolStripMenuItem AGVConfigureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 使用说明ToolStripMenuItem;
        private System.Windows.Forms.Label upLabel;
        private Button systemPauseButton;
        private Button addAllTaskRecordButton;
        private System.Windows.Forms.Label downLabel;

        List<SingleTask> singleTaskList = AGVCacheData.getSingleTaskList();  //获取所有的任务
        List<TaskButton> downButtonList = new List<TaskButton>();
        List<TaskButton> upButtonList = new List<TaskButton>();
        Hashtable buttonStHash = new Hashtable();
        private Panel userPanel;
        private Label userLabel;
        private PictureBox userPictureBox;
        private Panel agvPanel;
        private Label agvLabel;
        //private System.DirectoryServices.DirectorySearcher directorySearcher1;
        private Panel controlPanel;
        private ToolStripMenuItem ForkliftUseConfigureToolStripMenuItem;
        private ToolStripMenuItem ForkliftManualCtrlToolStripMenuItem;


    }
}

