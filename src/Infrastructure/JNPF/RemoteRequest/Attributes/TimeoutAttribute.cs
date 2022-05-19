using JNPF.Dependency;
using System;

namespace JNPF.RemoteRequest
{
    /// <summary>
    /// 配置客户端请求超时时间
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public class TimeoutAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="seconds"></param>
        public TimeoutAttribute(long seconds)
        {
            Seconds = seconds;
        }

        /// <summary>
        /// 超时时间（秒）
        /// </summary>
        public long Seconds { get; set; }
    }
}
