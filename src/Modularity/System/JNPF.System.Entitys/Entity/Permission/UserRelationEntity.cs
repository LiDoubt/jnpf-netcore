using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.System.Entitys.Permission
{
    /// <summary>
    /// 用户关系映射
    /// </summary>
    [SugarTable("BASE_USERRELATION")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class UserRelationEntity: CEntityBase
    {
        /// <summary>
        /// 获取或设置 用户编号
        /// </summary>
        [SugarColumn(ColumnName = "F_USERID", ColumnDescription = "用户编号")]
        public string UserId { get; set; }

        /// <summary>
        /// 获取或设置 对象类型
        /// </summary>
        [SugarColumn(ColumnName = "F_OBJECTTYPE", ColumnDescription = "对象类型")]
        public string ObjectType { get; set; }

        /// <summary>
        /// 获取或设置 对象主键
        /// </summary>
        [SugarColumn(ColumnName = "F_OBJECTID", ColumnDescription = "对象主键")]
        public string ObjectId { get; set; }

        /// <summary>
        /// 获取或设置 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE", ColumnDescription = "排序")]
        public long? SortCode { get; set; }
    }
}
