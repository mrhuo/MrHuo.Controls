using MrHuo.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MrHuo.Controls.Log
{
    /// <summary>
    /// 日志记录组件配置
    /// </summary>
    public class LogConfig : Configs.ConfigBase
    {
        #region [protected internal method]
        /// <summary>
        /// 获取一个值，该值表示日志记录组件的配置文件路径
        /// </summary>
        protected internal override string ConfigFilePath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory + "Configs\\LogConfig.xml"; }
        }

        /// <summary>
        /// 获取一个值，该值表示日志记录组件的配置文件根名称
        /// </summary>
        protected internal override string RootElement
        {
            get { return "LogConfig"; }
        }
        #endregion

        /// <summary>
        /// 获取一个值，该值表示预警的电子邮件
        /// </summary>
        public String LogWarnEmail
        {
            get
            {
                return this.GetConfig("WarningEmail");
            }
        }

        /// <summary>
        /// 获取一个值，该值表示预警接受短信的电话号码
        /// </summary>
        public String LogWarningSMSReciver
        {
            get
            {
                return this.GetConfig("WarningSMSReciver");
            }
        }

        /// <summary>
        /// 获取一个值，该值表示预警模式方式
        /// </summary>
        public LogWarnType LogWarnType
        {
            get
            {
                return (LogWarnType)Enum.Parse(typeof(LogWarnType), this.GetConfig("WarningMode"), true); 
            }
        }

        /// <summary>
        /// 获取一个值，该值表示日志记录方式
        /// </summary>
        public LogType LogType
        {
            get
            {
                return (LogType)Enum.Parse(typeof(LogType), this.GetConfig("LogType"), true);
            }
        }

        /// <summary>
        /// 获取一个值，该值表示以文件方式记录日志时，日志保存路径（绝对路径）
        /// </summary>
        public String SavePath
        {
            get
            {
                return this.GetConfig("SavePath");
            }
        }
        /// <summary>
        /// 获取一个值，该值表示以数据库方式记录日志时的SQLSERVER连接字符串
        /// </summary>
        public String DBConnectionString
        {
            get
            {
                return this.GetConfig("DBConnectionString");
            }
        }

        /// <summary>
        /// 获取一个值，该值表示日以数据库方式记录日志时用户保存数据库
        /// </summary>
        public String DBName
        {
            get
            {
                return this.GetConfig("DBName");
            }
        }

        /// <summary>
        /// 获取一个值，该值表示日以数据库方式记录日志时每隔多少天分表储存
        /// </summary>
        public Int32 SplitTableByDays
        {
            get
            {
                return Int32.Parse(this.GetConfig("SplitTableByDays"));
            }
        }

        /// <summary>
        /// 获取或者设置一个值，该值表示当前存储日志记录的表名称
        /// </summary>
        internal string CurrentTableName
        {
            get
            {
                var table = this.GetConfig("CurrentTableName"); ;
                if (string.IsNullOrEmpty(table))
                {
                    table = "Logger_" + DateTime.Now.ToString("yyyy_MM_dd");
                    this.SetConfig("CurrentTableName", table);
                }
                return table;
            }
            set { this.SetConfig("CurrentTableName", value); }
        }

        internal List<Table> LogTableList
        {
            get
            {
                return new List<Table>();
            }
        }
    }
}