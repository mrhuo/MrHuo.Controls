using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace MrHuo.Controls.MVC
{
    /// <summary>
    /// MVC分页组件扩展类
    /// <para>使用方法：</para>
    /// <para>View页面接受Model:@model PagerOptions<ModelClass></para>
    /// <para>分页位置加入：@Html.Pager(Model)</para>
    /// </summary>
    public static class PagerExtension
    {
        /// <summary>
        /// 分页扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="helper"></param>
        /// <param name="pagerOptions"></param>
        /// <returns></returns>
        public static MvcHtmlString Pager<T>(this HtmlHelper helper, PagerOptions<T> pagerOptions)
        {
            Int32 page = 1;
            String requestPage = helper.ViewContext.HttpContext.Request["page"];
            String currentPageUrl = helper.ViewContext.HttpContext.Request.Url.PathAndQuery.ToLower();
            Regex regexPage = new Regex(@"page=(\d+)", RegexOptions.IgnoreCase);
            if (regexPage.IsMatch(currentPageUrl))
            {
                Match match = regexPage.Matches(currentPageUrl)[0];
                currentPageUrl = currentPageUrl.Replace("?" + match.Groups[0].Value, String.Empty).Replace("&" + match.Groups[0].Value, String.Empty);
            }
            String spliter = currentPageUrl.Contains("?") ? "&" : "?";

            if (requestPage != null)
            {
                try
                {
                    //防止恶意传值
                    page = Convert.ToInt32(requestPage);
                }
                catch { page = 1; }
                page = page > pagerOptions.MaxPage ? pagerOptions.MaxPage : (page <= 0 ? 1 : page);
            }

            StringBuilder sbReturn = new StringBuilder();
            StringBuilder sbPager = new StringBuilder("<div class=\"" + pagerOptions.PagerCssClass + "\">");

            //输出“首页”链接
            sbPager.Append("<a" + (page == 1 ? " href=\"javascript:;\"  class=\"disabled\"" : " href=\"" + currentPageUrl + "\"") + ">首页</a>");

            if (pagerOptions.MaxRecord > pagerOptions.PageSize)
            {
                //只要页数大于1，则出现“上一页”链接
                if (page - 1 > 0)
                {
                    sbPager.Append("<a href=\"" + currentPageUrl + spliter + "Page=" + (page - 1) + "\">上一页</a>");
                }

                //输出指定的中间显示的页数
                if (page + pagerOptions.MorePage <= pagerOptions.MaxPage)
                {
                    for (long i = page; i <= page + pagerOptions.MorePage; i++)
                    {
                        if (i < pagerOptions.MaxPage)
                        {
                            sbPager.Append("<a href=\"" + currentPageUrl + spliter + "Page=" + i + "\" class=\"" + (i == page ? "on" : "") + "\">" + i + "</a>");
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    if (pagerOptions.MaxPage >= pagerOptions.MorePage)
                    {
                        for (long i = pagerOptions.MaxPage - pagerOptions.MorePage + 1; i < pagerOptions.MaxPage; i++)
                        {
                            sbPager.Append("<a href=\"" + currentPageUrl + spliter + "Page=" + i + "\" class=\"" + (i == page ? "on" : "") + "\">" + i + "</a>");
                        }
                    }
                    else
                    {
                        for (long i = 1; i <= pagerOptions.MaxPage; i++)
                        {
                            sbPager.Append("<a href=\"" + currentPageUrl + spliter + "Page=" + i + "\" class=\"" + (i == page ? "on" : "") + "\">" + i + "</a>");
                        }
                    }
                }

                //只要当前导航的页数没有到最后一页，则出现“下一页”链接
                if (page + 1 < pagerOptions.MaxPage)
                {
                    sbPager.Append("<a" + (page >= pagerOptions.MaxPage ? " href=\"javascript:;\"  class=\"disabled\"" : " href=\"" + currentPageUrl + spliter + "Page=" + (page + 1) + "\"") + ">下一页</a>");
                }

            }
            else
            {
                sbPager.Append("<a href=\"" + currentPageUrl + spliter + "Page=1\" class=\"on\">1</a>");

            }
            //输出“末页”链接
            sbPager.Append("<a" + (page >= pagerOptions.MaxPage ? " href=\"javascript:;\"  class=\"disabled\"" : " href=\"" + currentPageUrl + spliter + "Page=" + pagerOptions.MaxPage + "\"") + ">末页</a>");
            sbPager.Append("&nbsp;共" + pagerOptions.MaxPage + "页&nbsp;" + pagerOptions.MaxRecord + "条记录&nbsp;");
            sbPager.Append("<input type=\"text\" id=\"Pages\" class=\"inputNum\" /><input type=\"button\" class=\"btnGo\" onclick='if($(\"#Pages\").val()==\"\"){alert(\"请输入页码！\");}else{if(isNaN($(\"#Pages\").val())){alert(\"页码必须输入数字！\");}else{if(!($(\"#Pages\").val()>" + pagerOptions.MaxPage + ")){location.href=\"" + currentPageUrl + spliter + "Page=\"+$(\"#Pages\").val();}else{alert(\"输入的页数不能大于总页数！\");}}}' value=\"跳转\" />");
            sbPager.Append("</div>");

            sbReturn.AppendLine(sbPager.ToString());
            return MvcHtmlString.Create(sbReturn.ToString().Trim());
        }
    }
}
