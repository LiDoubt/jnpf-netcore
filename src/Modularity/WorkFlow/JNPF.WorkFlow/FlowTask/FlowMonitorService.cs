using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.WorkFlow.Entitys.Dto.FlowMonitor;
using JNPF.WorkFlow.Interfaces.FlowTask.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JNPF.WorkFlow.Core.Service.FlowTask
{
    /// <summary>
    /// 流程监控
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "WorkflowEngine", Name = "FlowMonitor", Order = 304)]
    [Route("api/workflow/Engine/[controller]")]
    public class FlowMonitorService : IDynamicApiController, ITransient
    {
        private readonly IFlowTaskRepository _flowTaskRepository;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flowTaskRepository"></param>
        public FlowMonitorService(IFlowTaskRepository flowTaskRepository)
        {
            _flowTaskRepository = flowTaskRepository;
        }

        #region GET
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] FlowMonitorListQuery input)
        {
            return await _flowTaskRepository.GetMonitorList(input);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task Delete([FromBody] FlowMonitorDeleteInput input)
        {
            var ids = new List<string>(input.ids.Split(","));
            var entityList = ids.Select(x => _flowTaskRepository.GetTaskInfo(x).Result).ToList();
            foreach (var item in entityList)
            {
                if (item == null)
                    throw JNPFException.Oh(ErrorCode.COM1005);
                if (!item.ParentId.Equals("0") && item.ParentId.IsNotEmptyOrNull())
                    throw JNPFException.Oh(ErrorCode.WF0003);
                if (item.FlowType == 1)
                    throw JNPFException.Oh(ErrorCode.WF0012);
                await _flowTaskRepository.DeleteTask(item);
            }
        }
        #endregion
    }
}
