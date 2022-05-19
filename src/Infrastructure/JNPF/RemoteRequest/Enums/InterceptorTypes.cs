using JNPF.Dependency;
using System.ComponentModel;

namespace JNPF.RemoteRequest
{
    /// <summary>
    /// 拦截类型
    /// </summary>
    [SuppressSniffer]
    public enum InterceptorTypes
    {
        /// <summary>
        /// HttpClient 拦截
        /// </summary>
        [Description("HttpClient 拦截")]
        Client,

        /// <summary>
        /// HttpRequestMessage 拦截
        /// </summary>
        [Description("HttpRequestMessage 拦截")]
        Request,

        /// <summary>
        /// HttpResponseMessage 拦截
        /// </summary>
        [Description("HttpResponseMessage 拦截")]
        Response,

        /// <summary>
        /// 异常拦截
        /// </summary>
        [Description("异常拦截")]
        Exception
    }
}