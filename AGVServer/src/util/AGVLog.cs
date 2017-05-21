using System;
using System.IO;
using System.Diagnostics;
using System.Xml;
using AGV.init;
using System.Windows.Forms;
using AGV.dao;

namespace AGV.util {
	public class AGVLog {
		private static readonly object obj = new object();
		private static byte logLevel = 3;  //定义三种日志输出级别0 不输出 1输出err 2输出err和warn 3输出err warn info
		public AGVLog() {
		}

		private byte parseLogLevel() {
			XmlDocument xml = new XmlDocument();//声明xml
			try {
				xml.Load(AGVEngine.AGVCONFIG_PATH);
				XmlNode config = xml.SelectSingleNode("config");
				XmlNode xLog = config.SelectSingleNode("log");//指向根节点 
				XmlNode xLevel = xLog.SelectSingleNode("level");
				byte logLevel = Convert.ToByte(xLevel.InnerText);//读出里面的值 注意读取的是string 需要类型转换的话自己做
				WriteNormalLogs("<INFO>", "AGV init log level: " + logLevel);
				if (logLevel >= 0 && logLevel < 4)
					return logLevel;
			} catch (Exception ex) {
				WriteNormalLogs("<ERROR>", "AGV init log level: " + logLevel); //出现异常该类则不能正常实例化
			}
			return 0xFF;  //解析错误返回0xFF
		}

		/// <summary>
		/// 初始化log，主要解析日志级别
		/// </summary>
		public void initAGVLog() {
			logLevel = parseLogLevel();
		}

		/// <summary>
		/// 记录日志，优先级低
		/// </summary>
		/// <param name="s">日志能容</param>
		public static void WriteInfo(string content, StackFrame sf) {
			if (logLevel < 3) //日志级别为3的时候才输出info
				return;
			string ct = sf.GetMethod().Name + ": ";
			WriteNormalLogs("<INFO>", ct + content);
		}

		/// <summary>
		/// 记录日志，优先级低
		/// </summary>
		/// <param name="s">日志能容</param>
		public static void WriteSendInfo(string content, StackFrame sf) {
			string ct = sf.GetMethod().Name + ": ";
			WriteSendLogs("<INFO>", ct + content);
		}

		public static void WriteSendInfo(string content) {
			WriteSendLogs(content);
		}

		/// <summary>
		/// 记录日志，优先级低
		/// </summary>
		/// <param name="s">日志能容</param>
		public static void WriteThreadInfo(string content, StackFrame sf) {
			string ct = sf.GetMethod().Name + ": ";
			WriteThreadLogs("<INFO>", ct + content);
		}

		/// <summary>
		/// 记录日志，优先级低
		/// </summary>
		/// <param name="s">日志能容</param>
		public static void WriteConnectInfo(string content, StackFrame sf) {
			string ct = sf.GetMethod().Name + ": ";
			TaskexeDao.getDao().InsertTaskexeSysInfo(content);
			WriteConnectLogs("<INFO>", ct + content);
		}

		/// <summary>
		/// 警告日志，优先级中等
		/// </summary>
		/// <param name="s">日志能容</param>
		public static void WriteWarn(string content, StackFrame sf) {
			if (logLevel < 2) //日志级别为2、3的时候输出warn
				return;
			string ct = sf.GetMethod().Name + ": ";
			WriteNormalLogs("<Warn>", ct + content);
		}

		/// <summary>
		/// 错误日志 必须记录
		/// 
		/// </summary>
		/// <param name="s">日志内容</param>
		public static void WriteError(string content, StackFrame sf) {
			if (logLevel < 1) //日志级别为1、2、3的时候输出Error
				return;
			string ct = sf.GetMethod().Name + ": ";
			WriteNormalLogs("<ERROR>", ct + content);
		}

		private static void WriteNormalLogs(string type, string content) {
			string filepath = getOrCreateFile(AppDomain.CurrentDomain.BaseDirectory + "log" + "\\" + DateTime.Now.ToString("yyMM"), "normal_" + DateTime.Now.ToString("dd") + ".txt");
			WriteLogsTo(type, content, filepath);
		}

		private static void WriteSendLogs(string type, string content) {
			string filepath = getOrCreateFile(AppDomain.CurrentDomain.BaseDirectory + "log" + "\\" + DateTime.Now.ToString("yyMM"), "send_" + DateTime.Now.ToString("dd") + ".txt");
			WriteLogsTo(type, content, filepath);
		}

		private static void WriteSendLogs(string content) {
			string filepath = getOrCreateFile(AppDomain.CurrentDomain.BaseDirectory + "log" + "\\" + DateTime.Now.ToString("yyMM"), "send_" + DateTime.Now.ToString("dd") + ".txt");
			WriteLogsTo(content, filepath);
		}

		private static void WriteConnectLogs(string type, string content) {
			string filepath = getOrCreateFile(AppDomain.CurrentDomain.BaseDirectory + "log" + "\\" + DateTime.Now.ToString("yyMM"), "connect_" + DateTime.Now.ToString("dd") + ".txt");
			WriteLogsTo(type, content, filepath);
		}


		private static void WriteThreadLogs(string type, string content) {
			string filepath = getOrCreateFile(AppDomain.CurrentDomain.BaseDirectory + "log" + "\\" + DateTime.Now.ToString("yyMM"), "thread_" + DateTime.Now.ToString("dd") + ".txt");
			WriteLogsTo(type, content, filepath);
		}

		private static void WriteLogsTo(string content, string filepath) {
			lock (obj) {
				if (File.Exists(filepath)) {
					StreamWriter sw = new StreamWriter(filepath, true, System.Text.Encoding.Default);
					sw.Write(" " + content);
					sw.Close();
				}
			}
		}

		private static void WriteLogsTo(string type, string content, string filepath) {
			try {
				lock (obj) {
					if (File.Exists(filepath)) {
						StreamWriter sw = new StreamWriter(filepath, true, System.Text.Encoding.Default);
						sw.WriteLine();
						sw.WriteLine(DateTime.Now + " " + type);
						sw.WriteLine("      " + content);
						sw.Close();
					}
				}
			} catch (Exception ex) {
				MessageBox.Show(filepath + "文件被占用，无法更新日志，请检查后重启AGV服务程序！");
				TaskexeDao.getDao().InsertTaskexeSysInfo(filepath + "文件被占用，无法更新日志，请检查后重启AGV服务程序！");
				System.Environment.Exit(0);
			}
		}

		private static string getOrCreateFile(string path, string filename) {
			string newPath = path;
			if (!string.IsNullOrEmpty(newPath)) {
				if (!Directory.Exists(newPath)) {
					Directory.CreateDirectory(newPath);
				}
				newPath = newPath + "\\" + filename;
				if (!File.Exists(newPath)) {
					FileStream fs = File.Create(newPath);
					fs.Close();
				}
				return newPath;
			}
			return "";
		}
	}
}
