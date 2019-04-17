using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCommon.Common
{
    public enum TimeUnit
    {
        /// <summary>
        /// 毫秒
        /// </summary>
        Microseconds=1,
        /// <summary>
        /// 秒
        /// </summary>
        Seconds= Microseconds*1000, //1秒=1000毫秒
        /// <summary>
        /// 分
        /// </summary>
        Minutes= Seconds*60//1分钟=60秒

    }

    public static class ExTimeUnit
    {
        /// <summary>
        /// 转换为微妙
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static long toMicros(this TimeUnit value,long number)
        {
            int microsToMicro = 10000;//1毫秒=10000微妙
            return LongHelper.SaturatedMultiply(Convert.ToInt32(value) * microsToMicro, number);
        }
    }
}
