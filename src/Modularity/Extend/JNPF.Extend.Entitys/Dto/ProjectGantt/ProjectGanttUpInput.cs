using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.ProjectGantt
{
    /// <summary>
    /// 修改项目
    /// </summary>
    [SuppressSniffer]
    public class ProjectGanttUpInput : ProjectGanttCrInput
    {
        /// <summary>
        /// 项目id
        /// </summary>
        public string id { get; set; }
    }
}
