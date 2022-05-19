using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.ReceiptSign
{
    [SuppressSniffer]
    public class ReceiptSignUpInput : ReceiptSignCrInput
    {
        public string id { get; set; }
    }
}
