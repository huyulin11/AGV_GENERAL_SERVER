using System;
using System.Windows.Forms;
using AGV.init;
using AGV.dao;

namespace AGV.form {
	public partial class LoginFrm : Form {
		public LoginFrm() {
			InitializeComponent();
		}


		private bool checkUser(string userName, string passwd) {
			foreach (User user in AGVCacheData.getUserList()) {
				if (user.userPasswd.Equals(passwd) && user.userName.Equals(userName)) {
					AGVEngine.getInstance().setCurrentUser(user);
					return true;
				}
			}

			return false;
		}

		private void loginFrm_Load(object sender, EventArgs e) {

		}

		private void loginFrm_Closing(object sender, FormClosingEventArgs e) {
			this.Hide();
			if (AGVEngine.getInstance().getCurrentUser() == null) {
				System.Environment.Exit(0);
			}
		}

		private void resetButton_Click(object sender, EventArgs e) {
			Button resetButton = (Button)sender;
			this.passwdText.Clear();
			this.userNameText.Clear();
			Console.WriteLine("reset click button name = " + resetButton.Name);
		}

		private void loginButton_Click(object sender, EventArgs e) {
			string userName = this.userNameText.Text;
			string userPasswd = this.passwdText.Text;

			Console.WriteLine(" name = " + userName + " userPasswd = " + userPasswd);
			if (checkUser(userName, userPasswd)) {
				//MessageBox.Show("登录成功");
				Dispose();
			} else {
				MessageBox.Show("登录失败");
				return;
			}
		}
	}
}
