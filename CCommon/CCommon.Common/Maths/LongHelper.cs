using CCommon.Common.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCommon.Common
{
    

    public class LongHelper
    {
        /// <summary>
        /// Long bit的长度(Long有多少位为1)
        /// 出处:java.lang.Long
        /// </summary>
        /// <param name="p_value"></param>
        /// <returns></returns>
        public static int BitCount(long p_value)
        {
            /*
              //Java 原始代码
              // HD, Figure 5-2
            i = i - ((i >>> 1) & 0x5555555555555555L);
            i = (i & 0x3333333333333333L) + ((i >>> 2) & 0x3333333333333333L);
            i = (i + (i >>> 4)) & 0x0f0f0f0f0f0f0f0fL;
            i = i + (i >>> 8);
            i = i + (i >>> 16);
            i = i + (i >>> 32);
            return (int)i & 0x7f;
             */
            long i = p_value;
            //ulong i = Convert.ToUInt64(p_value);
            // HD, Figure 5-2
            i = i - (LongHelper.UnsignedRightBitMove(i, 1) & 0x5555555555555555L);
            i = (i & 0x3333333333333333L) + (LongHelper.UnsignedRightBitMove(i, 2) & 0x3333333333333333L);
            i = (i + LongHelper.UnsignedRightBitMove(i, 4)) & 0x0f0f0f0f0f0f0f0fL;
            i = i + LongHelper.UnsignedRightBitMove(i, 8);
            i = i + LongHelper.UnsignedRightBitMove(i, 16);
            i = i + LongHelper.UnsignedRightBitMove(i, 32);
            return (int)i & 0x7f;
        }

        /// <summary>
        /// 获取Long高位1的位置(左侧1的位置)
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static int NumberOfTrailingZeros(long p_value)
        {
            /*
             //Java原始代码
             // HD, Figure 5-14
        int x, y;
        if (i == 0) return 64;
        int n = 63;
        y = (int)i; if (y != 0) { n = n -32; x = y; } else x = (int)(i>>>32);
        y = x <<16; if (y != 0) { n = n -16; x = y; }
        y = x << 8; if (y != 0) { n = n - 8; x = y; }
        y = x << 4; if (y != 0) { n = n - 4; x = y; }
        y = x << 2; if (y != 0) { n = n - 2; x = y; }
        return n - ((x << 1) >>> 31);
             */
            long i = p_value;

            // HD, Figure 5-14
            int x, y;
            if (i == 0) return 64;
            int n = 63;
            y = (int)i; if (y != 0) { n = n - 32; x = y; } else x = (int)LongHelper.UnsignedRightBitMove(i, 32);
            y = x << 16; if (y != 0) { n = n - 16; x = y; }
            y = x << 8; if (y != 0) { n = n - 8; x = y; }
            y = x << 4; if (y != 0) { n = n - 4; x = y; }
            y = x << 2; if (y != 0) { n = n - 2; x = y; }
            return n - IntHelper.UnsignedRightBitMove((x << 1), 31);
        }

        /// <summary>
        /// 该方法的作用是返回无符号整型i的最高非零位前面的0的个数，包括符号位在内；
        ///如果i为负数，这个方法将会返回0，符号位为1.
        ///比如说，10的二进制表示为 0000 0000 0000 0000 0000 0000 0000 1010
        ///java的整型长度为32位。那么这个方法返回的就是28
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static int NumberOfLeadingZeros(long i)
        {
            /*
                     // HD, Figure 5-6
         if (i == 0)
            return 64;
        int n = 1;
        int x = (int)(i >>> 32);
        if (x == 0) { n += 32; x = (int)i; }
        if (x >>> 16 == 0) { n += 16; x <<= 16; }
        if (x >>> 24 == 0) { n +=  8; x <<=  8; }
        if (x >>> 28 == 0) { n +=  4; x <<=  4; }
        if (x >>> 30 == 0) { n +=  2; x <<=  2; }
        n -= x >>> 31;
        return n;
             */
            // HD, Figure 5-6
            if (i == 0)
                return 64;
            int n = 1;
            int x = (int)LongHelper.UnsignedRightBitMove(i, 32);
            if (x == 0) { n += 32; x = (int)i; }
            if (LongHelper.UnsignedRightBitMove(x, 16) == 0) { n += 16; x <<= 16; }
            if (LongHelper.UnsignedRightBitMove(x, 24) == 0) { n += 8; x <<= 8; }
            if (LongHelper.UnsignedRightBitMove(x, 28) == 0) { n += 4; x <<= 4; }
            if (LongHelper.UnsignedRightBitMove(x, 30) == 0) { n += 2; x <<= 2; }
            n -= IntHelper.UnsignedRightBitMove(x, 31);
            return n;
        }

        public static void Test()
        {
            



        }

        /// <summary>
        /// Long 相加(防止溢出操作)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static long SaturatedAdd(long a, long b)
        {
            /*
                long naiveSum = a + b;
                if ((a ^ b) < 0 | (a ^ naiveSum) >= 0) {
                  // If a and b have different signs or a has the same sign as the result then there was no
                  // overflow, return.
                  return naiveSum;
                }
                // we did over/under flow, if the sign is negative we should return MAX otherwise MIN
                return Long.MAX_VALUE + ((naiveSum >>> (Long.SIZE - 1)) ^ 1);
             */
            long naiveSum = a + b;
            if ((a ^ b) < 0 | (a ^ naiveSum) >= 0)
            {
                // If a and b have different signs or a has the same sign as the result then there was no
                // overflow, return.
                return naiveSum;
            }
            // we did over/under flow, if the sign is negative we should return MAX otherwise MIN
            return long.MaxValue + (LongHelper.UnsignedRightBitMove(naiveSum, (64 - 1)) ^ 1);
        }

        /// <summary>
        /// Long 相减(防止溢出操作)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static long SaturatedSubtract(long a, long b)
        {
            long naiveDifference = a - b;
            if ((a ^ b) >= 0 | (a ^ naiveDifference) >= 0)
            {
                // If a and b have the same signs or a has the same sign as the result then there was no
                // overflow, return.
                return naiveDifference;
            }
            // we did over/under flow
            return long.MaxValue + (LongHelper.UnsignedRightBitMove(naiveDifference, (64 - 1)) ^ 1);
        }

        /// <summary>
        /// Long 相乘(防止溢出操作)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static long SaturatedMultiply(long a, long b)
        {
            // see checkedMultiply for explanation
            int leadingZeros =
                LongHelper.NumberOfLeadingZeros(a)
                    + LongHelper.NumberOfLeadingZeros(~a)
                    + LongHelper.NumberOfLeadingZeros(b)
                    + LongHelper.NumberOfLeadingZeros(~b);
            if (leadingZeros > 64 + 1)
            {
                return a * b;
            }
            // the return value if we will overflow (which we calculate by overflowing a long :) )
            long limit = long.MaxValue + LongHelper.UnsignedRightBitMove((a ^ b), (64 - 1));
            if (leadingZeros < 64 | (a < 0 & b == long.MinValue))
            {
                // overflow
                return limit;
            }
            long result = a * b;
            if (a == 0 || result / a == b)
            {
                return result;
            }
            return limit;
        }

        /// <summary>
        /// 将value向右进行无符号位移num位
        /// java value>>>num
        /// </summary>
        /// <param name="value"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static long UnsignedRightBitMove(long value, int num)
        {
            if (num != 0)  //移动 0 位时直接返回原值
            {
                long mask = long.MaxValue;     // int.MaxValue = 0x7FFFFFFF 整数最大值
                value >>= 1;               //无符号整数最高位不表示正负但操作数还是有符号的，有符号数右移1位，正数时高位补0，负数时高位补1
                value &= mask;     //和整数最大值进行逻辑与运算，运算后的结果为忽略表示正负值的最高位
                value >>= num - 1;     //逻辑运算后的值无符号，对无符号的值直接做右移运算，计算剩下的位
            }
            return value;
        }

        /// <summary>
        /// 将value向右进行无符号位移num位(未测试)
        /// java value>>>num
        /// </summary>
        /// <param name="value"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static ulong UnsignedRightBitMove(ulong value, int num)
        {
            if (num != 0)  //移动 0 位时直接返回原值
            {
                ulong mask = ulong.MaxValue;     // int.MaxValue = 0x7FFFFFFF 整数最大值
                value >>= 1;               //无符号整数最高位不表示正负但操作数还是有符号的，有符号数右移1位，正数时高位补0，负数时高位补1
                value &= mask;     //和整数最大值进行逻辑与运算，运算后的结果为忽略表示正负值的最高位
                value >>= num - 1;     //逻辑运算后的值无符号，对无符号的值直接做右移运算，计算剩下的位
            }
            return value;
        }
    }
}