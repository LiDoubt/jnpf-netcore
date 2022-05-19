using JNPF.Common.Filter;
using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.Permission.User
{
    /// <summary>
    /// 用户列表查询输入
    /// </summary>
    [SuppressSniffer]
    public class UserListQuery : PageInputBase
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public string organizeId { get; set; }
    }
}
