using JNPF.Dependency;

namespace JNPF.WorkFlow.Entitys.Model
{
    /// <summary>
    /// 流程经办
    /// 版 本：V3.0.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// </summary>
    [SuppressSniffer]
    public class FlowHandleModel
    {
        /// <summary>
        /// 意见
        /// </summary>
        public string handleOpinion { get; set; }
        /// <summary>
        /// 加签人
        /// </summary>
        public string freeApproverUserId { get; set; }
        /// <summary>
        /// 自定义抄送人
        /// </summary>
        public string copyIds { get; set; }
        /// <summary>
        /// 流程编码
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 表单数据
        /// </summary>
        public object formData { get; set; }
        /// <summary>
        /// 流程监控指派节点
        /// </summary>
        public string nodeCode { get; set; }
        /// <summary>
        /// 电子签名
        /// </summary>
        public string signImg { get; set; }
    }
}
