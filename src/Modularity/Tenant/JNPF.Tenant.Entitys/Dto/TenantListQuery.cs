using JNPF.Common.Filter;
using JNPF.Dependency;

namespace JNPF.Tenant.Entitys.Dtos
{
    [SuppressSniffer]
    public class TenantListQuery : PageInputBase
    {
        public long? startTime { get; set; }
        public long? endTime { get; set; }
    }
}
