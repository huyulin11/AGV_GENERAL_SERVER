namespace AGV.form {
    partial class LoginFrm
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
			System.Windows.Forms.Label 用户名;
			System.Windows.Forms.Label label1;
			this.userNameText = new System.Windows.Forms.TextBox();
			this.loginButton = new System.Windows.Forms.Button();
			this.resetButton = new System.Windows.Forms.Button();
			this.passwdText = new System.Windows.Forms.TextBox();
			用户名 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// 用户名
			// 
			用户名.AllowDrop = true;
			用户名.BackColor = System.Drawing.SystemColors.Control;
			用户名.CausesValidation = false;
			用户名.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			用户名.Location = new System.Drawing.Point(145, 132);
			用户名.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			用户名.MaximumSize = new System.Drawing.Size(133, 133);
			用户名.Name = "用户名";
			用户名.Size = new System.Drawing.Size(133, 39);
			用户名.TabIndex = 0;
			用户名.Text = "用户名";
			用户名.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			label1.AllowDrop = true;
			label1.BackColor = System.Drawing.SystemColors.Control;
			label1.CausesValidation = false;
			label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			label1.Location = new System.Drawing.Point(145, 212);
			label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label1.MaximumSize = new System.Drawing.Size(133, 133);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(133, 44);
			label1.TabIndex = 1;
			label1.Text = "密码";
			label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// userNameText
			// 
			this.userNameText.Location = new System.Drawing.Point(288, 145);
			this.userNameText.Margin = new System.Windows.Forms.Padding(4);
			this.userNameText.Name = "userNameText";
			this.userNameText.Size = new System.Drawing.Size(203, 26);
			this.userNameText.TabIndex = 2;
			this.userNameText.Text = "admin";
			// 
			// loginButton
			// 
			this.loginButton.Location = new System.Drawing.Point(384, 303);
			this.loginButton.Margin = new System.Windows.Forms.Padding(4);
			this.loginButton.Name = "loginButton";
			this.loginButton.Size = new System.Drawing.Size(107, 37);
			this.loginButton.TabIndex = 4;
			this.loginButton.Text = "登录";
			this.loginButton.UseVisualStyleBackColor = true;
			this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
			// 
			// resetButton
			// 
			this.resetButton.Location = new System.Drawing.Point(198, 303);
			this.resetButton.Margin = new System.Windows.Forms.Padding(4);
			this.resetButton.Name = "resetButton";
			this.resetButton.Size = new System.Drawing.Size(111, 37);
			this.resetButton.TabIndex = 5;
			this.resetButton.Text = "重置";
			this.resetButton.UseVisualStyleBackColor = true;
			this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
			// 
			// passwdText
			// 
			this.passwdText.Location = new System.Drawing.Point(286, 223);
			this.passwdText.Margin = new System.Windows.Forms.Padding(4);
			this.passwdText.Name = "passwdText";
			this.passwdText.Size = new System.Drawing.Size(203, 26);
			this.passwdText.TabIndex = 3;
			this.passwdText.Text = "123456";
			this.passwdText.UseSystemPasswordChar = true;
			// 
			// LoginFrm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(739, 493);
			this.Controls.Add(this.passwdText);
			this.Controls.Add(this.resetButton);
			this.Controls.Add(this.loginButton);
			this.Controls.Add(this.userNameText);
			this.Controls.Add(label1);
			this.Controls.Add(用户名);
			this.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "LoginFrm";
			this.Text = "用户登录";
			this.Load += new System.EventHandler(this.loginFrm_Load);
			this.FormClosing += loginFrm_Closing;
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox userNameText;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.TextBox passwdText;

    }
}