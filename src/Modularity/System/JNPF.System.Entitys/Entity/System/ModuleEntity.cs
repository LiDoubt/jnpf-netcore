using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.System.Entitys.System
{
    /// <summary>
    /// 系统功能
    /// </summary>
    [SugarTable("BASE_MODULE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ModuleEntity : CLDEntityBase
    {
        /// <summary>
        /// 功能上级
        /// </summary>
        [SugarColumn(ColumnName = "F_PARENTID")]
        public string ParentId { get; set; }

        /// <summary>
        /// 功能类别：【1-类别、2-页面】
        /// </summary>
        [SugarColumn(ColumnName = "F_TYPE")]
        public int? Type { get; set; }

        /// <summary>
        /// 功能名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }

        /// <summary>
        /// 功能编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }

        /// <summary>
        /// 功能地址
        /// </summary>
        [SugarColumn(ColumnName = "F_URLADDRESS")]
        public string UrlAddress { get; set; } = "";

        /// <summary>
        /// 按钮权限
        /// </summary>
        [SugarColumn(ColumnName = "F_ISBUTTONAUTHORIZE")]
        public int? IsButtonAuthorize { get; set; }

        /// <summary>
        /// 列表权限
        /// </summary>
        [SugarColumn(ColumnName = "F_ISCOLUMNAUTHORIZE")]
        public int? IsColumnAuthorize { get; set; }

        /// <summary>
        /// 数据权限
        /// </summary>
        [SugarColumn(ColumnName = "F_ISDATAAUTHORIZE")]
        public int? IsDataAuthorize { get; set; }

        /// <summary>
        /// 表单权限
        /// </summary>
        [SugarColumn(ColumnName = "F_IsFormAuthorize")]
        public int? IsFormAuthorize { get; set; }

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
        /// 菜单分类
        /// </summary>
        [SugarColumn(ColumnName = "F_CATEGORY")]
        public string Category { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        [SugarColumn(ColumnName = "F_ICON")]
        public string Icon { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        [SugarColumn(ColumnName = "F_LINKTARGET")]
        public string LinkTarget { get; set; }
    }
}
