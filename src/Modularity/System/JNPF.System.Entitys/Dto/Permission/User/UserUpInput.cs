using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.Permission.User
{
    /// <summary>
    /// 更新用户输入
    /// </summary>
    [SuppressSniffer]
    public class UserUpInput : UserCrInput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
    }
}
