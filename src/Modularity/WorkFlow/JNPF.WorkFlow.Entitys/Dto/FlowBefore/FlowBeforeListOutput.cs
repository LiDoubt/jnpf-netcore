using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.FlowBefore
{
    [SuppressSniffer]
    public class FlowBeforeListOutput
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string creatorUserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }
        /// <summary>
        /// 当前节点名
        /// </summary>
        public string thisStep { get; set; }
        /// <summary>
        /// 流程分类
        /// </summary>
        public string flowCategory { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 引擎名称
        /// </summary>
        public string flowName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 发起时间
        /// </summary>
        public DateTime? startTime { get; set; }

        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 发起人
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 引擎编码
        /// </summary>
        public string flowCode { get; set; }
        /// <summary>
        /// 引擎id
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 实例id
        /// </summary>
        public string processId { get; set; }
        /// <summary>
        /// 表单类型
        /// </summary>
        public int? formType { get; set; }
        /// <summary>
        /// 表单Json
        /// </summary>
        public string formData { get; set; }
        /// <summary>
        /// 紧急程度
        /// </summary>
        public int? flowUrgent { get; set; }

        /// <summary>
        /// 当前节点id
        /// </summary>
        public string thisStepId { get; set; }

        /// <summary>
        /// 流程进度
        /// </summary>
        public int? completion { get; set; }
    }
}
