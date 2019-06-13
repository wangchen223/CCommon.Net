using System;
using System.Collections.Generic;
using System.Text;

namespace CCommon.Common.Config
{
    public class ConfigurationBuild: IConfigurationBuild
    {
        private bool _isExceptionTip = false;
        List<IConfigurationProvider> providers = new List<IConfigurationProvider>();
        public IConfiguration Build()
        {
            return new Configuration(providers, _isExceptionTip);
        }

        /// <summary>
        /// 是否提示异常
        /// </summary>
        /// <param name="exceptionTip"></param>
        /// <returns></returns>
        public ConfigurationBuild ExceptionTip(bool exceptionTip)
        {
            _isExceptionTip = exceptionTip;
            return this;
        }

        public ConfigurationBuild AddXmlFile(string path)
        {
            providers.Add(new XmlConfigurationProvider(path));
            return this;
        }

        public ConfigurationBuild AddJsonFile(string path)
        {
            providers.Add(new JsonConfigurationProvider(path));
            return this;
        }
    }
}
