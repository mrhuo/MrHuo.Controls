using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MrHuo.Controls.MVC
{
    /// <summary>
    /// Mvc Authorization 特性配置文件（验证用户登录）
    /// </summary>
    public class AuthorizationConfig : Configs.ConfigBase
    {
        #region [Configs.ConfigBase's Members]
        /// <summary>
        /// 获取一个值，该值表示配置文件物理路径
        /// </summary>
        protected internal override string ConfigFilePath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory + "Configs\\AuthorizationConfig.xml"; }
        }

        /// <summary>
        /// 获取一个值，该值表示配置文件根键名称
        /// </summary>
        protected internal override string RootElement
        {
            get { return "AuthorizationConfig"; }
        }
        #endregion

        static AuthorizationConfig()
        {
            AuthorizationConfig _instance = new AuthorizationConfig();
            AuthUrl = _instance.GetConfig("AuthUrl");
            ReturnUrl = bool.Parse(_instance.GetConfig("ReturnUrl"));
            AuthSaveKey = _instance.GetConfig("AuthSaveKey");
            AuthSaveType = _instance.GetConfig("AuthSaveType");

            Check.NullOrEmpty(AuthUrl, "AuthUrl");
            Check.NullOrEmpty(ReturnUrl, "ReturnUrl");
            Check.NullOrEmpty(AuthSaveKey, "AuthSaveKey");
            Check.NullOrEmpty(AuthSaveType, "AuthSaveType");

            if (!AuthSaveType.Equals("SESSION", StringComparison.CurrentCultureIgnoreCase) && !AuthSaveType.Equals("COOKIE", StringComparison.CurrentCultureIgnoreCase))
            {
                throw new ArgumentException("配置文件中AuthSaveKey的值，只能为【SESSION/COOKIE】");
            }
        }

        /// <summary>
        /// 只读属性，该值表示验证登录失败后跳转的地址
        /// </summary>
        public readonly static string AuthUrl = string.Empty;

        /// <summary>
        /// 只读属性，该值表示验证登录失败跳转时是否代入验证失败时的地址
        /// <para>比如：在/News/Append这个地址验证失败，则会跳转到<paramref name="AuthUrl"/>指定的Url,带一个ReturnUrl的请求参数。</para>
        /// <para>如：<paramref name="AuthUrl"/>指定的Url为/User/Login,则跳转后的地址为：http://{host}/User/Login?ReturnUrl=/News/Append.</para>
        /// <para>方便了开发者自定义开发</para>
        /// </summary>
        public readonly static bool ReturnUrl = false;

        /// <summary>
        /// 只读属性，该值表示验证登录是用于保存用户凭据的键名称（即Session或Cookie键名）
        /// </summary>
        public readonly static string AuthSaveKey = string.Empty;

        /// <summary>
        /// 只读属性，该值表示验证登录的方式[Cookie/Session]
        /// </summary>
        public readonly static string AuthSaveType = string.Empty;

        /// <summary>
        /// 用于登录操作，自动加入登录凭据
        /// </summary>
        /// <param name="obj">dynamic类型的登录凭据（Cookie登陆时，只能为String类型数据。）</param>
        public static void AddCredential(dynamic obj)
        {
            Check.NullOrEmpty(obj, "obj");
            switch (AuthSaveKey.ToUpper())
            {
                case "SESSION":
                    HttpContext.Current.Session.Add(AuthSaveKey, obj);
                    break;
                case "COOKIE":
                    if (!(obj is string))
                    {
                        obj = obj.ToString();
                    }
                    HttpContext.Current.Response.Cookies.Add(new HttpCookie(AuthSaveKey, obj));
                    break;
            }
        }

        /// <summary>
        /// 用于退出操作，自动清除登录凭据
        /// </summary>
        public static void RemoveCredential()
        {
            switch (AuthSaveKey.ToUpper())
            {
                case "SESSION":
                    HttpContext.Current.Session.Remove(AuthSaveKey);
                    break;
                case "COOKIE":
                    HttpCookie cookie = HttpContext.Current.Request.Cookies[AuthSaveKey];
                    if (cookie != null)
                    {
                        cookie.Expires = DateTime.MinValue;
                        HttpContext.Current.Response.SetCookie(cookie);
                    }
                    break;
            }
        }

        /// <summary>
        /// 用户获取登录凭据，返回dynamic数据。
        /// <para>开发者注意：登录凭据中加入什么类型的数据，这里就返回什么类型的数据。</para>
        /// </summary>
        /// <returns></returns>
        public static dynamic GetCredential()
        {
            dynamic ret = null;
            switch (AuthSaveKey.ToUpper())
            {
                case "SESSION":
                    ret = HttpContext.Current.Session[AuthSaveKey];
                    break;
                case "COOKIE":
                    ret = HttpContext.Current.Request.Cookies[AuthSaveKey];
                    break;
            }
            return ret;
        }
    }
}