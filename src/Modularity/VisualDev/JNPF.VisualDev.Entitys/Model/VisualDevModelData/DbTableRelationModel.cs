namespace JNPF.VisualDev.Entitys.Model.VisualDevModelData
{
    /// <summary>
    /// 数据关联
    /// </summary>
    public class DbTableRelationModel
    {
        /// <summary>
        /// 类型：1-主表、0-子表
        /// </summary>
        public int typeId { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string table { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string tableName { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public string tableKey { get; set; }

        /// <summary>
        /// 外键字段
        /// </summary>
        public string tableField { get; set; }

        /// <summary>
        /// 关联主表
        /// </summary>
        public string relationTable { get; set; }

        /// <summary>
        /// 关联主键
        /// </summary>
        public string relationField { get; set; }
    }
}
