using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.OutboundOrder
{
    [SuppressSniffer]
    public class OutboundOrderUpInput : OutboundOrderCrInput
    {
        public string id { get; set; }
    }
}
