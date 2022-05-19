using JNPF.Dependency;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace JNPF.WorkFlow.Entitys.Model
{
    /// <summary>
    /// 流程节点模板实体
    /// </summary>
    [SuppressSniffer]
    public class FlowTemplateJsonModel
    {
        /// <summary>
        /// 节点类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 节点内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 节点属性
        /// </summary>
        public JObject properties { get; set; }
        /// <summary>
        /// 当前节点标识
        /// </summary>
        public string nodeId { get; set; }
        /// <summary>
        /// 上级节点标识
        /// </summary>
        public string prevId { get; set; }
        /// <summary>
        /// 子节点
        /// </summary>
        public FlowTemplateJsonModel childNode { get; set; }
        /// <summary>
        /// 节点条件
        /// </summary>
        public List<FlowTemplateJsonModel> conditionNodes { get; set; }
        /// <summary>
        /// 条件类型
        /// </summary>
        public string conditionType { get; set; }
        /// <summary>
        /// 是否分流
        /// </summary>
        public bool isInterflow { get; set; }
    }
}
