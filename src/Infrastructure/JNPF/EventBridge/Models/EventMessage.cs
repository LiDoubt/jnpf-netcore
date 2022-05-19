using JNPF.Dependency;

namespace JNPF.EventBridge
{
    /// <summary>
    /// 事件消息传输对象
    /// </summary>
    [SuppressSniffer]
    public sealed class EventMessage : EventMessage<object>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="category"></param>
        /// <param name="eventId"></param>
        /// <param name="payload"></param>
        public EventMessage(string category, string eventId, object payload = default)
            : base(category, eventId, payload)
        {
        }
    }

    /// <summary>
    /// 事件消息传输对象
    /// </summary>
    [SuppressSniffer]
    public class EventMessage<TPayload>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="category"></param>
        /// <param name="eventId"></param>
        /// <param name="payload"></param>
        public EventMessage(string category, string eventId, TPayload payload = default)
        {
            Category = category;
            EventId = eventId;
            Payload = payload;
        }

        /// <summary>
        /// 事件唯一 Id
        /// </summary>
        public string EventId { get; set; }

        /// <summary>
        /// 事件类别
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public TPayload Payload { get; set; }
    }
}