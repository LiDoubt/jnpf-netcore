using JNPF.Dependency;

namespace JNPF.OAuth.Service.Dto
{
    /// <summary>
    /// 用户登录输出参数
    /// </summary>
    [SuppressSniffer]
    public class LoginOutput
    {
        /// <summary>
        /// 主题
        /// </summary>
        public string theme { get; set; }

        /// <summary>
        /// token
        /// </summary>
        public string token { get; set; }
    }
}
