using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.BillRule
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressSniffer]
    public class BillRuleCrInput
    {
        /// <summary>
        /// 业务名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 业务编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 流水前缀
        /// </summary>
        public string prefix { get; set; }

        /// <summary>
        /// 流水日期
        /// </summary>
        public string dateFormat { get; set; }

        /// <summary>
        /// 流水位数
        /// </summary>
        public double digit { get; set; }

        /// <summary>
        /// 
        /// 流水起始
        /// </summary>
        public string startNumber { get; set; }

        /// <summary>
        /// 流水范例
        /// </summary>
        public string example { get; set; }

        /// <summary>
        /// 流水状态
        /// </summary>
        public int enabledMark { get; set; }

        /// <summary>
        /// 流水说明
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
