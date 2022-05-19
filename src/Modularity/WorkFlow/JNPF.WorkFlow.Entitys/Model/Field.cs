using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Model
{
    [SuppressSniffer]
    public class Field
    {
        public string filedId { get; set; }
        public string filedName { get; set; }
        public bool required { get; set; }
    }
}
