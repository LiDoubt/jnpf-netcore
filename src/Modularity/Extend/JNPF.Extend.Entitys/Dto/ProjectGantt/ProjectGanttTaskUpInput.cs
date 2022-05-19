using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.ProjectGantt
{
    /// <summary>
    /// 修改项目任务
    /// </summary>
    [SuppressSniffer]
    public class ProjectGanttTaskUpInput : ProjectGanttTaskCrInput
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
    }
}
