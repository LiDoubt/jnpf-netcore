using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 发文呈批表
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("WFORM_POSTBATCHTAB")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class PostBatchTabEntity : EntityBase<string>
    {
        /// <summary>
        /// 流程主键
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWID")]
        public string FlowId { get; set; }
        /// <summary>
        /// 流程标题
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWTITLE")]
        public string FlowTitle { get; set; }
        /// <summary>
        /// 紧急程度
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWURGENT")]
        public int? FlowUrgent { get; set; }
        /// <summary>
        /// 流程单据
        /// </summary>
        [SugarColumn(ColumnName = "F_BILLNO")]
        public string BillNo { get; set; }
        /// <summary>
        /// 文件标题
        /// </summary>
        [SugarColumn(ColumnName = "F_FILETITLE")]
        public string FileTitle { get; set; }
        /// <summary>
        /// 主办单位
        /// </summary>
        [SugarColumn(ColumnName = "F_DRAFTEDPERSON")]
        public string DraftedPerson { get; set; }
        /// <summary>
        /// 发往单位
        /// </summary>
        [SugarColumn(ColumnName = "F_SENDUNIT")]
        public string SendUnit { get; set; }
        /// <summary>
        /// 发文编码
        /// </summary>
        [SugarColumn(ColumnName = "F_WRITINGNUM")]
        public string WritingNum { get; set; }
        /// <summary>
        /// 发文日期
        /// </summary>
        [SugarColumn(ColumnName = "F_WRITINGDATE")]
        public DateTime? WritingDate { get; set; }
        /// <summary>
        /// 份数
        /// </summary>
        [SugarColumn(ColumnName = "F_SHARENUM")]
        public string ShareNum { get; set; }
        /// <summary>
        /// 相关附件
        /// </summary>
        [SugarColumn(ColumnName = "F_FILEJSON")]
        public string FileJson { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
