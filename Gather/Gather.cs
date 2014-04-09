using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MrHuo.Controls.Gather
{
    /// <summary>
    /// 通用简易采集组件
    /// </summary>
    public class Gather : IDisposable
    {
        /// <summary>
        /// 获取或者设置一个值，该值表示用于采集时的正则表达式
        /// </summary>
        public String RegexPattern { get; set; }
        private RegexOptions regexOptions = RegexOptions.IgnoreCase | RegexOptions.Multiline;
        /// <summary>
        /// 获取或者设置一个值，该值表示用于采集时的正则表达式匹配选项
        /// </summary>
        public RegexOptions RegexOptions
        {
            get
            {
                return regexOptions;
            }
            set
            {
                regexOptions = value;
            }
        }
        /// <summary>
        /// 获取或者设置一个值，该值表示采集的URL
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 执行采集之前执行的事件
        /// </summary>
        public event Action OnBeginCollect;
        /// <summary>
        /// 正在执行采集中前执行的事件
        /// </summary>
        public event Action<Match> OnCollecting;
        /// <summary>
        /// 完成采集时执行的事件
        /// </summary>
        public event Action OnEndCollect;
        /// <summary>
        /// 采集过程中出现异常或者采集操作被取消的时候会执行
        /// </summary>
        public event Action<Exception> OnError;
        /// <summary>
        /// 内部采集方式
        /// </summary>
        /// <param name="url"></param>
        /// <param name="completed"></param>
        private void _collect(string url, DownloadDataCompletedEventHandler completed)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadDataCompleted += completed;
                client.DownloadDataAsync(new Uri(url), client);
            }
        }
        /// <summary>
        /// 采集方法
        /// </summary>
        public void Collect()
        {
            if (OnBeginCollect != null)
                OnBeginCollect();

            _collect(Url, new DownloadDataCompletedEventHandler((s, e) =>
            {
                if (e.Error != null)
                {
                    if (this.OnError != null)
                        this.OnError(e.Error);
                }
                else if (e.Cancelled)
                {
                    if (this.OnError != null)
                        this.OnError(new Exception("操作已被取消！"));
                }
                else if (e.Result == null)
                {
                    if (this.OnError != null)
                        this.OnError(new Exception("服务器没有返回任何数据！"));
                }
                else
                {
                    var html = Encoding.UTF8.GetString(e.Result);
                    if (RegexPattern != null)
                    {
                        Regex regex = new Regex(RegexPattern, RegexOptions);
                        var matchs = regex.Matches(html);
                        foreach (Match item in matchs)
                        {
                            if (this.OnCollecting != null)
                                this.OnCollecting(item);
                        }
                    }

                    if (OnEndCollect != null)
                        OnEndCollect();
                }
            }));
        }

        /// <summary>
        /// 释放系统资源
        /// </summary>
        public void Dispose()
        {
            if (!String.IsNullOrEmpty(RegexPattern))
            {
                GC.ReRegisterForFinalize(RegexPattern);
            }
            if (!String.IsNullOrEmpty(Url))
            {
                GC.ReRegisterForFinalize(Url);
            }
            if (OnBeginCollect != null)
            {
                GC.ReRegisterForFinalize(OnBeginCollect);
            }
            if (OnCollecting != null)
            {
                GC.ReRegisterForFinalize(OnCollecting);
            }
            if (OnEndCollect != null)
            {
                GC.ReRegisterForFinalize(OnEndCollect);
            }
            if (OnError != null)
            {
                GC.ReRegisterForFinalize(OnError);
            }
            GC.Collect();
        }
    }
}