﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CCommon.Common
{
    public class TextHelper
    {
        /// <summary>
        /// 截取指定字节长度的字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="len">截取字节长度</param>
        /// <returns></returns>
        public static string Substring(string str, int len)
        {
            return Substring(str, len, "");
        }
        /// <summary>
        /// 截取指定字节长度的字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="len">截取字节长度</param>
        /// <param name="suffix">如果超出增加后缀字符串</param>
        /// <returns></returns>
        public static string Substring(string str, int len, string suffix)
        {
            string result = string.Empty;// 最终返回的结果
            if (string.IsNullOrEmpty(str)) { return result; }
            int byteLen = System.Text.Encoding.Default.GetByteCount(str);// 单字节字符长度
            int charLen = str.Length;// 把字符平等对待时的字符串长度
            int byteCount = 0;// 记录读取进度
            int pos = 0;// 记录截取位置
            if (byteLen > len)
            {
                len -= System.Text.Encoding.Default.GetByteCount(suffix);
                for (int i = 0; i < charLen; i++)
                {
                    if (Convert.ToInt32(str.ToCharArray()[i]) > 255)// 按中文字符计算加2
                    { byteCount += 2; }
                    else// 按英文字符计算加1
                    { byteCount += 1; }
                    if (byteCount > len)// 超出时只记下上一个有效位置
                    {
                        pos = i;
                        break;
                    }
                    else if (byteCount == len)// 记下当前位置
                    {
                        pos = i + 1;
                        break;
                    }
                }
                if (pos >= 0)
                {
                    result = str.Substring(0, pos) + suffix;

                }
            }
            else
            { result = str; }
            return result;
        }
    }
}
