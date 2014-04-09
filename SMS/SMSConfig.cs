using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MrHuo.Controls.SMS
{
    /// <summary>
    /// 短信发送组件配置类
    /// </summary>
    public class SMSConfig:Configs.ConfigBase
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public SMSConfig() { }
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public SMSConfig(string configPathName) : base(configPathName) { }
        /// <summary>
        /// 获取一个值，该值表示短信发送组件的配置文件路径
        /// </summary>
        protected internal override string ConfigFilePath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory + "Configs\\SMSConfig.xml"; }
        }

        /// <summary>
        /// 获取一个值，该值表示短信发送组件的配置文件根名称
        /// </summary>
        protected internal override string RootElement
        {
            get { return "SMSConfig"; }
        }

        /// <summary>
        /// 获取一个值，该值表示Url格式化字符串
        /// </summary>
        public String UrlFormat
        {
            get
            {
                return this.GetConfig("UrlFormat");
            }
        }

        /// <summary>
        /// 获取一个值，该值表示短信账户
        /// </summary>
        public String User
        {
            get
            {
                return this.GetConfig("User");
            }
        }

        /// <summary>
        /// 获取一个值，该值表示短信账户密码
        /// </summary>
        public String Key
        {
            get
            {
                return this.GetConfig("Key");
            }
        }
    }
}
