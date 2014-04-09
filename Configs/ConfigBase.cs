using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MrHuo.Controls.Configs
{
    /// <summary>
    /// 简易配置文件操作抽象类
    /// </summary>
    public abstract class ConfigBase : IDisposable
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ConfigBase() { }
        /// <summary>
        /// 以指定文件名初始化一个配置
        /// </summary>
        /// <param name="fileName">配置文件名</param>
        public ConfigBase(string fileName)
        {
            this.configPath = fileName;
        }
        private string configPath=null;
        /// <summary>
        /// 设置配置文件路径
        /// </summary>
        /// <param name="path"></param>
        public void SetConfigPath(string path)
        {
            this.configPath = path;
        }
        /// <summary>
        /// 只读属性，获取ConfigFile路径
        /// </summary>
        abstract protected internal string ConfigFilePath { get; }
        /// <summary>
        /// 只读属性，获取ConfigFile文件中XML根键
        /// </summary>
        abstract protected internal string RootElement { get; }
        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="value">配置值</param>
        public virtual void SetConfig(string key, string value)
        {
            var p = string.Empty;
            if (string.IsNullOrEmpty(configPath))
                p = ConfigFilePath;
            else
                p = configPath;

            XDocument doc = XDocument.Load(p);
            Check.NullOrEmpty(ConfigFilePath, "ConfigFilePath");
            Check.NullOrEmpty(RootElement, "RootElement");

            var el = (
                from x in doc.Elements(RootElement).Elements<XElement>()
                where x.Name == key
                select x
                ).Single<XElement>();
            Check.NullOrEmpty(el, "el");

            el.SetValue(value);
            doc.Save(ConfigFilePath);
        }
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="key">配置键</param>
        /// <returns></returns>
        public virtual string GetConfig(string key)
        {
            Check.NullOrEmpty(ConfigFilePath, "ConfigFilePath");
            Check.NullOrEmpty(RootElement, "RootElement");

            var v = selectElement(key);
            Check.NullOrEmpty(v, "v");
            return v.Value;
        }
        /// <summary>
        /// 内部类用于选择键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private XElement selectElement(string key)
        {
            var p = string.Empty;
            if (string.IsNullOrEmpty(configPath))
                p = ConfigFilePath;
            else
                p = configPath;

            XDocument doc = XDocument.Load(p);
            var el = (
                from x in doc.Elements(RootElement).Elements<XElement>()
                where x.Name == key
                select x
                ).Single<XElement>();
            return el;
        }
        /// <summary>
        /// 虚方法，获取当前配置文件下所有配置节信息
        /// </summary>
        public virtual Dictionary<String, Object> AllConfigs
        {
            get
            {
                Dictionary<String, Object> ret = new Dictionary<string, object>();
                var p = string.Empty;
                if (string.IsNullOrEmpty(configPath))
                    p = ConfigFilePath;
                else
                    p = configPath;

                XDocument doc = XDocument.Load(p);
                var el = from x in doc.Elements(RootElement).Elements<XElement>() select x;
                foreach (var e in el)
                {
                    ret.Add(e.Name.LocalName, e.Value);
                }
                return ret;
            }
        }
        /// <summary>
        /// 释放系统资源
        /// </summary>
        public void Dispose()
        {
            if (configPath != null)
            {
                GC.ReRegisterForFinalize(configPath);
            }
            if (ConfigFilePath != null)
            {
                GC.ReRegisterForFinalize(ConfigFilePath);
            }
            if (RootElement != null)
            {
                GC.ReRegisterForFinalize(RootElement);
            }
            GC.Collect();
        }
    }
}
