using AGV.dao;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AGV.form {
	public partial class InfoFrm:Form {
		public InfoFrm() {
			InitializeComponent();
		}

		private void mainForm_Closing(Object sender,FormClosingEventArgs e) {
			DialogResult dr = System.Windows.Forms.DialogResult.No;
			dr = MessageBox.Show("确认退出？","退出提示",MessageBoxButtons.OKCancel,MessageBoxIcon.Asterisk,MessageBoxDefaultButton.Button2);

			if (dr == System.Windows.Forms.DialogResult.OK) {
				this.Dispose();
				TaskexeDao.getDao().InsertTaskexeSysInfo("用户手动关闭AGV服务程序！");
				System.Environment.Exit(0);
			} else if (dr == System.Windows.Forms.DialogResult.Cancel) {
				e.Cancel = true;
			}
		}

		private void label1_Click(object sender,EventArgs e) {

		}
	}
}
