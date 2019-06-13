using CCommon.Common.Config.ResultValue;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace CCommon.Common.Config
{
    public class XmlConfigurationProvider : IConfigurationProvider
    {
        private string _path = string.Empty;
        private object lockObj = new object();
        Dictionary<string, IValue> _data = new Dictionary<string, IValue>();
        public Dictionary<string, IValue> Data
        {
            get { return _data; }
        }
        public XmlConfigurationProvider(string path)
        {
            _path = path;
        }
        public void Load()
        {
            //解析文件
            string filePath = "";
            filePath = System.IO.Path.Combine(Assembly.GetExecutingAssembly().CodeBase, _path);
            if (!File.Exists(filePath))
            {
                filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _path);
            }

            if (File.Exists(filePath))
            {
                lock (lockObj)
                {
                    int errorCount = 0;
                    while (errorCount < 3)
                    {
                        try
                        {
                            HashSet<string> pathChains = new HashSet<string>();

                            Load(pathChains, _path);

                            //写缓存
                            //CacheDependency dp = new CacheDependency(pathChains.ToArray());//建立缓存依赖项dp
                            //HttpRuntime.Cache.Insert(AppSettingsConfigCacheKey, dic, dp);
                            errorCount = 100;
                        }
                        catch (Exception ex)
                        {
                            //Plat.Util.DataLogHelper.D("AppSettingsConfigError:" + ex.Message + ex.StackTrace);
                            errorCount++;
                            if (errorCount == 3)
                            {
                                throw ex;
                            }
                            else
                            {
                                System.Threading.Thread.Sleep(1000);
                            }
                        }
                    }

                }

            }
        }

        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="pathChains"></param>
        /// <param name="path"></param>
        private void Load(HashSet<string> pathChains, string path)
        {
            try
            {
                XDocument doc;
                using (FileStream fsIn = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    doc = XDocument.Load(fsIn);
                }
                foreach (XElement element in doc.Element("appSettings").Descendants("add"))
                {
                    string key = element.Attribute("key").Value;
                    string value = element.Attribute("value").Value;
                    if (!string.IsNullOrEmpty(key) && !_data.ContainsKey(key))
                    {
                        _data.Add(key, new DefaultValue(value));
                    }
                }


                var childConfigSource = doc.Element("appSettings").Attribute("childConfigSource");
                if (childConfigSource != null && childConfigSource.Value != null)
                {
                    foreach (string childPth in childConfigSource.Value.Split(','))
                    {
                        var parantPath = System.IO.Path.Combine(System.IO.Directory.GetParent(path).FullName, childPth);

                        //如果没有加载过该配置则继续加载，防止死循环
                        if (!pathChains.Contains(parantPath))
                            Load(pathChains, parantPath);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public int Order
        {
            get { return 0; }
        }

        public IValue Get(string key)
        {
            if (_data.ContainsKey(key))
            {
                return _data[key];
            }
            else
            {
                return null;
            }
        }
    }
}
