using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 流程可见
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("FLOW_ENGINEVISIBLE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class FlowEngineVisibleEntity:CEntityBase
    {
        /// <summary>
        /// 流程主键
        /// </summary>
        [SugarColumn(ColumnName = "F_FLOWID")]
        public string FlowId { get; set; }
        /// <summary>
        /// 经办类型
        /// </summary>
        [SugarColumn(ColumnName = "F_OPERATORTYPE")]
        public string OperatorType { get; set; }
        /// <summary>
        /// 经办主键
        /// </summary>
        [SugarColumn(ColumnName = "F_OPERATORID")]
        public string OperatorId { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }
    }
}
