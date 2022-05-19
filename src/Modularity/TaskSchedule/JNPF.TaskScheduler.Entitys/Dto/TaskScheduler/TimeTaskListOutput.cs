using JNPF.Dependency;
using System;

namespace JNPF.TaskScheduler.Entitys.Dto.TaskScheduler
{
    [SuppressSniffer]
    public class TimeTaskListOutput
    {
        /// <summary>
        /// 任务标题
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 任务编码
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 执行次数
        /// </summary>
        public string runCount { get; set; } = "0";
        /// <summary>
        /// 执行说明
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 下次运行时间（时间戳）
        /// </summary>
        public DateTime? nextRunTime { get; set; }
        /// <summary>
        /// 最后执行时间（时间戳）
        /// </summary>
        public DateTime? lastRunTime { get; set; }
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 执行状态(1-正常,0-异常)
        /// </summary>
        public int? enabledMark { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
