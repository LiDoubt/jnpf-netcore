using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.JsonSerialization;
using JNPF.Message.Interfaces.Message;
using JNPF.WorkFlow.Entitys.Dto.FlowLaunch;
using JNPF.WorkFlow.Entitys.Enum;
using JNPF.WorkFlow.Interfaces.FlowTask;
using JNPF.WorkFlow.Interfaces.FlowTask.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JNPF.WorkFlow.Core.Service.FlowTask
{
    /// <summary>
    /// 流程发起
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "WorkflowEngine", Name = "FlowLaunch", Order = 305)]
    [Route("api/workflow/Engine/[controller]")]
    public class FlowLaunchService : IDynamicApiController, ITransient
    {
        private readonly IFlowTaskRepository _flowTaskRepository;
        private readonly IFlowTaskService _flowTaskService;
        private readonly IMessageService _messageService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flowTaskRepository"></param>
        public FlowLaunchService(IFlowTaskRepository flowTaskRepository, IFlowTaskService flowTaskService, IMessageService messageService)
        {
            _flowTaskRepository = flowTaskRepository;
            _flowTaskService = flowTaskService;
            _messageService = messageService;
        }

        #region GET
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] FlowLaunchListQuery input)
        {
            return await _flowTaskRepository.GetLaunchList(input);
        }
        #endregion

        #region POST
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _flowTaskRepository.GetTaskInfo(id);
            if (entity == null)
                throw JNPFException.Oh(ErrorCode.COM1005);
            if (!entity.ParentId.Equals("0") && entity.ParentId.IsNotEmptyOrNull())
                throw JNPFException.Oh(ErrorCode.WF0003);
            if (entity.FlowType == 1)
                throw JNPFException.Oh(ErrorCode.WF0012);
            var isOk = await _flowTaskRepository.DeleteTask(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 撤回
        /// 注意：在撤回流程时要保证你的下一节点没有处理这条记录；如已处理则无法撤销流程。
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">流程经办</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/Withdraw")]
        public async Task Revoke(string id, [FromBody] FlowLaunchActionWithdrawInput input)
        {
            var flowTaskEntity = await _flowTaskRepository.GetTaskInfo(id);
            var flowTaskNodeEntityList = await _flowTaskRepository.GetTaskNodeList(flowTaskEntity.Id);
            var flowTaskNodeEntity = flowTaskNodeEntityList.Find(m => m.SortCode == 2);
            await _flowTaskService.Revoke(flowTaskEntity, input.handleOpinion);
        }

        /// <summary>
        /// 催办
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("Press/{id}")]
        public async Task Press(string id)
        {
            var flowTaskEntity = await _flowTaskRepository.GetTaskInfo(id);
            var flowTaskOperatorEntityList = (await _flowTaskRepository.GetTaskOperatorList(id)).FindAll(x => x.Completion == 0 && x.State == "0");
            var processId = flowTaskOperatorEntityList.Select(x => x.HandleId).ToList();
            if (processId.Count == 0)
                throw JNPFException.Oh(ErrorCode.WF0009);
            Dictionary<string, object> message = new Dictionary<string, object>();
            message["type"] = FlowMessageEnum.wait;
            message["id"] = id;
            //审核提醒
            await _messageService.SentMessage(processId, flowTaskEntity.FullName + "【催办】", message.Serialize());
        }

        #endregion
    }
}
