using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.System.Entitys.System
{
    /// <summary>
    /// 系统功能按钮
    /// </summary>
    [SugarTable("BASE_MODULECOLUMN")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ModuleColumnEntity : CLDEntityBase
    {
        /// <summary>
        /// 列表上级
        /// </summary>
        [SugarColumn(ColumnName = "F_PARENTID")]
        public string ParentId { get; set; }

        /// <summary>
        /// 列表名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }


        /// <summary>
        /// 列表编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }

        /// <summary>
        /// 绑定表格Id
        /// </summary>
        [SugarColumn(ColumnName = "F_BINDTABLE")]
        public string BindTable { get; set; }

        /// <summary>
        /// 绑定表格描述
        /// </summary>
        [SugarColumn(ColumnName = "F_BINDTABLENAME")]
        public string BindTableName { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        [SugarColumn(ColumnName = "F_PROPERTYJSON")]
        public string PropertyJson { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }

        /// <summary>
        /// 功能主键
        /// </summary>
        [SugarColumn(ColumnName = "F_MODULEID")]
        public string ModuleId { get; set; }
    }
}
