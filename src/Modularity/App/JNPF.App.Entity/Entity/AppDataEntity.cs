using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.Apps.Entitys
{
    /// <summary>
    /// App常用数据
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("BASE_APPDATA")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class AppDataEntity:CDEntityBase
    {
        /// <summary>
        /// 对象类型
        /// </summary>
        [SugarColumn(ColumnName = "F_OBJECTTYPE")]
        public string ObjectType { get; set; }
        /// <summary>
        /// 对象主键
        /// </summary>
        [SugarColumn(ColumnName = "F_OBJECTID")]
        public string ObjectId { get; set; }
        /// <summary>
        /// 对象json
        /// </summary>
        [SugarColumn(ColumnName = "F_OBJECTDATA")]
        public string ObjectData { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
