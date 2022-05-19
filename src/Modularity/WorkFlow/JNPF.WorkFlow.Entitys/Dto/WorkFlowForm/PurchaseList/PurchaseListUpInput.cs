using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.PurchaseList
{
    [SuppressSniffer]
    public class PurchaseListUpInput : PurchaseListCrInput
    {
        public string id { get; set; }
    }
}
