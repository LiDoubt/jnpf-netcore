using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.Order
{
    /// <summary>
    /// 获取商品列表
    /// </summary>
    [SuppressSniffer]
    public class OrderGoodsOutput
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>
        public string specifications { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// 售价
        /// </summary>
        public double price { get; set; }
    }
}
