using JNPF.Common.Filter;
using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.Email
{
    /// <summary>
    /// (带分页)获取邮件列表入参(收件箱、标星件、草稿箱、已发送)
    /// </summary>
    [SuppressSniffer]
    public class EmailListQuery : PageInputBase
    {
        /// <summary>
        /// 开始时间，时间戳
        /// </summary>
        public long? startTime { get; set; }
        /// <summary>
        /// 结束时间，时间戳
        /// </summary>
        public long? endTime { get; set; }
        /// <summary>
        ///类型
        /// </summary>
        public string type { get; set; }

    }
}
