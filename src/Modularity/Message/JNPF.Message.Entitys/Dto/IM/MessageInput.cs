using JNPF.Dependency;

namespace JNPF.Message.Entitys.Dto.IM
{
    /// <summary>
    /// 消息接收类
    /// </summary>
    [SuppressSniffer]
    public class MessageInput
    {
        /// <summary>
        /// 发送发送客户端ID
        /// </summary>
        public string sendClientId { get; set; }

        /// <summary>
        /// 方法
        /// </summary>
        public string method { get; set; }

        /// <summary>
        /// 移动设备
        /// </summary>
        public bool mobileDevice { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// 发送者ID
        /// </summary>
        public string toUserId { get; set; }

        /// <summary>
        /// 接收者ID
        /// </summary>
        public string formUserId { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string messageType { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public object messageContent { get; set; }

        /// <summary>
        /// 当前页数
        /// </summary>
        public int currentPage { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int pageSize { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public string sord { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string keyword { get; set; }
    }
}
