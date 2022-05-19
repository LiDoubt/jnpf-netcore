using JNPF.Dependency;
using System.Collections.Generic;

namespace JNPF.WorkFlow.Entitys.Model.Properties
{
    [SuppressSniffer]
    public class ApproversProperties
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 指定审批人
        /// </summary>
        public List<string> approvers { get; set; } = new List<string>();
        /// <summary>
        /// 指定审批岗位
        /// </summary>
        public List<string> approverPos { get; set; } = new List<string>();
        /// <summary>
        /// 审批类型（类型参考FlowTaskOperatorEnum类）
        /// </summary>
        public int assigneeType { get; set; }
        /// <summary>
        /// 表单权限数据
        /// </summary>
        public List<FormOperatesModels> formOperates { get; set; }
        /// <summary>
        /// 指定抄送岗位
        /// </summary>
        public List<string> circulatePosition { get; set; }
        /// <summary>
        /// 指定抄送人
        /// </summary>
        public List<string> circulateUser { get; set; }
        /// <summary>
        /// 进度
        /// </summary>
        public string progress { get; set; }
        /// <summary>
        /// 驳回节点
        /// </summary>
        public string rejectStep { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 审批事件(同意)
        /// </summary>
        public bool hasApproverFunc { get; set; }
        /// <summary>
        /// 审批事件接口url(同意)
        /// </summary>
        public string approverInterfaceUrl { get; set; }
        /// <summary>
        /// 审批事件(拒绝)
        /// </summary>
        public bool hasApproverRejectFunc { get; set; }
        /// <summary>
        /// 审批事件接口url(拒绝)
        /// </summary>
        public string approverRejectInterfaceUrl { get; set; }
        /// <summary>
        /// 审批事件接口请求类型
        /// </summary>
        public string approverInterfaceType { get; set; }
        /// <summary>
        /// 定时器到时时间
        /// </summary>
        public List<TimerProperties> timerList { get; set; } = new List<TimerProperties>();
        /// <summary>
        /// 审批召回事件
        /// </summary>
        public bool hasRecallFunc { get; set; }
        /// <summary>
        /// 审批召回事件接口url
        /// </summary>
        public string recallInterfaceUrl { get; set; }
        /// <summary>
        /// 指定审批角色
        /// </summary>
        public List<string> approverRole { get; set; } = new List<string>();
        /// <summary>
        /// 抄送角色
        /// </summary>
        public List<string> circulateRole { get; set; } = new List<string>();
        /// <summary>
        /// 自定义抄送人
        /// </summary>
        public bool isCustomCopy { get; set; }
        /// <summary>
        /// 发起人的第几级主管
        /// </summary>
        public int? managerLevel { get; set; } = 1;
        /// <summary>
        /// 会签比例
        /// </summary>
        public int? countersignRatio { get; set; } = 100;
        /// <summary>
        /// 审批类型（0：或签 1：会签） 
        /// </summary>
        public int? counterSign { get; set; } = 0;
        /// <summary>
        /// 表单字段
        /// </summary>
        public string formField { get; set; }
        /// <summary>
        /// 指定复审审批节点
        /// </summary>
        public string nodeId { get; set; }
        /// <summary>
        /// 服务 请求路径
        /// </summary>
        public string getUserUrl { get; set; }
        /// <summary>
        /// 审批人为空时是否自动通过
        /// </summary>
        public bool noApproverHandler { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool hasAuditBtn { get; set; } = true;
        /// <summary>
        /// 
        /// </summary>
        public string auditBtnText { get; set; } = "通 过";
        /// <summary>
        /// 
        /// </summary>
        public bool hasRejectBtn { get; set; } = true;
        /// <summary>
        /// 
        /// </summary>
        public string rejectBtnText { get; set; } = "拒 绝";
        /// <summary>
        /// 
        /// </summary>
        public bool hasRevokeBtn { get; set; } = true;
        /// <summary>
        /// 
        /// </summary>
        public string revokeBtnText { get; set; } = "撤 回";
        /// <summary>
        /// 
        /// </summary>
        public bool hasTransferBtn { get; set; } = true;
        /// <summary>
        /// 
        /// </summary>
        public string transferBtnText { get; set; } = "转 办";
        /// <summary>
        /// 是否有签名
        /// </summary>
        public bool hasSign { get; set; }
        /// <summary>
        /// 超时设置
        /// </summary>
        public TimeOutConfig timeoutConfig { get; set; }
        /// <summary>
        /// 通知设置 1-站内信 2-邮箱 3-短信 4-钉钉 5-企业微信
        /// </summary>
        public List<string> messageType { get; set; }
        /// <summary>
        /// 是否可以加签
        /// </summary>
        public bool hasFreeApprover { get; set; }
        /// <summary>
        /// 是否打印
        /// </summary>
        public bool hasPrintBtn { get; set; } = false;
        /// <summary>
        /// 打印
        /// </summary>
        public string printBtnText { get; set; } = "打 印";
        /// <summary>
        /// 打印id
        /// </summary>
        public string printId { get; set; }

    }

    /// <summary>
    /// 流程节点表单权限
    /// </summary>
    public class FormOperatesModels
    {
        /// <summary>
        /// 可读
        /// </summary>
        public bool read { get; set; }
        /// <summary>
        /// 控件名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 控件id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 可写
        /// </summary>
        public bool write { get; set; }
    }

    /// <summary>
    /// 审批超时配置
    /// </summary>
    public class TimeOutConfig
    {
        /// <summary>
        /// 开关
        /// </summary>
        public bool on { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int quantity { get; set; }
        /// <summary>
        /// 类型 day、 hour、 minute
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 同意1 拒绝2
        /// </summary>
        public int handler { get; set; }
    }
}
