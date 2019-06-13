using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Linq;
namespace CCommon.Common.Config
{
    public class Configuration: IConfiguration
    {
        private bool _isExceptionTip = false;
        private List<IConfigurationProvider> _providers = new List<IConfigurationProvider>();
        public IEnumerable<IConfigurationProvider> Providers { get { return _providers; } }
        private readonly static object lockObj = new object();
        public Configuration(List<IConfigurationProvider> providers,bool isExceptionTip=false)
        {
            _isExceptionTip = isExceptionTip;
            _providers = providers.OrderByDescending(info => info.Order).ToList();
            ReLoad();
        }

        public void ReLoad()
        {
            foreach (var item in Providers)
            {
                item.Load();
            }
        }
        /// <summary>
        /// 获得配置节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetValue<T>(string key)
        {
            return GetValue(key, default(T));
        }


        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValue<T>(string key, T defaultValue)
        {
            T value = defaultValue;

            foreach (var item in _providers)
            {
                try
                {
                    if (item.Get(key) != null)
                    {
                        value = item.Get(key).ToObject<T>();
                    }
                }
                catch (Exception ex)
                {
                    if (_isExceptionTip)
                    {
                        throw ex;
                    }
                }
            }
            return value;
        }

    }
}
