using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.LetterService
{
    [SuppressSniffer]
    public class LetterServiceUpInput : LetterServiceCrInput
    {
        public string id { get; set; }
    }
}
