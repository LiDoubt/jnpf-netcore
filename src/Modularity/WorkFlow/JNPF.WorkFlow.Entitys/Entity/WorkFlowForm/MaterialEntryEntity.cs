using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 领料明细
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2018-07-16 
    /// </summary>
    [SugarTable("WFORM_MATERIALREQUISITIONENTRY")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class MaterialEntryEntity : EntityBase<string>
    {
        /// <summary>
        /// 领料主键
        /// </summary>
        [SugarColumn(ColumnName = "F_LEADEID")]
        public string LeadeId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [SugarColumn(ColumnName = "F_GOODSNAME")]
        public string GoodsName { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>
        [SugarColumn(ColumnName = "F_SPECIFICATIONS")]
        public string Specifications { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        [SugarColumn(ColumnName = "F_UNIT")]
        public string Unit { get; set; }
        /// <summary>
        /// 需数量
        /// </summary>
        [SugarColumn(ColumnName = "F_MATERIALDEMAND")]
        public string MaterialDemand { get; set; }
        /// <summary>
        /// 配数量
        /// </summary>
        [SugarColumn(ColumnName = "F_PROPORTIONING")]
        public string Proportioning { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        [SugarColumn(ColumnName = "F_PRICE")]
        public decimal? Price { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        [SugarColumn(ColumnName = "F_AMOUNT")]
        public decimal? Amount { get; set; }
        /// <summary>
        /// 备注
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