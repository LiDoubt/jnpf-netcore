using JNPF.Common.Util;
using JNPF.Dependency;
using System;

namespace JNPF.Extend.Entitys.Dto.ProjectGantt
{
    /// <summary>
    /// 获取项目任务树形
    /// </summary>
    [SuppressSniffer]
    public class ProjectGanttTaskTreeViewOutput : TreeModel
    {
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 完成度
        /// </summary>
        public int? schedule { get; set; }
        /// <summary>
        /// 项目id
        /// </summary>
        public string projectId { get; set; }

        /// <summary>
        /// 标记颜色
        /// </summary>
        public string signColor { get; set; }
        /// <summary>
        /// 标记
        /// </summary>
        public string sign { get; set; }
    }
}
