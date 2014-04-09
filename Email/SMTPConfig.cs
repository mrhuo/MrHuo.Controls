using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Xml.Linq;

namespace MrHuo.Controls.Email
{
    /// <summary>
    /// SMTP邮件发送插件配置类
    /// </summary>
    public partial class SMTPConfig : Configs.ConfigBase
    {
        #region [Private Properties]
        private string _smtpServer;
        private string _userName;
        private string _userPasswod;
        private int _port;
        private bool _enableSSL;
        private Encoding _encoding;
        private string _defaultSender;
        private string _defaultSenderName;
        private bool _isBodyHtml;
        private bool _isAsyncSend;
        #endregion

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public SMTPConfig()
        {
            _smtpServer = this.GetConfig("SMTPServer");
            _userName = this.GetConfig("UserName");
            _userPasswod = this.GetConfig("UserPassword");
            _port = Int32.Parse(this.GetConfig("Port"));
            _enableSSL = Boolean.Parse(this.GetConfig("EnableSsl"));
            _encoding = Encoding.GetEncoding(this.GetConfig("Encoding"));
            _defaultSender = this.GetConfig("DefaultSender");
            _defaultSenderName = this.GetConfig("DefaultSenderName");
            _isBodyHtml = Boolean.Parse(this.GetConfig("IsBodyHtml"));
            _isAsyncSend = Boolean.Parse(this.GetConfig("IsAsyncSend"));
        }
        /// <summary>
        /// 构造函数重载
        /// </summary>
        /// <param name="filename"></param>
        public SMTPConfig(string filename) : base(filename) { }
        /// <summary>
        /// 构造函数重载
        /// </summary>
        /// <param name="server">服务器IP</param>
        /// <param name="user">用户名</param>
        /// <param name="pass">密码</param>
        /// <param name="port">端口号</param>
        /// <param name="enabledSSL">SSL</param>
        /// <param name="encoding">编码</param>
        public SMTPConfig(string server, string user, string pass, int port = 25, bool enabledSSL = true, string encoding = "UTF-8")
        {
            _smtpServer = server;
            _userName = user;
            _userPasswod = pass;
            _port = port;
            _enableSSL = enabledSSL;
            _encoding = Encoding.GetEncoding(encoding);
        }

        /// <summary>
        /// 获取一个值，该值表示SMTP服务器
        /// </summary>
        public string SMTPServer
        {
            get { return _smtpServer; }
            private set { _smtpServer = value; }
        }
        /// <summary>
        /// 获取一个值，该值表示用于发送邮件的SMTP账户
        /// </summary>
        public string UserName
        {
            get { return _userName; }
            private set { _userName = value; }
        }
        /// <summary>
        /// 获取一个值，该值表示用于发送邮件的SMTP账户密码
        /// </summary>
        public string UserPassword
        {
            get { return _userPasswod; }
            private set { _userPasswod = value; }
        }
        /// <summary>
        /// 获取一个值，该值表示用于发送邮件的SMTP端口号（默认为25）
        /// </summary>
        public int Port
        {
            get { return _port; }
            private set { _port = value; }
        }
        /// <summary>
        /// 获取一个值，该值表示发送时是否允许SSL
        /// </summary>
        public bool EnabledSSL
        {
            get { return _enableSSL; }
            private set { _enableSSL = value; }
        }
        /// <summary>
        /// 获取一个值，该值表示发送时的编码
        /// </summary>
        public Encoding Encoding
        {
            get { return _encoding; }
            private set { _encoding = value; }
        }
        /// <summary>
        /// 获取一个值，该值表示默认邮件发送者邮件地址
        /// </summary>
        public string DefaultSender
        {
            get { return _defaultSender; }
            private set { _defaultSender = value; }
        }
        /// <summary>
        /// 获取一个值，该值表示默认邮件发送者显示名称
        /// </summary>
        public string DefaultSenderName
        {
            get { return _defaultSenderName; }
            private set { _defaultSenderName = value; }
        }
        /// <summary>
        /// 获取一个值，该值表示发送时是否以HTML形式发送邮件
        /// </summary>
        public bool IsBodyHtml
        {
            get { return _isBodyHtml; }
            private set { _isBodyHtml = value; }
        }
        /// <summary>
        /// 获取一个值，该值表示是否以异步方式发送电子邮件
        /// </summary>
        public bool IsAsyncSend
        {
            get { return _isAsyncSend; }
            private set { _isAsyncSend = value; }
        }

        #region [protected internal method]
        /// <summary>
        /// 获取一个值，该值表示邮件发送组件的配置文件路径
        /// </summary>
        protected internal override string ConfigFilePath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory + "Configs\\SMTPConfig.xml"; }
        }

        /// <summary>
        /// 获取一个值，该值表示邮件发送组件的配置文件根名称
        /// </summary>
        protected internal override string RootElement
        {
            get { return "SMTPConfig"; }
        }
        #endregion
    }
}