namespace JNPF.VisualDev.Entitys.Model.CodeGen
{
    /// <summary>
    /// 数据库表列
    /// </summary>
    public class TableColumnConfigModel
    {
        /// <summary>
        /// 字段名-大写(剔除"F_","f_")
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 原本名称
        /// </summary>
        public string OriginalColumnName { get; set; }

        /// <summary>
        /// 数据库字段名(首字母小写)
        /// </summary>
        public string LowerColumnName => string.IsNullOrWhiteSpace(ColumnName)
                                      ? null
                                      : ColumnName.Substring(0, 1).ToLower() + ColumnName[1..];

        /// <summary>
        /// 数据库中名称
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 数据库中类型
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// .NET字段类型
        /// </summary>
        public string NetType { get; set; }

        /// <summary>
        /// 字段描述
        /// </summary>
        public string ColumnComment { get; set; }

        /// <summary>
        /// 是否是查询条件
        /// </summary>
        public bool QueryWhether { get; set; }

        /// <summary>
        /// 查询方式
        /// </summary>
        public int QueryType { get; set; }

        /// <summary>
        /// 是否展示
        /// </summary>
        public bool IsShow { get; set; }

        /// <summary>
        /// 是否多选
        /// </summary>
        public bool IsMultiple { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool PrimaryKey { get; set; }

        /// <summary>
        /// 控件类型
        /// </summary>
        public string jnpfKey { get; set; }

        /// <summary>
        /// 单据规则
        /// </summary>
        public string Rule { get; set; }
    }
}