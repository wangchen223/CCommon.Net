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
        /// 文件加载
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="pathChains"></param>
        /// <param name="path"></param>
        public void Load(HashSet<string> pathChains, string path)
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
