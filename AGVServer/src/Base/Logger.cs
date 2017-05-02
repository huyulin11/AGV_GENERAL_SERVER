using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Layout;
using log4net.Layout.Pattern;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace GiatiaAGV.Base
{

    /// <summary>
    /// 日志类，利用log4net
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// 错误日志
        /// </summary>
        private static readonly ILog logError = LogManager.GetLogger("logerror");
        /// <summary>
        /// 信息日志
        /// </summary>
        private static readonly ILog logInfo = LogManager.GetLogger("loginfo");
        /// <summary>
        /// 调试日志
        /// </summary>
        private static readonly ILog logDebug = LogManager.GetLogger("logdebug");
        /// <summary>
        /// 警告日志
        /// </summary>
        private static readonly ILog logWarn = LogManager.GetLogger("logwarn");
        /// <summary>
        /// 失败日志
        /// </summary>
        private static readonly ILog logFatal = LogManager.GetLogger("logfatal");
        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="message">日志信息</param>
        public static void Error(string message)
        {
            if (logError.IsErrorEnabled)
            {
                logError.Error(message);
            }

        }
        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="ex">异常信息</param>
        public static void Error(string message, Exception ex)
        {
            if (logError.IsErrorEnabled)
            {
                string msg = message.Trim();
                logError.Error(message, ex);
            }
        }
        /// <summary>
        /// 记录调试日志
        /// </summary>
        /// <param name="message">日志信息</param>
        public static void Debug(string message)
        {

            if (logDebug.IsDebugEnabled)
            {
                logDebug.Debug(message);
            }

        }
        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="ex">异常信息</param>
        public static void Debug(string message, Exception ex)
        {

            if (logDebug.IsDebugEnabled)
            {
                logDebug.Debug(message, ex);
            }

        }



        /// <summary>
        /// 严重错误，失败错误
        /// </summary>
        /// <param name="message">错误信息</param>
        public static void Fatal(string message)
        {

            if (logFatal.IsFatalEnabled)
            {
                logFatal.Fatal(message);
            }
        }
        /// <summary>
        /// 记录严重错误信息
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="ex">异常</param>
        public static void Fatal(string message, Exception ex)
        {

            if (logFatal.IsFatalEnabled)
            {
                logFatal.Fatal(message, ex);
            }
        }

        /// <summary>
        /// 消息日志
        /// </summary>
        /// <param name="message">消息</param>
        public static void Info(string message)
        {
            if (logInfo.IsInfoEnabled)
            {
                logInfo.Info(message);
            }
        }
        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="message">警告信息</param>
        public static void Warn(string message)
        {
            if (logWarn.IsWarnEnabled)
            {
                logWarn.Warn(message);
            }
        }
        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="message">警告信息</param>
        /// <param name="ex">异常信息</param>
        public static void Warn(string message, Exception ex)
        {
            if (logWarn.IsWarnEnabled)
            {
                logWarn.Warn(message, ex);
            }
        }
    }
}
