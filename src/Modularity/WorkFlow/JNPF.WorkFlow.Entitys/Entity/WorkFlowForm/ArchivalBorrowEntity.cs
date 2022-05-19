using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 档案借阅申请
    /// 版 本：V3.0.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2018-07-13
    /// </summary>
    [SugarTable("WFORM_ARCHIVALBORROW")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ArchivalBorrowEntity : EntityBase<string>
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
        /// 借阅部门
        /// </summary>
        [SugarColumn(ColumnName = "F_BORROWINGDEPARTMENT")]
        public string BorrowingDepartment { get; set; }
        /// <summary>
        /// 档案名称
        /// </summary>
        [SugarColumn(ColumnName = "F_ARCHIVESNAME")]
        public string ArchivesName { get; set; }
        /// <summary>
        /// 借阅时间
        /// </summary>
        [SugarColumn(ColumnName = "F_BORROWINGDATE")]
        public DateTime? BorrowingDate { get; set; }
        /// <summary>
        /// 归还时间
        /// </summary>
        [SugarColumn(ColumnName = "F_RETURNDATE")]
        public DateTime? ReturnDate { get; set; }
        /// <summary>
        /// 档案属性
        /// </summary>
        [SugarColumn(ColumnName = "F_ARCHIVALATTRIBUTES")]
        public string ArchivalAttributes { get; set; }
        /// <summary>
        /// 借阅方式
        /// </summary>
        [SugarColumn(ColumnName = "F_BORROWMODE")]
        public string BorrowMode { get; set; }
        /// <summary>
        /// 申请原因
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYREASON")]
        public string ApplyReason { get; set; }
        /// <summary>
        /// 档案编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ARCHIVESID")]
        public string ArchivesId { get; set; }
    }
}
