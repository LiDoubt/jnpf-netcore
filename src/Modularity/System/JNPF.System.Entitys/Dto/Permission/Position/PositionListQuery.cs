using JNPF.Common.Filter;
using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.Permission.Position
{
    /// <summary>
    /// 岗位列表查询输入
    /// </summary>
    [SuppressSniffer]
    public class PositionListQuery : PageInputBase
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public string organizeId { get; set; }
    }
}
