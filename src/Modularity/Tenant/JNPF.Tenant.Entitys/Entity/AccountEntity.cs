using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.Tenant.Entitys.Entity
{
    /// <summary>
    /// 租户登录配置
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("BASE_ACCOUNT")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class AccountEntity : CLDEntityBase
    {
        /// <summary>
        /// 账户
        /// </summary>
        [SugarColumn(ColumnName = "F_ACCOUNT")]
        public string Account { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [SugarColumn(ColumnName = "F_REALNAME")]
        public string RealName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [SugarColumn(ColumnName = "F_GENDER")]
        public int? Gender { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_PASSWORD")]
        public string Password { get; set; }

        /// <summary>
        /// 是否管理员【0-普通、1-管理员】
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_ISADMINISTRATOR")]
        public int? IsAdministrator { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }
    }
}
