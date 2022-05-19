using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 销售支持表
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2018-07-20 
    /// </summary>
    [SugarTable("WFORM_SALESSUPPORT")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class SalesSupportEntity : EntityBase<string>
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
        /// 申请部门
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYDEPT")]
        public string ApplyDept { get; set; }
        /// <summary>
        /// 相关客户
        /// </summary>
        [SugarColumn(ColumnName = "F_CUSTOMER")]
        public string Customer { get; set; }
        /// <summary>
        /// 相关项目
        /// </summary>
        [SugarColumn(ColumnName = "F_PROJECT")]
        public string Project { get; set; }
        /// <summary>
        /// 售前支持
        /// </summary>
        [SugarColumn(ColumnName = "F_PSALESUPINFO")]
        public string PSaleSupInfo { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [SugarColumn(ColumnName = "F_STARTDATE")]
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        [SugarColumn(ColumnName = "F_ENDDATE")]
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 支持天数
        /// </summary>
        [SugarColumn(ColumnName = "F_PSALESUPDAYS")]
        public string PSaleSupDays { get; set; }
        /// <summary>
        /// 准备天数
        /// </summary>
        [SugarColumn(ColumnName = "F_PSALEPREDAYS")]
        public string PSalePreDays { get; set; }
        /// <summary>
        /// 机构咨询
        /// </summary>
        [SugarColumn(ColumnName = "F_CONSULMANAGER")]
        public string ConsulManager { get; set; }
        /// <summary>
        /// 售前顾问
        /// </summary>
        [SugarColumn(ColumnName = "F_PSALSUPCONSUL")]
        public string PSalSupConsul { get; set; }
        /// <summary>
        /// FileJson
        /// </summary>
        [SugarColumn(ColumnName = "F_FILEJSON")]
        public string FileJson { get; set; }
        /// <summary>
        /// 销售总结
        /// </summary>
        [SugarColumn(ColumnName = "F_SALSUPCONCLU")]
        public string SalSupConclu { get; set; }
        /// <summary>
        /// 交付说明
        /// </summary>
        [SugarColumn(ColumnName = "F_CONSULTRESULT")]
        public string ConsultResult { get; set; }
        /// <summary>
        /// 咨询评价
        /// </summary>
        [SugarColumn(ColumnName = "F_IEVALUATION")]
        public string IEvaluation { get; set; }
        /// <summary>
        /// 发起人总结
        /// </summary>
        [SugarColumn(ColumnName = "F_CONCLUSION")]
        public string Conclusion { get; set; }
    }
}
