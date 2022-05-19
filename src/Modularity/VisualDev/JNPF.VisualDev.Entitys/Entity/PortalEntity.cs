using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.VisualDev.Entitys
{
    /// <summary>
    /// 门户表
    /// </summary>
    [SugarTable("BASE_PORTAL")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class PortalEntity : CLDEntityBase
    {
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
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }

        /// <summary>
        /// 分类(数据字典维护)
        /// </summary>
        [SugarColumn(ColumnName = "F_CATEGORY")]
        public string Category { get; set; }

        /// <summary>
        /// 表单配置JSON
        /// </summary>
        [SugarColumn(ColumnName = "F_FORMDATA")]
        public string FormData { get; set; }
    }
}
