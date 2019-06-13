using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCommon.Common.Config.ResultValue
{
    public class JsonValue : IValue
    {
        private string _value;
        public JsonValue(string value)
        {
            _value = value;
        }
        public T ToObject<T>()
        {
            if (typeof(T) == typeof(String))
            {
                return (T)Convert.ChangeType(_value, typeof(T));
            }
            return _value.ToObject<T>();
        }
    }
}
