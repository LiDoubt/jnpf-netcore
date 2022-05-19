using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.JsonSerialization;
using JNPF.System.Interfaces.System;
using JNPF.WorkFlow.Entitys;
using JNPF.WorkFlow.Entitys.Dto.WorkFlowForm.OutboundOrder;
using JNPF.WorkFlow.Entitys.Model;
using JNPF.WorkFlow.Interfaces.FlowTask;
using JNPF.WorkFlow.Interfaces.WorkFlowForm;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace JNPF.WorkFlow.WorkFlowForm
{
    /// <summary>
    /// 出库单
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "WorkflowForm", Name = "OutboundOrder", Order = 521)]
    [Route("api/workflow/Form/[controller]")]
    public class OutboundOrderService : IOutboundOrderService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<OutboundOrderEntity> _sqlSugarRepository;
        private readonly ISqlSugarRepository<OutboundEntryEntity> _sqlItemSugarRepository;
        private readonly IBillRullService _billRuleService;
        private readonly IFlowTaskService _flowTaskService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlSugarRepository"></param>
        /// <param name="sqlItemSugarRepository"></param>
        /// <param name="billRuleService"></param>
        /// <param name="flowTaskService"></param>
        public OutboundOrderService(ISqlSugarRepository<OutboundOrderEntity> sqlSugarRepository, ISqlSugarRepository<OutboundEntryEntity> sqlItemSugarRepository, IBillRullService billRuleService, IFlowTaskService flowTaskService)
        {
            _sqlSugarRepository = sqlSugarRepository;
            _sqlItemSugarRepository = sqlItemSugarRepository;
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
            var data = (await _sqlSugarRepository.FirstOrDefaultAsync(x => x.Id == id)).Adapt<OutboundOrderInfoOutput>();
            data.entryList = (await _sqlItemSugarRepository.Entities.Where(x => x.OutboundId == id).ToListAsync()).Adapt<List<EntryListItem>>();
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
        public async Task Save([FromBody] OutboundOrderCrInput input)
        {
            var entity = input.Adapt<OutboundOrderEntity>();
            var itemList = input.entryList.Adapt<List<OutboundEntryEntity>>();
            if (input.status == 1)
            {
                await Save(entity.Id, entity, itemList);
            }
            else
            {
                await Submit(entity.Id, entity, itemList);
            }

        }
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="id">表单信息</param>
        /// <param name="input">表单信息</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Submit(string id, [FromBody] OutboundOrderUpInput input)
        {
            input.id = id;
            var entity = input.Adapt<OutboundOrderEntity>();
            var itemList = input.entryList.Adapt<List<OutboundEntryEntity>>();
            if (input.status == 1)
            {
                await Save(entity.Id, entity, itemList);
            }
            else
            {
                await Submit(entity.Id, entity, itemList);
            }

        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="itemList"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private async Task Save(string id, OutboundOrderEntity entity, List<OutboundEntryEntity> itemList, int type = 0)
        {
            try
            {
                DbScoped.SugarScope.BeginTran();
                #region 表单信息
                await HandleForm(id, entity, itemList);
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
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="itemList"></param>
        /// <returns></returns>
        private async Task Submit(string id, OutboundOrderEntity entity, List<OutboundEntryEntity> itemList)
        {
            try
            {
                DbScoped.SugarScope.BeginTran();
                #region 表单信息
                await HandleForm(id, entity, itemList);
                #endregion

                #region 流程信息
                await _flowTaskService.Submit(id, entity.FlowId, entity.Id, entity.FlowTitle, entity.FlowUrgent, entity.BillNo, entity.Adapt<OutboundOrderUpInput>(), 0, 0, true);
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
        /// <param name="itemList"></param>
        /// <returns></returns>
        private async Task HandleForm(string id, OutboundOrderEntity entity, List<OutboundEntryEntity> itemList)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    entity.Id = YitIdHelper.NextId().ToString();
                    foreach (var item in itemList)
                    {
                        item.Id = YitIdHelper.NextId().ToString();
                        item.OutboundId = entity.Id;
                        item.SortCode = itemList.IndexOf(item);
                    }
                    await _sqlItemSugarRepository.InsertAsync(itemList);
                    await _sqlSugarRepository.InsertAsync(entity);
                    _billRuleService.UseBillNumber("WF_OutboundOrderNo");
                }
                else
                {
                    entity.Id = id;
                    foreach (var item in itemList)
                    {
                        item.Id = YitIdHelper.NextId().ToString();
                        item.OutboundId = entity.Id;
                        item.SortCode = itemList.IndexOf(item);
                    }
                    await _sqlItemSugarRepository.DeleteAsync(x => x.OutboundId == id);
                    await _sqlItemSugarRepository.InsertAsync(itemList);
                    await _sqlSugarRepository.UpdateAsync(entity);
                }
            }
            catch (Exception ex)
            {

                throw;
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
                var input = obj.Serialize().Deserialize<OutboundOrderUpInput>();
                var entity = input.Adapt<OutboundOrderEntity>();
                var entityList = input.entryList.Adapt<List<OutboundEntryEntity>>();
                if (type == 0)
                {
                    await this.HandleForm(id, entity, entityList);
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
