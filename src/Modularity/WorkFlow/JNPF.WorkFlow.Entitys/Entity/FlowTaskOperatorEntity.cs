using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 流程经办
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("FLOW_TASKOPERATOR")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class FlowTaskOperatorEntity: EntityBase<string>
    {
        /// <summary>
        /// 经办对象
        /// </summary>
        [SugarColumn(ColumnName = "F_HANDLETYPE")]
        public string HandleType { get; set; }
        /// <summary>
        /// 经办主键
        /// </summary>
        [SugarColumn(ColumnName = "F_HANDLEID")]
        public string HandleId { get; set; }
        /// <summary>
        /// 处理状态：【0-拒绝、1-同意】
        /// </summary>
        [SugarColumn(ColumnName = "F_HANDLESTATUS")]
        public int? HandleStatus { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        [SugarColumn(ColumnName = "F_HANDLETIME")]
        public DateTime? HandleTime { get; set; }
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
        /// 是否完成
        /// </summary>
        [SugarColumn(ColumnName = "F_COMPLETION")]
        public int? Completion { get; set; }
        /// <summary>
        /// 描述(超时时间)
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CREATORTIME")]
        public DateTime? CreatorTime { get; set; }
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
        /// 类型
        /// </summary>
        [SugarColumn(ColumnName = "F_TYPE")]
        public string Type { get; set; }
        /// <summary>
        /// 审批状态(-1:审批拒绝)
        /// </summary>
        [SugarColumn(ColumnName = "F_STATE")]
        public string State { get; set; }
        /// <summary>
        /// 加签人
        /// </summary>
        [SugarColumn(ColumnName = "F_PARENTID")]
        public string ParentId { get; set; }
    }
}