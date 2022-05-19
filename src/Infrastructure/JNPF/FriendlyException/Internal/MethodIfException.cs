using JNPF.Dependency;
using System.Collections.Generic;
using System.Reflection;

namespace JNPF.FriendlyException
{
    /// <summary>
    /// 方法异常类
    /// </summary>
    [SuppressSniffer]
    internal sealed class MethodIfException
    {
        /// <summary>
        /// 出异常的方法
        /// </summary>
        public MethodBase ErrorMethod { get; set; }

        /// <summary>
        /// 异常特性
        /// </summary>
        public IEnumerable<IfExceptionAttribute> IfExceptionAttributes { get; set; }
    }
}