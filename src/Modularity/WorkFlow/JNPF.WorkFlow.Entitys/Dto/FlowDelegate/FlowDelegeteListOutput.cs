using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.FlowDelegete
{
    [SuppressSniffer]
    public class FlowDelegeteListOutput
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        public string creatorUserId { get; set; }
        /// <summary>
        /// 删除标志
        /// </summary>
        public int? deleteMark { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? deleteTime { get; set; }
        /// <summary>
        /// 删除用户
        /// </summary>
        public string deleteUserId { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 有效标志
        /// </summary>
        public int enabledMark { get; set; }
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
        /// 委托流程名称
        /// </summary>
        public string flowName { get; set; }
        /// <summary>
        /// 自然主键
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? lastModifyTime { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>
        public string lastModifyUserId { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        public long? sortCode { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set; }
        /// <summary>
        /// 被委托人名字
        /// </summary>
        public string toUserName { get; set; }
        /// <summary>
        /// 被委托人
        /// </summary>
        public string toUserId { get; set; }
    }
}
