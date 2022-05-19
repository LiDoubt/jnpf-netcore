using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.Database
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressSniffer]
    public class DatabaseTableFieldsSelectorOutput
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string field { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string fieldName { get; set; }
    }
}
