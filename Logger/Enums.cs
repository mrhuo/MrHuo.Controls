using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MrHuo.Controls.Log
{
    /// <summary>
    /// 日志类型级别枚举
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 信息类日志
        /// </summary>
        Info,
        // <summary>
        // 警告类日志
        // </summary>
        //Warning,
        /// <summary>
        /// 错误类日志
        /// </summary>
        Error,
        /// <summary>
        /// Debug日志（最详细的记录）
        /// </summary>
        Debug,
        /// <summary>
        /// 预警类日志（发送电子邮件、短信到指定地址）
        /// </summary>
        Warn
    }

    /// <summary>
    /// 日志记录方式
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 以数据库的方式记录
        /// </summary>
        DataBase,
        /// <summary>
        /// 以文件的方式记录
        /// </summary>
        File
    }
    
    /// <summary>
    /// 日志预警方式
    /// </summary>
    public enum LogWarnType
    {
        /// <summary>
        /// 关闭预警
        /// </summary>
        OFF = 0,
        /// <summary>
        /// 邮件方式
        /// </summary>
        Email = 1,
        /// <summary>
        /// 短信方式
        /// </summary>
        SMS = 2
    }
}
