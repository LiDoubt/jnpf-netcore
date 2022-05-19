using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.Extend.Entitys
{
    /// <summary>
    /// 邮件接收
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("EXT_EMAILRECEIVE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class EmailReceiveEntity : CLDEntityBase
    {
        /// <summary>
        /// 类型：【1-外部、0-内部】
        /// </summary>
        [SugarColumn(ColumnName = "F_TYPE")]
        public int? Type { get; set; }
        /// <summary>
        /// 邮箱账户
        /// </summary>
        [SugarColumn(ColumnName = "F_MACCOUNT")]
        public string MAccount { get; set; }
        /// <summary>
        /// MID
        /// </summary>
        [SugarColumn(ColumnName = "F_MID")]
        public string MID { get; set; }
        /// <summary>
        /// 发件人
        /// </summary>
        [SugarColumn(ColumnName = "F_SENDER")]
        public string Sender { get; set; }
        /// <summary>
        /// 发件人名称
        /// </summary>
        [SugarColumn(ColumnName = "F_SENDERNAME")]
        public string SenderName { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        [SugarColumn(ColumnName = "F_SUBJECT")]
        public string Subject { get; set; }
        /// <summary>
        /// 正文
        /// </summary>
        [SugarColumn(ColumnName = "F_BODYTEXT")]
        public string BodyText { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        [SugarColumn(ColumnName = "F_ATTACHMENT")]
        public string Attachment { get; set; }
        /// <summary>
        /// 阅读
        /// </summary>
        [SugarColumn(ColumnName = "F_READ")]
        public int? Read { get; set; }
        /// <summary>
        /// Date
        /// </summary>
        [SugarColumn(ColumnName = "F_DATE")]
        public DateTime? Date { get; set; }
        /// <summary>
        /// 星标
        /// </summary>
        [SugarColumn(ColumnName = "F_STARRED")]
        public int? Starred { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }
    }
}