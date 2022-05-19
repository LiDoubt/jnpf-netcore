using JNPF.Dependency;

namespace JNPF.System.Entitys.Model.Permission.Authorize
{
    /// <summary>
    /// 模块表单权限模型
    /// </summary>
    [SuppressSniffer]
    public class AuthorizeModuleFormModel
    {
        /// <summary>
        /// 表单主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 表单上级
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 表单名称
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 表单编码
        /// </summary>
        public string EnCode { get; set; }

        /// <summary>
        /// 功能主键
        /// </summary>
        public string ModuleId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? SortCode { get; set; }
    }
}
