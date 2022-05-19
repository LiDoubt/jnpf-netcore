using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.Tenant.Entitys
{
    /// <summary>
    /// 租户信息
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("BASE_TENANT")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class TenantEntity:CLDEntityBase
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }
        /// <summary>
        /// 公司
        /// </summary>
        [SugarColumn(ColumnName = "F_COMPANYNAME")]
        public string CompanyName { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        [SugarColumn(ColumnName = "F_EXPIRESTIME")]
        public DateTime? ExpiresTime { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        [SugarColumn(ColumnName = "F_DBNAME")]
        public string DbName { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        [SugarColumn(ColumnName = "F_IPADDRESS")]
        public string IPAddress { get; set; }
        /// <summary>
        /// ip归属地
        /// </summary>
        [SugarColumn(ColumnName = "F_IPADDRESSNAME")]
        public string IPAddressName { get; set; }
        /// <summary>
        /// 来源网站
        /// </summary>
        [SugarColumn(ColumnName = "F_SOURCEWEBSITE")]
        public string SourceWebsite { get; set; }
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
    }
}
