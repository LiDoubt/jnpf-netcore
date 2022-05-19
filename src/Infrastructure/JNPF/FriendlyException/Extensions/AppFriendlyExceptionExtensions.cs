using JNPF.Dependency;
using Microsoft.AspNetCore.Http;

namespace JNPF.FriendlyException
{
    /// <summary>
    /// 异常拓展
    /// </summary>
    [SuppressSniffer]
    public static class AppFriendlyExceptionExtensions
    {
        /// <summary>
        /// 设置异常状态码
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static AppFriendlyException StatusCode(this AppFriendlyException exception, int statusCode = StatusCodes.Status500InternalServerError)
        {
            exception.StatusCode = statusCode;
            return exception;
        }
    }
}