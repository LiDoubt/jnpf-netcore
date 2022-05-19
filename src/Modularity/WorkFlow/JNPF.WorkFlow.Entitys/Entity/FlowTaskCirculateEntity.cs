using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 流程传阅
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("FLOW_TASKCIRCULATE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class FlowTaskCirculateEntity: EntityBase<string>
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
        /// 节点编码
        /// </summary>
        [SugarColumn(ColumnName = "F_NODECODE")]
        public string NodeCode { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        [SugarColumn(ColumnName = "F_NODENAME")]
        public string NodeName { get; set; }
        /// <summary>
        /// 节点主键
        /// </summary>
        [SugarColumn(ColumnName = "F_TASKNODEID")]
        public string TaskNodeId { get; set; }
        /// <summary>
        /// 任务主键
        /// </summary>
        [SugarColumn(ColumnName = "F_TASKID")]
        public string TaskId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CREATORTIME")]
        public DateTime? CreatorTime { get; set; }
    }
}