using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.Message.Entitys
{
    /// <summary>
    /// 在线聊天
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("BASE_IMCONTENT")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class IMContentEntity : EntityBase<string>
    {
        /// <summary>
        /// 发送者
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_SENDUSERID")]
        public string SendUserId { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_SENDTIME")]
        public DateTime? SendTime { get; set; }
        /// <summary>
         /// 接收者
         /// </summary>
         /// <returns></returns>
        [SugarColumn(ColumnName = "F_RECEIVEUSERID")]
        public string ReceiveUserId { get; set; }
        /// <summary>
        /// 接收时间
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_RECEIVETIME")]
        public DateTime? ReceiveTime { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_CONTENT")]
        public string Content { get; set; }
        /// <summary>
        /// 内容类型：text、img、file
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTENTTYPE")]
        public string ContentType { get; set; }
        /// <summary>
        /// 状态（0:未读、1：已读）
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_STATE")]
        public int? State { get; set; }
    }
}
