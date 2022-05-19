using JNPF.Common.Filter;
using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.Permission.UsersCurrent
{
    /// <summary>
    /// 当前用户系统日记查询输入
    /// </summary>
    [SuppressSniffer]
    public class UsersCurrentSystemLogQuery : PageInputBase
    {
        /// <summary>
        /// 日记类型
        /// </summary>
        public int category { get; set; }

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
