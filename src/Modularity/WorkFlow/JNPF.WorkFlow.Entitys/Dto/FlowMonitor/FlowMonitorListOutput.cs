using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.FlowMonitor
{
    [SuppressSniffer]
    public class FlowMonitorListOutput
    {
        /// <summary>
        /// 流程编码
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 发起人员id
        /// </summary>
        public string creatorUserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }
        /// <summary>
        /// 当前节点
        /// </summary>
        public string thisStep { get; set; }
        /// <summary>
        /// 所属分类
        /// </summary>
        public string flowCategory { get; set; }
        /// <summary>
        /// 流程标题
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 所属流程
        /// </summary>
        public string flowName { get; set; }
        /// <summary>
        /// 流程状态
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set; }
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 流程主键
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 流程编码
        /// </summary>
        public string flowCode { get; set; }
        /// <summary>
        /// 实例进程
        /// </summary>
        public string processId { get; set; }
        /// <summary>
        /// 完成情况
        /// </summary>
        public int? completion { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string userName{ get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 表单类型 1-系统表单、2-动态表单
        /// </summary>
        public int? formType { get; set; }
        /// <summary>
        /// 表单数据
        /// </summary>
        public string formData { get; set; }
        /// <summary>
        /// 紧急程度
        /// </summary>
        public int? formUrgent { get; set; }
        /// <summary>
        /// 委托节点id(待审页面使用，其他默认为0)
        /// </summary>
        public string delegateId { get; set; } = "0";
    }
}
