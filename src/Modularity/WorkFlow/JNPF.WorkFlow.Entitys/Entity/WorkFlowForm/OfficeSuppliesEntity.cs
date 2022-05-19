using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 领用办公用品申请表
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2018-07-16
    /// </summary>
    [SugarTable("WFORM_OFFICESUPPLIES")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class OfficeSuppliesEntity : EntityBase<string>
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
        /// 申请部门
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYUSER")]
        public string ApplyUser { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        [SugarColumn(ColumnName = "F_DEPARTMENT")]
        public string Department { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYDATE")]
        public DateTime? ApplyDate { get; set; }
        /// <summary>
        /// 领用仓库
        /// </summary>
        [SugarColumn(ColumnName = "F_USESTOCK")]
        public string UseStock { get; set; }
        /// <summary>
        /// 用品分类
        /// </summary>
        [SugarColumn(ColumnName = "F_CLASSIFICATION")]
        public string Classification { get; set; }
        /// <summary>
        /// 用品名称
        /// </summary>
        [SugarColumn(ColumnName = "F_ARTICLESNAME")]
        public string ArticlesName { get; set; }
        /// <summary>
        /// 用品数量
        /// </summary>
        [SugarColumn(ColumnName = "F_ARTICLESNUM")]
        public string ArticlesNum { get; set; }
        /// <summary>
        /// 用品编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ARTICLESID")]
        public string ArticlesId { get; set; }
        /// <summary>
        /// 申请原因
        /// </summary>
        [SugarColumn(ColumnName = "F_APPLYREASONS")]
        public string ApplyReasons { get; set; }
    }
}
