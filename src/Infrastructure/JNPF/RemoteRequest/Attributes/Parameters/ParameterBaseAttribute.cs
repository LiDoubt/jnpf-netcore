using JNPF.Dependency;
using System;

namespace JNPF.RemoteRequest
{
    /// <summary>
    /// 代理参数基类特性
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Parameter)]
    public class ParameterBaseAttribute : Attribute
    {
    }
}