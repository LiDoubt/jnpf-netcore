using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.JsonSerialization;
using JNPF.System.Interfaces.System;
using JNPF.WorkFlow.Entitys;
using JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.BatchPack;
using JNPF.WorkFlow.Interfaces.FlowTask;
using JNPF.WorkFlow.Interfaces.WorkFlowForm;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace JNPF.WorkFlow.WorkFlowForm
{
    /// <summary>
    /// 批包装指令
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "WorkflowForm", Name = "BatchPack", Order = 505)]
    [Route("api/workflow/Form/[controller]")]
    public class BatchPackService : IBatchPackService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<BatchPackEntity> _sqlSugarRepository;
        private readonly IBillRullService _billRuleService;
        private readonly IFlowTaskService _flowTaskService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlSugarRepository"></param>
        /// <param name="billRuleService"></param>
        /// <param name="flowTaskService"></param>
        public BatchPackService(ISqlSugarRepository<BatchPackEntity> sqlSugarRepository, IBillRullService billRuleService, IFlowTaskService flowTaskService)
        {
            _sqlSugarRepository = sqlSugarRepository;
            _billRuleService = billRuleService;
            _flowTaskService = flowTaskService;
        }

        #region GET
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var data = (await _sqlSugarRepository.FirstOrDefaultAsync(x => x.Id == id)).Adapt<BatchPackInfoOutput>();
            return data;
        }
        #endregion

        #region POST
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="input">表单信息</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Save([FromBody] BatchPackCrInput input)
        {
            var entity = input.Adapt<BatchPackEntity>();
            if (input.status == 1)
            {
                await Save(entity.Id, entity);
            }
            else
            {
                await Submit(entity.Id, entity);
            }

        }
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="id">表单信息</param>
        /// <param name="input">表单信息</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Submit(string id, [FromBody] BatchPackUpInput input)
        {
            input.id = id;
            var entity = input.Adapt<BatchPackEntity>();
            if (input.status == 1)
            {
                await Save(entity.Id, entity);
            }
            else
            {
                await Submit(entity.Id, entity);
            }

        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private async Task Save(string id, BatchPackEntity entity, int type = 0)
        {
            try
            {
                DbScoped.SugarScope.BeginTran();

                #region 表单信息
                await HandleForm(id, entity);
                #endregion

                #region 流程信息
                await _flowTaskService.Save(id, entity.FlowId, entity.Id, entity.FlowTitle, entity.FlowUrgent, entity.BillNo, null, 1, type, true);
                #endregion

                DbScoped.SugarScope.CommitTran();

            }
            catch (Exception ex)
            {
                DbScoped.Sugar.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="entity">实体对象</param>
        private async Task Submit(string id, BatchPackEntity entity)
        {
            try
            {
                DbScoped.SugarScope.BeginTran();

                #region 表单信息
                await HandleForm(id, entity);
                #endregion

                #region 流程信息
                await _flowTaskService.Submit(id, entity.FlowId, entity.Id, entity.FlowTitle, entity.FlowUrgent, entity.BillNo, entity.Adapt<BatchPackUpInput>(), 0, 0, true);
                #endregion

                DbScoped.SugarScope.CommitTran();

            }
            catch (Exception ex)
            {
                DbScoped.Sugar.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 表单操作
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private async Task HandleForm(string id, BatchPackEntity entity)
        {
            if (string.IsNullOrEmpty(id))
            {
                entity.Id = YitIdHelper.NextId().ToString();
                await _sqlSugarRepository.InsertAsync(entity);
                _billRuleService.UseBillNumber("WF_BatchPackNo");
            }
            else
            {
                entity.Id = id;
                await _sqlSugarRepository.UpdateAsync(entity);
            }
        }
        #endregion

        #region PublicMethod
        /// <summary>
        /// 工作流表单操作
        /// </summary>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <param name="type">0：事前审批，1：创建子流程</param>
        /// <returns></returns>
        [NonAction]
        public async Task Save(string id, object obj, int type)
        {
            try
            {
                var input = obj.Serialize().Deserialize<BatchPackUpInput>();
                var entity = input.Adapt<BatchPackEntity>();
                if (type == 0)
                {
                    await this.HandleForm(id, entity);
                }
                else
                {
                    entity.Id = id;
                    await _sqlSugarRepository.InsertAsync(entity);
                }

            }
            catch (Exception e)
            {

                throw;
            }
        }
        #endregion
    }
}
