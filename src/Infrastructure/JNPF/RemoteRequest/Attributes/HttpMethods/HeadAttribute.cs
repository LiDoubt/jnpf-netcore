using JNPF.Dependency;
using System;
using System.Net.Http;

namespace JNPF.RemoteRequest
{
    /// <summary>
    /// HttpHead 请求
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Method)]
    public class HeadAttribute : HttpMethodBaseAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="requestUrl"></param>
        public HeadAttribute(string requestUrl) : base(requestUrl, HttpMethod.Head)
        {
        }
    }
}