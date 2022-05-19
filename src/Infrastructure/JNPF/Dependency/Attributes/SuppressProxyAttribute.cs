using System;

namespace JNPF.Dependency
{
    /// <summary>
    /// 跳过全局代理
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Class)]
    public class SuppressProxyAttribute : Attribute
    {
    }
}