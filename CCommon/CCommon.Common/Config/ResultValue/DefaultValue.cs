using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCommon.Common.Config.ResultValue
{
    public class DefaultValue : IValue
    {
        private string _value;
        public DefaultValue(string value)
        {
            _value = value;
        }

        public T ToObject<T>()
        {
            return (T)Convert.ChangeType(_value, typeof(T));
        }
    }
}
