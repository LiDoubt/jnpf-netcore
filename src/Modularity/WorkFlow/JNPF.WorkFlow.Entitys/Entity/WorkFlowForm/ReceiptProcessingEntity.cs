using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 收文处理表
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("WFORM_RECEIPTPROCESSING")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ReceiptProcessingEntity : EntityBase<string>
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
        /// 来文单位
        /// </summary>
        [SugarColumn(ColumnName = "F_COMMUNICATIONUNIT")]
        public string CommunicationUnit { get; set; }
        /// <summary>
        /// 来文字号
        /// </summary>
        [SugarColumn(ColumnName = "F_LETTERNUM")]
        public string LetterNum { get; set; }
        /// <summary>
        /// 收文日期
        /// </summary>
        [SugarColumn(ColumnName = "F_RECEIPTDATE")]
        public DateTime? ReceiptDate { get; set; }
        /// <summary>
        /// 相关附件
        /// </summary>
        [SugarColumn(ColumnName = "F_FILEJSON")]
        public string FileJson { get; set; }
    }
}
