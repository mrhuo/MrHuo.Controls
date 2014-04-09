using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MrHuo.Controls.ImportTools
{
    /// <summary>
    /// 导入参数类
    /// </summary>
    public class ImportArgument
    {
        /// <summary>
        /// 默认构造函数，从Web.Config中的ConnectionStrings节中提取名称为“DataImportConnection”的数据库连接字符串
        /// </summary>
        public ImportArgument()
            : this("DataImportConnection")
        {
        }
        /// <summary>
        /// 构造函数重载，从Web.Config中的ConnectionStrings节中提取名称为connectionStringName的数据库连接字符串
        /// </summary>
        /// <param name="connectionStringName">数据库连接字符串名称</param>
        public ImportArgument(string connectionStringName)
        {
            _dataConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }
        private string _dataConnectionString = string.Empty;
        /// <summary>
        ///获取或者设置一个值，该值表示导入过程中远程SQLSERVER数据库的字符串
        /// </summary>
        public String DataConnectionString
        {
            get
            {
                return _dataConnectionString;
            }
            set
            {
                _dataConnectionString = value;
            }
        }
        /// <summary>
        /// 获取或者设置一个值，该值表示远程SQLSERVER数据库的数据表名称
        /// </summary>
        public String DataTable { get; set; }
        /// <summary>
        /// 获取或者设置一个值，该值表示本地CSV文件地址
        /// </summary>
        public String FilePath { get; set; }
        private bool _FirstRowIsHead = true;
        /// <summary>
        /// 获取或者设置一个值，该指标是第一行是否为标题行，默认为True
        /// </summary>
        public Boolean FirstRowIsHead
        {
            get
            {
                return _FirstRowIsHead;
            }
            set
            {
                _FirstRowIsHead = value;
            }
        }
    }
}
