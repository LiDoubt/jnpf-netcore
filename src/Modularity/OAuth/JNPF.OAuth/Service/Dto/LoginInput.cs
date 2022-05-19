using JNPF.Dependency;
using System.ComponentModel.DataAnnotations;

namespace JNPF.OAuth.Service.Dto
{
    /// <summary>
    /// 登录输入参数
    /// </summary>
    [SuppressSniffer]
    public class LoginInput
    {
        /// <summary>
        /// 用户名
        /// </summary>
        /// <example>admin</example>
        [Required(ErrorMessage = "用户名不能为空")]
        public string account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        /// <example>e10adc3949ba59abbe56e057f20f883e</example>
        [Required(ErrorMessage = "密码不能为空")]
        public string password { get; set; }
    }
}
