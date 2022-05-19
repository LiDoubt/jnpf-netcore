using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.BatchPack
{
    [SuppressSniffer]
    public class BatchPackUpInput : BatchPackCrInput
    {
        public string id { get; set; }
    }
}
