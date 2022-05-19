using JNPF.Common.Filter;
using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.FlowMonitor
{
    [SuppressSniffer]
    public class FlowMonitorListQuery : PageInputBase
    {

        /// <summary>
        /// 发起人员id
        /// </summary>
        public string creatorUserId { get; set; }

        /// <summary>
        /// 所属分类
        /// </summary>
        public string flowCategory { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public long? startTime { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public long? endTime { get; set; }

        /// <summary>
        /// 流程主键
        /// </summary>
        public string flowId { get; set; }


    }
}
