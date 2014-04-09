using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;

namespace MrHuo.Controls.SMS
{
    /// <summary>
    /// 短信发送接口
    /// </summary>
    public class SMSSender : IDisposable
    {
        private SMSSender()
        {
            Config = new SMSConfig();
        }

        /// <summary>
        /// 返回一个配置文件
        /// </summary>
        public static SMSConfig Config = null;

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="mobile">电话号码</param>
        /// <param name="content">短信内容</param>
        /// <param name="callback">发送完毕后执行的回调函数</param>
        /// <returns>string</returns>
        public static void Send(string mobile, string content, Action<String, Exception> callback=null)
        {
            Check.NullOrEmpty(mobile, "mobile");
            Check.NullOrEmpty(content, "content");

            string url = Config.UrlFormat;
            url = url.Replace("{User}", Config.User);
            url = url.Replace("{Key}", Config.Key);
            url = url.Replace("{Mobile}", mobile);
            url = url.Replace("{Content}", content);

            WebClient client = new WebClient();
            client.DownloadDataCompleted += (result, error) =>
            {
                var ret = Encoding.UTF8.GetString(error.Result);
                if (callback != null)
                {
                    callback(ret, error.Error ?? error.Error);
                }
            };
            client.DownloadDataAsync(new Uri(url), null);
        }

        /// <summary>
        /// 释放系统资源
        /// </summary>
        public void Dispose()
        {
            if (Config != null)
            {
                GC.ReRegisterForFinalize(Config);
            }
            GC.Collect();
        }
    }
}