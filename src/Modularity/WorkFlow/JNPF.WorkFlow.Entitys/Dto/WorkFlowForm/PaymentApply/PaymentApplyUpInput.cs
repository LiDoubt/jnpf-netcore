using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.PaymentApply
{
    [SuppressSniffer]
    public class PaymentApplyUpInput : PaymentApplyCrInput
    {
        public string id { get; set; }
    }
}
