using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.System.Entitys.System
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("BASE_MODULEDATAAUTHORIZESCHEME")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ModuleDataAuthorizeSchemeEntity : CLDEntityBase
    {
        /// <summary>
        /// 方案编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }

        /// <summary>
        /// 方案名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }


        /// <summary>
        /// 条件规则Json
        /// </summary>
        [SugarColumn(ColumnName = "F_CONDITIONJSON")]
        public string ConditionJson { get; set; }

        /// <summary>
        /// 条件规则描述
        /// </summary>
        [SugarColumn(ColumnName = "F_CONDITIONTEXT")]
        public string ConditionText { get; set; }

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
