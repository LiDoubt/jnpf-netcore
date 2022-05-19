using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Model
{
    [SuppressSniffer]
    public class FlowTaskNodeModel
    {
        /// <summary>
        ///id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        ///节点编码
        /// </summary>
        public string nodeCode { get; set; }
        /// <summary>
        ///节点名称
        /// </summary>
        public string nodeName { get; set; }
        /// <summary>
        ///节点类型
        /// </summary>
        public string nodeType { get; set; }
        /// <summary>
        ///节点属性
        /// </summary>
        public string nodePropertyJson { get; set; }
        /// <summary>
        ///驳回节点
        /// </summary>
        public string nodeUp { get; set; }
        /// <summary>
        ///下一节点
        /// </summary>
        public string nodeNext { get; set; }
        /// <summary>
        ///完成情况
        /// </summary>
        public int? completion { get; set; }
        /// <summary>
        ///备注
        /// </summary>
        public string description { get; set; }
        /// <summary>
        ///排序码
        /// </summary>
        public long? sortCode { get; set; }
        /// <summary>
        ///创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }
        /// <summary>
        ///任务id
        /// </summary>
        public string taskId { get; set; }
        /// <summary>
        ///经办人集合
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        ///节点标识
        /// </summary>
        public string assigneeName { get; set; }
        /// <summary>
        /// 流程图节点颜色类型(0:绿色，1：蓝色，其他：灰色)
        /// </summary>
        public string type { get; set; }
    }
}
