using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Model
{
    [SuppressSniffer]
    public class FlowTaskOperatorRecordModel
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 节点编码
        /// </summary>
        public string nodeCode { get; set; }
        /// <summary>
        /// 节点名
        /// </summary>
        public string nodeName { get; set; }
        /// <summary>
        /// 经办状态
        /// </summary>
        public int? handleStatus { get; set; }
        /// <summary>
        /// 经办人
        /// </summary>
        public string handleId { get; set; }
        /// <summary>
        /// 经办时间
        /// </summary>
        public DateTime? handleTime { get; set; }
        /// <summary>
        /// 经办意见
        /// </summary>
        public string handleOpinion { get; set; }
        /// <summary>
        /// 经办id
        /// </summary>
        public string taskOperatorId { get; set; }
        /// <summary>
        /// 节点id
        /// </summary>
        public string taskNodeId { get; set; }
        /// <summary>
        /// 任务id
        /// </summary>
        public string taskId { get; set; }
        /// <summary>
        /// 经办人名称
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string signImg { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? status { get; set; } = 0;
        /// <summary>
        /// 流转操作人
        /// </summary>
        public string operatorId { get; set; }
    }
}
