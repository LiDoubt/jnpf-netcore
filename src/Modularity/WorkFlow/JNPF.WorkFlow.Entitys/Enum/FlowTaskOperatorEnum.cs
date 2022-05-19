using JNPF.Dependency;
using System.ComponentModel;

namespace JNPF.WorkFlow.Entitys.Enum
{
    /// <summary>
    /// 经办对象
    /// 版 本：V3.0.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2018-06-08 
    /// </summary>
    [SuppressSniffer]
    public enum FlowTaskOperatorEnum
    {
        /// <summary>
        /// 发起者主管
        /// </summary>
        [Description("发起者主管")]
        LaunchCharge = 1,
        /// <summary>
        /// 发起者部门主管
        /// </summary>
        [Description("发起者部门主管")]
        DepartmentCharge = 2,
        /// <summary>
        /// 发起者本人
        /// </summary>
        [Description("发起者本人")]
        InitiatorMe = 3,
        /// <summary>
        /// 获取表单某个值为审批人
        /// </summary>
        [Description("变量")]
        VariableApprover = 4,
        /// <summary>
        /// 之前节点的审批人
        /// </summary>
        [Description("环节")]
        LinkApprover = 5,
        /// <summary>
        /// 授权审批人
        /// </summary>
        [Description("授权审批人")]
        FreeApprover = 7,
        /// <summary>
        /// 固定审批人（任意一人）
        /// </summary>
        [Description("或签")]
        FixedApprover = 6,
        /// <summary>
        /// 固定审批人（多人会签）
        /// </summary>
        [Description("会签")]
        FixedJointlyApprover = 8,
        /// <summary>
        /// 服务（调用指定接口获取数据）
        /// </summary>
        [Description("服务")]
        ServiceApprover = 9,
        /// <summary>
        /// 子流程
        /// </summary>
        [Description("子流程")]
        SubProcesses = 10
    }
}