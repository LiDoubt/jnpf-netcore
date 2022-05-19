using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.ModuleDataAuthorize
{
    /// <summary>
    /// 功能权限数据输出
    /// </summary>
    [SuppressSniffer]
    public class ModuleDataAuthorizeOutput
    {
        /// <summary>
        /// 资源主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 资源名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 功能主键
        /// </summary>
        public string moduleId { get; set; }
    }
}
