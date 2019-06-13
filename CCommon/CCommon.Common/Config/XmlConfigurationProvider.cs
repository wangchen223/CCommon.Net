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
            //解析文件
            string filePath = "";
            filePath = System.IO.Path.Combine(Assembly.GetExecutingAssembly().CodeBase, path);
            if (!File.Exists(filePath))
            {
                filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            }
            _path = filePath;
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

        public void Load()
        {            
            if (File.Exists(_path))
            {

                var result = ErrorRetryHelper.Handle("1,1,1", () =>
                {
                    lock (lockObj)
                    {
                        ReSetting();
                        return true;
                    };
                });
                if (!result.IsValid)
                {
                    throw result.Exception;
                }
                new FileWatcherHelper(_path, ReSetting);
            }
        }

        #region 私有方法
        /// <summary>
        /// 设置
        /// </summary>
        private void ReSetting()
        {

            _data.Clear();
            HashSet<string> pathChains = new HashSet<string>();
            if (File.Exists(_path))
            {
                Load(pathChains, _path);
            }
        }

        /// <summary>
        /// 文件加载
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

        #endregion

    }
}
