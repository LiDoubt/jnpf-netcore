using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.Permission.UsersCurrent
{
    /// <summary>
    /// 个人资料下属输出
    /// </summary>
    [SuppressSniffer]
    public class UsersCurrentSubordinateOutput
    {
        /// <summary>
        /// 头像地址
        /// </summary>
        public string avatar { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string department { get; set; }
    }
}
