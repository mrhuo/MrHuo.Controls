using MrHuo.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace MrHuo.Controls.Log
{
    /// <summary>
    /// 数据库日志记录器
    /// </summary>
    public class DataBaseLogger : Logger
    {
        #region [常规SQL语句]
        /// <summary>
        /// 自动创建日志记录表的SQL语句
        /// </summary>
        private const string SQL_CREATE_TABLE =
        @"CREATE TABLE {0}
        (
	        [ID] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	        [LEVEL] VARCHAR(10) NOT NULL,
	        [PAGE] VARCHAR(200) NOT NULL,
	        [MESSAGE] VARCHAR(MAX) NOT NULL,
            [USER] VARCHAR(50) NOT NULL,
	        [LOGTIME] DATETIME DEFAULT(GETDATE()) NOT NULL
        ) ON [PRIMARY]";
        /// <summary>
        /// 记录日志的SQL语句
        /// </summary>
        private const string SQL_INSERT_DATA = @"INSERT INTO {0} VALUES('{1}','{2}','{3}','{4}','{5}')";
        private const string SQL_DROP_TRIGGER = @"IF EXISTS(SELECT * FROM sys.triggers  WHERE name = N'T_Monitor_Table_Create' AND parent_class_desc = N'DATABASE') DROP TRIGGER T_Monitor_Table_Create ON DATABASE";
        /// <summary>
        /// 创建数据库触发器，用于记录日志表
        /// </summary>
        private const string SQL_CREATE_TRIGGER =
        @"CREATE TRIGGER T_Monitor_Table_Create ON DATABASE 
	        FOR CREATE_TABLE
        AS 
        IF IS_MEMBER ('db_owner') = 0
	        BEGIN
	           PRINT 'Access Denied' 
	           ROLLBACK TRANSACTION
	        END
        ELSE
	        BEGIN
		        IF NOT EXISTS(SELECT * FROM sys.tables WHERE NAME='ALLTABLES' AND TYPE='U')
			        BEGIN
				        CREATE TABLE ALLTABLES(
					        ID BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
					        TABLENAME VARCHAR(MAX) NOT NULL,
					        CREATETIME DATETIME NOT NULL
				        )ON [PRIMARY]
			        END
		
		        DECLARE @TABLE_NAME VARCHAR(MAX)
		        DECLARE @DELETE_REPEATE_DATA VARCHAR(MAX)
		        SET @TABLE_NAME=EVENTDATA().value('(/EVENT_INSTANCE/ObjectName)[1]','varchar(max)')
		        IF EXISTS(SELECT * FROM ALLTABLES WHERE TABLENAME=@TABLE_NAME)
			        BEGIN
				        DELETE FROM ALLTABLES WHERE TABLENAME=@TABLE_NAME
			        END
		        INSERT INTO ALLTABLES VALUES(@TABLE_NAME,GETDATE());
	        END";
        #endregion

        /// <summary>
        /// 多线程自动初始化日志数据库
        /// </summary>
        void Init()
        {
            new Action(() =>
            {
                string tableName = LogConfig.CurrentTableName;
                Check.NullOrEmpty(tableName, "tableName");
                Check.NullOrEmpty(LogConfig.DBConnectionString, "DBConnectionString");

                using (DataBaseHelper helper = new DataBaseHelper(LogConfig.DBConnectionString))
                {
                    if (!helper.QueryDatabaseExists(LogConfig.DBName))
                    {
                        helper.CreateDatabase(LogConfig.DBName);
                        helper.ChangeDatabase(LogConfig.DBName);
                        helper.ExcuteSql(SQL_DROP_TRIGGER);
                        helper.ExcuteSql(SQL_CREATE_TRIGGER);
                    }
                    helper.ChangeDatabase(LogConfig.DBName);
                    var table = helper.GetTables().Where(p => p.TableName.StartsWith("Logger_")).OrderByDescending(p => p.CreateDate).FirstOrDefault();
                    if (table != null)
                    {
                        int y = 0, m = 0, d = 0;
                        var year = table.TableName.Substring("Logger_".Length, 4);
                        var month = table.TableName.Substring(("Logger_" + year + "_").Length, 2);
                        var day = table.TableName.Substring(("Logger_" + year + "_" + month + "_").Length, 2);

                        Check.True(!int.TryParse(year, out y), () =>
                        {
                            throw new ArgumentException("Yeah");
                        });
                        Check.True(!int.TryParse(month, out m), () =>
                        {
                            throw new ArgumentException("month");
                        });
                        Check.True(!int.TryParse(day, out d), () =>
                        {
                            throw new ArgumentException("day");
                        });
                        DateTime tableTime = new DateTime(y, m, d);

                        if (DateTime.Now.Subtract(tableTime).TotalDays >= LogConfig.SplitTableByDays)
                        {
                            var name = "Logger_" + tableTime.AddDays(LogConfig.SplitTableByDays).ToString("yyyy_MM_dd");
                            var ret = helper.ExcuteSql(string.Format(SQL_CREATE_TABLE, name));
                            LogConfig.CurrentTableName = name;
                        }
                    }
                    else
                    {
                        var ret = helper.ExcuteSql(string.Format(SQL_CREATE_TABLE, tableName));
                    }
                }
            }).DynamicInvoke();
        }

        #region [构造函数]
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DataBaseLogger()
        {
            preInstanceTime = DateTime.Now;
            Init();
        }
        #endregion

        private static DateTime preInstanceTime = DateTime.MaxValue;
        /// <summary>
        /// 继承方法
        /// </summary>
        /// <param name="LogLevel"></param>
        /// <param name="message"></param>
        protected override void InnerLogMethod(LogLevel LogLevel, string message)
        {
            //每隔一分钟，检测是否需要分表
            if (DateTime.Now.Subtract(preInstanceTime).TotalSeconds >= 60)
            {
                Init();
            }
            using (DataBaseHelper db = new DataBaseHelper(LogConfig.DBConnectionString))
            {
                db.ChangeDatabase(LogConfig.DBName);
                db.ExcuteSql(string.Format(SQL_INSERT_DATA, LogConfig.CurrentTableName, LogLevel.ToString(), Page, message, User, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            }
        }
    }
}
