﻿using JNPF.Dependency;
using System;

namespace JNPF.RemoteRequest
{
    /// <summary>
    /// 配置请求失败重试策略
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public class RetryPolicyAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="numRetries"></param>
        /// <param name="retryTimeout">每次延迟时间（毫秒）</param>
        public RetryPolicyAttribute(int numRetries, int retryTimeout = 1000)
        {
            NumRetries = numRetries;
            RetryTimeout = retryTimeout;
        }

        /// <summary>
        /// 重试次数
        /// </summary>
        public int NumRetries { get; set; }

        /// <summary>
        /// 每次延迟时间（毫秒）
        /// </summary>
        public int RetryTimeout { get; set; } = 1000;
    }
}
