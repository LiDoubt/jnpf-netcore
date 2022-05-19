using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.Email
{
    /// <summary>
    /// 发邮件
    /// </summary>
    [SuppressSniffer]
    public class EmailSendInput
    {
        /// <summary>
        /// 主题
        /// </summary>
        public string subject { get; set; }
        /// <summary>
        /// 收件人
        /// </summary>
        public string recipient { get; set; }
        /// <summary>
        /// 正文
        /// </summary>
        public string bodyText { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        public string attachment { get; set; }
        /// <summary>
        /// 抄送人	
        /// </summary>
        public string cc { get; set; }
        /// <summary>
        /// 密送人	
        /// </summary>
        public string bcc { get; set; }
    }
}
