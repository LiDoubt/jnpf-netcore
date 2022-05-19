using JNPF.Dependency;
using System;

namespace JNPF.FriendlyException
{
    /// <summary>
    /// 错误代码类型特性
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Enum)]
    public sealed class ErrorCodeTypeAttribute : Attribute
    {
    }
}