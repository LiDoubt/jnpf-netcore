using JNPF.Common.Filter;
using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.DataInterFace
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressSniffer]
    public class DataInterfaceListQuery : PageInputBase
    {
        /// <summary>
        /// 分类id
        /// </summary>
        public string categoryId { get; set; }
    }
}
