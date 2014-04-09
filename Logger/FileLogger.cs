using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace MrHuo.Controls.Log
{
    /// <summary>
    /// 文件日志记录器
    /// </summary>
    public class FileLogger : Logger
    {
        private ReaderWriterLock writeLock = new ReaderWriterLock();

        private String logFileName = "LOG_" + DateTime.Now.ToString("yyyy_MM_dd_HH") + ".log";
        private String warnFileName = "WARN_" + DateTime.Now.ToString("yyyy_MM_dd_HH") + ".log";
        private String errorFileName = "ERROR_" + DateTime.Now.ToString("yyyy_MM_dd_HH") + ".log";
        private String debugFileName = "DEBUG_" + DateTime.Now.ToString("yyyy_MM_dd_HH") + ".log";

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public FileLogger()
        {
            if (!Directory.Exists(LogConfig.SavePath))
            {
                Directory.CreateDirectory(LogConfig.SavePath);
            }
        }
        /// <summary>
        /// 继承方法，记录日志核心
        /// </summary>
        /// <param name="LogLevel"></param>
        /// <param name="message"></param>
        protected override void InnerLogMethod(LogLevel LogLevel, string message)
        {
            writeLock.AcquireWriterLock(5000);
            string msg =
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").PadRight(30) +
                Page.PadRight(40) +
                LogLevel.ToString().PadRight(10) +
                User.PadRight(20) +
                message + "\r\n";

            string file = string.Empty;
            switch (LogLevel)
            {
                case Log.LogLevel.Debug:
                    file = LogConfig.SavePath + debugFileName;
                    break;
                case Log.LogLevel.Error:
                    file = LogConfig.SavePath + errorFileName;
                    break;
                case Log.LogLevel.Info:
                    file = LogConfig.SavePath + logFileName;
                    break;
                case Log.LogLevel.Warn:
                    file = LogConfig.SavePath + warnFileName;
                    break;
            }
            if (!File.Exists(file))
            {
                File.AppendAllText(file, "[Time]".PadRight(30) + "[Page]".PadRight(40) + "[Level]".PadRight(10) + "[User]".PadRight(20) + "[Message]\r\n", Encoding.UTF8);
            }
            File.AppendAllText(file, msg, Encoding.UTF8);
            writeLock.ReleaseWriterLock();
        }
    }
}