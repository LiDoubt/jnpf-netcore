using JNPF.Dependency;
using System;

namespace JNPF.Extend.Entitys.Dto.Schedule
{
    /// <summary>
    /// 获取日程安排列表
    /// </summary>
    [SuppressSniffer]
    public class ScheduleListOutput
    {
        /// <summary>
        /// 日程内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string colour { get; set; }
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
    }
}
