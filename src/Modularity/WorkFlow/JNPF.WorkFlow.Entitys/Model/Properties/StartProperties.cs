using JNPF.Dependency;
using System.Collections.Generic;

namespace JNPF.WorkFlow.Entitys.Model.Properties
{
    [SuppressSniffer]
    public class StartProperties
    {
        /// <summary>
        /// 发起节点标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 发起事件
        /// </summary>
        public bool hasInitFunc { get; set; }
        /// <summary>
        /// 发起事件接口url
        /// </summary>
        public string initInterfaceUrl { get; set; }
        /// <summary>
        /// 发起事件接口请求类型
        /// </summary>
        public string initInterfaceType { get; set; }
        /// <summary>
        /// 流程结束事件
        /// </summary>
        public bool hasEndFunc { get; set; }
        /// <summary>
        /// 结束事件接口url
        /// </summary>
        public string endInterfaceUrl { get; set; }
        /// <summary>
        /// 流程撤回事件
        /// </summary>
        public bool hasFlowRecallFunc { get; set; }
        /// <summary>
        /// 撤回事件接口url
        /// </summary>
        public string flowRecallInterfaceUrl { get; set; }
        /// <summary>
        /// 结束事件接口请求类型
        /// </summary>
        public string endInterfaceType { get; set; }
        /// <summary>
        /// 指定发起人（为空则是所有人）
        /// </summary>
        public List<string> initiator { get; set; }
        /// <summary>
        /// 指定发起岗位（为空则是所有人）
        /// </summary>
        public List<string> initiatePos { get; set; }
        /// <summary>
        /// 指定发起角色
        /// </summary>
        public List<string> initiateRole { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<FormOperatesModels> formOperates { get; set; }
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
        public string submitBtnText { get; set; } = "提交审核";
        /// <summary>
        /// 
        /// </summary>
        public bool hasSubimtBtn { get; set; } = true;
        /// <summary>
        /// 
        /// </summary>
        public string saveBtnText { get; set; } = "保存草稿";
        /// <summary>
        /// 
        /// </summary>
        public bool hasSaveBtn { get; set; } = true;
        /// <summary>
        /// 
        /// </summary>
        public string pressBtnText { get; set; } = "催 办";
        /// <summary>
        /// 
        /// </summary>
        public bool hasPressBtn { get; set; } = true;
        /// <summary>
        /// 
        /// </summary>
        public string printBtnText { get; set; } = "打 印";
        /// <summary>
        /// 
        /// </summary>
        public bool hasPrintBtn { get; set; } = true;
        /// <summary>
        /// 打印id
        /// </summary>
        public string printId { get; set; }
    }
}
