using JNPF.Common.Util;
using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.DbLink
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressSniffer]
    public class DbLinkSelectorOutput:TreeModel
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string dbType { get; set; }

        /// <summary>
        /// 库名	
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
