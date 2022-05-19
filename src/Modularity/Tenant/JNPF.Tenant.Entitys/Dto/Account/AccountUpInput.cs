using JNPF.Dependency;

namespace JNPF.Tenant.Entitys.Dto.Account
{
    [SuppressSniffer]
    public class AccountUpInput:AccountCrInput
    {
        public string id { get; set; }
    }
}
