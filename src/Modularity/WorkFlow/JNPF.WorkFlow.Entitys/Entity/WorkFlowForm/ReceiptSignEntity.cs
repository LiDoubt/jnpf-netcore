using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 收文签呈单
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("WFORM_RECEIPTSIGN")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ReceiptSignEntity : EntityBase<string>
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
        /// 收文标题
        /// </summary>
        [SugarColumn(ColumnName = "F_RECEIPTTITLE")]
        public string ReceiptTitle { get; set; }
        /// <summary>
        /// 收文部门
        /// </summary>
        [SugarColumn(ColumnName = "F_DEPARTMENT")]
        public string Department { get; set; }
        /// <summary>
        /// 收文人
        /// </summary>
        [SugarColumn(ColumnName = "F_COLLECTOR")]
        public string Collector { get; set; }
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
        /// <summary>
        /// 收文简述
        /// </summary>
        [SugarColumn(ColumnName = "F_RECEIPTPAPER")]
        public string ReceiptPaper { get; set; }
    }
}
