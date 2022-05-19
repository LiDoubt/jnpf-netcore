using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 合同申请单表
    /// 版 本：V3.0.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2018-07-13 
    /// </summary>
    [SugarTable("WFORM_CONTRACTAPPROVALSHEET")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ContractApprovalSheetEntity : EntityBase<string>
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
        /// 申请人
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYUSER")]
        public string ApplyUser { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYDATE")]
        public DateTime? ApplyDate { get; set; }
        /// <summary>
        /// 编码支出
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTRACTID")]
        public string ContractId { get; set; }
        /// <summary>
        /// 合同号
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTRACTNUM")]
        public string ContractNum { get; set; }
        /// <summary>
        /// 签署方(甲方)
        /// </summary>
        [SugarColumn(ColumnName = "F_FIRSTPARTY")]
        public string FirstParty { get; set; }
        /// <summary>
        /// 乙方
        /// </summary>
        [SugarColumn(ColumnName = "F_SECONDPARTY")]
        public string SecondParty { get; set; }
        /// <summary>
        /// 合同名称
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTRACTNAME")]
        public string ContractName { get; set; }
        /// <summary>
        /// 合同类型
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTRACTTYPE")]
        public string ContractType { get; set; }
        /// <summary>
        /// 合作负责人
        /// </summary>
        [SugarColumn(ColumnName = "F_PERSONCHARGE")]
        public string PersonCharge { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        [SugarColumn(ColumnName = "F_LEADDEPARTMENT")]
        public string LeadDepartment { get; set; }
        /// <summary>
        /// 签订地区
        /// </summary>
        [SugarColumn(ColumnName = "F_SIGNAREA")]
        public string SignArea { get; set; }
        /// <summary>
        /// 收入金额
        /// </summary>
        [SugarColumn(ColumnName = "F_INCOMEAMOUNT")]
        public decimal? IncomeAmount { get; set; }
        /// <summary>
        /// 支出总额
        /// </summary>
        [SugarColumn(ColumnName = "F_TOTALEXPENDITURE")]
        public decimal? TotalExpenditure { get; set; }
        /// <summary>
        /// 合同期限
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTRACTPERIOD")]
        public string ContractPeriod { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        [SugarColumn(ColumnName = "F_PAYMENTMETHOD")]
        public string PaymentMethod { get; set; }
        /// <summary>
        /// 预算批付
        /// </summary>
        [SugarColumn(ColumnName = "F_BUDGETARYAPPROVAL")]
        public string BudgetaryApproval { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [SugarColumn(ColumnName = "F_STARTCONTRACTDATE")]
        public DateTime? StartContractDate { get; set; }
        /// <summary>
        /// EndContractDate
        /// </summary>
        [SugarColumn(ColumnName = "F_ENDCONTRACTDATE")]
        public DateTime? EndContractDate { get; set; }
        /// <summary>
        /// 相关附件
        /// </summary>
        [SugarColumn(ColumnName = "F_FILEJSON")]
        public string FileJson { get; set; }
        /// <summary>
        /// 内容简要
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTRACTCONTENT")]
        public string ContractContent { get; set; }
    }
}
