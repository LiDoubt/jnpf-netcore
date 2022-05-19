using JNPF.Dependency;

namespace JNPF.Tenant.Entitys.Dto.Account
{
    [SuppressSniffer]
    public class AccountModifyPasswordInput
    {
        public string userPassword { get; set; }
    }
}
