using JNPF.Dependency;

namespace JNPF.Common.Filter
{
    /// <summary>
    /// 通用分页输入参数
    /// </summary>
    [SuppressSniffer]
    public class PageInputBase : KeywordInput
    {
        /// <summary>
        /// 查询条件
        /// </summary>
        public virtual string queryJson { get; set; }

        /// <summary>
        /// 当前页码:pageIndex
        /// </summary>
        public virtual int currentPage { get; set; } = 1;

        /// <summary>
        /// 每页行数
        /// </summary>
        public virtual int pageSize { get; set; } = 50;

        /// <summary>
        /// 排序字段:sortField
        /// </summary>
        public virtual string sidx { get; set; }

        /// <summary>
        /// 排序类型:sortType
        /// </summary>
        public virtual string sort { get; set; } = "desc";
    }
}
