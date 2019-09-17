using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text;

namespace HZC.Utils
{
    public static class HttpContextExtensions
    {
        #region 获取客户端IP地址
        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetClientIpAddress(this HttpContext context)
        {
            return context.Connection.RemoteIpAddress.ToString();
        } 
        #endregion

        #region HttpRequest 扩展
        /// <summary>
        /// 判断是不是AJAX请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsAjax(this HttpRequest request)
        {
            var result = false;
            var xReq = request.Headers.ContainsKey("X-Requested-With");
            if (xReq)
            {
                result = request.Headers["X-Requested-With"] == "XMLHttpRequest";
            }
            return result;
        }

        /// <summary>
        /// 获取请求的全部信息，包含模式、域名、controller、action和查询参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetFullPath(this HttpRequest request)
        {
            return new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .Append(request.Path)
                .Append(request.QueryString)
                .ToString();
        }

        /// <summary>
        /// 获取请求的完整路径，不包含查询参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetFullPathWithoutQueryString(this HttpRequest request)
        {
            return new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .Append(request.Path)
                .ToString();
        } 
        #endregion

        #region IdentityUser 相关
        /// <summary>
        /// 获取Int类型的资产声明
        /// </summary>
        /// <param name="user"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetInt(this ClaimsPrincipal user, string key)
        {
            var val = user.FindFirst(c => c.Type == key);
            if (val == null)
            {
                return 0;
            }
            if (int.TryParse(val.Value, out var result))
            {
                return result;
            }
            return 0;
        }

        /// <summary>
        /// 获取string类型的资产声明
        /// </summary>
        /// <param name="user"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetString(this ClaimsPrincipal user, string key)
        {
            var val = user.FindFirst(c => c.Type == key);
            if (val == null)
            {
                return string.Empty;
            }
            return val.Value;
        } 
        #endregion
    }
}
