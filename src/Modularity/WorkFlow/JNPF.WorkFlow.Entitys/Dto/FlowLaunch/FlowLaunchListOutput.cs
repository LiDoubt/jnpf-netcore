using JNPF.Dependency;
using System;

namespace JNPF.WorkFlow.Entitys.Dto.FlowLaunch
{
    [SuppressSniffer]
    public class FlowLaunchListOutput
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 创建人
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
        /// 流程分类
        /// </summary>
        public string flowCategory { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string flowName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set; }
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { get; set; }
        /// <summary>
        /// 完成度
        /// </summary>
        public int? completion { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 流程编码
        /// </summary>
        public string flowCode { get; set; }
        /// <summary>
        /// 流程id
        /// </summary>
        public string flowId { get; set; }
        /// <summary>
        /// 表单分类
        /// </summary>
        public int? formType { get; set; } = 2;
        /// <summary>
        /// 表单json
        /// </summary>
        public string formData { get; set; }
        /// <summary>
        /// 委托节点id(待审页面使用，其他默认为0)
        /// </summary>
        public string delegateId { get; set; } = "0";
    }
}
