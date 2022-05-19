using JNPF.Common.Filter;
using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.FlowBefore
{
    [SuppressSniffer]
    public class FlowBeforeListQuery : PageInputBase
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
        /// 引擎id
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 引擎分类
        /// </summary>
        public string flowCategory { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string creatorUserId { get; set; }
    }
}
