using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.Message.Entitys
{
    /// <summary>
    /// 消息实例
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("BASE_MESSAGE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class MessageEntity : CLDEntityBase
    {
        /// <summary>
        /// 类别：1-通知公告，2-系统消息、3-私信消息
        /// </summary>
        [SugarColumn(ColumnName = "F_TYPE")]
        public int? Type { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [SugarColumn(ColumnName = "F_TITLE")]
        public string Title { get; set; } = "";

        /// <summary>
        /// 正文
        /// </summary>
        [SugarColumn(ColumnName = "F_BODYTEXT")]
        public string BodyText { get; set; }

        /// <summary>
        /// 优先
        /// </summary>
        [SugarColumn(ColumnName = "F_PRIORITYLEVEL")]
        public int? PriorityLevel { get; set; }

        /// <summary>
        /// 收件用户
        /// </summary>
        [SugarColumn(ColumnName = "F_TOUSERIDS")]
        public string ToUserIds { get; set; }

        /// <summary>
        /// 是否阅读
        /// </summary>
        [SugarColumn(ColumnName = "F_ISREAD")]
        public int? IsRead { get; set; }

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
