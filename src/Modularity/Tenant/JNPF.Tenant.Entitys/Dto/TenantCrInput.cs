using JNPF.Dependency;

namespace JNPF.Tenant.Entitys.Dtos
{
    [SuppressSniffer]
    public class TenantCrInput
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        public string companyName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public long? expiresTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }
    }
}
