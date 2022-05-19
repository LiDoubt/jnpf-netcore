using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.Email
{
    /// <summary>
    /// 更新邮件配置
    /// </summary>
    [SuppressSniffer]
    public class EmailConfigUpInput
    {
        /// <summary>
        /// POP3服务地址
        /// </summary>
        public string pop3Host { get; set; }
        /// <summary>
        /// POP3端口
        /// </summary>
        public string pop3Port { get; set; }
        /// <summary>
        /// SMTP服务器地址
        /// </summary>
        public string smtpHost { get; set; }
        /// <summary>
        /// SMTP端口
        /// </summary>
        public string smtpPort { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string senderName { get; set; }
        /// <summary>
        /// 是否开户SSL登录(1-是,0否)
        /// </summary>
        public int? emailSsl { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string account { get; set; }
    }
}
