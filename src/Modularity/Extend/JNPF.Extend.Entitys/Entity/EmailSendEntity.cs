using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.Extend.Entitys
{
    /// <summary>
    /// 邮件发送
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01  
    /// </summary>
    [SugarTable("EXT_EMAILSEND")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class EmailSendEntity : CLDEntityBase
    {
        /// <summary>
        /// 类型：【1-外部、0-内部】
        /// </summary>
        [SugarColumn(ColumnName = "F_TYPE")]
        public int? Type { get; set; }
        /// <summary>
        /// 发件人
        /// </summary>
        [SugarColumn(ColumnName = "F_SENDER")]
        public string Sender { get; set; }
        /// <summary>
        /// 收件人
        /// </summary>
        [SugarColumn(ColumnName = "F_TO")]
        public string To { get; set; }
        /// <summary>
        /// 抄送人
        /// </summary>
        [SugarColumn(ColumnName = "F_CC")]
        public string CC { get; set; }
        /// <summary>
        /// 密送人
        /// </summary>
        [SugarColumn(ColumnName = "F_BCC")]
        public string BCC { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        [SugarColumn(ColumnName = "F_COLOUR")]
        public string Colour { get; set; }
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
        /// 状态:【-1-草稿、0-正在投递、1-投递成功】
        /// </summary>
        [SugarColumn(ColumnName = "F_STATE")]
        public int? State { get; set; }
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