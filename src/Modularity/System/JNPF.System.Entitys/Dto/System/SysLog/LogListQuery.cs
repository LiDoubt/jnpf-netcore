using JNPF.Common.Filter;
using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.SysLog
{
    /// <summary>
    /// 系统日志列表入参
    /// </summary>
    [SuppressSniffer]
    public class LogListQuery : PageInputBase
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public long? startTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public long? endTime { get; set; }
    }
}
