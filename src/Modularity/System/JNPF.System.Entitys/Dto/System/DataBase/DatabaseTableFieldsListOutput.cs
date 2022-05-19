using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.Database
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressSniffer]
    public class DatabaseTableFieldsListOutput
    {
        /// <summary>
        /// 长度
        /// </summary>
        public string dataLength { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public string primaryKey { get; set; }

        /// <summary>
        /// 是否允许为空
        /// </summary>
        public int? allowNull { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string dataType { get; set; }

        /// <summary>
        /// 字段
        /// </summary>
        public string field { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        public string fieldName { get; set; }
    }
}
