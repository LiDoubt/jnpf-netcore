using JNPF.Dependency;

namespace JNPF.System.Entitys.Model.System.DataBase
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressSniffer]
    public class DbTableModel
    {
        /// <summary>
        /// 标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string table { get; set; }

        /// <summary>
        /// 表说明
        /// </summary>
        public string tableName { get; set; }

        /// <summary>
        /// 大小
        /// </summary>
        public string size { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int? sum { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string description
        {
            get
            {
                return this.table + "（" + this.tableName + "）";
            }
        }

        /// <summary>
        /// 主键
        /// </summary>
        public string primaryKey { get; set; }

        /// <summary>
        /// 数据源主键
        /// </summary>
        public string dataSourceId { get; set; }
    }
}
