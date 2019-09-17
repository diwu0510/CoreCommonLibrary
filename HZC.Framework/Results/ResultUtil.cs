using System;
using System.Collections.Generic;

namespace HZC.Framework
{
    public class ResultUtil
    {
        #region 通用返回结果
        /// <summary>
        /// 返回操作结果及说明，无返回值
        /// </summary>
        /// <param name="code">结果码</param>
        /// <param name="message">结果说明</param>
        /// <returns></returns>
        public static Result Return(int code, string message)
        {
            return new Result { Code = code, Message = message };
        }

        /// <summary>
        /// 返回操作结果及说明，有返回值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code">结果码</param>
        /// <param name="body">操作结果</param>
        /// <param name="message">结果说明</param>
        /// <returns></returns>
        public static Result<T> Return<T>(int code, T body, string message)
        {
            return new Result<T> { Code = code, Body = body, Message = message };
        } 
        #endregion

        #region 操作成功
        /// <summary>
        /// 操作成功
        /// </summary>
        /// <param name="msg">操作结果说明</param>
        /// <returns></returns>
        public static Result Ok(string msg = "")
        {
            return Return(ResultCodes.Ok, msg);
        }

        /// <summary>
        /// 操作成功
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="body">操作结果</param>
        /// <param name="msg">操作结果说明</param>
        /// <returns></returns>
        public static Result<T> Ok<T>(T body, string msg = "")
        {
            return Return(ResultCodes.Ok, body, msg);
        }
        #endregion

        #region 操作失败
        /// <summary>
        /// 一般类型操作失败，无返回
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Result Fail(string msg = "操作失败")
        {
            return Return(ResultCodes.Fail, msg);
        }

        public static Result<T> Fail<T>(T body, string msg = "操作失败")
        {
            return Return(ResultCodes.Fail, body, msg);
        }
        #endregion

        #region 身份验证失败
        /// <summary>
        /// 身份验证失败，未登录
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Result AuthenticationError(string msg = "您尚未登录")
        {
            return Return(ResultCodes.UnAuthenticate, msg);
        } 
        #endregion

        #region 权限验证失败
        /// <summary>
        /// 权限验证失败，无模块权限或数据权限
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Result AuthorizeError(string msg = "您无权使用此功能")
        {
            return Return(ResultCodes.UnAuthorize, msg);
        }
        #endregion

        #region 验证失败
        /// <summary>
        /// 数据验证失败，模型验证或请求参数验证失败等
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Result BadRequest(string msg = "无效访问")
        {
            return Return(ResultCodes.BadRequest, msg);
        }

        /// <summary>
        /// 数据验证失败，模型验证失败或请求参数验证失败等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="body"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Result<T> BadRequest<T>(T body, string msg = "无效访问")
        {
            return Return(ResultCodes.BadRequest, body, msg);
        }
        #endregion

        #region 空数据
        /// <summary>
        /// 数据获取失败，请求的数据不存在
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Result NotFound(string msg = "请求的资源不存在")
        {
            return Return(ResultCodes.NotFound, msg);
        }
        #endregion

        #region 系统错误
        /// <summary>
        /// 返回系统错误
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static Result Exception(Exception ex)
        {
            return Return(ResultCodes.Exception, ex.Message);
        }

        public static Result Exception(string message)
        {
            return Return(ResultCodes.Exception, message);
        }

        public static Result<T> Exception<T>(string message)
        {
            return Return(ResultCodes.Exception, default(T), message);
        }
        #endregion

        #region 列表
        /// <summary>
        /// 返回列表结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="body"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Result<IEnumerable<T>> List<T>(IEnumerable<T> body, int code = 200, string message = "")
        {
            return Return(code, body, message);
        }

        /// <summary>
        /// 返回分页列表结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="body"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static PageListResult<T> PageList<T>(PageListResult<T> body, int code = 200, string message = "")
        {
            return new PageListResult<T>
            {
                Code = code,
                Message = message,
                Body = body.Body,
                PageIndex = body.PageIndex,
                PageSize = body.PageSize,
                RecordCount = body.RecordCount
            };
        }
        #endregion
    }
}
