using JNPF.Dependency;

namespace JNPF.Common.Const
{
    /// <summary>
    /// Claim常量
    /// </summary>
    [SuppressSniffer]
    public class ClaimConst
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public const string CLAINM_USERID = "UserId";

        /// <summary>
        /// 用户姓名
        /// </summary>
        public const string CLAINM_REALNAME = "UserName";

        /// <summary>
        /// 账号
        /// </summary>
        public const string CLAINM_ACCOUNT = "Account";

        /// <summary>
        /// 是否超级管理
        /// </summary>
        public const string CLAINM_ADMINISTRATOR = "Administrator";

        /// <summary>
        /// 租户ID
        /// </summary>
        public const string TENANT_ID = "TenantId";

        /// <summary>
        /// 租户ID
        /// </summary>
        public const string TENANT_DB_NAME = "TenantDbName";
    }
}
