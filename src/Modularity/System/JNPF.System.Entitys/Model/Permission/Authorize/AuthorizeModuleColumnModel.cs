using JNPF.Dependency;

namespace JNPF.System.Entitys.Model.Permission.Authorize
{
    /// <summary>
    /// 模块列表权限模型
    /// </summary>
    [SuppressSniffer]
    public class AuthorizeModuleColumnModel
    {
        /// <summary>
        /// 列表主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 列表上级
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 列表名称
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 列表编码
        /// </summary>
        public string EnCode { get; set; }

        /// <summary>
        /// 绑定表格
        /// </summary>
        public string BindTable { get; set; }

        /// <summary>
        /// 绑定表格
        /// </summary>
        public string BindTableName { get; set; }

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
