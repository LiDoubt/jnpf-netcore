using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.Order
{
    /// <summary>
    /// 获取订单列表-订单商品
    /// </summary>
    [SuppressSniffer]
    public class OrderItemsOutput
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string goodsName { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>
        public string specifications { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal? qty { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal? price { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal? amount { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal? discount { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        public decimal? cess { get; set; }
        /// <summary>
        /// 实际单价
        /// </summary>
        public decimal? actualPrice { get; set; }
        /// <summary>
        /// 实际金额
        /// </summary>
        public decimal? actualAmount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }
    }
}
