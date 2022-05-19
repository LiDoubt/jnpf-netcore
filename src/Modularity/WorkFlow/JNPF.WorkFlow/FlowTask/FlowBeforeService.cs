using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.WorkFlow.Entitys;
using JNPF.WorkFlow.Entitys.Dto.FlowBefore;
using JNPF.WorkFlow.Entitys.Model;
using JNPF.WorkFlow.Interfaces.FlowEngine;
using JNPF.WorkFlow.Interfaces.FlowTask;
using JNPF.WorkFlow.Interfaces.FlowTask.Repository;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace JNPF.WorkFlow.Core.Service.FlowTask
{
    /// <summary>
    /// 流程审批
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "WorkflowEngine", Name = "FlowBefore", Order = 303)]
    [Route("api/workflow/Engine/[controller]")]
    public class FlowBeforeService : IDynamicApiController, ITransient
    {
        private readonly IFlowTaskRepository _flowTaskRepository;
        private readonly IFlowTaskService _flowTaskService;
        private readonly IFlowEngineService _flowEngineService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flowTaskRepository"></param>
        /// <param name="flowTaskService"></param>
        /// <param name="flowEngineService"></param>
        public FlowBeforeService(IFlowTaskRepository flowTaskRepository, IFlowTaskService flowTaskService, IFlowEngineService flowEngineService)
        {
            _flowTaskRepository = flowTaskRepository;
            _flowTaskService = flowTaskService;
            _flowEngineService = flowEngineService;
        }

        #region Get
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <param name="category">分类</param>
        /// <returns></returns>
        [HttpGet("List/{category}")]
        public async Task<dynamic> GetList([FromQuery] FlowBeforeListQuery input, string category)
        {
            try
            {
                switch (category)
                {
                    case "1":
                        return await _flowTaskRepository.GetWaitList(input);
                    case "2":
                        return await _flowTaskRepository.GetTrialList(input);
                    case "3":
                        return await _flowTaskRepository.GetCirculateList(input);
                    default:
                        break;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="taskNodeId">主键值</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id, [FromQuery] string taskNodeId)
        {
            try
            {
                var output = await _flowTaskService.GetFlowBeforeInfo(id, taskNodeId);
                return output;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        #region POST
        /// <summary>
        /// 审核同意
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="flowHandleModel">流程经办</param>
        /// <returns></returns>
        [HttpPost("Audit/{id}")]
        [SqlSugarUnitOfWork]
        public async Task Audit(string id, [FromBody] FlowHandleModel flowHandleModel)
        {
            var flowTaskOperatorEntity = await _flowTaskRepository.GetTaskOperatorInfo(id);
            var flowTaskEntity = await _flowTaskRepository.GetTaskInfo(flowTaskOperatorEntity.TaskId);
            var flowEngine = await _flowEngineService.GetInfo(flowTaskEntity.FlowId);
            if (flowTaskOperatorEntity == null)
                throw JNPFException.Oh(ErrorCode.COM1005);
            if (flowTaskOperatorEntity.Completion != 0)
                throw JNPFException.Oh(ErrorCode.WF0006);
            await _flowTaskService.Audit(flowTaskEntity, flowTaskOperatorEntity, flowHandleModel, (int)flowEngine.FormType);
            await _flowTaskService.ApproveBefore(flowEngine, flowTaskEntity, flowHandleModel);
        }

        /// <summary>
        /// 审核拒绝
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="flowHandleModel">流程经办</param>
        /// <returns></returns>
        [HttpPost("Reject/{id}")]
        public async Task Reject(string id, [FromBody] FlowHandleModel flowHandleModel)
        {
            var flowTaskOperatorEntity = await _flowTaskRepository.GetTaskOperatorInfo(id);
            var flowTaskEntity = await _flowTaskRepository.GetTaskInfo(flowTaskOperatorEntity.TaskId);
            var flowEngine = await _flowEngineService.GetInfo(flowTaskEntity.FlowId);
            if (flowTaskOperatorEntity == null)
                throw JNPFException.Oh(ErrorCode.COM1005);
            if (flowTaskOperatorEntity.Completion != 0)
                throw JNPFException.Oh(ErrorCode.WF0006);
            if (await _flowTaskService.IsSubFlowUpNode(flowTaskOperatorEntity))
                throw JNPFException.Oh(ErrorCode.WF0019);
            flowTaskEntity = await _flowTaskRepository.GetTaskInfo(flowTaskOperatorEntity.TaskId);
            await _flowTaskService.Reject(flowTaskEntity, flowTaskOperatorEntity, flowHandleModel, (int)flowEngine.FormType);
        }
        /// <summary>
        /// 撤回审核
        /// 注意：在撤销流程时要保证你的下一节点没有处理这条记录；如已处理则无法撤销流程。
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="flowHandleModel">流程经办</param>
        /// <returns></returns>
        [HttpPost("Recall/{id}")]
        public async Task Recall(string id, [FromBody] FlowHandleModel flowHandleModel)
        {
            if (_flowTaskRepository.AnySubFlowTask(id))
                throw JNPFException.Oh(ErrorCode.WF0018);
            await _flowTaskService.Recall(id, flowHandleModel);
        }

        /// <summary>
        /// 终止审核
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="flowHandleModel">流程经办</param>
        /// <returns></returns>
        [HttpPost("Cancel/{id}")]
        public async Task Cancel(string id, [FromBody] FlowHandleModel flowHandleModel)
        {
            if (_flowTaskRepository.AnySubFlowTask(id))
                throw JNPFException.Oh(ErrorCode.WF0017);
            var flowTaskEntity = await _flowTaskRepository.GetTaskInfo(id);
            if (!flowTaskEntity.ParentId.Equals("0") && flowTaskEntity.ParentId.IsNotEmptyOrNull())
                throw JNPFException.Oh(ErrorCode.WF0015);
            if (flowTaskEntity.FlowType == 1)
                throw JNPFException.Oh(ErrorCode.WF0016);
            await _flowTaskService.Cancel(flowTaskEntity, flowHandleModel);
        }

        /// <summary>
        /// 转办
        /// </summary>
        /// <param name="id">流程经办主键id</param>
        /// <param name="flowHandleModel"></param>
        /// <returns></returns>
        [HttpPost("Transfer/{id}")]
        public async Task Transfer(string id, [FromBody] FlowHandleModel flowHandleModel)
        {
            await _flowTaskService.Transfer(id, flowHandleModel);
        }

        /// <summary>
        /// 指派
        /// </summary>
        /// <param name="id">流程经办主键id</param>
        /// <param name="flowHandleModel"></param>
        /// <returns></returns>
        [HttpPost("Assign/{id}")]
        public async Task Assigned(string id, [FromBody] FlowHandleModel flowHandleModel)
        {
            var nodeEntity = (await _flowTaskRepository.GetTaskNodeList(id)).Find(x => x.State.Equals("0") && x.NodeType.Equals("subFlow"));
            if (nodeEntity.IsNotEmptyOrNull())
                throw JNPFException.Oh(ErrorCode.WF0014);
            await _flowTaskService.Assigned(id, flowHandleModel);
        }
        #endregion
    }
}
