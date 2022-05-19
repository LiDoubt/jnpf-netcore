using JNPF.Dependency;

namespace JNPF.Tenant.Entitys.Dto
{
    [SuppressSniffer]
    public class TenantLoginOutput
    {
        public string account { get; set; }

        public string realName { get; set; }

        public string token { get; set; }
    }
}
