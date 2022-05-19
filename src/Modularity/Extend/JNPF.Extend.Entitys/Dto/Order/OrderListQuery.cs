using JNPF.Common.Filter;
using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.Order
{
    /// <summary>
    /// 获取订单列表入参(带分页)
    /// </summary>
    [SuppressSniffer]
    public class OrderListQuery : PageInputBase
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public long? startTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public long? endTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? enabledMark { get; set; }

    }
}
