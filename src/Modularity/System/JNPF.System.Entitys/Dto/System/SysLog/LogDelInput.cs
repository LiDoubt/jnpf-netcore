using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.SysLog
{
    /// <summary>
    /// 日记批量删除输入
    /// </summary>
    [SuppressSniffer]
    public class LogDelInput
    {
        /// <summary>
        /// 删除id
        /// </summary>
        public string[] ids { get; set; }
    }
}
