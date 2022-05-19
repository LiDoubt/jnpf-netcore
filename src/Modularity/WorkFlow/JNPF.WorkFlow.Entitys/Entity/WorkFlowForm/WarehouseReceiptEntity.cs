using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 入库申请单
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("WFORM_WAREHOUSERECEIPT")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class WarehouseReceiptEntity : EntityBase<string>
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
        /// 供应商名称
        /// </summary>
        [SugarColumn(ColumnName = "F_SUPPLIERNAME")]
        public string SupplierName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTACTPHONE")]
        public string ContactPhone { get; set; }
        /// <summary>
        /// 入库类别
        /// </summary>
        [SugarColumn(ColumnName = "F_WAREHOUSCATEGORY")]
        public string WarehousCategory { get; set; }
        /// <summary>
        /// 仓库
        /// </summary>
        [SugarColumn(ColumnName = "F_WAREHOUSE")]
        public string Warehouse { get; set; }
        /// <summary>
        /// 入库人
        /// </summary>
        [SugarColumn(ColumnName = "F_WAREHOUSESPEOPLE")]
        public string WarehousesPeople { get; set; }
        /// <summary>
        /// 送货单号
        /// </summary>
        [SugarColumn(ColumnName = "F_DELIVERYNO")]
        public string DeliveryNo { get; set; }
        /// <summary>
        /// 入库单号
        /// </summary>
        [SugarColumn(ColumnName = "F_WAREHOUSENO")]
        public string WarehouseNo { get; set; }
        /// <summary>
        /// 入库日期
        /// </summary>
        [SugarColumn(ColumnName = "F_WAREHOUSDATE")]
        public DateTime? WarehousDate { get; set; }
    }
}