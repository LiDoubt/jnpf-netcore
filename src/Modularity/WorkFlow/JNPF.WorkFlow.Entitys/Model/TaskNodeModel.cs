using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Model
{
    [SuppressSniffer]
    public class TaskNodeModel
    {
        /// <summary>
        /// 任务id
        /// </summary>
        public string taskId { get; set; }
        /// <summary>
        /// 节点编码
        /// </summary>
        public string nodeId { get; set; }
        /// <summary>
        /// 上节点编码
        /// </summary>
        public string upNodeId { get; set; }
        /// <summary>
        /// 下节点编码
        /// </summary>
        public string nextNodeId { get; set; }
        /// <summary>
        /// 属性json
        /// </summary>
        public dynamic propertyJson { get; set; }
        /// <summary>
        /// 节点类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 状态（0：正常，-2：作废）
        /// </summary>
        public int status { get; set; } = 0;
        /// <summary>
        /// 是否分流
        /// </summary>
        public bool isInterflow { get; set; }
        /// <summary>
        /// 子节点编码
        /// </summary>
        public string childNodeId { get; set; }
    }
}
