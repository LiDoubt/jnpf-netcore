using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.BatchTable
{
    [SuppressSniffer]
    public class BatchTableUpInput : BatchTableCrInput
    {
        public string id { get; set; }
    }
}
