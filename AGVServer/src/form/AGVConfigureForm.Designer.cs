namespace AGV.form {
	partial class AGVConfigureForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.disableF1 = new System.Windows.Forms.RadioButton();
            this.disableF2 = new System.Windows.Forms.RadioButton();
            this.confirm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // disableF1
            // 
            this.disableF1.AutoSize = true;
            this.disableF1.Location = new System.Drawing.Point(58, 46);
            this.disableF1.Name = "disableF1";
            this.disableF1.Size = new System.Drawing.Size(77, 16);
            this.disableF1.TabIndex = 0;
            this.disableF1.TabStop = true;
            this.disableF1.Text = "禁用1号车";
            this.disableF1.UseVisualStyleBackColor = true;
            // 
            // disableF2
            // 
            this.disableF2.AutoSize = true;
            this.disableF2.Location = new System.Drawing.Point(58, 90);
            this.disableF2.Name = "disableF2";
            this.disableF2.Size = new System.Drawing.Size(77, 16);
            this.disableF2.TabIndex = 1;
            this.disableF2.TabStop = true;
            this.disableF2.Text = "禁用2号车";
            this.disableF2.UseVisualStyleBackColor = true;
            // 
            // confirm
            // 
            this.confirm.Location = new System.Drawing.Point(178, 155);
            this.confirm.Name = "confirm";
            this.confirm.Size = new System.Drawing.Size(75, 23);
            this.confirm.TabIndex = 2;
            this.confirm.Text = "确定";
            this.confirm.UseVisualStyleBackColor = true;
            this.confirm.Click += new System.EventHandler(this.confirm_Click);
            // 
            // AGVConfigureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(291, 251);
            this.Controls.Add(this.confirm);
            this.Controls.Add(this.disableF1);
            this.Controls.Add(this.disableF2);
            this.Name = "AGVConfigureForm";
            this.Text = "单车使用配置";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton disableF1;
        private System.Windows.Forms.RadioButton disableF2;
        private System.Windows.Forms.Button confirm;
    }
}