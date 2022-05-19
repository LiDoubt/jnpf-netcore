using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.DocumentSigning
{
    [SuppressSniffer]
    public class DocumentSigningUpInput : DocumentSigningCrInput
    {
        public string id { get; set; }
    }
}
