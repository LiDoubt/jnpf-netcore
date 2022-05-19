using JNPF.Common.Filter;
using JNPF.Dependency;

namespace JNPF.Message.Entitys.Dto.Message
{
    [SuppressSniffer]
    public class MessageListInput : PageInputBase
    {
        /// <summary>
        /// 类型
        /// </summary>
        public int? type { get; set; }
    }
}
