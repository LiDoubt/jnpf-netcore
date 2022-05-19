using JNPF.Dependency;
using JNPF.Extend.Entitys.Model;
using System;
using System.Collections.Generic;

namespace JNPF.Extend.Entitys.Dto.Order
{
    /// <summary>
    /// 获取/查看订单信息
    /// </summary>
    [SuppressSniffer]
    public class OrderInfoOutput
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string creatorUserId { get; set; }
        /// <summary>
        /// 客户id
        /// </summary>
        public string customerId { get; set; }
        /// <summary>
        /// 客户名
        /// </summary>
        public string customerName { get; set; }
        /// <summary>
        /// 交易地点
        /// </summary>
        public string deliveryAddress { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime? deliveryDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 定金比率
        /// </summary>
        public string earnestRate { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? enabledMark { get; set; }
        /// <summary>
        /// 相关附件
        /// </summary>
        public string fileJson { get; set; }
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? lastModifyTime { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string lastModifyUserId { get; set; }
        /// <summary>
        /// 订单编码
        /// </summary>
        public string orderCode { get; set; }
        /// <summary>
        /// 订单日期
        /// </summary>
        public DateTime? orderDate { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public string paymentMode { get; set; }
        /// <summary>
        /// 预付定金
        /// </summary>
        public string prepayEarnest { get; set; }
        /// <summary>
        /// 付款金额
        /// </summary>
        public decimal? receivableMoney { get; set; }
        /// <summary>
        /// 业务人id
        /// </summary>
        public string salesmanId { get; set; }
        /// <summary>
        /// 业务人名
        /// </summary>
        public string salesmanName { get; set; }
        /// <summary>
        /// 运输方式
        /// </summary>
        public string transportMode { get; set; }
        /// <summary>
        /// 订单商品
        /// </summary>
        public List<GoodsModel> goodsList { get; set; }
        /// <summary>
        /// 收款计划
        /// </summary>
        public List<CollectionPlanModel> collectionPlanList { get; set; }
    }
}
