using JNPF.Common.Filter;
using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.Database
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressSniffer]
    public class DatabaseTablePreviewQuery : PageInputBase
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string field { get; set; }
    }
}
