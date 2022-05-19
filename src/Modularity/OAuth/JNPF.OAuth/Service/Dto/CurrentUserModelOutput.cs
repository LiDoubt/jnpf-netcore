using JNPF.Dependency;
using JNPF.System.Entitys.Dto.System.Module;
using JNPF.System.Entitys.Dto.System.ModuleButton;
using JNPF.System.Entitys.Dto.System.ModuleColumn;
using JNPF.System.Entitys.Dto.System.ModuleDataAuthorizeScheme;
using System.Collections.Generic;

namespace JNPF.OAuth.Service
{
    /// <summary>
    /// 当前用户模型输出
    /// </summary>
    [SuppressSniffer]
    public class CurrentUserModelOutput
    {
        /// <summary>
        /// 菜单权限
        /// </summary>
        public List<ModuleOutput> moduleList { get; set; }

        /// <summary>
        /// 按钮权限
        /// </summary>
        public List<ModuleButtonOutput> buttonList { get; set; }

        /// <summary>
        /// 列表权限
        /// </summary>
        public List<ModuleColumnOutput> columnList { get; set; }

        /// <summary>
        /// 表单权限
        /// </summary>
        public List<ModuleColumnOutput> formList { get; set; }

        /// <summary>
        /// 数据权限
        /// </summary>
        public List<ModuleDataAuthorizeSchemeOutput> resourceList { get; set; }
    }
}
