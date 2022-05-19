using JNPF.Dependency;
using System;
using System.Text.Json.Serialization;

namespace JNPF.Message.Entitys.Dto.Message
{
    [SuppressSniffer]
    public class MessageInfoOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 正文内容	
        /// </summary>
        public string bodyText { get; set; }
        /// <summary>
        /// 发送人员
        /// </summary>
        public string creatorUser { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime? lastModifyTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int? deleteMark { get; set; }
    }
}
