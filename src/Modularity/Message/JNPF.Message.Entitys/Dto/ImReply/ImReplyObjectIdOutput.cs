using Newtonsoft.Json;
using System;

namespace JNPF.Message.Entitys.Dto.ImReply
{
    /// <summary>
    /// 聊天会话对象ID
    /// </summary>
    public class ImReplyObjectIdOutput
    {
        /// <summary>
        /// 对象id
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// 最新时间
        /// </summary>
        public DateTime? latestDate { get; set; }
    }
}
