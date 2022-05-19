using JNPF.Dependency;

namespace JNPF.Message.Entitys.Dto.Message
{
    [SuppressSniffer]
    public class MessageUpInput : MessageCrInput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
    }
}
