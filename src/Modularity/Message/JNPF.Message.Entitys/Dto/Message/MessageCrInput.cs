using JNPF.Dependency;

namespace JNPF.Message.Entitys.Dto.Message
{
    [SuppressSniffer]
    public class MessageCrInput
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 正文内容	
        /// </summary>
        public string bodyText { get; set; }

    }
}
