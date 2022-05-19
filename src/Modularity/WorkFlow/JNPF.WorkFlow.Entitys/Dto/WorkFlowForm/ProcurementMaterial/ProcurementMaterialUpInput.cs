using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.ProcurementMaterial
{
    [SuppressSniffer]
    public class ProcurementMaterialUpInput : ProcurementMaterialCrInput
    {
        public string id { get; set; }
    }
}
