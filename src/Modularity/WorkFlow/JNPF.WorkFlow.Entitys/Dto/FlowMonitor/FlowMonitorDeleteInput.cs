using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.FlowMonitor
{
    [SuppressSniffer]
    public class FlowMonitorDeleteInput
    {
        /// <summary>
        /// 流程任务id集合
        /// </summary>
        public string ids { get; set; }
    }
}
