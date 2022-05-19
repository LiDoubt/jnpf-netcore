using JNPF.WorkFlow.Entitys;
using JNPF.WorkFlow.Entitys.Dto.FlowBefore;
using JNPF.WorkFlow.Entitys.Model;
using System.Threading.Tasks;

namespace JNPF.WorkFlow.Interfaces.FlowTask
{
    /// <summary>
    /// 流程任务
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IFlowTaskService
    {
        /// <summary>
        /// 任务详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="taskNodeId"></param>
        /// <returns></returns>
        Task<FlowBeforeInfoOutput> GetFlowBeforeInfo(string id, string taskNodeId);

        /// <summary>
        /// 审批事前操作
        /// </summary>
        /// <param name="flowEngineEntity"></param>
        /// <param name="flowTaskEntity"></param>
        /// <param name="flowHandleModel"></param>
        /// <returns></returns>
        Task ApproveBefore(FlowEngineEntity flowEngineEntity, FlowTaskEntity flowTaskEntity, FlowHandleModel flowHandleModel);

        /// <summary>
        /// 审批(同意)
        /// </summary>
        /// <param name="flowTaskEntity"></param>
        /// <param name="flowTaskOperatorEntity"></param>
        /// <param name="flowHandleModel"></param>
        /// <param name="formType">表单类型</param>
        /// <returns></returns>
        Task Audit(FlowTaskEntity flowTaskEntity, FlowTaskOperatorEntity flowTaskOperatorEntity, FlowHandleModel flowHandleModel, int formType);

        /// <summary>
        /// 审批(拒绝)
        /// </summary>
        /// <param name="flowTaskEntity"></param>
        /// <param name="flowTaskOperatorEntity"></param>
        /// <param name="flowHandleModel"></param>
        /// <param name="formType"></param>
        /// <returns></returns>
        Task Reject(FlowTaskEntity flowTaskEntity, FlowTaskOperatorEntity flowTaskOperatorEntity, FlowHandleModel flowHandleModel, int formType);

        /// <summary>
        /// 审批(撤回)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flowHandleModel"></param>
        /// <returns></returns>
        Task Recall(string id, FlowHandleModel flowHandleModel);

        /// <summary>
        /// 流程撤回
        /// </summary>
        /// <param name="flowTaskEntity"></param>
        /// <param name="flowHandleModel"></param>
        Task Revoke(FlowTaskEntity flowTaskEntity, string flowHandleModel);

        /// <summary>
        /// 终止
        /// </summary>
        /// <param name="flowTaskEntity"></param>
        /// <param name="flowHandleModel"></param>
        /// <returns></returns>
        Task Cancel(FlowTaskEntity flowTaskEntity, FlowHandleModel flowHandleModel);

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="id">任务主键id（通过空值判断是修改还是新增）</param>
        /// <param name="flowId">引擎id</param>
        /// <param name="processId">关联id</param>
        /// <param name="flowTitle">任务名</param>
        /// <param name="flowUrgent">紧急程度（自定义默认为1）</param>
        /// <param name="billNo">单据规则</param>
        /// <param name="formData">表单数据</param>
        /// <param name="status">状态 1:保存，0提交</param>
        /// <param name="approvaUpType">审批修改权限1：可写，0：可读</param>
        /// <param name="isSysTable">true：系统表单，false：自定义表单</param>
        /// <param name="parentId">任务父id</param>
        /// <param name="crUser">子流程发起人</param>
        /// <param name="isDev">是否功能设计</param>
        /// <returns></returns>
        Task<FlowTaskEntity> Save(string id, string flowId, string processId, string flowTitle, int? flowUrgent, string billNo, object formData, int status, int? approvaUpType = 0, bool isSysTable = true, string parentId = "0", string crUser = null, bool isDev = false);

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="id">主键id（通过空值判断是修改还是新增）</param>
        /// <param name="flowId">引擎id</param>
        /// <param name="processId">关联id</param>
        /// <param name="flowTitle">任务名</param>
        /// <param name="flowUrgent">紧急程度（自定义默认为1）</param>
        /// <param name="billNo">单据规则</param>
        /// <param name="formData">表单数据</param>
        /// <param name="status">状态 1:保存，0提交</param>
        /// <param name="approvaUpType">审批修改权限1：可写，0：可读</param>
        /// <param name="isSysTable">true：系统表单，false：自定义表单</param>
        /// <param name="isDev">是否功能设计</param>
        /// <returns></returns>
        Task Submit(string id, string flowId, string processId, string flowTitle, int? flowUrgent, string billNo, object formData, int status, int? approvaUpType = 0, bool isSysTable = true, bool isDev = false);

        /// <summary>
        /// 指派
        /// </summary>
        /// <param name="id">任务id</param>
        /// <param name="flowHandleModel">参数</param>
        /// <returns></returns>
        Task Assigned(string id, FlowHandleModel flowHandleModel);

        /// <summary>
        /// 转办
        /// </summary>
        /// <param name="id">任务id</param>
        /// <param name="flowHandleModel">参数</param>
        /// <returns></returns>
        Task Transfer(string id, FlowHandleModel flowHandleModel);

        /// <summary>
        /// 判断驳回节点是否存在子流程
        /// </summary>
        /// <param name="flowTaskOperatorEntity"></param>
        /// <returns></returns>
        Task<bool> IsSubFlowUpNode(FlowTaskOperatorEntity flowTaskOperatorEntity);
    }
}
