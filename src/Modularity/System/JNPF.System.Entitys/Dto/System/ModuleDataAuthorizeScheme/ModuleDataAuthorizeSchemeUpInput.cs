using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.ModuleDataAuthorizeScheme
{
    /// <summary>
    /// 功能权限数据计划修改输入
    /// </summary>
    [SuppressSniffer]
    public class ModuleDataAuthorizeSchemeUpInput : ModuleDataAuthorizeSchemeCrInput
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }
    }
}
