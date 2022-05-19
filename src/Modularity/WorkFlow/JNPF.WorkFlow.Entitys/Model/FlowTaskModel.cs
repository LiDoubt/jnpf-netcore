using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Model
{
    [SuppressSniffer]
    public class FlowTaskModel
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 实例id
        /// </summary>
        public string processId { get; set; }
        /// <summary>
        /// 任务编码
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 紧急程度
        /// </summary>
        public int? flowUrgent { get; set; }
        /// <summary>
        /// 流程id
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 流程编码
        /// </summary>
        public string flowCode { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string flowName { get; set; }
        /// <summary>
        /// 流程类型
        /// </summary>
        public int? flowType { get; set; }
        /// <summary>
        /// 流程分类
        /// </summary>
        public string flowCategory { get; set; }
        /// <summary>
        /// 表单json
        /// </summary>
        public string flowForm { get; set; }
        /// <summary>
        /// 表单数据
        /// </summary>
        public string flowFormContentJson { get; set; }
        /// <summary>
        /// 流程json
        /// </summary>
        public string flowTemplateJson { get; set; }
        /// <summary>
        /// 流程版本
        /// </summary>
        public string flowVersion { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { get; set; }
        /// <summary>
        /// 当前节点
        /// </summary>
        public string thisStep { get; set; }
        /// <summary>
        /// 当前节点id
        /// </summary>
        public string thisStepId { get; set; }
        /// <summary>
        /// 重要等级
        /// </summary>
        public string grade { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 完成情况
        /// </summary>
        public int? completion { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        public long? sortCode { get; set; }
        /// <summary>
        /// 标识
        /// </summary>
        public int? enabledMark { get; set; }
        /// <summary>
        /// app表单路径
        /// </summary>
        public string appFormUrl { get; set; }
        /// <summary>
        /// pc表单路径
        /// </summary>
        public string formUrl { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        public int? type { get; set; }
    }
}
