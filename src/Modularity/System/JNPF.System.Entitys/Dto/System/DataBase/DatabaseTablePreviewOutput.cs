using JNPF.Dependency;
using System.Collections.Generic;

namespace JNPF.System.Entitys.Dto.System.Database
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressSniffer]
    public class DatabaseTablePreviewOutput
    {
        /// <summary>
        /// 数据表对应数据列表
        /// </summary>
        public List<object> myProperty { get; set; }
    }
}
