using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCommon.Common
{
    /// <summary>
    /// Json助手类
    /// </summary>

    public static class JsonHelper
    {


        /// <summary>
        /// json默认设置
        /// </summary>
        private static JsonSerializerSettings _defaultSettings;
        

        /// <summary>
        /// 构造函数
        /// </summary>
        static JsonHelper()
        {
            _defaultSettings = new JsonSerializerSettings();
            _defaultSettings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
            _defaultSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
            _defaultSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            _defaultSettings.Converters.Add(new IsoDateTimeConverter {
                DateTimeFormat= "yyyy-MM-dd HH:mm:ss,ffff"
            });
        }

        /// <summary>
        /// Json序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            return ToJson(obj, _defaultSettings);
        }

        /// <summary>
        /// Json序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj,string dateTimeFormat)
        {
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
            jsonSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
            jsonSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            jsonSettings.Converters.Add(new IsoDateTimeConverter {
                DateTimeFormat= dateTimeFormat
            });

            return ToJson(obj, jsonSettings);
        }

        /// <summary>
        /// Json序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, JsonSerializerSettings jsonSettings)
        {
            return JsonConvert.SerializeObject(obj, jsonSettings);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ToObject<T>(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(str, _defaultSettings);
        }
    }
}
