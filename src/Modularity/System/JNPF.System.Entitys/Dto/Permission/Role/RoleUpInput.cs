using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.Permission.Role
{
    /// <summary>
    /// 更新角色输入
    /// </summary>
    [SuppressSniffer]
    public class RoleUpInput : RoleCrInput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
    }
}
