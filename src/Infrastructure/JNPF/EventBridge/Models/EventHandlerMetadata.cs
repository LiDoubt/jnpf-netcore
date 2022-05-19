using JNPF.Dependency;
using System;

namespace JNPF.EventBridge
{
    /// <summary>
    /// 事件处理程序元数据
    /// </summary>
    [SuppressSniffer]
    public class EventHandlerMetadata
    {
        /// <summary>
        /// 只允许程序集内创建
        /// </summary>
        internal EventHandlerMetadata()
        {
        }

        /// <summary>
        /// 程序集名称
        /// </summary>
        public string AssemblyName { get; internal set; }

        /// <summary>
        /// 处理程序名称
        /// </summary>
        public string TypeFullName { get; internal set; }

        /// <summary>
        /// 分类名
        /// </summary>
        public string Category { get; internal set; }

        /// <summary>
        /// 创建事件
        /// </summary>
        public DateTimeOffset CreatedTime { get; internal set; } = DateTimeOffset.UtcNow;
    }
}