using JNPF.Dependency;
using System;

namespace JNPF.Tenant.Entitys.Dtos
{
    [SuppressSniffer]
    public class TenantInfoOutput
    {
        public string id { get; set; }
        public string companyName { get; set; }
        public string enCode { get; set; }
        public string fullName { get; set; }
        public string description { get; set; }
        public DateTime? expiresTime { get; set; }
    }
}
