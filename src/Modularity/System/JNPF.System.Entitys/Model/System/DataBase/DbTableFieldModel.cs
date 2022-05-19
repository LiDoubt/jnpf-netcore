using JNPF.Dependency;

namespace JNPF.System.Entitys.Model.System.DataBase
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressSniffer]
    public class DbTableFieldModel
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string field { get; set; }

        /// <summary>
        /// 字段说明
        /// </summary>
        public string fieldName { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string dataType { get; set; }

        /// <summary>
        /// 数据长度
        /// </summary>
        public string dataLength { get; set; }

        /// <summary>
        /// 自增
        /// </summary>
        public string identity { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public int? primaryKey { get; set; }

        /// <summary>
        /// 允许null值
        /// </summary>
        public int? allowNull { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string defaults { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string description
        {
            get
            {
                return this.field + "（" + this.fieldName + "）";
            }
        }
    }
}
