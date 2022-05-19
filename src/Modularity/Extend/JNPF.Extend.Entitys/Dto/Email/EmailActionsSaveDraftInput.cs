using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.Email
{
    /// <summary>
    /// 存草稿
    /// </summary>
    [SuppressSniffer]
    public class EmailActionsSaveDraftInput : EmailSendInput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
    }
}
