using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.VisualDev.Entitys
{
    /// <summary>
    /// 可视化开发功能实体
    /// </summary>
    [SugarTable("BASE_VISUALDEV")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class VisualDevEntity : CLDEntityBase
    {
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
        /// 状态(0-暂存（默认），1-发布)
        /// </summary>
        [SugarColumn(ColumnName = "F_STATE")]
        public int? State { get; set; } = 0;

        /// <summary>
        /// 类型
        /// 1-Web设计,2-App设计,3-流程表单,4-Web表单,5-App表单
        /// </summary>
        [SugarColumn(ColumnName = "F_TYPE")]
        public int? Type { get; set; }

        /// <summary>
        /// 关联的表
        /// </summary>
        [SugarColumn(ColumnName = "F_TABLE")]
        public string Tables { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        [SugarColumn(ColumnName = "F_CATEGORY")]
        public string Category { get; set; }

        /// <summary>
        /// 表单配置JSON
        /// </summary>
        [SugarColumn(ColumnName = "F_FORMDATA")]
        public string FormData { get; set; }

        /// <summary>
        /// 列表配置JSON
        /// </summary>
        [SugarColumn(ColumnName = "F_COLUMNDATA")]
        public string ColumnData { get; set; }

        /// <summary>
        /// 排序码(默认0)
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }

        /// <summary>
        /// 描述或说明
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }

        /// <summary>
        /// 关联数据连接id
        /// </summary>
        [SugarColumn(ColumnName = "F_DBLINKID")]
        public string DbLinkId { get; set; }

        /// <summary>
        /// 工作流模板JSON
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWTEMPLATEJSON")]
        public string FlowTemplateJson { get; set; }

        /// <summary>
        /// 页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）
        /// </summary>
        [SugarColumn(ColumnName = "F_WEBTYPE")]
        public int? WebType { get; set; }

        /// <summary>
        /// 工作流引擎ID
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWID")]
        public string FlowId { get; set; }
    }
}
