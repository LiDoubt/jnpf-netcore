using JNPF.Dependency;
using System.Collections.Generic;

namespace JNPF.WorkFlow.Entitys.Model.Properties
{
    [SuppressSniffer]
    public class ChildTaskProperties
    {
        /// <summary>
        /// 子流程
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 子流程发起人(1:自定义，2：部门主管，3：发起者主管，4：发起者本人)
        /// </summary>
        public int initiateType { get; set; }
        /// <summary>
        /// 主管级别
        /// </summary>
        public int managerLevel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> initiator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> initiatePos { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> initiateRole { get; set; }
        /// <summary>
        /// 子流程引擎
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 继承父流程字段数据
        /// </summary>
        public List<Assign> assignList { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public List<string> messageType { get; set; }
        /// <summary>
        /// 子流程id
        /// </summary>
        public List<string> childTaskId { get; set; } = new List<string>();
        /// <summary>
        /// 子流程数据
        /// </summary>
        public string formData { get; set; }
    }

    public class Assign
    {
        /// <summary>
        /// 父流程字段
        /// </summary>
        public string parentField { get; set; }
        /// <summary>
        /// 子流程字段
        /// </summary>
        public string childField { get; set; }
    }
}
