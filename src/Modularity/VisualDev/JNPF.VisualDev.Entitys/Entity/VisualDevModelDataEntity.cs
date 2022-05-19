using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.VisualDev.Entitys.Entity
{
    /// <summary>
    /// 可视化开发功能实体
    /// 版 本：V2.6.200612
    /// 版 权：引迈信息技术有限公司（https://www.yinmaisoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2020-10-28 
    /// </summary>
    [SugarTable("BASE_VISUALDEV_MODELDATA")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class VisualDevModelDataEntity : CLDEntityBase
    {
        /// <summary>
        /// 功能ID
        /// </summary>
        [SugarColumn(ColumnName = "F_VISUALDEVID")]
        public string VisualDevId { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }

        /// <summary>
        /// 区分主子表-
        /// </summary>
        [SugarColumn(ColumnName = "F_PARENTID")]
        public string ParentId { get; set; }

        /// <summary>
        /// 数据包
        /// </summary>
        [SugarColumn(ColumnName = "F_DATA")]
        public string Data { get; set; }
    }
}
