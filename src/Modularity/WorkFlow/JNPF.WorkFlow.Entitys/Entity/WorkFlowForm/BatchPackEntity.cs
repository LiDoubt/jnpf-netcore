using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 批包装指令
    /// 版 本：V3.0.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2018-07-13 
    /// </summary>
    [SugarTable("WFORM_BATCHPACK")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class BatchPackEntity : EntityBase<string>
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
        /// 产品名称
        /// </summary>
        [SugarColumn(ColumnName = "F_PRODUCTNAME")]
        public string ProductName { get; set; }
        /// <summary>
        /// 生产车间
        /// </summary>
        [SugarColumn(ColumnName = "F_PRODUCTION")]
        public string Production { get; set; }
        /// <summary>
        /// 编制人员
        /// </summary>
        [SugarColumn(ColumnName = "F_COMPACTOR")]
        public string Compactor { get; set; }
        /// <summary>
        /// 编制日期
        /// </summary>
        [SugarColumn(ColumnName = "F_COMPACTORDATE")]
        public DateTime? CompactorDate { get; set; }
        /// <summary>
        /// 产品规格
        /// </summary>
        [SugarColumn(ColumnName = "F_STANDARD")]
        public string Standard { get; set; }
        /// <summary>
        /// 入库序号
        /// </summary>
        [SugarColumn(ColumnName = "F_WAREHOUSNO")]
        public string WarehousNo { get; set; }
        /// <summary>
        /// 批产数量
        /// </summary>
        [SugarColumn(ColumnName = "F_PRODUCTIONQUTY")]
        public string ProductionQuty { get; set; }
        /// <summary>
        /// 操作日期
        /// </summary>
        [SugarColumn(ColumnName = "F_OPERATIONDATE")]
        public DateTime? OperationDate { get; set; }
        /// <summary>
        /// 工艺规程
        /// </summary>
        [SugarColumn(ColumnName = "F_REGULATIONS")]
        public string Regulations { get; set; }
        /// <summary>
        /// 包装规格
        /// </summary>
        [SugarColumn(ColumnName = "F_PACKING")]
        public string Packing { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
