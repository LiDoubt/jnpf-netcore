using JNPF.Common.Util;
using JNPF.Dependency;
using System;

namespace JNPF.Extend.Entitys.Dto.ProjectGantt
{
    /// <summary>
    /// 获取项目任务列表
    /// </summary>
    [SuppressSniffer]
    public class ProjectGanttTaskListOutput : TreeModel
    {

        /// <summary>
        /// 任务名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 标记
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 标记颜色
        /// </summary>
        public string signColor { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { get; set; }
        /// <summary>
        /// 完成进度
        /// </summary>
        public int? schedule { get; set; }

    }
}
