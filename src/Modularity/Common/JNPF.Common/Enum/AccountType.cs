using JNPF.Dependency;
using System.ComponentModel;

namespace JNPF.Common.Enum
{
    /// <summary>
    /// 账号类型
    /// </summary>
    [SuppressSniffer]
    public enum AccountType
    {
        /// <summary>
        /// 普通账号
        /// </summary>
        [Description("普通账号")]
        None = 0,

        /// <summary>
        /// 超级管理员
        /// </summary>
        [Description("超级管理员")]
        Administrator = 1,
    }
}
