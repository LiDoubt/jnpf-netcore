using JNPF.Dependency;

namespace JNPF.EventBridge
{
    /// <summary>
    /// 承载事件传输元数据
    /// </summary>
    [SuppressSniffer]
    public class EventMessageMetadata : EventHandlerMetadata
    {
        /// <summary>
        /// 只允许程序集内创建
        /// </summary>
        internal EventMessageMetadata()
        {
        }

        /// <summary>
        /// 事件 Id
        /// </summary>
        public string EventId { get; internal set; }

        /// <summary>
        /// 承载数据值（进行序列化存储）
        /// </summary>
        public string Payload { get; internal set; }

        /// <summary>
        /// 承载数据程序集名称
        /// </summary>
        public string PayloadAssemblyName { get; internal set; }

        /// <summary>
        /// 承载数据类型完整限定名
        /// </summary>
        public string PayloadTypeFullName { get; internal set; }
    }
}