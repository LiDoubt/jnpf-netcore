using JNPF.Dependency;

namespace JNPF.System.Entitys.Model.Permission.Authorize
{
    /// <summary>
    /// 模块资源权限模型
    /// </summary>
    [SuppressSniffer]
    public class AuthorizeModuleResourceModel
    {
        /// <summary>
        /// 资源主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 资源名称
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 资源编码
        /// </summary>
        public string EnCode { get; set; }

        /// <summary>
        /// 条件规则Json
        /// </summary>
        public string ConditionJson { get; set; }

        /// <summary>
        /// 条件规则描述
        /// </summary>
        public string ConditionText { get; set; }

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
