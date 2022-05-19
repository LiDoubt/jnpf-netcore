using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.Document
{
    /// <summary>
    /// 共享文件
    /// </summary>
    [SuppressSniffer]
    public class DocumentActionsShareInput
    {
        /// <summary>
        /// 共享用户
        /// </summary>
        public string userId { get; set; }
    }
}
