using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Filter;
using JNPF.Common.Helper;
using JNPF.Common.Util;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.JsonSerialization;
using JNPF.System.Entitys.Permission;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.Common;
using JNPF.System.Interfaces.System;
using JNPF.VisualDev.Entitys.Model.VisualDevModelData;
using JNPF.WorkFlow.Entitys;
using JNPF.WorkFlow.Entitys.Dto.FlowEngine;
using JNPF.WorkFlow.Entitys.Model;
using JNPF.WorkFlow.Interfaces.FlowEngine;
using JNPF.WorkFlow.Interfaces.FlowTask.Repository;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JNPF.WorkFlow.Core.Service.FlowEngine
{
    /// <summary>
    /// 流程引擎
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "WorkflowEngine", Name = "FlowEngine", Order = 301)]
    [Route("api/workflow/Engine/[controller]")]
    public class FlowEngineService : IFlowEngineService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<FlowEngineEntity> _flowEngineRepository;
        private readonly ISqlSugarRepository<FlowEngineVisibleEntity> _flowEngineVisibleRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope db;// 核心对象：拥有完整的SqlSugar全部功能
        private readonly IFlowTaskRepository _flowTaskRepository;
        private readonly IDictionaryDataService _dictionaryDataService;
        private readonly IFileService _fileService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flowEngineRepository"></param>
        /// <param name="flowEngineVisibleRepository"></param>
        /// <param name="userManager"></param>
        /// <param name="flowTaskRepository"></param>
        /// <param name="dictionaryDataService"></param>
        /// <param name="fileService"></param>
        public FlowEngineService(ISqlSugarRepository<FlowEngineEntity> flowEngineRepository, ISqlSugarRepository<FlowEngineVisibleEntity> flowEngineVisibleRepository, IUserManager userManager, IFlowTaskRepository flowTaskRepository, IDictionaryDataService dictionaryDataService, IFileService fileService)
        {
            _flowEngineRepository = flowEngineRepository;
            _flowEngineVisibleRepository = flowEngineVisibleRepository;
            _userManager = userManager;
            db = flowEngineRepository.Context;
            _flowTaskRepository = flowTaskRepository;
            _dictionaryDataService = dictionaryDataService;
            _fileService = fileService;
        }

        #region GET
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] KeywordInput input)
        {
            var list1 =await GetOutList();
            var dicDataInfo =await _dictionaryDataService.GetInfo(list1.First().parentId);
            var dicDataList = (await _dictionaryDataService.GetList(dicDataInfo.DictionaryTypeId)).FindAll(x=>x.EnabledMark==1);
            var list2 = new List<FlowEngineListOutput>();
            foreach (var item in dicDataList)
            {
                var index = list1.FindAll(x => x.category == item.EnCode).Count;
                if (index>0)
                {
                    list2.Add(new FlowEngineListOutput()
                    {
                        fullName = item.FullName,
                        parentId = "0",
                        id = item.Id,
                        num = index
                    });
                }
            }
            var output = list1.Union(list2).ToList();
            if (!input.keyword.IsNullOrEmpty())
                output = output.TreeWhere(o => o.fullName.Contains(input.keyword) || (o.enCode.IsNotEmptyOrNull()&& o.enCode.Contains(input.keyword)), o => o.id, o => o.parentId);
            return new { list = output.ToTree() };
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("ListAll")]
        public async Task<dynamic> GetListAll()
        {
            var list1 = await GetFlowFormList();
            var dicDataInfo = await _dictionaryDataService.GetInfo(list1.First().parentId);
            var dicDataList = (await _dictionaryDataService.GetList(dicDataInfo.DictionaryTypeId)).FindAll(x => x.EnabledMark == 1);
            var list2 = new List<FlowEngineListOutput>();
            foreach (var item in dicDataList)
            {
                list2.Add(new FlowEngineListOutput()
                {
                    fullName = item.FullName,
                    parentId = "0",
                    id = item.Id,
                    num = list1.FindAll(x => x.category == item.EnCode).Count
                });
            }
            var output = list1.Union(list2).ToList().ToTree();
            return new { list = output };
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            var flowEntity = await GetInfo(id);
            var output = flowEntity.Adapt<FlowEngineInfoOutput>();
            return output;
        }

        /// <summary>
        /// 获取流程设计列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> ListSelect(int type)
        {
            var list1 = (await GetOutList(1)).FindAll(x => x.enabledMark == 1);
            if (type.IsEmpty())
            {
                list1 = list1.FindAll(x => x.formType == type);
            }
            var dicDataInfo = await _dictionaryDataService.GetInfo(list1.First().parentId);
            var dicDataList = (await _dictionaryDataService.GetList(dicDataInfo.DictionaryTypeId)).FindAll(x => x.EnabledMark == 1);
            var list2 = new List<FlowEngineListOutput>();
            foreach (var item in dicDataList)
            {
                var index = list1.FindAll(x => x.category == item.EnCode).Count;
                if (index>0)
                {
                    list2.Add(new FlowEngineListOutput()
                    {
                        fullName = item.FullName,
                        parentId = "0",
                        id = item.Id,
                        num = index
                    });
                }
            }
            var output = list1.Union(list2).ToList().ToTree();
            return new { list = output };
        }

        /// <summary>
        /// 表单主表属性
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/FormDataFields")]
        public async Task<dynamic> getFormDataField(string id)
        {
            var entity = await GetInfo(id);
            List<FormDataField> formDataFieldList = new List<FormDataField>();
            if (entity.FormType == 1)
            {
                var filedList = entity.FormTemplateJson.ToList<Field>();
                foreach (var item in filedList)
                {
                    formDataFieldList.Add(new FormDataField()
                    {
                        vmodel = item.filedId,
                        label = item.filedName
                    });
                }
            }
            else
            {
                FormDataModel formData = entity.FormTemplateJson.ToObject<FormDataModel>();
                List<FieldsModel> list = formData.fields;
                foreach (var item in list)
                {
                    if (!"table".Equals(item.__config__.jnpfKey))
                    {
                        formDataFieldList.Add(new FormDataField()
                        {
                            vmodel = item.__vModel__,
                            label = item.__config__.label
                        });
                    }
                }
            }
            return new { list = formDataFieldList };
        }

        /// <summary>
        /// 表单列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/FieldDataSelect")]
        public async Task<dynamic> getFormData(string id)
        {
            var flowTaskList = await _flowTaskRepository.GetTaskList(id);
            var output = flowTaskList.Select(x => new FlowEngineListSelectOutput()
            {
                id = x.Id,
                fullName = x.FullName + "/" + x.EnCode
            }).ToList();
            return flowTaskList;
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Actions/ExportData")]
        public async Task<dynamic> ActionsExport(string id)
        {
            var importModel = new FlowEngineImportModel();
            importModel.flowEngine = await _flowEngineRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
            importModel.visibleList = await _flowEngineVisibleRepository.Entities.Where(x => x.FlowId == id).ToListAsync();
            var jsonStr = importModel.Serialize();
            return _fileService.Export(jsonStr, importModel.flowEngine.FullName);
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
            var flowEngineEntity = await GetInfo(id);
            if (flowEngineEntity == null)
                throw JNPFException.Oh(ErrorCode.COM1005);
            var isOk = await Delete(flowEngineEntity);
            if(isOk<1)
                throw JNPFException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] FlowEngineCrInput input)
        {
            if (await _flowEngineRepository.AnyAsync(x=>x.EnCode==input.enCode&&x.DeleteMark==null)|| await _flowEngineRepository.AnyAsync(x => x.FullName== input.fullName && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.COM1004);
            var flowEngineEntity = input.Adapt<FlowEngineEntity>();
            var flowVisibleList = GetFlowEngineVisibleList(input.flowTemplateJson);
            var result=await Create(flowEngineEntity, flowVisibleList);
            _ = result ?? throw JNPFException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] FlowEngineUpInput input)
        {
            if (await _flowEngineRepository.AnyAsync(x =>x.Id!= id&&x.EnCode == input.enCode && x.DeleteMark == null) || await _flowEngineRepository.AnyAsync(x => x.Id != id && x.FullName==input.fullName&&x.DeleteMark==null))
                throw JNPFException.Oh(ErrorCode.COM1004);
            var flowEngineEntity = input.Adapt<FlowEngineEntity>();
            var flowVisibleList = GetFlowEngineVisibleList(input.flowTemplateJson);
            var isOk=await Update(id, flowEngineEntity, flowVisibleList);
            if(isOk<1)
                throw JNPFException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPost("{id}/Actions/Copy")]
        public async Task ActionsCopy(string id)
        {
            var entity = await GetInfo(id);
            entity.FullName = entity.FullName + Ext.GetTimeStamp;
            entity.EnCode = entity.EnCode + Ext.GetTimeStamp;
            var flowVisibleList = GetFlowEngineVisibleList(entity.FlowTemplateJson);
            var result= await Create(entity, flowVisibleList);
            _ = result ?? throw JNPFException.Oh(ErrorCode.WF0002);
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPost("Release/{id}")]
        public async Task Release(string id)
        {
            var entity = await GetInfo(id);
            if (entity == null)
                throw JNPFException.Oh(ErrorCode.COM1005);
            entity.EnabledMark = 1;
            var isOk = await Update(id, entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1003);
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPost("Stop/{id}")]
        public async Task Stop(string id)
        {
            var entity = await GetInfo(id);
            if (entity == null)
                throw JNPFException.Oh(ErrorCode.COM1005);
            entity.EnabledMark = 0;
            var isOk = await Update(id, entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1003);
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("Actions/ImportData")]
        public async Task ActionsImport(IFormFile file)
        {
            var josn = _fileService.Import(file);
            var model = josn.Deserialize<FlowEngineImportModel>();
            if (model == null)
                throw JNPFException.Oh(ErrorCode.D3006);
            await ImportData(model);
        }
        #endregion

        #region PublicMethod
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="visibleList"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<FlowEngineEntity> Create(FlowEngineEntity entity, List<FlowEngineVisibleEntity> visibleList)
        {
            try
            {
                db.BeginTran();
                entity.VisibleType = visibleList.Count == 0 ? 0 : 1;
                var result= await _flowEngineRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();
                if (result == null)
                    throw new Exception();
                foreach (var item in visibleList)
                {
                    item.FlowId = entity.Id;
                    item.SortCode = visibleList.IndexOf(item);
                }
                if(visibleList.Count>0)
                    await _flowEngineVisibleRepository.Context.Insertable(visibleList).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
                db.CommitTran();
                return result;
            }
            catch (Exception ex)
            {
                db.RollbackTran();
                return null;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(FlowEngineEntity entity)
        {
            try
            {
                db.BeginTran();
                await _flowEngineVisibleRepository.DeleteAsync(a => a.FlowId == entity.Id);
                var isOk=await _flowEngineRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
                db.CommitTran();
                return isOk;
            }
            catch (Exception ex)
            {
                db.RollbackTran();
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<FlowEngineEntity> GetInfo(string id)
        {
            return await _flowEngineRepository.FirstOrDefaultAsync(a => a.Id == id && a.DeleteMark == null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enCode"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<FlowEngineEntity> GetInfoByEnCode(string enCode)
        {
            return await _flowEngineRepository.FirstOrDefaultAsync(a => a.EnCode == enCode && a.EnabledMark == 1 && a.DeleteMark == null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<FlowEngineEntity>> GetList()
        {
            return await _flowEngineRepository.Entities.Where(a=>a.DeleteMark==null).OrderBy(a=>a.SortCode).OrderBy(o => o.CreatorTime).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<FlowEngineVisibleEntity>> GetVisibleFlowList(string userId)
        {
            return await db.Queryable<FlowEngineVisibleEntity, UserRelationEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.OperatorId == b.ObjectId)).Select((a, b) => new { Id = a.Id, FlowId = a.FlowId, OperatorType = a.OperatorType, OperatorId = a.OperatorId, SortCode = a.SortCode, CreatorTime = a.CreatorTime, CreatorUserId = a.CreatorUserId, UserId = b.UserId }).MergeTable().Where(a => a.OperatorId == _userManager.UserId||a.UserId== _userManager.UserId).Select<FlowEngineVisibleEntity>().ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="visibleList"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(string id, FlowEngineEntity entity, List<FlowEngineVisibleEntity> visibleList)
        {
            try
            {
                db.BeginTran();
                entity.VisibleType = visibleList.Count == 0 ? 0 : 1;
                await _flowEngineVisibleRepository.DeleteAsync(a => a.FlowId == entity.Id);
                foreach (var item in visibleList)
                {
                    item.FlowId = entity.Id;
                    item.SortCode = visibleList.IndexOf(item);
                }
                if(visibleList.Count>0)
                    await _flowEngineVisibleRepository.Context.Insertable(visibleList).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
                var isOk = await _flowEngineRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
                db.CommitTran();
                return isOk;
            }
            catch (Exception ex)
            {
                db.RollbackTran();
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(string id, FlowEngineEntity entity)
        {
           return await _flowEngineRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<FlowEngineListOutput>> GetFlowFormList()
        {
            var list = (await GetOutList()).FindAll(x => x.enabledMark == 1 && x.type == 0);
            if (_userManager.User.IsAdministrator == 0)
            {
                var data = new List<FlowEngineListOutput>();
                //部分看见
                var flowVisibleData = await GetVisibleFlowList(_userManager.UserId);
                //去重
                var ids = new List<string>();
                foreach (var item in flowVisibleData)
                {
                    FlowEngineListOutput flowEngineEntity = list.Find(m => m.id == item.FlowId);
                    if (flowEngineEntity != null && !ids.Contains(flowEngineEntity.id))
                    {
                        data.Add(flowEngineEntity);
                        ids.Add(flowEngineEntity.id);
                    }
                }
                ////全部看见
                foreach (FlowEngineListOutput flowEngineEntity in list.FindAll(m => m.visibleType == 0))
                {
                    data.Add(flowEngineEntity);
                }
                return data;
            }
            else
            {
                return list;
            }
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 解析流程可见参数
        /// </summary>
        /// <param name="josnStr"></param>
        /// <returns></returns>
        private List<FlowEngineVisibleEntity> GetFlowEngineVisibleList(string josnStr)
        {
            var output = new List<FlowEngineVisibleEntity>();
            var jobj = JSON.Deserialize<FlowTemplateJsonModel>(josnStr).properties;
            var initiator = jobj["initiator"] as JArray;
            var initiatePos = jobj["initiatePos"] as JArray;
            var initiateRole = jobj["initiateRole"] as JArray;
            if (initiator != null && initiator.Count != 0)
            {
                foreach (var item in initiator)
                {
                    var entity = new FlowEngineVisibleEntity();
                    entity.OperatorId = item.ToString();
                    entity.OperatorType = "user";
                    output.Add(entity);
                }
            }
            if (initiatePos != null && initiatePos.Count != 0)
            {
                foreach (var item in initiatePos)
                {
                    var entity = new FlowEngineVisibleEntity();
                    entity.OperatorId = item.ToString();
                    entity.OperatorType = "Position";
                    output.Add(entity);
                }
            }
            if (initiateRole != null && initiateRole.Count != 0)
            {
                foreach (var item in initiateRole)
                {
                    var entity = new FlowEngineVisibleEntity();
                    entity.OperatorId = item.ToString();
                    entity.OperatorType = "Role";
                    output.Add(entity);
                }
            }
            return output;
        }

        /// <summary>
        /// 流程列表(功能流程不显示)
        /// </summary>
        /// <param name="type">0:流程设计，1：下拉列表</param>
        /// <returns></returns>
        private async Task<List<FlowEngineListOutput>> GetOutList(int type=0)
        {
           return  await db.Queryable<FlowEngineEntity, UserEntity, UserEntity, DictionaryDataEntity>((a, b, c, d) => new JoinQueryInfos(JoinType.Left, b.Id == a.CreatorUserId, JoinType.Left, c.Id == a.LastModifyUserId, JoinType.Left, a.Category==d.EnCode)).Select((a, b, c, d) => new
            {
                category = a.Category,
                id = a.Id,
                description = a.Description,
                creatorTime = a.CreatorTime,
                creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account),
                enCode = a.EnCode,
                enabledMark = a.EnabledMark,
                flowTemplateJson = a.FlowTemplateJson,
                formData = a.FormTemplateJson,
                fullName = a.FullName,
                formType = a.FormType,
                icon = a.Icon,
                iconBackground = a.IconBackground,
                lastModifyTime = a.LastModifyTime,
                lastModifyUser = SqlFunc.MergeString(c.RealName, "/", c.Account),
                sortCode = a.SortCode,
                type = a.Type,
                visibleType = a.VisibleType,
                deleteMark = a.DeleteMark,
                parentId = d.Id,
                d.DictionaryTypeId
            }).MergeTable().Where(x => x.deleteMark == null&&x.DictionaryTypeId== "507f4f5df86b47588138f321e0b0dac7")
            .Where(x=>!(x.formType==2&&x.type==1)).WhereIF(type!=0,x=>x.type!=1).Select<FlowEngineListOutput>().OrderBy(x => x.sortCode).OrderBy(x=>x.creatorTime,OrderByType.Desc).ToListAsync();
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task ImportData(FlowEngineImportModel model)
        {
            try
            {
                db.BeginTran();
                var stor = db.Storageable(model.flowEngine).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await stor.AsInsertable.ExecuteCommandAsync(); //执行插入
                await stor.AsUpdateable.ExecuteCommandAsync(); //执行更新　
                var stor1 = db.Storageable(model.visibleList).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await stor1.AsInsertable.ExecuteCommandAsync(); //执行插入
                await stor1.AsUpdateable.ExecuteCommandAsync(); //执行更新
                db.CommitTran();
            }
            catch (Exception ex)
            {
                db.RollbackTran();
                throw JNPFException.Oh(ErrorCode.D3006);
            }
        }
        #endregion
    }
}
