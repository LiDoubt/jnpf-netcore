using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.Database
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressSniffer]
    public class DatabaseTableListOutput
    {
        /// <summary>
        /// 说明
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 表记录数
        /// </summary>
        public int sum { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string table { get; set; }

        /// <summary>
        /// 表说明
        /// </summary>
        public string tableName { get; set; } 
    }
}
