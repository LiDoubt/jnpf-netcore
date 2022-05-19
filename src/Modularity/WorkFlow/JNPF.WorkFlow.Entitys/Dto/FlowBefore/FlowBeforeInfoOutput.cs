using JNPF.Dependency;
using JNPF.WorkFlow.Entitys.Model;
using JNPF.WorkFlow.Entitys.Model.Properties;
using System.Collections.Generic;

namespace JNPF.WorkFlow.Entitys.Dto.FlowBefore
{
    [SuppressSniffer]
    public class FlowBeforeInfoOutput
    {
        /// <summary>
        /// 表单json
        /// </summary>
        public string flowFormInfo { get; set; }
        /// <summary>
        /// 流程任务
        /// </summary>
        public FlowTaskModel flowTaskInfo { get; set; }
        /// <summary>
        /// 流程任务节点
        /// </summary>
        public List<FlowTaskNodeModel> flowTaskNodeList { get; set; }
        /// <summary>
        /// 流程任务经办
        /// </summary>
        public List<FlowTaskOperatorModel> flowTaskOperatorList { get; set; }
        /// <summary>
        /// 流程任务经办记录
        /// </summary>
        public List<FlowTaskOperatorRecordModel> flowTaskOperatorRecordList { get; set; }
        /// <summary>
        /// 当前节点权限
        /// </summary>
        public List<FormOperatesModel> formOperates { get; set; } = new List<FormOperatesModel>();
        /// <summary>
        /// 当前节点属性
        /// </summary>
        public ApproversProperties approversProperties { get; set; } = new ApproversProperties();
    }
}
