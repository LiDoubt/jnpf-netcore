using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.System.Entitys.System
{
    /// <summary>
    /// 数据权限
    /// 版 本：V3.0.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2017.09.20
    /// </summary>
    [SugarTable("BASE_MODULEDATAAUTHORIZE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ModuleDataAuthorizeEntity : CLDEntityBase
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }

        /// <summary>
        /// 字段编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        [SugarColumn(ColumnName = "F_TYPE")]
        public string Type { get; set; }

        /// <summary>
        /// 条件符号
        /// </summary>
        [SugarColumn(ColumnName = "F_CONDITIONSYMBOL")]
        public string ConditionSymbol { get; set; }

        /// <summary>
        /// 条件符号Json
        /// </summary>
        [SugarColumn(ColumnName = "F_CONDITIONSYMBOLJSON")]
        public string ConditionSymbolJson { get; set; }

        /// <summary>
        /// 条件内容
        /// </summary>
        [SugarColumn(ColumnName = "F_CONDITIONTEXT")]
        public string ConditionText { get; set; }

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
