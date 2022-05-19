using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.WorkFlow.Entitys
{
    /// <summary>
    /// 流程经办记录
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01
    /// </summary>
    [SugarTable("FLOW_TASKOPERATORRECORD")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class FlowTaskOperatorRecordEntity : EntityBase<string>
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
        /// 经办状态：【0-拒绝、1-同意、2-提交、3-撤回、4-终止】
        /// </summary>
        [SugarColumn(ColumnName = "F_HANDLESTATUS")]
        public int? HandleStatus { get; set; }
        /// <summary>
        /// 经办人员
        /// </summary>
        [SugarColumn(ColumnName = "F_HANDLEID")]
        public string HandleId { get; set; }
        /// <summary>
        /// 经办时间
        /// </summary>
        [SugarColumn(ColumnName = "F_HANDLETIME")]
        public DateTime? HandleTime { get; set; }
        /// <summary>
        /// 经办理由
        /// </summary>
        [SugarColumn(ColumnName = "F_HANDLEOPINION")]
        public string HandleOpinion { get; set; }
        /// <summary>
        /// 经办主键
        /// </summary>
        [SugarColumn(ColumnName = "F_TASKOPERATORID")]
        public string TaskOperatorId { get; set; }
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
        /// 电子签名
        /// </summary>
        [SugarColumn(ColumnName = "F_SIGNIMG")]
        public string SignImg { get; set; }
        /// <summary>
        /// 审批标识(1:加签人)
        /// </summary>
        [SugarColumn(ColumnName = "F_STATUS")]
        public int? Status { get; set; }
        /// <summary>
        /// 流转操作人
        /// </summary>
        [SugarColumn(ColumnName = "F_OPERATORID")]
        public string OperatorId { get; set; }
    }
}