using JNPF.Common.Const;
using SqlSugar;

namespace JNPF.VisualData.Entity
{
    /// <summary>
    /// 可视化地图配置表
    /// </summary>
    [SugarTable("BLADE_VISUAL_MAP")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class VisualMapEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "ID", ColumnDescription = "主键", IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "Name", ColumnDescription = "名称")]
        public string Name { get; set; }

        /// <summary>
        /// 地图数据
        /// </summary>
        [SugarColumn(ColumnName = "DATA", ColumnDescription = "地图数据")]
        public string data { get; set; }

    }
}
