using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.FlowDelegete
{
    [SuppressSniffer]
    public class FlowDelegeteInfoOutput
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { get; set; }
        /// <summary>
        /// 流程分类
        /// </summary>
        public string flowCategory { get; set; }
        /// <summary>
        /// 委托流程
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 委托名称
        /// </summary>
        public string flowName { get; set; }
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set; }
        /// <summary>
        /// 被委托人名
        /// </summary>
        public string toUserName { get; set; }
        /// <summary>
        /// 被委托人id
        /// </summary>
        public string toUserId { get; set; }
    }
}
