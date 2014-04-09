using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace MrHuo.Controls.Email
{
    /// <summary>
    /// 邮件发送事件参数
    /// </summary>
    public class SendEmailEventArgs : EventArgs
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public SendEmailEventArgs() { }
        /// <summary>
        /// 以邮件地址初始化属性值的构造函数
        /// </summary>
        /// <param name="mailAddress">电子邮件地址</param>
        public SendEmailEventArgs(string mailAddress)
        {
            this.MailAddress = mailAddress;
        }
        /// <summary>
        /// 获取或者设置一个值，该值表示当前处理中的邮件地址
        /// </summary>
        public string MailAddress { get; set; }
    }

    /// <summary>
    /// 邮件发送出错事件处理的参数
    /// </summary>
    public class SendEmailFaildEventArgs : SendEmailEventArgs
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public SendEmailFaildEventArgs() {  }
        /// <summary>
        /// 构造函数重载
        /// </summary>
        /// <param name="mailAddress">MailMessage</param>
        /// <param name="exception">Exception</param>
        public SendEmailFaildEventArgs(string mailAddress, Exception exception)
        {
            this.MailAddress = mailAddress;
            this.Exception = exception;
        }
        /// <summary>
        /// 获取或者设置一个值，该值表示发送过程中发生异常的异常类
        /// </summary>
        public Exception Exception { get; set; }
    }
}