using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 收入确认分析表
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2018-07-16 
    /// </summary>
    [SugarTable("WFORM_INCOMERECOGNITION")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class IncomeRecognitionEntity : EntityBase<string>
    {
        /// <summary>
        /// 流程主题
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
        /// 结算月份
        /// </summary>
        [SugarColumn(ColumnName = "F_SETTLEMENTMONTH")]
        public string SettlementMonth { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        [SugarColumn(ColumnName = "F_CUSTOMERNAME")]
        public string CustomerName { get; set; }
        /// <summary>
        /// 合同编码
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTRACTNUM")]
        public string ContractNum { get; set; }
        /// <summary>
        /// 合同金额
        /// </summary>
        [SugarColumn(ColumnName = "F_TOTALAMOUNT")]
        public decimal? TotalAmount { get; set; }
        /// <summary>
        /// 到款银行
        /// </summary>
        [SugarColumn(ColumnName = "F_MONEYBANK")]
        public string MoneyBank { get; set; }
        /// <summary>
        /// 到款金额
        /// </summary>
        [SugarColumn(ColumnName = "F_ACTUALAMOUNT")]
        public decimal? ActualAmount { get; set; }
        /// <summary>
        /// 到款日期
        /// </summary>
        [SugarColumn(ColumnName = "F_PAYMENTDATE")]
        public DateTime? PaymentDate { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTACTNAME")]
        public string ContactName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTACPHONE")]
        public string ContacPhone { get; set; }
        /// <summary>
        /// 联系QQ
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTACTQQ")]
        public string ContactQQ { get; set; }
        /// <summary>
        /// 未付金额
        /// </summary>
        [SugarColumn(ColumnName = "F_UNPAIDAMOUNT")]
        public decimal? UnpaidAmount { get; set; }
        /// <summary>
        /// 已付金额
        /// </summary>
        [SugarColumn(ColumnName = "F_AMOUNTPAID")]
        public decimal? AmountPaid { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
