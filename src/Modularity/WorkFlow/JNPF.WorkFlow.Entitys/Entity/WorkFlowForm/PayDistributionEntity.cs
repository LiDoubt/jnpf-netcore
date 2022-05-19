using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 薪酬发放
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("WFORM_PAYDISTRIBUTION")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class PayDistributionEntity : EntityBase<string>
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
        /// 所属月份
        /// </summary>
        [SugarColumn(ColumnName = "F_MONTH")]
        public string Month { get; set; }
        /// <summary>
        /// 发放单位
        /// </summary>
        [SugarColumn(ColumnName = "F_ISSUINGUNIT")]
        public string IssuingUnit { get; set; }
        /// <summary>
        /// 员工部门
        /// </summary>
        [SugarColumn(ColumnName = "F_DEPARTMENT")]
        public string Department { get; set; }
        /// <summary>
        /// 员工职位
        /// </summary>
        [SugarColumn(ColumnName = "F_POSITION")]
        public string Position { get; set; }
        /// <summary>
        /// 基本薪资
        /// </summary>
        [SugarColumn(ColumnName = "F_BASESALARY")]
        public decimal? BaseSalary { get; set; }
        /// <summary>
        /// 出勤天数
        /// </summary>
        [SugarColumn(ColumnName = "F_ACTUALATTENDANCE")]
        public string ActualAttendance { get; set; }
        /// <summary>
        /// 员工津贴
        /// </summary>
        [SugarColumn(ColumnName = "F_ALLOWANCE")]
        public decimal? Allowance { get; set; }
        /// <summary>
        /// 所得税
        /// </summary>
        [SugarColumn(ColumnName = "F_INCOMETAX")]
        public decimal? IncomeTax { get; set; }
        /// <summary>
        /// 员工保险
        /// </summary>
        [SugarColumn(ColumnName = "F_INSURANCE")]
        public decimal? Insurance { get; set; }
        /// <summary>
        /// 员工绩效
        /// </summary>
        [SugarColumn(ColumnName = "F_PERFORMANCE")]
        public decimal? Performance { get; set; }
        /// <summary>
        /// 加班费用
        /// </summary>
        [SugarColumn(ColumnName = "F_OVERTIMEPAY")]
        public decimal? OvertimePay { get; set; }
        /// <summary>
        /// 应发工资
        /// </summary>
        [SugarColumn(ColumnName = "F_GROSSPAY")]
        public decimal? GrossPay { get; set; }
        /// <summary>
        /// 实发工资
        /// </summary>
        [SugarColumn(ColumnName = "F_PAYROLL")]
        public decimal? Payroll { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
