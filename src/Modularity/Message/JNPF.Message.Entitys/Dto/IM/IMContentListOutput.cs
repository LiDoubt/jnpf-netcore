using System;

namespace JNPF.Message.Entitys.Dto.IM
{
    public class IMContentListOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 发送者
        /// </summary>
        /// <returns></returns>
        public string sendUserId { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        /// <returns></returns>
        public DateTime? sendTime { get; set; }
        /// <summary>
         /// 接收者
         /// </summary>
         /// <returns></returns>
        public string receiveUserId { get; set; }
        /// <summary>
        /// 接收时间
        /// </summary>
        /// <returns></returns>
        public DateTime? receiveTime { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        /// <returns></returns>
        public string content { get; set; }
        /// <summary>
        /// 内容类型：text、img、file
        /// </summary>
        public string contentType { get; set; }
        /// <summary>
        /// 状态（0:未读、1：已读）
        /// </summary>
        /// <returns></returns>
        public int? state { get; set; }
    }
}
