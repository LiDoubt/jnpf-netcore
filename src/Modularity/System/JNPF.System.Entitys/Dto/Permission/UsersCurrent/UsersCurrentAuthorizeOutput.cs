using JNPF.Dependency;
using JNPF.System.Entitys.Model.Permission.UsersCurrent;
using System.Collections.Generic;

namespace JNPF.System.Entitys.Dto.Permission.UsersCurrent
{
    /// <summary>
    /// 当前用户权限输出
    /// </summary>
    [SuppressSniffer]
    public class UsersCurrentAuthorizeOutput
    {
        /// <summary>
        /// 模块
        /// </summary>
        public List<UsersCurrentAuthorizeMoldel> module { get; set; }

        /// <summary>
        /// 列
        /// </summary>
        public List<UsersCurrentAuthorizeMoldel> column { get; set; }

        /// <summary>
        /// 按钮
        /// </summary>
        public List<UsersCurrentAuthorizeMoldel> button { get; set; }

        /// <summary>
        /// 资源
        /// </summary>
        public List<UsersCurrentAuthorizeMoldel> resource { get; set; }
    }
}
