using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.FinishedProduct
{
    [SuppressSniffer]
    public class FinishedProductUpInput : FinishedProductCrInput
    {
        public string id { get; set; }
    }
}
