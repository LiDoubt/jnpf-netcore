using JNPF.Dependency;
using JNPF.System.Entitys.Model.Permission.UsersCurrent;
using System.Collections.Generic;

namespace JNPF.System.Entitys.Dto.Permission.Authorize
{
    /// <summary>
    /// 当前用权限输出
    /// </summary>
    [SuppressSniffer]
    public class UsersCurrentAuthorizeOutput
    {
        /// <summary>
        /// 模型
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
        /// 数据权限资源
        /// </summary>
        public List<UsersCurrentAuthorizeMoldel> resource { get; set; }
    }
}
