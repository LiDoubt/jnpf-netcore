using JNPF.Common.Const;
using SqlSugar;

namespace JNPF.VisualData.Entity
{
    /// <summary>
    /// 可视化分类表
    /// </summary>
    [SugarTable("BLADE_VISUAL_CATEGORY")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class VisualCategoryEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "ID", ColumnDescription = "主键", IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 分类键值
        /// </summary>
        [SugarColumn(ColumnName = "CATEGORY_KEY", ColumnDescription = "分类键值")]
        public string CategoryKey { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        [SugarColumn(ColumnName = "CATEGORY_VALUE", ColumnDescription = "分类名称")]
        public string CategoryValue { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        [SugarColumn(ColumnName = "IS_DELETED", ColumnDescription = "是否已删除")]
        public string IsDeleted { get; set; }

        /// <summary>
        /// 删除
        /// </summary>
        public virtual void Delete()
        {
            this.IsDeleted = "1";
        }
    }
}
