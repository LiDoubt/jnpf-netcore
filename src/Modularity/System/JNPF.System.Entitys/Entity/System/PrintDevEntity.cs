using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.System.Entitys.Entity.System
{
    /// <summary>
    /// 打印模板配置
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("BASE_PRINTDEV")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class PrintDevEntity: CLDEntityBase
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
        /// 分类
        /// </summary>
        [SugarColumn(ColumnName = "F_CATEGORY")]
        public string Category { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [SugarColumn(ColumnName = "F_TYPE")]
        public int? Type { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }

        /// <summary>
        /// 数据连接id
        /// </summary>
        [SugarColumn(ColumnName = "F_DBLINKID")]
        public string DbLinkId { get; set; }

        /// <summary>
        /// sql模板
        /// </summary>
        [SugarColumn(ColumnName = "F_SQLTEMPLATE")]
        public string SqlTemplate { get; set; }

        /// <summary>
        /// 左侧字段
        /// </summary>
        [SugarColumn(ColumnName = "F_LEFTFIELDS")]
        public string LeftFields { get; set; }

        /// <summary>
        /// 打印模板
        /// </summary>
        [SugarColumn(ColumnName = "F_PRINTTEMPLATE")]
        public string PrintTemplate { get; set; }
    }
}
