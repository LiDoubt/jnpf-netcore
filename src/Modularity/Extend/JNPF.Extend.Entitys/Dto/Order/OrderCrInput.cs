using JNPF.Dependency;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace JNPF.Extend.Entitys.Dto.Order
{
    /// <summary>
    /// 新建订单
    /// </summary>
    [SuppressSniffer]
    public class OrderCrInput
    {
        /// <summary>
        /// 客户名称
        /// </summary>
        public string customerName { get; set; }
        /// <summary>
        /// 业务人员id
        /// </summary>
        public string salesmanId { get; set; }
        /// <summary>
        /// 订单日期
        /// </summary>
        public DateTime? orderDate { get; set; }
        /// <summary>
        /// 订单编码
        /// </summary>
        public string orderCode { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public string paymentMode { get; set; }
        /// <summary>
        /// 付款金额
        /// </summary>
        public decimal? receivableMoney { get; set; }
        /// <summary>
        /// 定金比率
        /// </summary>
        public decimal? earnestRate { get; set; }
        /// <summary>
        /// 预付定金
        /// </summary>
        public decimal? prepayEarnest { get; set; }
        /// <summary>
        /// 运输方式	
        /// </summary>
        public string transportMode { get; set; }
        /// <summary>
        /// 发货日期
        /// </summary>
        public DateTime? deliveryDate { get; set; }
        /// <summary>
        /// 发货地址
        /// </summary>
        public string deliveryAddress { get; set; }
        /// <summary>
        /// 相关附近
        /// </summary>
        public string fileJson { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        public string customerId { get; set; }
        /// <summary>
        /// 业务人员名字	
        /// </summary>
        public string salesmanName { get; set; }
        /// <summary>
        ///备注
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        public List<GoodsList> goodsList { get; set; }
        /// <summary>
        /// 收款计划列表
        /// </summary>
        public List<CollectionPlanList> collectionPlanList { get; set; }
    }
    /// <summary>
    /// 订单明细
    /// </summary>
    public class OrderInfo
    {
        /// <summary>
        /// 客户名称
        /// </summary>
        public string customerName { get; set; }
        /// <summary>
        /// 业务人员id
        /// </summary>
        public string salesmanId { get; set; }
        /// <summary>
        /// 订单日期
        /// </summary>
        public DateTime? orderDate { get; set; }
        /// <summary>
        /// 订单编码
        /// </summary>
        public string orderCode { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public string paymentMode { get; set; }
        /// <summary>
        /// 付款金额
        /// </summary>
        public decimal? receivableMoney { get; set; }
        /// <summary>
        /// 定金比率
        /// </summary>
        public string earnestRate { get; set; }
        /// <summary>
        /// 预付定金
        /// </summary>
        public string prepayEarnest { get; set; }
        /// <summary>
        /// 运输方式	
        /// </summary>
        public string transportMode { get; set; }
        /// <summary>
        /// 发货日期
        /// </summary>
        public DateTime? deliveryDate { get; set; }
        /// <summary>
        /// 发货地址
        /// </summary>
        public string deliveryAddress { get; set; }
        /// <summary>
        /// 相关附近
        /// </summary>
        public string fileJson { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        public string customerId { get; set; }
        /// <summary>
        /// 业务人员名字	
        /// </summary>
        public string salesmanName { get; set; }
        /// <summary>
        ///备注
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
    }
    /// <summary>
    /// 商品明细
    /// </summary>
    public class GoodsList
    {
        /// <summary>
        /// 商品id
        /// </summary>
        public string goodsId { get; set; }
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
    /// <summary>
    /// 收款明细
    /// </summary>
    [SuppressSniffer]
    public class CollectionPlanList
    {
        /// <summary>
        /// 收款日期
        /// </summary>
        public DateTime? receivableDate { get; set; }
        /// <summary>
        /// 收款比率
        /// </summary>
        public decimal? receivableRate { get; set; }
        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal? receivableMoney { get; set; }
        /// <summary>
        /// 收款方式
        /// </summary>
        public string receivableMode { get; set; }
        /// <summary>
        /// 收款摘要
        /// </summary>
        [JsonProperty("abstract")]
        public string abstracts { get; set; }
    }
}
