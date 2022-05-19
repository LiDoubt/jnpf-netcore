using System.Collections.Generic;

namespace SqlSugar
{
    /// <summary>
    /// 分页泛型集合
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class SqlSugarPagedList<TEntity> where TEntity : new()
    {
        /// <summary>
        /// 数据
        /// </summary>
        public PagedModel pagination { get; set; }

        /// <summary>
        /// 当前页集合
        /// </summary>
        public IEnumerable<TEntity> list { get; set; }
    }

    /// <summary>
    /// 分页集合
    /// </summary>
    public class PagedModel : SqlSugarPagedList<object>
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页容量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总条数
        /// </summary>
        public int Total { get; set; }
    }
}