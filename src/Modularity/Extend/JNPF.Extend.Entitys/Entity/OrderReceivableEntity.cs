using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.Extend.Entitys
{
    /// <summary>
    /// 订单收款
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01  
    /// </summary>
    [SugarTable("EXT_ORDERRECEIVABLE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class OrderReceivableEntity : EntityBase<string>
    {
        /// <summary>
        /// 订单主键
        /// </summary>
        [SugarColumn(ColumnName = "F_ORDERID")]
        public string OrderId { get; set; }
        /// <summary>
        /// 收款摘要
        /// </summary>
        [SugarColumn(ColumnName = "F_ABSTRACT")]
        public string Abstract { get; set; }
        /// <summary>
        /// 收款日期
        /// </summary>
        [SugarColumn(ColumnName = "F_RECEIVABLEDATE")]
        public DateTime? ReceivableDate { get; set; }
        /// <summary>
        /// 收款比率
        /// </summary>
        [SugarColumn(ColumnName = "F_RECEIVABLERATE")]
        public decimal? ReceivableRate { get; set; }
        /// <summary>
        /// 收款金额
        /// </summary>
        [SugarColumn(ColumnName = "F_RECEIVABLEMONEY")]
        public decimal? ReceivableMoney { get; set; }
        /// <summary>
        /// 收款方式
        /// </summary>
        [SugarColumn(ColumnName = "F_RECEIVABLEMODE")]
        public string ReceivableMode { get; set; }
        /// <summary>
        /// 收款状态
        /// </summary>
        [SugarColumn(ColumnName = "F_RECEIVABLESTATE")]
        public int? ReceivableState { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }
    }
}