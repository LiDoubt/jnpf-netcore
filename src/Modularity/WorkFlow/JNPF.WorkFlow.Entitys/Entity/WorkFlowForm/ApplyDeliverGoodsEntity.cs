using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 发货申请单
    /// 版 本：V3.0.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("WFORM_APPLYDELIVERGOODS")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ApplyDeliverGoodsEntity : EntityBase<string>
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
        /// 客户名称
        /// </summary>
        [SugarColumn(ColumnName = "F_CUSTOMERNAME")]
        public string CustomerName { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTACTS")]
        public string Contacts { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        [SugarColumn(ColumnName = "F_CONTACTPHONE")]
        public string ContactPhone { get; set; }
        /// <summary>
        /// 客户地址
        /// </summary>
        [SugarColumn(ColumnName = "F_CUSTOMERADDRES")]
        public string CustomerAddres { get; set; }
        /// <summary>
        /// 货品所属
        /// </summary>
        [SugarColumn(ColumnName = "F_GOODSBELONGED")]
        public string GoodsBelonged { get; set; }
        /// <summary>
        /// 发货日期
        /// </summary>
        [SugarColumn(ColumnName = "F_INVOICEDATE")]
        public DateTime? InvoiceDate { get; set; }
        /// <summary>
        /// 货运公司
        /// </summary>
        [SugarColumn(ColumnName = "F_FREIGHTCOMPANY")]
        public string FreightCompany { get; set; }
        /// <summary>
        /// 发货类型
        /// </summary>
        [SugarColumn(ColumnName = "F_DELIVERYTYPE")]
        public string DeliveryType { get; set; }
        /// <summary>
        /// 货运单号
        /// </summary>
        [SugarColumn(ColumnName = "F_RRANSPORTNUM")]
        public string RransportNum { get; set; }
        /// <summary>
        /// 货运费
        /// </summary>
        [SugarColumn(ColumnName = "F_FREIGHTCHARGES")]
        public decimal? FreightCharges { get; set; }
        /// <summary>
        /// 保险金额
        /// </summary>
        [SugarColumn(ColumnName = "F_CARGOINSURANCE")]
        public decimal? CargoInsurance { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 发货金额
        /// </summary>
        [SugarColumn(ColumnName = "F_INVOICEVALUE")]
        public decimal? InvoiceValue { get; set; }
    }
}