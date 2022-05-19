using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.Schedule
{
    /// <summary>
    /// 更新日程安排
    /// </summary>
    [SuppressSniffer]
    public class ScheduleUpInput : ScheduleCrInput
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
    }
}
