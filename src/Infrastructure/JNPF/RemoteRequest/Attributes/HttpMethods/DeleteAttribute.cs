using JNPF.Dependency;
using System;
using System.Net.Http;

namespace JNPF.RemoteRequest
{
    /// <summary>
    /// HttpDelete 请求
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Method)]
    public class DeleteAttribute : HttpMethodBaseAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="requestUrl"></param>
        public DeleteAttribute(string requestUrl) : base(requestUrl, HttpMethod.Delete)
        {
        }
    }
}