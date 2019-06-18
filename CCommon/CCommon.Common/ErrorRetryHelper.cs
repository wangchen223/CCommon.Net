using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCommon.Common
{
    /// <summary>
    /// 错误重试帮助类
    /// </summary>
    public class ErrorRetryHelper
    {
        public static ReturnResult Handle(string errorRetryConfig, Func<bool> methed,TimeUnit timeUnit=TimeUnit.Seconds)
        {
            return Handle(errorRetryConfig, () =>
            {
                var istrue = methed();
                return istrue ? ReturnResult.SuccessResult() : ReturnResult.FailResult();
            }, timeUnit);
        }

        /// <summary>
        /// 错误自动重试公用方法
        /// </summary>
        /// <param name="errorRetryConfig">重试策略(单位:秒)</param>
        /// <param name="methed"></param>
        /// <returns></returns>
        public static ReturnResult<T> Handle<T>(string errorRetryConfig, Func<ReturnResult<T>> methed, TimeUnit timeUnit = TimeUnit.Seconds)
        {
            return Handle(errorRetryConfig, () =>
            {
                var istrue = methed();
                return istrue as ReturnResult;
            }, timeUnit) as ReturnResult<T>;
        }

        /// <summary>
        /// 错误自动重试公用方法
        /// </summary>
        /// <param name="errorRetryConfig">重试策略(单位:秒)</param>
        /// <param name="methed"></param>
        /// <returns></returns>
        public static ReturnResult Handle(string errorRetryConfig, Func<ReturnResult> methed, TimeUnit timeUnit = TimeUnit.Seconds)
        {
            if (string.IsNullOrEmpty(errorRetryConfig))
            {
                errorRetryConfig = "0";
            }

            string[] errTime = errorRetryConfig.Split(',');

            int errNum = 0;
            ReturnResult returnResult;
            do
            {
                try
                {
                    returnResult = methed();
                }
                catch (Exception ex)
                {
                    returnResult = ReturnResult.FailResult(ex.ToString(),ex);
                }

                if (!returnResult.IsValid)
                {
                    System.Threading.Thread.Sleep((int)timeUnit.toMicroseconds(long.Parse(errTime[errNum])));
                    errNum++;
                }
            } while (!returnResult.IsValid && errNum < errTime.Length);

            return returnResult;
        }
    }
}
