using JNPF.Common.Filter;
using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.FlowLaunch
{
    [SuppressSniffer]
    public class FlowLaunchListQuery : PageInputBase
    {
        /// <summary>
        /// 所属分类
        /// </summary>
        public string flowCategory { get; set; }
        /// <summary>
        /// 所属流程
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public long? startTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public long? endTime { get; set; }
    }
}
