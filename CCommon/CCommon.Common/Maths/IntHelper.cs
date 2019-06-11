using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCommon.Common.Maths
{
    public class IntHelper
    {
        /// <summary>
        /// 将value向右进行无符号位移num位
        /// java value>>>num
        /// </summary>
        /// <param name="value"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int UnsignedRightBitMove(int value, int num)
        {
            if (num != 0)  //移动 0 位时直接返回原值
            {
                int mask = int.MaxValue;     // int.MaxValue = 0x7FFFFFFF 整数最大值
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
        public static uint UnsignedRightBitMove(uint value, int num)
        {
            if (num != 0)  //移动 0 位时直接返回原值
            {
                uint mask = uint.MaxValue;     // int.MaxValue = 0x7FFFFFFF 整数最大值
                value >>= 1;               //无符号整数最高位不表示正负但操作数还是有符号的，有符号数右移1位，正数时高位补0，负数时高位补1
                value &= mask;     //和整数最大值进行逻辑与运算，运算后的结果为忽略表示正负值的最高位
                value >>= num - 1;     //逻辑运算后的值无符号，对无符号的值直接做右移运算，计算剩下的位
            }
            return value;
        }
    }
}
