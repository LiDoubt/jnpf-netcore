using JNPF.Dependency;
using System;

namespace JNPF.DataValidation
{
    /// <summary>
    /// 验证类型特性
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Enum)]
    public sealed class ValidationTypeAttribute : Attribute
    {
    }
}