using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.Order
{
    /// <summary>
    /// 获取客户列表
    /// </summary>
    [SuppressSniffer]
    public class OrderCustomerOutput
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string code { get; set; }

    }
}
