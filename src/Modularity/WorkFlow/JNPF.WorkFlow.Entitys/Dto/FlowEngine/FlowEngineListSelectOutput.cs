using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.FlowEngine
{
    [SuppressSniffer]
    public class FlowEngineListSelectOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        public string fullName { get; set; }

    }
}
