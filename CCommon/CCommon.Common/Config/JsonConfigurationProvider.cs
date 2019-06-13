using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using CCommon.Common.Config.ResultValue;

namespace CCommon.Common.Config
{
    public class JsonConfigurationProvider: IConfigurationProvider
    {
        private string _path = string.Empty;
        private object lockObj = new object();

        Dictionary<string, IValue> _data = new Dictionary<string, IValue>();
        public Dictionary<string, IValue> Data
        {
            get { return _data; }
        }
        public JsonConfigurationProvider(string path)
        {
            // 解析文件
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

                var result=ErrorRetryHelper.Handle("1,1,1", () =>
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
                string fileContent = string.Empty;
                using (StreamReader fsIn = new StreamReader(path, Encoding.UTF8))
                {
                    fileContent = fsIn.ReadToEnd();
                }

                var jsonDicss = JsonConvert.DeserializeObject<JObject>(fileContent);
                Dictionary<string, object> jsonDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(fileContent);

                foreach (KeyValuePair<string, object> item in jsonDic)
                {
                    string key = item.Key;
                    string value = item.Value.ToString();
                    if (!string.IsNullOrEmpty(key) && !_data.ContainsKey(key))
                    {
                        if (item.Value is JObject)
                        {
                            _data.Add(key, new JsonValue(value));
                        }
                        else
                        {
                            _data.Add(key, new DefaultValue(value));
                        }
                    }
                }

                if (jsonDic.Keys.Contains("AppSettings_Property"))
                {
                    Dictionary<string, string> appSettings_Property = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDic["AppSettings_Property"].ToString());

                    if (appSettings_Property.ContainsKey("childConfigSource") && !string.IsNullOrEmpty(appSettings_Property["childConfigSource"]))
                    {
                        foreach (string childPth in appSettings_Property["childConfigSource"].Split(','))
                        {
                            var parantPath = System.IO.Path.Combine(System.IO.Directory.GetParent(path).FullName, childPth);

                            //如果没有加载过该配置则继续加载，防止死循环
                            if (!pathChains.Contains(parantPath))
                                Load(pathChains, parantPath);
                        }
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
