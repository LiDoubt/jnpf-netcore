using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.System.Entitys.System
{
    /// <summary>
    /// 第三方工具对象同步表
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("BASE_SYNTHIRDINFO")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class SynThirdInfoEntity : CLEntityBase
    {
        /// <summary>
        /// 第三方类型(1:企业微信;2:钉钉)
        /// </summary>
        [SugarColumn(ColumnName = "F_THIRDTYPE")]
        public int? ThirdType { get; set; }

        /// <summary>
        /// 数据类型(1:公司;2:部门;3:用户)
        /// </summary>
        [SugarColumn(ColumnName = "F_DATATYPE")]
        public int? DataType { get; set; }

        /// <summary>
        /// 系统对象ID
        /// </summary>
        [SugarColumn(ColumnName = "F_SYSOBJID")]
        public string SysObjId { get; set; }

        /// <summary>
        /// 第三对象ID
        /// </summary>
        [SugarColumn(ColumnName = "F_THIRDOBJID")]
        public string ThirdObjId { get; set; }

        /// <summary>
        /// 0:未同步;1:同步成功;2:同步失败
        /// </summary>
        [SugarColumn(ColumnName = "F_SYNSTATE")]
        public string SynState { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
