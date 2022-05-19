using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 费用支出单
    /// 版 本：V3.0.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2018-07-16 
    /// </summary>
    [SugarTable("WFORM_EXPENSEEXPENDITURE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ExpenseExpenditureEntity : EntityBase<string>
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
        /// 流程等级
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWURGENT")]
        public int? FlowUrgent { get; set; }
        /// <summary>
        /// 流程单据
        /// </summary>
        [SugarColumn(ColumnName = "F_BILLNO")]
        public string BillNo { get; set; }
        /// <summary>
        /// 申请人员
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYUSER")]
        public string ApplyUser { get; set; }
        /// <summary>
        /// 所在部门
        /// </summary>
        [SugarColumn(ColumnName = "F_DEPARTMENT")]
        public string Department { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYDATE")]
        public DateTime? ApplyDate { get; set; }
        /// <summary>
        /// 合同编码
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTRACTNUM")]
        public string ContractNum { get; set; }
        /// <summary>
        /// 非合同支出
        /// </summary>
        [SugarColumn(ColumnName = "F_NONCONTRACT")]
        public string NonContract { get; set; }
        /// <summary>
        /// 开户银行
        /// </summary>
        [SugarColumn(ColumnName = "F_ACCOUNTOPENINGBANK")]
        public string AccountOpeningBank { get; set; }
        /// <summary>
        /// 银行账号
        /// </summary>
        [SugarColumn(ColumnName = "F_BANKACCOUNT")]
        public string BankAccount { get; set; }
        /// <summary>
        /// 开户姓名
        /// </summary>
        [SugarColumn(ColumnName = "F_OPENACCOUNT")]
        public string OpenAccount { get; set; }
        /// <summary>
        /// 合计费用
        /// </summary>
        [SugarColumn(ColumnName = "F_TOTAL")]
        public decimal? Total { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        [SugarColumn(ColumnName = "F_PAYMENTMETHOD")]
        public string PaymentMethod { get; set; }
        /// <summary>
        /// 支付金额
        /// </summary>
        [SugarColumn(ColumnName = "F_AMOUNTPAYMENT")]
        public decimal? AmountPayment { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
