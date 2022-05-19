using JNPF.Dependency;

namespace JNPF.Tenant.Entitys.Dto.Account
{
    [SuppressSniffer]
    public class AccountLoginInput
    {
        public string account { get; set; }

        public string password { get; set; }
    }
}
