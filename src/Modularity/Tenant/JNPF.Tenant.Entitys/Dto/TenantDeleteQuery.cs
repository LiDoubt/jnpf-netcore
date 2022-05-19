using JNPF.Dependency;

namespace JNPF.Tenant.Entitys.Dtos
{
    [SuppressSniffer]
    public class TenantDeleteQuery
    {
        /// <summary>
        /// (0-不删除，1-删除)是否删除租户数据库
        /// </summary>
        public int? isClear { get; set; }
    }
}
