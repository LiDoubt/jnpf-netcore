using JNPF.Common.Util;
using JNPF.Dependency;
using System.Collections.Generic;

namespace JNPF.System.Entitys.Dto.System.DataInterFace
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressSniffer]
    public class DataInterfaceSelectorOutput : TreeModel
    {
        /// <summary>
        /// 分类id
        /// </summary>
        public string categoryId { get; set; }
        /// <summary>
        /// 接口名
        /// </summary>
        public string fullName { get; set; }
    }
}
