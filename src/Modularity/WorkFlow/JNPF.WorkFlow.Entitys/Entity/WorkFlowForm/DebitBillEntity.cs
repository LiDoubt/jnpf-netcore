using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 借支单
    /// 版 本：V3.0.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2018-07-13 
    /// </summary>
    [SugarTable("WFORM_DEBITBILL")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class DebitBillEntity : EntityBase<string>
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
        /// 工作部门
        /// </summary>
        [SugarColumn(ColumnName = "F_DEPARTMENTAL")]
        public string Departmental { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYDATE")]
        public DateTime? ApplyDate { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        [SugarColumn(ColumnName = "F_STAFFNAME")]
        public string StaffName { get; set; }
        /// <summary>
        /// 员工职务
        /// </summary>
        [SugarColumn(ColumnName = "F_STAFFPOST")]
        public string StaffPost { get; set; }
        /// <summary>
        /// 员工编码
        /// </summary>
        [SugarColumn(ColumnName = "F_STAFFID")]
        public string StaffId { get; set; }
        /// <summary>
        /// 借款方式
        /// </summary>
        [SugarColumn(ColumnName = "F_LOANMODE")]
        public string LoanMode { get; set; }
        /// <summary>
        /// 借支金额
        /// </summary>
        [SugarColumn(ColumnName = "F_AMOUNTDEBIT")]
        public decimal? AmountDebit { get; set; }
        /// <summary>
        /// 转账账户
        /// </summary>
        [SugarColumn(ColumnName = "F_TRANSFERACCOUNT")]
        public string TransferAccount { get; set; }
        /// <summary>
        /// 还款票据
        /// </summary>
        [SugarColumn(ColumnName = "F_REPAYMENTBILL")]
        public string RepaymentBill { get; set; }
        /// <summary>
        /// 还款日期
        /// </summary>
        [SugarColumn(ColumnName = "F_TEACHINGDATE")]
        public DateTime? TeachingDate { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        [SugarColumn(ColumnName = "F_PAYMENTMETHOD")]
        public string PaymentMethod { get; set; }
        /// <summary>
        /// 借款原因
        /// </summary>
        [SugarColumn(ColumnName = "F_REASON")]
        public string Reason { get; set; }
    }
}
