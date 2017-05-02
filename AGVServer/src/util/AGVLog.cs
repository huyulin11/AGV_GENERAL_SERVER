using System;
using System.IO;
using System.Diagnostics;
using System.Xml;
using AGV.init;
namespace AGV.util {
	public class AGVLog
    {
        private static readonly object obj = new object();
        private static byte logLevel = 3;  //定义三种日志输出级别0 不输出 1输出err 2输出err和warn 3输出err warn info
        public AGVLog()
        {
        }

        private byte parseLogLevel()
        {
            XmlDocument xml = new XmlDocument();//声明xml
            try
            {
                xml.Load(AGVEngine.AGVCONFIG_PATH);
                XmlNode config = xml.SelectSingleNode("config");
                XmlNode xLog = config.SelectSingleNode("log");//指向根节点 
                XmlNode xLevel = xLog.SelectSingleNode("level");
                byte logLevel = Convert.ToByte(xLevel.InnerText);//读出里面的值 注意读取的是string 需要类型转换的话自己做
                WriteLogs("<INFO>", "AGV init log level: " + logLevel);
                if (logLevel >= 0 && logLevel < 4)
                    return logLevel;
            }
            catch (Exception ex)
            {
                WriteLogs("<ERROR>", "AGV init log level: " + logLevel); //出现异常该类则不能正常实例化
            }
            return 0xFF;  //解析错误返回0xFF
        }

        /// <summary>
        /// 初始化log，主要解析日志级别
        /// </summary>
        public void initAGVLog()
        {
            logLevel = parseLogLevel();
        }

        /// <summary>
        /// 记录日志，优先级低
        /// </summary>
        /// <param name="s">日志能容</param>
        public static void WriteInfo(string content, StackFrame sf)
        {
            if (logLevel < 3) //日志级别为3的时候才输出info
                return;
            string ct = sf.GetMethod().Name + ": ";
            WriteLogs("<INFO>", ct + content);
        }

        /// <summary>
        /// 警告日志，优先级中等
        /// </summary>
        /// <param name="s">日志能容</param>
        public static void WriteWarn(string content, StackFrame sf)
        {
            if (logLevel < 2) //日志级别为2、3的时候输出warn
                return;
            string ct = sf.GetMethod().Name + ": ";
            WriteLogs("<Warn>", ct + content);
        }

        /// <summary>
        /// 错误日志 必须记录
        /// 
        /// </summary>
        /// <param name="s">日志内容</param>
        public static void WriteError(string content, StackFrame sf)
        {
            if (logLevel < 1) //日志级别为1、2、3的时候输出Error
                return;
            string ct = sf.GetMethod().Name + ": ";
            WriteLogs("<ERROR>", ct + content);
        }

        /// <summary>
        /// 日志输出
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="content"></param>
        private static void WriteLogs(string type, string content)
        {
            lock (obj)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                if (!string.IsNullOrEmpty(path))
                {
                    path = AppDomain.CurrentDomain.BaseDirectory + "log";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    path = path + "\\" + DateTime.Now.ToString("yyMM");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    path = path + "\\" + DateTime.Now.ToString("dd") + ".txt";
                    if (!File.Exists(path))
                    {
                        FileStream fs = File.Create(path);
                        fs.Close();
                    }
                    if (File.Exists(path))
                    {
                        StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default);
                        Console.WriteLine("content: " + content);
                        sw.WriteLine(DateTime.Now + " " + type);
                        sw.WriteLine("      " + content);
                        sw.Close();
                    }
                }
            }
        }

    }
}
