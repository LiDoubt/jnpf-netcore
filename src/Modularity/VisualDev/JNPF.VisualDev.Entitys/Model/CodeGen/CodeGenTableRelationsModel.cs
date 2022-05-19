using System.Collections.Generic;

namespace JNPF.VisualDev.Entitys.Model.CodeGen
{
    /// <summary>
    /// 代码生成表关系模型
    /// </summary>
    public class CodeGenTableRelationsModel
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 表名(首字母小写)
        /// </summary>
        public string LowerTableName => string.IsNullOrWhiteSpace(TableName)
                                      ? null
                                      : TableName.Substring(0, 1).ToLower() + TableName[1..];

        /// <summary>
        /// 主键
        /// </summary>
        public string PrimaryKey { get; set; }

        /// <summary>
        /// 表描述
        /// </summary>
        public string TableComment { get; set; }

        /// <summary>
        /// 外键字段
        /// </summary>
        public string TableField { get; set; }

        /// <summary>
        /// 关联主键
        /// </summary>
        public string RelationField { get; set; }

        /// <summary>
        /// 子表控件配置
        /// </summary>
        public List<TableColumnConfigModel> ChilderColumnConfigList { get; set; }
    }
}
