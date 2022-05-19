using JNPF.WorkFlow.Entitys.Dto.FlowBefore;
using JNPF.WorkFlow.Entitys.Dto.FlowLaunch;
using JNPF.WorkFlow.Entitys.Dto.FlowMonitor;
using JNPF.WorkFlow.Entitys;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.WorkFlow.Interfaces.FlowTask.Repository
{
    /// <summary>
    /// 流程任务
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IFlowTaskRepository
    {
        /// <summary>
        /// 列表（流程监控）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<dynamic> GetMonitorList(FlowMonitorListQuery input);

        /// <summary>
        /// 列表（我发起的）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<dynamic> GetLaunchList(FlowLaunchListQuery input);

        /// <summary>
        /// 列表（待我审批）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<dynamic> GetWaitList(FlowBeforeListQuery input);

        /// <summary>
        /// 列表（待我审批）
        /// </summary>
        /// <returns></returns>
        Task<List<FlowTaskEntity>> GetWaitList();

        /// <summary>
        /// 列表（我已审批）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<dynamic> GetTrialList(FlowBeforeListQuery input);

        /// <summary>
        /// 列表（我已审批）
        /// </summary>
        /// <returns></returns>
        Task<List<FlowTaskEntity>> GetTrialList();

        /// <summary>
        /// 列表（抄送我的）
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetCirculateList(FlowBeforeListQuery input);

        /// <summary>
        /// 任务列表
        /// </summary>
        /// <returns></returns>
        Task<List<FlowTaskEntity>> GetTaskList();

        /// <summary>
        /// 任务列表
        /// </summary>
        /// <param name="flowId">引擎id</param>
        /// <returns></returns>
        Task<List<FlowTaskEntity>> GetTaskList(string flowId);

        /// <summary>
        /// 任务信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<FlowTaskEntity> GetTaskInfo(string id);

        /// <summary>
        /// 任务信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        FlowTaskEntity GetTaskInfoNoAsync(string id);

        /// <summary>
        /// 任务删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> DeleteTask(FlowTaskEntity entity);

        /// <summary>
        /// 任务删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int DeleteTaskNoAsync(FlowTaskEntity entity);

        /// <summary>
        /// 任务创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<FlowTaskEntity> CreateTask(FlowTaskEntity entity);

        /// <summary>
        /// 任务更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> UpdateTask(FlowTaskEntity entity);

        /// <summary>
        /// 节点列表
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<List<FlowTaskNodeEntity>> GetTaskNodeList(string taskId);

        /// <summary>
        /// 节点信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<FlowTaskNodeEntity> GetTaskNodeInfo(string id);

        /// <summary>
        /// 节点删除
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<int> DeleteTaskNode(string taskId);

        /// <summary>
        /// 节点创建
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        Task<int> CreateTaskNode(List<FlowTaskNodeEntity> entitys);

        /// <summary>
        /// 节点更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> UpdateTaskNode(FlowTaskNodeEntity entity);

        /// <summary>
        /// 经办列表
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<List<FlowTaskOperatorEntity>> GetTaskOperatorList(string taskId);

        /// <summary>
        /// 经办列表
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskNodeId"></param>
        /// <returns></returns>
        Task<List<FlowTaskOperatorEntity>> GetTaskOperatorList(string taskId,string taskNodeId);

        /// <summary>
        /// 经办信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<FlowTaskOperatorEntity> GetTaskOperatorInfo(string id);
        
        /// <summary>
        /// 经办删除
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<int> DeleteTaskOperator(string taskId);

        /// <summary>
        /// 经办删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<int> DeleteTaskOperator(List<string> ids);

        /// <summary>
        /// 经办创建
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        Task<int> CreateTaskOperator(List<FlowTaskOperatorEntity> entitys);

        /// <summary>
        /// 经办创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> CreateTaskOperator(FlowTaskOperatorEntity entity);

        /// <summary>
        /// 经办更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> UpdateTaskOperator(FlowTaskOperatorEntity entity);

        /// <summary>
        /// 经办更新
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        Task<int> UpdateTaskOperator(List<FlowTaskOperatorEntity> entitys);

        /// <summary>
        /// 经办记录列表
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<List<FlowTaskOperatorRecordEntity>> GetTaskOperatorRecordList(string taskId);

        /// <summary>
        /// 经办信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<FlowTaskOperatorRecordEntity> GetTaskOperatorRecordInfo(string id);

        /// <summary>
        /// 经办创建
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        Task<int> CreateTaskOperatorRecord(List<FlowTaskOperatorRecordEntity> entitys);

        /// <summary>
        /// 经办创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> CreateTaskOperatorRecord(FlowTaskOperatorRecordEntity entity);

        /// <summary>
        /// 传阅删除
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<int> DeleteTaskCirculate(string taskId);

        /// <summary>
        /// 传阅创建
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        Task<int> CreateTaskCirculate(List<FlowTaskCirculateEntity> entitys);

        /// <summary>
        /// 打回流程删除所有相关数据
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task DeleteFlowTaskAllData(string taskId);

        /// <summary>
        /// 获取允许删除任务列表
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        List<string> GetAllowDeleteFlowTaskList(List<string> ids);

        /// <summary>
        /// 经办记录列表
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskNodeIds"></param>
        /// <returns></returns>
        List<FlowTaskOperatorRecordEntity> GetTaskOperatorRecordList(string taskId, string[] taskNodeIds);

        /// <summary>
        /// 经办记录作废
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task DeleteTaskOperatorRecord(List<string> ids);

        /// <summary>
        /// 经办记录详情
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskNodeId"></param>
        /// <param name="taskOperatorId"></param>
        /// <returns></returns>
        Task<FlowTaskOperatorRecordEntity> GetTaskOperatorRecordInfo(string taskId, string taskNodeId, string taskOperatorId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        Task<FlowTaskOperatorEntity> GetTaskOperatorInfoByParentId(string parentId);

        /// <summary>
        /// 判断是否有子流程
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool AnySubFlowTask(string id);
    }
}
