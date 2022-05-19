using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.Schedule
{
    /// <summary>
    /// 获取日程安排列表入参
    /// </summary>
    [SuppressSniffer]
    public class ScheduleListQuery
    {

        /// <summary>
        /// 开始时间
        /// </summary>
        public string startTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string endTime { get; set; }

        public string dateTime { get; set; }

    }
}
