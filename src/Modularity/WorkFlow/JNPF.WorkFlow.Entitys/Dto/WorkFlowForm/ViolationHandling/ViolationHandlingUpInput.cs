using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.ViolationHandling
{
    [SuppressSniffer]
    public class ViolationHandlingUpInput : ViolationHandlingCrInput
    {
        public string id { get; set; }
    }
}
