using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MrHuo.Controls.Log;
using System.Web;
using System.Diagnostics;
using System.Threading;
using MrHuo.Controls.SMS;
using System.Reflection;

namespace MrHuo.Controls.Log
{
    /// <summary>
    /// 日志基类
    /// </summary>
    public abstract class Logger : IDisposable
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Logger() { }

        #region [属性和字段]
        private string _User = "Not Login";
        /// <summary>
        /// 获取或者设置一个值，该值表示当前操作进行的用户
        /// </summary>
        public String User
        {
            get
            {
                return _User;
            }
            set
            {
                _User = value;
            }
        }
        /// <summary>
        /// 获取或者设置一个值，该值表示当前操作所在的类
        /// </summary>
        public String Page { get; set; }
        #endregion

        #region [静态方法]
        /// <summary>
        /// 日志记录组件配置文件
        /// </summary>
        protected static LogConfig LogConfig = new LogConfig();
        /// <summary>
        /// 静态方法，根据当前操作的类创建日志
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static Logger CreateLogger(String page)
        {
            return CreateLogger(page, "Not Login");
        }
        /// <summary>
        /// 静态方法，根据当前操作的类的类型创建日志
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static Logger CreateLogger(Type page)
        {
            return CreateLogger(page.ToString());
        }
        /// <summary>
        /// 静态方法，根据当前操作的类和当前操作人创建日志
        /// </summary>
        /// <param name="page"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static Logger CreateLogger(String page, string user)
        {
            Logger ret = null;
            switch (LogConfig.LogType)
            {
                case LogType.File:
                    ret = new FileLogger();
                    break;
                case LogType.DataBase:
                    ret = new DataBaseLogger();
                    break;
            }
            if (ret != null)
            {
                ret.Page = page != null ? page : "unkown";
                ret.User = user;
            }
            return ret;
        }
        #endregion

        #region [记录方法]
        /// <summary>
        /// 用于记录Debug日志。在Debug模式下，只有定义了DEBUG预编译指令时才会记录日志
        /// </summary>
        /// <param name="message"></param>
        public virtual void Debug(string message)
        {
#if DEBUG
            InnerLogMethod(LogLevel.Debug, message);
#endif
        }
        /// <summary>
        /// 记录常规日志
        /// </summary>
        /// <param name="message"></param>
        public virtual void Info(string message)
        {
            InnerLogMethod(LogLevel.Info, message);
        }
        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="message"></param>
        public virtual void Error(string message)
        {
            InnerLogMethod(LogLevel.Error, message);
        }
        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="ex"></param>
        public virtual void Error(Exception ex)
        {
            Error(ex.ToString().Replace("\r\n", "$"));
        }
        /// <summary>
        /// 记录警告日志
        /// <para>默认会发送短信或邮件给指定账号</para>
        /// </summary>
        /// <param name="message"></param>
        public virtual void Warn(string message)
        {
            WarnIf(true, message);
        }
        /// <summary>
        /// 当条件为真，则发送短信或邮件。否则只记录日志。
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        public virtual void WarnIf(bool condition, string message)
        {
            if (condition)
            {
                string msg = String.Format("<b>{0}</b>&nbsp;&nbsp;{1}&nbsp;&nbsp;{2}<br/>", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), User, message);
                switch (LogConfig.LogWarnType)
                {
                    case LogWarnType.OFF:
                        break;
                    case LogWarnType.Email:
                        new Thread(new ThreadStart(() =>
                        {
                            using (Email.EmailSender sender = new Email.EmailSender()
                            {
                                Subject = "[Event Log System]",
                                EmailBody = msg
                            })
                            {
                                sender.AddReceiver(LogConfig.LogWarnEmail);
                                sender.Send();
                            }
                        })).Start();
                        break;
                    case LogWarnType.SMS:
                        new Thread(new ThreadStart(() =>
                        {
                            SMSSender.Send(LogConfig.LogWarningSMSReciver, msg);
                        })).Start();
                        break;
                }
            }
            InnerLogMethod(LogLevel.Warn, message);
        }
        /// <summary>
        /// 内部核心日志记录逻辑，需重写。
        /// </summary>
        /// <param name="LogLevel"></param>
        /// <param name="message"></param>
        protected abstract void InnerLogMethod(LogLevel LogLevel, String message);
        #endregion

        /// <summary>
        /// 释放系统资源
        /// </summary>
        public void Dispose()
        {
            if (!String.IsNullOrEmpty(_User))
            {
                GC.ReRegisterForFinalize(_User);
            }
            if (!String.IsNullOrEmpty(User))
            {
                GC.ReRegisterForFinalize(User);
            }
            if (!String.IsNullOrEmpty(Page))
            {
                GC.ReRegisterForFinalize(Page);
            }
            if (LogConfig != null)
            {
                GC.ReRegisterForFinalize(Page);
            }
            GC.Collect();
        }
    }
}