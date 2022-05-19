using JNPF.Dependency;
using System.Collections.Generic;

namespace JNPF.System.Entitys.Dto.System.Database
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressSniffer]
    public class DatabaseTableInfoOutput
    {
        /// <summary>
        /// 表信息
        /// </summary>
        public TableInfo tableInfo { get; set; }

        /// <summary>
        /// 表字段
        /// </summary>
        public List<TableFieldList> tableFieldList { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TableFieldList
    {
        /// <summary>
        /// 允许空(0-不允许，1-允许)
        /// </summary>
        public int allowNull { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        public string dataLength { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string dataType { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string field { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string fieldName { get; set; }

        /// <summary>
        /// 是否是主键（1-是，0-否）
        /// </summary>
        public int primaryKey { get; set; }
    }
}
