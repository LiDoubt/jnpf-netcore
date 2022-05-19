using JNPF.Dependency;
using System;
using System.Collections.Generic;

namespace JNPF.Extend.Entitys.Dto.Order
{
    /// <summary>
    /// 修改订单
    /// </summary>
    [SuppressSniffer]
    public class OrderUpInput
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
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        public List<GoodsListUp> goodsList { get; set; }
        /// <summary>
        /// 收款计划列表
        /// </summary>
        public List<CollectionPlanListUp> collectionPlanList { get; set; }
    }
    [SuppressSniffer]
    public class GoodsListUp : GoodsList
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        public string orderId { get; set; }
    }
    [SuppressSniffer]
    public class CollectionPlanListUp : CollectionPlanList
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        public string orderId { get; set; }
    }
}
