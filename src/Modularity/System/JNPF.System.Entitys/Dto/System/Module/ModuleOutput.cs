using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.Module
{
    /// <summary>
    /// 功能
    /// </summary>
    [SuppressSniffer]
    public class ModuleOutput
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string fullName { get; set; }
    }
}
