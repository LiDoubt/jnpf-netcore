using JNPF.Dependency;
using System;

namespace JNPF.EventBridge
{
    /// <summary>
    /// 事件消息特性
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class EventMessageAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public EventMessageAttribute()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="eventId"></param>
        public EventMessageAttribute(string eventId)
        {
            EventId = eventId;
        }

        /// <summary>
        /// 事件 Id
        /// </summary>
        public string EventId { get; set; }
    }
}