using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.ModuleDataAuthorizeScheme
{
    /// <summary>
    /// 功能权限数据计划输出
    /// </summary>
    [SuppressSniffer]
    public class ModuleDataAuthorizeSchemeOutput
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
