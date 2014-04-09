using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MrHuo.Controls.MVC
{
    /// <summary>
    /// 表示需要用户登录才可以使用的特性
    /// <para>如果不需要处理用户登录，则请指定AllowAnonymousAttribute属性</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AuthorizationAttribute : FilterAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public AuthorizationAttribute()
        {
            this._AuthUrl = AuthorizationConfig.AuthUrl;
            this._AuthSaveKey = AuthorizationConfig.AuthSaveKey;
            this._AuthSaveType = AuthorizationConfig.AuthSaveType;
        }
        /// <summary>
        /// 构造函数重载
        /// </summary>
        /// <param name="authUrl">表示没有登录跳转的登录地址</param>
        public AuthorizationAttribute(String authUrl)
            : this()
        {
            this._AuthUrl = authUrl;
        }
        /// <summary>
        /// 构造函数重载
        /// </summary>
        /// <param name="authUrl">表示没有登录跳转的登录地址</param>
        /// <param name="saveKey">表示登录用来保存登陆信息的键名</param>
        public AuthorizationAttribute(String authUrl, String saveKey)
            : this(authUrl)
        {
            this._AuthSaveKey = saveKey;
            this._AuthSaveType = "Session";
        }
        /// <summary>
        /// 构造函数重载
        /// </summary>
        /// <param name="authUrl">表示没有登录跳转的登录地址</param>
        /// <param name="saveKey">表示登录用来保存登陆信息的键名</param>
        /// <param name="saveType">表示登录用来保存登陆信息的方式</param>
        public AuthorizationAttribute(String authUrl, String saveKey, String saveType)
            : this(authUrl, saveKey)
        {
            this._AuthSaveType = saveType;
        }

        private String _AuthUrl = String.Empty;
        /// <summary>
        /// 获取或者设置一个值，改值表示登录地址
        /// <para>此处定义的此属性为了在特性定义时特殊指定，不同于Config中属性。</para>
        /// </summary>
        public String AuthUrl
        {
            get {
                var url = _AuthUrl.Trim();
                if (AuthorizationConfig.ReturnUrl)
                {
                    url += "?ReturnUrl=" + HttpUtility.UrlEncode(HttpContext.Current.Request.Url.ToString());
                }
                return url;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(RS.get("Authorization_Error_AuthUrl_Is_Null"));
                }
                else
                {
                    _AuthUrl = value.Trim();
                }
            }
        }

        private String _AuthSaveKey = String.Empty;
        /// <summary>
        /// 获取或者设置一个值，改值表示登录用来保存登陆信息的键名
        /// <para>此处定义的此属性为了在特性定义时特殊指定，不同于Config中属性。</para>
        /// </summary>
        public String AuthSaveKey
        {
            get { return _AuthSaveKey.Trim(); }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(RS.get("Authorization_Error_AuthSaveKey_Is_Null"));
                }
                else
                {
                    this._AuthSaveKey = value.Trim();
                }
            }
        }

        private String _AuthSaveType = String.Empty;
        /// <summary>
        /// 获取或者设置一个值，该值表示用来保存登陆信息的方式
        /// <para>此处定义的此属性为了在特性定义时特殊指定，不同于Config中属性。</para>
        /// </summary>
        public String AuthSaveType
        {
            get { return _AuthSaveType.Trim().ToUpper(); }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(RS.get("Authorization_Error_AuthSaveType_Is_Null"));
                }
                else
                {
                    _AuthSaveType = value.Trim();
                }
            }
        }

        /// <summary>
        /// 处理用户登录
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext == null)
            {
                throw new Exception(RS.get("Authorization_Error_Only_Web_Application"));
            }
            else
            {
                switch (AuthSaveType)
                {
                    case "SESSION":
                        if (filterContext.HttpContext.Session == null)
                        {
                            throw new Exception(RS.get("Authorization_Error_Session_Unavailable"));
                        }
                        else if (!filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) && !filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
                        {
                            if (filterContext.HttpContext.Session[_AuthSaveKey] == null)
                            {
                                filterContext.Result = new RedirectResult(_AuthUrl);
                            }
                        }
                        break;
                    case "COOKIE":
                        if (!filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) && !filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
                        {
                            if (filterContext.HttpContext.Request.Cookies[_AuthSaveKey] == null)
                            {
                                filterContext.Result = new RedirectResult(_AuthUrl);
                            }
                        }
                        break;
                    default:
                        throw new ArgumentNullException(RS.get("Authorization_Error_AuthSaveType_Is_Null"));
                }
            }
        }
    }
}
