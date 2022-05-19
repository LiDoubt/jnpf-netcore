using JNPF.Dependency;

namespace JNPF.Tenant.Entitys.Dtos
{
    [SuppressSniffer]
    public class TenantDbContentOutput
    {
        public string dbName { get; set; }
    }
}
