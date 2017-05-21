using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using MySql.Data.MySqlClient;
namespace AGV.dao {
	public class DBConnect {
		private static MySqlConnection connection;
		static string server = "192.168.3.13";//"172.18.57.221";
		private static string database = "inoma_agv_haitian";
		private static string uid = "root";
		private static string password = "12050901HH";//"12050901HH";
		private static Object lockDB = new Object();


		private DBConnect() {
		}

		private static MySqlConnection newConnection() {
			return new MySqlConnection("server=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";");
		}

		public static MySqlConnection getConnection() {
			try {
				if (connection == null) {
					Console.WriteLine("启用一个新的连接！");
					connection = newConnection();
				}

				if (connection.State == System.Data.ConnectionState.Closed) { //如果当前是关闭状态
					Console.WriteLine("打开关闭的连接！");
					connection.Open();
				}
				return connection;
			} catch (MySqlException ex) {
				switch (ex.Number) {
					case 0:
						MessageBox.Show("Cannot connect to server.  Contact administrator");
						break;
					case 1045:
						MessageBox.Show("Invalid username/password, please try again");
						break;
				}
				return null;
			}
		}

		private static bool CloseConnection() {
			try {
				if (connection.State == System.Data.ConnectionState.Open)
					connection.Close();
				return true;
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
				return false;
			}
		}

		public static void Backup() {
			try {
				DateTime Time = DateTime.Now;
				int year = Time.Year;
				int month = Time.Month;
				int day = Time.Day;
				int hour = Time.Hour;
				int minute = Time.Minute;
				int second = Time.Second;
				int millisecond = Time.Millisecond;

				string path;
				path = "C:\\" + year + "-" + month + "-" + day + "-" + hour + "-" + minute + "-" + second + "-" + millisecond + ".sql";
				StreamWriter file = new StreamWriter(path);

				ProcessStartInfo psi = new ProcessStartInfo();
				psi.FileName = "mysqldump";
				psi.RedirectStandardInput = false;
				psi.RedirectStandardOutput = true;
				psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", uid, password, server, database);
				psi.UseShellExecute = false;

				Process process = Process.Start(psi);

				string output;
				output = process.StandardOutput.ReadToEnd();
				file.WriteLine(output);
				process.WaitForExit();
				file.Close();
				process.Close();
			} catch (IOException ex) {
				MessageBox.Show("Error , unable to backup!");
			}
		}

		//Restore
		public static void Restore() {
			try {
				string path;
				path = "C:\\MySqlBackup.sql";
				StreamReader file = new StreamReader(path);
				string input = file.ReadToEnd();
				file.Close();

				ProcessStartInfo psi = new ProcessStartInfo();
				psi.FileName = "mysql";
				psi.RedirectStandardInput = true;
				psi.RedirectStandardOutput = false;
				psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", uid, password, server, database);
				psi.UseShellExecute = false;

				Process process = Process.Start(psi);
				process.StandardInput.WriteLine(input);
				process.StandardInput.Close();
				process.WaitForExit();
				process.Close();
			} catch (IOException ex) {
				MessageBox.Show("Error , unable to Restore!");
			}
		}
	}
}
