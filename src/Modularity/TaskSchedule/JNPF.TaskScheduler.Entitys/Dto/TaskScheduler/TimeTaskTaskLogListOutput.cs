using JNPF.Dependency;
using System;

namespace JNPF.TaskScheduler.Entitys.Dto.TaskScheduler
{
    [SuppressSniffer]
    public class TimeTaskTaskLogListOutput
    {
        /// <summary>
        /// 执行说明
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 执行结果
        /// </summary>
        public int runResult { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime? runTime { get; set; }
    }
}
