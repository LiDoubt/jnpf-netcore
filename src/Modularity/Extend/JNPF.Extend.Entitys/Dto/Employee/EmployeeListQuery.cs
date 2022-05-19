using JNPF.Common.Filter;
using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.Employee
{
    /// <summary>
    /// 获取职员列表(分页)
    /// </summary>
    [SuppressSniffer]
    public class EmployeeListQuery : PageInputBase
    {
        /// <summary>
        /// 查询内容
        /// </summary>
        public string condition { get; set; }
        /// <summary>
        /// 查询字段
        /// </summary>
        public string selectKey { get; set; }
        /// <summary>
        /// 是否分页（0：分页）
        /// </summary>
        public string dataType { get; set; }

    }
}
