using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.DataSync
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressSniffer]
    public class DbSyncActionsExecuteInput
    {
        /// <summary>
        /// 源数据库id
        /// </summary>
        public string dbConnectionFrom { get; set; }

        /// <summary>
        /// 目前数据库id
        /// </summary>
        public string dbConnectionTo { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string dbTable { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public int type { get; set; }
    }
}
