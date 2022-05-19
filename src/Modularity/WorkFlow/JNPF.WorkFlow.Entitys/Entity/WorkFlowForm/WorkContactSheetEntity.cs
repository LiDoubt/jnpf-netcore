using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 工作联系单
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("WFORM_WORKCONTACTSHEET")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class WorkContactSheetEntity : EntityBase<string>
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
        /// 发件人
        /// </summary>
        [SugarColumn(ColumnName = "F_DRAWPEOPLE")]
        public string DrawPeople { get; set; }
        /// <summary>
        /// 发件部门
        /// </summary>
        [SugarColumn(ColumnName = "F_ISSUINGDEPARTMENT")]
        public string IssuingDepartment { get; set; }
        /// <summary>
        /// 发件日期
        /// </summary>
        [SugarColumn(ColumnName = "F_TODATE")]
        public DateTime? ToDate { get; set; }
        /// <summary>
        /// 收件部门
        /// </summary>
        [SugarColumn(ColumnName = "F_SERVICEDEPARTMENT")]
        public string ServiceDepartment { get; set; }
        /// <summary>
        /// 收件人
        /// </summary>
        [SugarColumn(ColumnName = "F_RECIPIENTS")]
        public string Recipients { get; set; }
        /// <summary>
        /// 收件日期
        /// </summary>
        [SugarColumn(ColumnName = "F_COLLECTIONDATE")]
        public DateTime? CollectionDate { get; set; }
        /// <summary>
        /// 协调事项
        /// </summary>
        [SugarColumn(ColumnName = "F_COORDINATION")]
        public string Coordination { get; set; }
        /// <summary>
        /// 相关附件
        /// </summary>
        [SugarColumn(ColumnName = "F_FILEJSON")]
        public string FileJson { get; set; }
    }
}
