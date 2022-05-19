using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.SalesOrder
{
    [SuppressSniffer]
    public class SalesOrderUpInput : SalesOrderCrInput
    {
        public string id { get; set; }
    }
}
