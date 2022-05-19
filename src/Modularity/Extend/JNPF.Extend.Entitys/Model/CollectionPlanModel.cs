using JNPF.Dependency;
using System;

namespace JNPF.Extend.Entitys.Model
{
    [SuppressSniffer]
    public class CollectionPlanModel
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
        public string fabstract { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 订单id
        /// </summary>
        public string orderId { get; set; }
        /// <summary>
        /// 接收状态
        /// </summary>
        public int? receivableState { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
