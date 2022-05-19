using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.System.Entitys.System
{
    /// <summary>
    /// 常用字段
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("BASE_COMFIELDS")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ComFieldsEntity : CLDEntityBase
    {
        /// <summary>
        /// 字段注释
        /// </summary>
        [SugarColumn(ColumnName = "F_FIELDNAME")]
        public string FieldName { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        [SugarColumn(ColumnName = "F_FIELD")]
        public string Field { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [SugarColumn(ColumnName = "F_DATATYPE")]
        public string DataType { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        [SugarColumn(ColumnName = "F_DATALENGTH")]
        public string DataLength { get; set; }

        /// <summary>
        /// 允许空
        /// </summary>
        [SugarColumn(ColumnName = "F_ALLOWNULL")]
        public int? AllowNull { get; set; }

        /// <summary>
        /// 排序码(默认0)
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }

        /// <summary>
        /// 描述说明
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
