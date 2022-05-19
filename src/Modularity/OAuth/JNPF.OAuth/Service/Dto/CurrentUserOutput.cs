using JNPF.Dependency;
using JNPF.System.Entitys.Dto.System.Module;
using JNPF.System.Entitys.Model.Permission.User;
using System.Collections.Generic;

namespace JNPF.OAuth.Service.Dto
{
    /// <summary>
    /// 当前客户信息输出
    /// </summary>
    [SuppressSniffer]
    public class CurrentUserOutput
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        public UserInfo userInfo { get; set; }

        /// <summary>
        /// 菜单列表
        /// </summary>
        public List<ModuleNodeOutput> menuList { get; set; }

        /// <summary>
        /// 权限列表
        /// </summary>
        public List<PermissionModel> permissionList { get; set; }
    }

    /// <summary>
    /// 权限
    /// </summary>
    [SuppressSniffer]
    public class PermissionModel
    {
        /// <summary>
        /// 模块ID
        /// </summary>
        public string modelId { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string moduleName { get; set; }

        /// <summary>
        /// 列
        /// </summary>
        public List<AuthorizeModuleColumnModel> column { get; set; }

        /// <summary>
        /// 按钮
        /// </summary>
        public List<AuthorizeModuleButtonModel> button { get; set; }

        /// <summary>
        /// 表单
        /// </summary>
        public List<AuthorizeModuleFormModel> form { get; set; }

        /// <summary>
        /// 资源
        /// </summary>
        public List<AuthorizeModuleResourceModel> resource { get; set; }
    }

    /// <summary>
    /// 授权模块列
    /// </summary>
    [SuppressSniffer]
    public class AuthorizeModuleColumnModel
    {
        /// <summary>
        /// 按钮主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 按钮名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 按钮编码
        /// </summary>
        public string enCode { get; set; }
    }

    /// <summary>
    /// 授权模块按钮
    /// </summary>
    public class AuthorizeModuleButtonModel
    {
        /// <summary>
        /// 按钮主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 按钮名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 按钮编码
        /// </summary>
        public string enCode { get; set; }
    }

    /// <summary>
    /// 授权模块表单
    /// </summary>
    public class AuthorizeModuleFormModel
    {
        /// <summary>
        /// 表单主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 表单名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 表单编码
        /// </summary>
        public string enCode { get; set; }
    }

    /// <summary>
    /// 授权模块资源
    /// </summary>
    public class AuthorizeModuleResourceModel
    {
        /// <summary>
        /// 资源主键
        /// </summary>

        public string id { get; set; }
        /// <summary>
        /// 资源名称
        /// </summary>

        public string fullName { get; set; }
    }
}
