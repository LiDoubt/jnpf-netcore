using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.Tenant.Entitys
{
    /// <summary>
    /// 租户日志
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("BASE_TENANTLOG")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class TenantLogEntity: EntityBase<string>
    {
        /// <summary>
        /// 租户主键
        /// </summary>
        [SugarColumn(ColumnName = "F_TENANTID")]
        public string TenantId { get; set; }
        /// <summary>
        /// 登陆账户
        /// </summary>
        [SugarColumn(ColumnName = "F_LOGINACCOUNT")]
        public string LoginAccount { get; set; }
        /// <summary>
        /// 登陆IP地址
        /// </summary>
        [SugarColumn(ColumnName = "F_LOGINIPADDRESS")]
        public string LoginIPAddress { get; set; }
        /// <summary>
        /// 登陆IP归属地
        /// </summary>
        [SugarColumn(ColumnName = "F_LOGINIPADDRESSNAME")]
        public string LoginIPAddressName { get; set; }
        /// <summary>
        /// 来源网站
        /// </summary>
        [SugarColumn(ColumnName = "F_LOGINSOURCEWEBSITE")]
        public string LoginSourceWebsite { get; set; }
        /// <summary>
        /// 登陆时间
        /// </summary>
        [SugarColumn(ColumnName = "F_LOGINTIME")]
        public DateTime? LoginTime { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
