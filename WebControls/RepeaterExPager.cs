using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.HtmlControls;
using System.Text;

namespace MrHuo.Controls.WebControls
{
    /// <summary>
    /// RepeaterEx分页组件
    /// </summary>
    public class RepeaterExPager : Label
    {
        #region [Privates]
        private int _maxRecord
        {
            get
            {
                return RepeaterExControl != null ? RepeaterExControl.MaxRecord : 0;
            }
        }
        private int _maxPages
        {
            get
            {
                return RepeaterExControl != null ? RepeaterExControl.MaxPages : 0;
            }
        }
        private int _pageSize
        {
            get
            {
                return RepeaterExControl != null ? RepeaterExControl.PageSize : 10;
            }
        }

        /// <summary>
        /// 内部变量，用于处理页码Url
        /// </summary>
        private String GetCurrentFullUrl
        {
            get
            {
                var url = HttpContext.Current.Request.Url;
                var u = String.Empty;
                Regex reg = new Regex(@"page=(\w+)", RegexOptions.IgnoreCase);
                if (reg.IsMatch(url.PathAndQuery))
                {
                    Match match = reg.Matches(url.PathAndQuery)[0];
                    u = url.PathAndQuery.Replace("?" + match.Groups[0].Value, String.Empty).Replace("&" + match.Groups[0].Value, String.Empty);
                }
                else
                {
                    u = url.PathAndQuery;
                }
                return u.Contains("?") ? u + "&Page=" : u + "?Page=";
            }
        }

        private bool showPageInfo = true;

        private RepeaterEx repeaterExControl = null;

        /// <summary>
        /// 内部变量，获取当前页数
        /// </summary>
        private Int32 CurrentPageIndex
        {
            get
            {
                int page = 1;
                try
                {
                    var reqPage = HttpContext.Current.Request["Page"];
                    if (reqPage != null && Int32.TryParse(reqPage, out page))
                    {
                        page = page > _maxPages ? _maxPages : (page < 1 ? 1 : page);
                    }
                }
                catch { }
                return page;
            }
        }
        #endregion

        /// <summary>
        /// 获取或者设置一个值，该值表示是否显示分页信息
        /// </summary>
        [Description("是否显示分页信息，即统计信息，默认true。")]
        public bool ShowPageInfo { get { return showPageInfo; } set { showPageInfo = value; } }

        /// <summary>
        /// 获取或者设置一个值，该值表示是否显示更多页
        /// </summary>
        [Description("是否显示更多页，默认false。（中间页码，暂不提供）")]
        public bool ShowMorePage { get; set; }

        /// <summary>
        /// 获取或者设置一个值，该值表示需要分页的RepeaterEx控件
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("指定一个需要分页的RepeaterEx控件")]
        public RepeaterEx RepeaterExControl
        {
            get
            {
                Check.NullOrEmpty(repeaterExControl, "RepeaterExControl");
                return repeaterExControl;
            }
            set
            {
                this.repeaterExControl = value;
            }
        }

        private bool showPagerText = true;
        /// <summary>
        /// 获取或者设置一个值，该值表示是否显示分页中的链接文字
        /// </summary>
        [Description("是否显示分页中的链接文字")]
        public bool ShowPagerText
        {
            get { return showPagerText; }
            set { showPagerText = value; }
        }

        private PagerShowKind pagerShowKind = PagerShowKind.Auto;
        /// <summary>
        /// 获取或者设置一个值，该值设置分页显示方式
        /// </summary>
        [Description("设置分页显示方式")]
        public PagerShowKind PagerShowKind { get; set; }

        /// <summary>
        /// 渲染控件
        /// </summary>
        /// <param name="writer"></param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (pagerShowKind == PagerShowKind.Auto && _maxRecord <= _pageSize)
            {
                return;
            }
            StringBuilder sb = new StringBuilder();
            if (ShowPageInfo)
                sb.AppendLine(getLinkHtml(PagerType.PageInfo));
            sb.AppendLine(getLinkHtml(PagerType.FirstPage));
            sb.AppendLine(getLinkHtml(PagerType.PrePage));
            if (ShowMorePage)
                sb.AppendLine(getLinkHtml(PagerType.MorePage));
            sb.AppendLine(getLinkHtml(PagerType.NextPage));
            sb.AppendLine(getLinkHtml(PagerType.LastPage));

            writer.Write(sb.ToString());
        }

        /// <summary>
        /// 以DIV元素显示
        /// </summary>
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        enum PagerType
        {
            FirstPage,
            PrePage,
            NextPage,
            LastPage,
            MorePage,
            PageInfo
        }

        private string getLinkHtml(PagerType type)
        {
            string linkTemplate = "<a href=\"{0}\" class=\"{1}\">{2}</a>";
            string pageInfoTemplate = RS.get("RepeaterEx_Pager_PageInfoTemplate");
            string link = string.Empty;
            string @class = string.Empty;
            string text = string.Empty;

            switch (type)
            {
                case PagerType.FirstPage:
                    link = CurrentPageIndex <= 1 ? "javascript:;" : GetCurrentFullUrl + "1";
                    @class = "firstPage";
                    text = ShowPagerText ? RS.get("RepeaterEx_Pager_FirstPageText") : "";
                    break;
                case PagerType.PrePage:
                    link = CurrentPageIndex <= 1 ? "javascript:;" : GetCurrentFullUrl + (CurrentPageIndex - 1);
                    @class = "prePage";
                    text = ShowPagerText ? RS.get("RepeaterEx_Pager_PrePageText") : "";
                    break;
                case PagerType.NextPage:
                    link = CurrentPageIndex >= _maxPages ? "javascript:;" : GetCurrentFullUrl + (CurrentPageIndex + 1);
                    @class = "nextPage";
                    text = ShowPagerText ? RS.get("RepeaterEx_Pager_NextPageText") : "";
                    break;
                case PagerType.LastPage:
                    link = _maxPages == 0 ? "javascript:;" : (GetCurrentFullUrl + _maxPages);
                    @class = "lastPage";
                    text = ShowPagerText ? RS.get("RepeaterEx_Pager_LastPageText") : "";
                    break;
                case PagerType.PageInfo:
                    return string.Format(pageInfoTemplate + "&nbsp;&nbsp;", _maxRecord, _maxPages == 0 ? 0 : CurrentPageIndex, _maxPages);
                default:
                    break;
            }

            return string.Format(linkTemplate, link, @class, text);
        }
    }

    /// <summary>
    /// 分页显示方式
    /// </summary>
    public enum PagerShowKind
    {
        /// <summary>
        /// 永远显示
        /// </summary>
        Allways,
        /// <summary>
        /// 当前记录数少于一页时不显示
        /// </summary>
        Auto
    }
}