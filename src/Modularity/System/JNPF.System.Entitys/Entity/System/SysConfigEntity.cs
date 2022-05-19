using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.System.Entitys.System
{
    /// <summary>
    /// 系统设置
    /// </summary>
    [SugarTable("BASE_SYSCONFIG")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class SysConfigEntity : EntityBase<string>
    {
        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "F_NAME", ColumnDescription = "名称")]
        public string Name { get; set; }

        /// <summary>
        /// 键
        /// </summary>
        [SugarColumn(ColumnName = "F_KEY", ColumnDescription = "键")]
        public string Key { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [SugarColumn(ColumnName = "F_VALUE", ColumnDescription = "值")]
        public string Value { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        [SugarColumn(ColumnName = "F_CATEGORY", ColumnDescription = "分类")]
        public string Category { get; set; }
    }
}
