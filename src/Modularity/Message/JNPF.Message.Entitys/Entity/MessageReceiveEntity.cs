using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.Message.Entitys
{
    /// <summary>
    /// 消息接收
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("BASE_MESSAGERECEIVE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class MessageReceiveEntity : EntityBase<string>
    {
        /// <summary>
        /// 消息主键
        /// </summary>
        [SugarColumn(ColumnName = "F_MESSAGEID")]
        public string MessageId { get; set; }

        /// <summary>
        /// 用户主键
        /// </summary>
        [SugarColumn(ColumnName = "F_USERID")]
        public string UserId { get; set; }

        /// <summary>
        /// 是否阅读
        /// </summary>
        [SugarColumn(ColumnName = "F_ISREAD")]
        public int? IsRead { get; set; }

        /// <summary>
        /// 阅读时间
        /// </summary>
        [SugarColumn(ColumnName = "F_READTIME")]
        public DateTime? ReadTime { get; set; }

        /// <summary>
        /// 阅读次数
        /// </summary>
        [SugarColumn(ColumnName = "F_READCOUNT")]
        public int? ReadCount { get; set; }
    }
}
