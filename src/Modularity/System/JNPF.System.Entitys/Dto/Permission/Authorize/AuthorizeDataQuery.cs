using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.Permission.Authorize
{
    /// <summary>
    /// 权限数据查询输入
    /// </summary>
    [SuppressSniffer]
    public class AuthorizeDataQuery
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 菜单ids
        /// </summary>
        public string moduleIds { get; set; }
    }
}
