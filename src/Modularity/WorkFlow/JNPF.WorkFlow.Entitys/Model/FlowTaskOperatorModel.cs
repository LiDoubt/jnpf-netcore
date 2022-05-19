using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Model
{
    [SuppressSniffer]
    public class FlowTaskOperatorModel
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 经办类型
        /// </summary>
        public string handleType { get; set; }
        /// <summary>
        /// 经办人
        /// </summary>
        public string handleId { get; set; }
        /// <summary>
        /// 经办状态
        /// </summary>
        public int? handleStatus { get; set; }
        /// <summary>
        /// 经办时间
        /// </summary>
        public DateTime? handleTime { get; set; }
        /// <summary>
        /// 节点编码
        /// </summary>
        public string nodeCode { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string nodeName { get; set; }
        /// <summary>
        /// 完成情况
        /// </summary>
        public int? completion { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }
        /// <summary>
        /// 节点id
        /// </summary>
        public string taskNodeId { get; set; }
        /// <summary>
        /// 任务id
        /// </summary>
        public string taskId { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string signImg { get; set; }
    }
}
