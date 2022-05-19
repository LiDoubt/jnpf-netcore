using JNPF.Dependency;
using System.Collections.Generic;

namespace JNPF.System.Entitys.Model.Permission.Authorize
{
    /// <summary>
    /// 权限集合模型
    /// </summary>
    [SuppressSniffer]
    public class AuthorizeModel
    {
        /// <summary>
        /// 功能
        /// </summary>
        public List<AuthorizeModuleModel> ModuleList { get; set; }

        /// <summary>
        /// 按钮
        /// </summary>
        public List<AuthorizeModuleButtonModel> ButtonList { get; set; }

        /// <summary>
        /// 视图
        /// </summary>
        public List<AuthorizeModuleColumnModel> ColumnList { get; set; }

        /// <summary>
        /// 表单
        /// </summary>
        public List<AuthorizeModuleFormModel> FormList { get; set; }

        /// <summary>
        /// 资源
        /// </summary>
        public List<AuthorizeModuleResourceModel> ResourceList { get; set; }
    }
}
