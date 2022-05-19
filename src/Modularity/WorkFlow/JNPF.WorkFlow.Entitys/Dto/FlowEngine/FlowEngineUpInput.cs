using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.FlowEngine
{
    [SuppressSniffer]
    public class FlowEngineUpInput : FlowEngineCrInput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
    }
}
