using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.ReceiptProcessing
{
    [SuppressSniffer]
    public class ReceiptProcessingUpInput : ReceiptProcessingCrInput
    {
        public string id { get; set; }
    }
}
