using JNPF.Dependency;

namespace JNPF.System.Entitys.Model.Permission.Authorize
{
    /// <summary>
    /// 模块权限模型
    /// </summary>
    [SuppressSniffer]
    public class AuthorizeModuleModel
    {
        /// <summary>
        /// 功能主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 功能上级
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 功能类别【1-类别、2-页面】
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 功能名称
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 功能编码
        /// </summary>
        public string EnCode { get; set; }

        /// <summary>
        /// 功能地址
        /// </summary>
        public string UrlAddress { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? SortCode { get; set; }
    }
}
