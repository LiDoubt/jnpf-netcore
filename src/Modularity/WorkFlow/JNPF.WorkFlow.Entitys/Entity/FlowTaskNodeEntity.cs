using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 流程节点
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("FLOW_TASKNODE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class FlowTaskNodeEntity : EntityBase<string>
    {
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
        /// 节点类型
        /// </summary>
        [SugarColumn(ColumnName = "F_NODETYPE")]
        public string NodeType { get; set; }
        /// <summary>
        /// 节点属性Json
        /// </summary>
        [SugarColumn(ColumnName = "F_NODEPROPERTYJSON")]
        public string NodePropertyJson { get; set; }
        /// <summary>
        /// 驳回节点(0:驳回发起，1：驳回指定或上一节点)
        /// </summary>
        [SugarColumn(ColumnName = "F_NODEUP")]
        public string NodeUp { get; set; }
        /// <summary>
        /// 下一节点
        /// </summary>
        [SugarColumn(ColumnName = "F_NODENEXT")]
        public string NodeNext { get; set; }
        /// <summary>
        /// 是否完成：【0-未处理、1-已审核、-1-被驳回】
        /// </summary>
        [SugarColumn(ColumnName = "F_COMPLETION")]
        public int? Completion { get; set; }
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
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CREATORTIME")]
        public DateTime? CreatorTime { get; set; }
        /// <summary>
        /// 任务主键
        /// </summary>
        [SugarColumn(ColumnName = "F_TASKID")]
        public string TaskId { get; set; }
        /// <summary>
        /// 节点状态（0：正常，-2：作废）
        /// </summary>
        [SugarColumn(ColumnName = "F_STATE")]
        public string State { get; set; }


    }
}