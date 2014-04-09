using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;

namespace MrHuo.Controls.Email
{
    /// <summary>
    /// 邮件发送插件
    /// </summary>
    public class EmailSender : IDisposable
    {
        private String copyright = "<br /><hr /><div style='font-size:12px;color:gray;'>From <a href='http://www.mrhuo.com' target='_blank'>MrHuo Studio</a> EmailSender!</div>";
        /// <summary>
        /// 发送邮件之前触发此事件
        /// </summary>
        public event EventHandler<SendEmailEventArgs> OnBeginSend;
        /// <summary>
        /// 邮件发送出错时触发此事件
        /// </summary>
        public event EventHandler<SendEmailFaildEventArgs> OnError;
        /// <summary>
        /// 邮件发送完毕时触发此事件
        /// </summary>
        public event EventHandler<SendEmailEventArgs> OnEndSend;

        private SMTPConfig _config = new SMTPConfig();
        /// <summary>
        /// 获取或者设置一个值，该值表示SMTP配置
        /// </summary>
        public SMTPConfig Config
        {
            get
            {
                return _config;
            }
            set
            {
                Check.NullOrEmpty(value, "Config");
                this._config = value;
            }
        }
        /// <summary>
        /// 内部变量，该值表示邮件队列
        /// </summary>
        private Queue<String> _mails = new Queue<String>();

        /// <summary>
        /// 获取或者设置一个值，该值表示批量发送时设置的邮件主题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 获取或者设置一个值，该值表示批量发送时设置的邮件内容
        /// </summary>
        public string EmailBody { get; set; }
        /// <summary>
        /// 获取或者设置一个值，该值表示附件列表
        /// </summary>
        public List<Attachment> Attachments { get; set; }
        /// <summary>
        /// 增加一个邮件接收者
        /// </summary>
        /// <param name="email">email地址</param>
        public void AddReceiver(string email)
        {
            Check.NullOrEmpty(email, "email");
            this._mails.Enqueue(email);
        }

        /// <summary>
        /// 默认发送方法
        /// </summary>
        public void Send()
        {
            if (_mails.Count <= 0)
            {
                throw new Exception(RS.get("Email_Error_No_Receiver"));
            }
            while (_mails.Count != 0)
            {
                if (Config.IsAsyncSend)
                {
                    Thread t = new Thread(new ThreadStart(() =>
                    {
                        var mail = _mails.Dequeue();
                        this.Send(mail);
                    }));
                    t.Start();
                    Thread.Sleep(10);
                }
                else
                {
                    var mail = _mails.Dequeue();
                    this.Send(mail);
                }
            }
        }

        /// <summary>
        /// 发送方法重载，适用于自定义邮件的发送
        /// </summary>
        /// <param name="receiver">EmailReceiver</param>
        public void Send(String receiver)
        {
            try
            {
                Check.NullOrEmpty(receiver, "receiver");
               
                if (this.OnBeginSend != null)
                {
                    this.OnBeginSend(this, new SendEmailEventArgs(receiver));
                }
                _sendEmail(receiver, this.Subject, this.EmailBody, Config.IsBodyHtml, Config.Encoding, this.Attachments);
            }
            catch (Exception ex)
            {
                if (this.OnError != null)
                {
                    this.OnError(this, new SendEmailFaildEventArgs(receiver, ex));
                }
            }
        }

        /// <summary>
        /// 内部发送邮件入口
        /// </summary>
        /// <param name="receiverEmail">邮件接收者邮件地址</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="emailBody">邮件内容</param>
        /// <param name="isHtml">是否HTML编码</param>
        /// <param name="encoding">编码方式</param>
        /// <param name="attachments">附件列表</param>
        private void _sendEmail(string receiverEmail, string subject, string emailBody, bool isHtml, Encoding encoding, List<Attachment> attachments)
        {
            MailAddress sender = null;
            MailAddress receiver = null;

            Check.NullOrEmpty(Config.DefaultSender, "Config.DefaultSender");
            Check.NullOrEmpty(receiverEmail, "receiverEmail");

            sender = new MailAddress(Config.DefaultSender, Config.DefaultSenderName);
            receiver = new MailAddress(receiverEmail);

            using (MailMessage mail = new MailMessage())
            {
                mail.From = sender;
                mail.To.Add(receiver);
                mail.Subject = this.Subject;
                mail.Body = this.EmailBody + copyright;
                mail.SubjectEncoding = mail.BodyEncoding = Config.Encoding;
                mail.IsBodyHtml = Config.IsBodyHtml;
                mail.Priority = MailPriority.Normal;
                if (Attachments != null)
                {
                    foreach (var attach in Attachments)
                    {
                        attach.ContentId = Guid.NewGuid().ToString();
                        mail.Attachments.Add(attach);
                    }
                }
                using (SmtpClient client = new SmtpClient()
                {
                    Host = Config.SMTPServer,
                    Port = Config.Port,
                    Credentials = new NetworkCredential(Config.UserName, Config.UserPassword),
                    EnableSsl = Config.EnabledSSL,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                })
                {
                    client.Send(mail);

                    if (this.OnEndSend != null)
                    {
                        this.OnEndSend(this, new SendEmailEventArgs(receiverEmail));
                    }
                }
            }
        }

        /// <summary>
        /// 释放系统资源
        /// </summary>
        public void Dispose()
        {
            if (OnBeginSend != null)
            {
                GC.SuppressFinalize(OnBeginSend);
                GC.ReRegisterForFinalize(OnBeginSend);
                OnBeginSend = null;
            }
            if (OnError != null)
            {
                GC.SuppressFinalize(OnError);
                GC.ReRegisterForFinalize(OnError);
                OnError = null;
            }
            if (OnEndSend != null)
            {
                GC.SuppressFinalize(OnEndSend);
                GC.ReRegisterForFinalize(OnEndSend);
                OnEndSend = null;
            }
            if (Config!=null)
            {
                GC.SuppressFinalize(Config);
                GC.ReRegisterForFinalize(Config);
                Config = null;
            }
            if (_mails != null && _mails.Count > 0)
            {
                _mails.Clear();
                GC.SuppressFinalize(_mails);
                GC.ReRegisterForFinalize(_mails);
            }
            if (Subject != null)
            {
                GC.SuppressFinalize(Subject);
                GC.ReRegisterForFinalize(Subject);
                Subject = null;
            }
            if (EmailBody != null)
            {
                GC.SuppressFinalize(EmailBody);
                GC.ReRegisterForFinalize(EmailBody);
                EmailBody = null;
            }
            GC.Collect();
        }
    }
}