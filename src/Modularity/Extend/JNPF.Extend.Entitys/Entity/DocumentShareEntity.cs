using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.Extend.Entitys
{
    /// <summary>
    /// 知识文档共享
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01  
    /// </summary>
    [SugarTable("EXT_DOCUMENTSHARE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class DocumentShareEntity : EntityBase<string>
    {
        /// <summary>
        /// 文档主键
        /// </summary>
        [SugarColumn(ColumnName = "F_DOCUMENTID")]
        public string DocumentId { get; set; }
        /// <summary>
        /// 共享人员
        /// </summary>
        [SugarColumn(ColumnName = "F_SHAREUSERID")]
        public string ShareUserId { get; set; }
        /// <summary>
        /// 共享时间
        /// </summary>
        [SugarColumn(ColumnName = "F_SHARETIME")]
        public DateTime? ShareTime { get; set; }
    }
}