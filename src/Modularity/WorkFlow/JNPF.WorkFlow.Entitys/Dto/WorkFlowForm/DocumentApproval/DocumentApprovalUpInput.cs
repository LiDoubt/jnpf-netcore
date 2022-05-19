using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.DocumentApproval
{
    [SuppressSniffer]
    public class DocumentApprovalUpInput : DocumentApprovalCrInput
    {
        public string id { get; set; }
    }
}
