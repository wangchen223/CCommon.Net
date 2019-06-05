using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCommon.Common
{
    /// <summary>
    /// Api状态码
    /// </summary>
    /// <remarks>
    /// 200-300 成功
    /// 400-500 授权失败 
    /// </remarks>
    public enum EReturnResultState : int
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 200,
        /// <summary>
        /// 已创建返回成功
        /// </summary>
        Created = 201,
        /// <summary>
        /// 时间戳过期
        /// </summary>
        Timeout_Success = 210,
        /// <summary>
        /// 没有授权
        /// </summary>
        NoAuth = 400,
        /// <summary>
        /// 失败
        /// </summary>
        Fail = 500,
        /// <summary>
        /// 时间戳过期
        /// </summary>
        TimeoutError = 510,
        /// <summary>
        /// 数据存在
        /// </summary>
        ExistError = 520,
        /// <summary>
        /// 数据不存在
        /// </summary>
        NoExistError = 540,
        /// <summary>
        /// 参数错误
        /// </summary>
        ParamError = 530,
        /// <summary>
        /// 存储设备异常
        /// </summary>
        StorageDeviceError = 550
    }

    /// <summary>
    /// 返回结果封装
    /// </summary>
    [Serializable]
    public class ReturnResult
    {
        /// <summary>
        /// 返回状态
        /// </summary>
        public EReturnResultState State { get; set; }
        /// <summary>
        /// 业务状态
        /// </summary>
        public int BusinessState { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        public bool IsValid
        {
            get { return (Int16)State >= 200 && (Int16)State < 300; }
        }

        #region 成功
        /// <summary>
        /// 成功
        /// </summary>
        /// <returns></returns>
        public static ReturnResult SuccessResult()
        {
            return new ReturnResult { State = EReturnResultState.Success };
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ReturnResult SuccessResult(string message)
        {
            return new ReturnResult
            {
                State = EReturnResultState.Success,
                Message = message
            };
        }
        #endregion

        #region 失败

        public static ReturnResult FailResult()
        {
            return new ReturnResult { State = EReturnResultState.Fail };
        }
        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ReturnResult FailResult(string message)
        {
            return new ReturnResult { Message = message, State = EReturnResultState.Fail };
        }
        #endregion

        #region 自定义状态结果
        /// <summary>
        /// 自定义状态结果
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static ReturnResult OtherResult(EReturnResultState state)
        {
            return OtherResult(state, null);
        }
        /// <summary>
        /// 自定义状态结果
        /// </summary>
        /// <param name="state"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ReturnResult OtherResult(EReturnResultState state, string message)
        {
            return new ReturnResult
            {
                State = state,
                Message = message
            };
        }
        #endregion
    }
    /// <summary>
    /// 返回结果封装
    /// </summary>
    [Serializable]
    public class ReturnResult<T> : ReturnResult
    {

        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }


        #region 成功
        /// <summary>
        /// 成功
        /// </summary>
        /// <returns></returns>
        public static new ReturnResult<T> SuccessResult()
        {
            return new ReturnResult<T> { State = EReturnResultState.Success };
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static new ReturnResult<T> SuccessResult(string message)
        {
            return SuccessResult(message, default(T));
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ReturnResult<T> SuccessResult(T data)
        {
            return SuccessResult(null, data);
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ReturnResult<T> SuccessResult(string message, T data)
        {
            return new ReturnResult<T>
            {
                State = EReturnResultState.Success,
                Message = message,
                Data = data
            };
        }
        #endregion

        #region 失败
        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ReturnResult<T> FailResult(string message)
        {
            return new ReturnResult<T> { Message = message, State = EReturnResultState.Fail };
        }
        #endregion

        #region 自定义状态结果
        /// <summary>
        /// 自定义状态结果
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static ReturnResult<T> OtherResult(EReturnResultState state)
        {
            return OtherResult(state, null, default(T));
        }
        /// <summary>
        /// 自定义状态结果
        /// </summary>
        /// <param name="state"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ReturnResult<T> OtherResult(EReturnResultState state, string message)
        {
            return OtherResult(state, message, default(T));
        }
        /// <summary>
        /// 自定义状态结果
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static ReturnResult<T> OtherResult(EReturnResultState state, T data)
        {
            return OtherResult(state, null, data);
        }


        /// <summary>
        /// 自定义状态结果
        /// </summary>
        /// <param name="state"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ReturnResult<T> OtherResult(EReturnResultState state, string message, T data)
        {
            return new ReturnResult<T> { Message = message, State = state, Data = data };
        }

        #endregion
    }
}
