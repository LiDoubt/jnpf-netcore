using JNPF.Dependency;
using Mapster;
using SqlSugar;
using System.Collections.Generic;

namespace JNPF.Common.Filter
{
    /// <summary>
    /// 分页结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SuppressSniffer]
    public class PageResult<T> where T : new()
    {
        public PageResult pagination { get; set; }

        public List<T> list { get; set; }

        // <summary>
        /// 替换sqlsugar分页
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static dynamic SqlSugarPageResult(SqlSugarPagedList<T> page)
        {
            return new
            {
                pagination = page.pagination.Adapt<PageResult>(),
                list = page.list
            };
        }
    }

    /// <summary>
    /// 分页结果
    /// </summary>
    [SuppressSniffer]
    public class PageResult : PageResult<object>
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int pageIndex { get; set; }

        /// <summary>
        /// 页容量
        /// </summary>
        public int pageSize { get; set; }

        /// <summary>
        /// 总条数
        /// </summary>
        public int total { get; set; }
    }
}
