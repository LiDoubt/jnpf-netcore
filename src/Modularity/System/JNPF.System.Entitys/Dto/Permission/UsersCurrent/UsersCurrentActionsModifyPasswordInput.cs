using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.Permission.UsersCurrent
{
    /// <summary>
    /// 当前用户修改密码输入
    /// </summary>
    [SuppressSniffer]
    public class UsersCurrentActionsModifyPasswordInput
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        public string oldPassword { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 验证码时间戳
        /// </summary>
        public string timestamp { get; set; }
    }
}
