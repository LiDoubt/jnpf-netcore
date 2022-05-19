using JNPF.Dependency;
using System;

namespace JNPF.DynamicApiController
{
    /// <summary>
    /// 动态 WebApi 特性
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Class)]
    public sealed class DynamicApiControllerAttribute : Attribute
    {
    }
}