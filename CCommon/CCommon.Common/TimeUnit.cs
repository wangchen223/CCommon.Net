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
        /// 转换为微秒
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static long toMicros(this TimeUnit value,long number)
        {
            int microsToMicro = 10000;//1毫秒=10000微秒
            return LongHelper.SaturatedMultiply(Convert.ToInt32(value) * microsToMicro, number);
        }

        /// <summary>
        /// 转换为毫秒
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static long toMicroseconds(this TimeUnit value, long number)
        {
            double microsToMicroseconds = 1;//1毫秒=1秒
            return LongHelper.SaturatedMultiply(Convert.ToInt64(Convert.ToDouble(value) * microsToMicroseconds), number);
        }

        /// <summary>
        /// 转换为秒
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static long toSeconds(this TimeUnit value, long number)
        {
            double microsToSeconds = 1/1000d;//1毫秒=1秒
            return LongHelper.SaturatedMultiply(Convert.ToInt64(Convert.ToDouble(value) * microsToSeconds), number);
        }
    }
}
