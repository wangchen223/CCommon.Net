using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCommon.Common.Config.ResultValue
{
    public interface IValue
    {
        T ToObject<T>();
    }
}
