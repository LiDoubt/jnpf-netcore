using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Helper;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.Expand.Thirdparty;
using JNPF.Expand.Thirdparty.DingDing;
using JNPF.Expand.Thirdparty.Email;
using JNPF.Expand.Thirdparty.Email.Model;
using JNPF.Expand.Thirdparty.Sms;
using JNPF.Expand.Thirdparty.Sms.Model;
using JNPF.FriendlyException;
using JNPF.JsonSerialization;
using JNPF.Message.Interfaces.Message;
using JNPF.RemoteRequest.Extensions;
using JNPF.System.Entitys.Dto.System.SysConfig;
using JNPF.System.Entitys.Entity.System;
using JNPF.System.Entitys.Permission;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.Permission;
using JNPF.System.Interfaces.System;
using JNPF.TaskScheduler;
using JNPF.UnifyResult;
using JNPF.VisualDev.Entitys;
using JNPF.VisualDev.Entitys.Dto.VisualDevModelData;
using JNPF.VisualDev.Entitys.Model.VisualDevModelData;
using JNPF.VisualDev.Interfaces;
using JNPF.VisualDev.Run.Interfaces;
using JNPF.WorkFlow.Entitys;
using JNPF.WorkFlow.Entitys.Dto.FlowBefore;
using JNPF.WorkFlow.Entitys.Dto.FlowTask;
using JNPF.WorkFlow.Entitys.Enum;
using JNPF.WorkFlow.Entitys.Model;
using JNPF.WorkFlow.Entitys.Model.Properties;
using JNPF.WorkFlow.Interfaces.FLowDelegate;
using JNPF.WorkFlow.Interfaces.FlowEngine;
using JNPF.WorkFlow.Interfaces.FlowTask;
using JNPF.WorkFlow.Interfaces.FlowTask.Repository;
using JNPF.WorkFlow.Interfaces.WorkFlowForm;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Yitter.IdGenerator;

namespace JNPF.WorkFlow.Core.Service.FlowTask
{
    /// <summary>
    /// 流程任务
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "WorkflowEngine", Name = "FlowTask", Order = 306)]
    [Route("api/workflow/Engine/[controller]")]
    public class FlowTaskService : IFlowTaskService, IDynamicApiController, ITransient
    {
        private readonly IFlowTaskRepository _flowTaskRepository;
        private readonly IDictionaryDataService _dictionaryDataService;
        private readonly IFlowDelegateService _flowDelegateService;
        private readonly IFlowEngineService _flowEngineService;
        private readonly IDataInterfaceService _dataInterfaceService;
        private readonly IUsersService _usersService;
        private readonly IOrganizeService _organizeService;
        private readonly IMessageService _messageService;
        private readonly IUserRelationService _userRelationService;
        private readonly IUserManager _userManager;
        private readonly IRunService _runService;
        private readonly IBillRullService _billRullService;
        private readonly IVisualDevService _visualDevServce;
        private readonly IDataBaseService _dataBaseService;
        private readonly IDbLinkService _dbLinkService;
        private readonly ISysConfigService _sysConfigService;
        private readonly ISynThirdInfoService _synThirdInfoService;
        private readonly ILogger<FlowTaskService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flowTaskRepository"></param>
        /// <param name="dictionaryDataService"></param>
        /// <param name="usersService"></param>
        /// <param name="flowEngineService"></param>
        /// <param name="organizeService"></param>
        /// <param name="dataInterfaceService"></param>
        /// <param name="flowDelegateService"></param>
        /// <param name="messageService"></param>
        /// <param name="userRelationService"></param>
        /// <param name="userManager"></param>
        /// <param name="runService"></param>
        /// <param name="billRullService"></param>
        /// <param name="visualDevServce"></param>
        /// <param name="dataBaseService"></param>
        /// <param name="dbLinkService"></param>
        /// <param name="sysConfigService"></param>
        /// <param name="synThirdInfoService"></param>
        /// <param name="logger"></param>
        public FlowTaskService(IFlowTaskRepository flowTaskRepository,
            IDictionaryDataService dictionaryDataService, IUsersService usersService,
            IFlowEngineService flowEngineService, IOrganizeService organizeService,
            IDataInterfaceService dataInterfaceService, IFlowDelegateService flowDelegateService,
            IMessageService messageService, IUserRelationService userRelationService,
            IUserManager userManager, IRunService runService, IBillRullService billRullService,
            IVisualDevService visualDevServce, IDataBaseService dataBaseService,
            IDbLinkService dbLinkService, ISysConfigService sysConfigService,
            ISynThirdInfoService synThirdInfoService, ILogger<FlowTaskService> logger)
        {
            _flowTaskRepository = flowTaskRepository;
            _dictionaryDataService = dictionaryDataService;
            _flowEngineService = flowEngineService;
            _usersService = usersService;
            _dataInterfaceService = dataInterfaceService;
            _organizeService = organizeService;
            _flowDelegateService = flowDelegateService;
            _messageService = messageService;
            _userRelationService = userRelationService;
            _userManager = userManager;
            _runService = runService;
            _billRullService = billRullService;
            _visualDevServce = visualDevServce;
            _dataBaseService = dataBaseService;
            _dbLinkService = dbLinkService;
            _sysConfigService = sysConfigService;
            _synThirdInfoService = synThirdInfoService;
            _logger = logger;
        }

        #region Get
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var flowEntity = await _flowTaskRepository.GetTaskInfo(id);
            var output = await GetFlowDynamicDataManage(flowEntity);
            return output;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="enCode"></param>
        ///// <returns></returns>
        //[HttpPost("{enCode}")]
        //[AllowAnonymous]
        //public async Task GetSysTableFromService(string enCode = "applyBanquet")
        //{
        //    var flowMethod = App.EffectiveTypes.Where(u => u.IsClass && !u.IsInterface && !u.IsAbstract && typeof(IApprovalProcesWorker).IsAssignableFrom(u))
        //        .SelectMany(u => u.GetMethods(BindingFlags.Public | BindingFlags.Instance)
        //        .Where(m => m.IsDefined(typeof(ApprovalProcesAttribute), false) &&
        //        m.GetParameters().Length == 1 &&
        //        m.GetParameters()[0].ParameterType == typeof(ApprovalProces) &&
        //        m.ReturnType == typeof(Task))// && m.CustomAttributes.Last().ConstructorArguments.FirstOrDefault().Value.Equals(enCode)
        //        .Select(m =>
        //        {
        //            return m;
        //        })).FirstOrDefault();
        //    //var flowInstance = Activator.CreateInstance(taskMethod.DeclaringType);
        //    // var type=typeof(flowMethod).GetMethod("Save");
        //    //var type = flowMethod.GetType().GetMethod("Save").Invoke();
        //    //var typeInstance = Activator.CreateInstance(flowMethod);
        //}

        #endregion

        #region Post
        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] FlowTaskCrInput input)
        {
            try
            {
                if (input.status == 1)
                {
                    await Save(null, input.flowId, null, null, 1, null, input.data.ToObject(), 1, 0, false);
                }
                else
                {
                    await Submit(null, input.flowId, null, null, 1, null, input.data.ToObject(), 0, 0, false);
                }
            }
            catch (Exception ex)
            {
                throw JNPFException.Oh(ErrorCode.WF0005);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] FlowTaskUpInput input)
        {
            try
            {
                //if (_userManager.UserId.Equals("admin"))
                //    throw JNPFException.Oh(ErrorCode.WF0004);
                if (input.status == 1)
                {
                    await Save(id, input.flowId, null, null, 1, null, input.data.ToObject(), 1, 0, false);
                }
                else
                {
                    await Submit(id, input.flowId, null, null, 1, null, input.data.ToObject(), 0, 0, false);
                }
            }
            catch (Exception ex)
            {
                throw JNPFException.Oh(ErrorCode.WF0005);
            }
        }
        #endregion

        #region PublicMethod
        /// <summary>
        /// 获取任务详情
        /// </summary>
        /// <param name="id">任务id</param>
        /// <param name="taskNodeId">节点id</param>
        /// <returns></returns>
        [NonAction]
        public async Task<FlowBeforeInfoOutput> GetFlowBeforeInfo(string id, string taskNodeId)
        {
            try
            {
                var output = new FlowBeforeInfoOutput();
                var flowTaskEntity = await _flowTaskRepository.GetTaskInfo(id);
                var flowEngineEntity = await _flowEngineService.GetInfo(flowTaskEntity.FlowId);
                var flowTaskNodeEntityList = (await _flowTaskRepository.GetTaskNodeList(flowTaskEntity.Id)).FindAll(x => "0".Equals(x.State));
                var flowTaskNodeList = flowTaskNodeEntityList.Adapt<List<FlowTaskNodeModel>>().OrderBy(x => x.sortCode).ToList();
                var flowTaskOperatorList = (await _flowTaskRepository.GetTaskOperatorList(flowTaskEntity.Id)).FindAll(t => "0".Equals(t.State));
                var flowTaskOperatorRecordList = (await _flowTaskRepository.GetTaskOperatorRecordList(flowTaskEntity.Id)).Adapt<List<FlowTaskOperatorRecordModel>>();
                var colorFlag = true;
                foreach (var item in flowTaskOperatorRecordList)
                {
                    item.userName = await _usersService.GetUserName(item.handleId);
                    item.operatorId = await _usersService.GetUserName(item.operatorId);
                }
                foreach (var item in flowTaskNodeList)
                {
                    #region 流程图节点颜色类型
                    if (colorFlag || item.completion == 1)
                    {
                        item.type = "0";
                    }
                    if (flowTaskEntity.ThisStepId.Contains(item.nodeCode))
                    {
                        item.type = "1";
                        colorFlag = false;
                    }
                    if (flowTaskEntity.ThisStepId == "end")
                    {
                        item.type = "0";
                    }
                    #endregion
                    item.userName = await GetApproverUserName(item, flowTaskEntity, flowTaskEntity.FlowFormContentJson, flowTaskNodeEntityList);
                }
                var thisNode = (await _flowTaskRepository.GetTaskNodeList(flowTaskEntity.Id)).Find(x => x.Id == taskNodeId);
                if (thisNode.IsNotEmptyOrNull())
                {
                    var thisNodeProperties = thisNode.NodePropertyJson.Deserialize<ApproversProperties>();
                    output.approversProperties = thisNodeProperties;
                    output.formOperates = thisNodeProperties.formOperates.Adapt<List<FormOperatesModel>>();
                }
                output.flowFormInfo = (await _flowEngineService.GetInfo(flowTaskEntity.FlowId)).FormTemplateJson;
                output.flowTaskInfo = flowTaskEntity.Adapt<FlowTaskModel>();
                output.flowTaskInfo.appFormUrl = flowEngineEntity.AppFormUrl;
                output.flowTaskInfo.formUrl = flowEngineEntity.FormUrl;
                output.flowTaskInfo.type = flowEngineEntity.Type;
                output.flowTaskNodeList = flowTaskNodeList;
                output.flowTaskOperatorList = flowTaskOperatorList.Adapt<List<FlowTaskOperatorModel>>();
                output.flowTaskOperatorRecordList = flowTaskOperatorRecordList;
                return output;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

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
        [NonAction]
        public async Task<FlowTaskEntity> Save(string id, string flowId, string processId, string flowTitle, int? flowUrgent, string billNo, object formData, int status, int? approvaUpType = 0, bool isSysTable = true, string parentId = "0", string crUser = null, bool isDev = false)
        {
            try
            {
                var flowTaskEntity = new FlowTaskEntity();
                if (!isSysTable)
                {
                    var flowTaskEntityDynamic = await FlowDynamicDataManage(id, flowId, processId, flowTitle, flowUrgent, billNo, formData, crUser, isDev);
                    processId = flowTaskEntityDynamic.ProcessId;
                    flowTitle = flowTaskEntityDynamic.FlowName;
                    flowUrgent = flowTaskEntityDynamic.FlowUrgent;
                    billNo = flowTaskEntityDynamic.EnCode;
                    formData = flowTaskEntityDynamic.FlowFormContentJson.ToObject();
                }
                if (id.IsEmpty())
                {
                    FlowEngineEntity flowEngineEntity = await _flowEngineService.GetInfo(flowId);
                    flowTaskEntity.Id = processId;
                    flowTaskEntity.ProcessId = processId;
                    flowTaskEntity.EnCode = billNo;
                    flowTaskEntity.FullName = parentId.Equals("0") ? flowTitle : flowTitle + "(子流程)";
                    flowTaskEntity.FlowUrgent = flowUrgent;
                    flowTaskEntity.FlowId = flowEngineEntity.Id;
                    flowTaskEntity.FlowCode = flowEngineEntity.EnCode;
                    flowTaskEntity.FlowName = flowEngineEntity.FullName;
                    flowTaskEntity.FlowType = flowEngineEntity.Type;
                    flowTaskEntity.FlowCategory = flowEngineEntity.Category;
                    flowTaskEntity.FlowForm = flowEngineEntity.FormTemplateJson;
                    flowTaskEntity.FlowFormContentJson = formData == null ? "" : formData.ToJson();
                    flowTaskEntity.FlowTemplateJson = flowEngineEntity.FlowTemplateJson;
                    flowTaskEntity.FlowVersion = flowEngineEntity.Version;
                    flowTaskEntity.Status = FlowTaskStatusEnum.Draft;
                    flowTaskEntity.Completion = 0;
                    flowTaskEntity.ThisStep = "开始";
                    flowTaskEntity.CreatorTime = DateTime.Now;
                    flowTaskEntity.CreatorUserId = crUser.IsEmpty() ? _userManager.UserId : crUser;
                    flowTaskEntity.ParentId = parentId;
                    if (status == 0)
                    {
                        flowTaskEntity.Status = FlowTaskStatusEnum.Handle;
                        flowTaskEntity.EnabledMark = FlowTaskStatusEnum.Handle;
                        flowTaskEntity.StartTime = DateTime.Now;
                        flowTaskEntity.CreatorTime = DateTime.Now;
                    }
                    await _flowTaskRepository.CreateTask(flowTaskEntity);
                }
                else
                {
                    flowTaskEntity = await _flowTaskRepository.GetTaskInfo(id);
                    if (!CheckStatus(flowTaskEntity.Status) && approvaUpType == 0)
                        throw new Exception("当前流程正在运行不能重复保存");
                    if (status == 0)
                    {
                        flowTaskEntity.Status = FlowTaskStatusEnum.Handle;
                        flowTaskEntity.StartTime = DateTime.Now;
                        flowTaskEntity.LastModifyTime = DateTime.Now;
                        flowTaskEntity.LastModifyUserId = _userManager.UserId;
                    }
                    if (approvaUpType == 0)
                    {
                        flowTaskEntity.FullName = parentId.Equals("0") ? flowTitle : flowTitle + "(子流程)";
                        flowTaskEntity.FlowUrgent = flowUrgent;
                    }
                    if (formData != null)
                    {
                        flowTaskEntity.FlowFormContentJson = formData.Serialize();
                    }
                    await _flowTaskRepository.UpdateTask(flowTaskEntity);
                }
                return flowTaskEntity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 自定义表单数据处理(保存或提交)
        /// </summary>
        /// <param name="id">主键id（通过空值判断是修改还是新增）</param>
        /// <param name="flowId">流程id</param>
        /// <param name="processId"></param>
        /// <param name="flowTitle">流程任务名</param>
        /// <param name="flowUrgent">紧急程度（自定义默认为1）</param>
        /// <param name="billNo"></param>
        /// <param name="formData">表单填写的数据</param>
        /// <param name="crUser">子流程发起人</param>
        /// <param name="isDev">是否功能设计</param>
        /// <returns></returns>
        [NonAction]
        public async Task<FlowTaskEntity> FlowDynamicDataManage(string id, string flowId, string processId, string flowTitle, int? flowUrgent, string billNo, object formData, string crUser, bool isDev)
        {
            try
            {
                FlowEngineEntity flowEngineEntity = await _flowEngineService.GetInfo(flowId);
                billNo = "单据规则不存在";
                flowTitle = _userManager.User.RealName + "的" + flowEngineEntity.FullName;
                if (crUser.IsNotEmptyOrNull())
                {
                    flowTitle = _usersService.GetInfoByUserId(crUser).RealName + "的" + flowEngineEntity.FullName;
                }
                processId = processId.IsNullOrEmpty() ? YitIdHelper.NextId().ToString() : processId;
                flowUrgent = 1;
                //表单模板list
                List<FieldsModel> fieldsModelList = flowEngineEntity.FormTemplateJson.Deserialize<FormDataModel>().fields;
                //剔除布局控件
                fieldsModelList = _runService.TemplateDataConversion(fieldsModelList);
                //待保存表单数据
                Dictionary<string, object> formDataDic = formData.ToObeject<Dictionary<string, object>>();
                //有表无表
                bool isTable = flowEngineEntity.Tables.Equals("[]");
                //新增或修改
                bool type = id.IsEmpty();
                if (!type)
                {
                    var entity = await _flowTaskRepository.GetTaskInfo(id);
                    processId = id;
                    flowTitle = entity.FullName;
                    billNo = entity.EnCode;
                    flowUrgent = entity.FlowUrgent;
                }
                #region 待保存表单数据
                VisualDevEntity visualdevEntity = new VisualDevEntity() { Id = processId, FormData = flowEngineEntity.FormTemplateJson, Tables = flowEngineEntity.Tables, DbLinkId = flowEngineEntity.DbLinkId };
                VisualDevModelDataCrInput visualdevModelDataCrForm = new VisualDevModelDataCrInput() { data = formData.ToJson() };
                var dbLink = await _dbLinkService.GetInfo(flowEngineEntity.DbLinkId);
                #endregion
                if (!isTable)
                {
                    var sql = "";
                    if (type)
                    {
                        sql = await _runService.CreateHaveTableSql(visualdevEntity, visualdevModelDataCrForm, processId);
                    }
                    else
                    {
                        sql = await _runService.UpdateHaveTableSql(visualdevEntity, visualdevModelDataCrForm.Adapt<VisualDevModelDataUpInput>(), id);
                    }
                    if (!isDev)
                    {
                        await _dataBaseService.ExecuteSql(dbLink, sql);
                    }
                }
                else
                {
                    //修改时单据规则不做修改
                    if (type)
                    {
                        var fieLdsModel = fieldsModelList.FindAll(x => "billRule".Equals(x.__config__.jnpfKey));
                        if (fieLdsModel.Count > 0)
                        {
                            string ruleKey = fieLdsModel.FirstOrDefault().__config__.rule;
                            billNo = await _billRullService.GetBillNumber(ruleKey, false);
                        }
                    }
                    //无表处理后待保存数据
                    formData = await _runService.GenerateFeilds(fieldsModelList, formDataDic, type);
                }
                var flowTaskEntity = new FlowTaskEntity();
                flowTaskEntity.Id = id;
                flowTaskEntity.FlowId = flowId;
                flowTaskEntity.ProcessId = processId;
                flowTaskEntity.FlowName = flowTitle;
                flowTaskEntity.FlowUrgent = flowUrgent;
                flowTaskEntity.EnCode = billNo;
                flowTaskEntity.FlowFormContentJson = formData.ToJson();
                return flowTaskEntity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

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
        [NonAction]
        public async Task Submit(string id, string flowId, string processId, string flowTitle, int? flowUrgent, string billNo, object formData, int status, int? approvaUpType = 0, bool isSysTable = true, bool isDev = false)
        {
            try
            {
                DbScoped.SugarScope.BeginTran();
                #region 流程引擎
                FlowEngineEntity flowEngineEntity = await _flowEngineService.GetInfo(flowId);
                //判断字段类型
                Dictionary<string, string> jnpfKey = new Dictionary<string, string>();
                //下拉、单选、高级控件的list
                Dictionary<string, object> keyList = new Dictionary<string, object>();
                if (flowEngineEntity.FormType == 2)
                {
                    List<FieldsModel> fieldsModelList = flowEngineEntity.FormTemplateJson.Deserialize<FormDataModel>().fields;
                    await tempJson(fieldsModelList, jnpfKey, keyList);
                }
                #endregion

                #region 流程任务
                FlowTaskEntity flowTaskEntity = await this.Save(id, flowId, processId, flowTitle, flowUrgent, billNo, formData, status, approvaUpType, isSysTable, "0", null, isDev);
                #endregion

                #region 流程节点
                List<FlowTaskNodeEntity> flowTaskNodeEntityList = new List<FlowTaskNodeEntity>();
                //所有节点
                var taskNodeList = new List<TaskNodeModel>();
                var flowTemplateJsonModel = flowEngineEntity.FlowTemplateJson.Deserialize<FlowTemplateJsonModel>();
                var dic = new Dictionary<int, string>();
                #region 流程模板所有节点
                var flowTemplateJsonModelList = new List<FlowTemplateJsonModel>();
                var childNodeIdList = new List<string>();
                GetChildNodeIdList(flowTemplateJsonModel, childNodeIdList);
                GetFlowTemplateList(flowTemplateJsonModel, flowTemplateJsonModelList);
                #endregion
                GetFlowTemplateAll(flowTemplateJsonModel, taskNodeList, flowTemplateJsonModelList, childNodeIdList);
                DeleteConditionTaskNodeModel(taskNodeList, formData.Serialize(), jnpfKey, keyList);
                foreach (var item in taskNodeList)
                {
                    var flowTaskNodeEntity = new FlowTaskNodeEntity();
                    flowTaskNodeEntity.Id = YitIdHelper.NextId().ToString();
                    flowTaskNodeEntity.CreatorTime = DateTime.Now;
                    flowTaskNodeEntity.TaskId = flowTaskEntity.Id;
                    flowTaskNodeEntity.NodeCode = item.nodeId;
                    flowTaskNodeEntity.NodeType = item.type;
                    flowTaskNodeEntity.Completion = item.type == "start" ? 1 : 0;
                    flowTaskNodeEntity.NodeName = item.type == "start" ? "开始" : item.propertyJson.title;
                    flowTaskNodeEntity.NodeUp = item.type != "approver" ? null : item.propertyJson.rejectStep;
                    flowTaskNodeEntity.NodeNext = item.nextNodeId;
                    flowTaskNodeEntity.NodePropertyJson = JsonConvert.SerializeObject(item.propertyJson);
                    flowTaskNodeEntity.State = "-2";
                    flowTaskNodeEntityList.Add(flowTaskNodeEntity);
                }
                DeleteEmptyOrTimerTaskNode(flowTaskNodeEntityList);
                await CreateNode(flowTaskNodeEntityList);
                #endregion


                List<FlowTaskOperatorEntity> flowTaskOperatorEntityList = new List<FlowTaskOperatorEntity>();
                //开始节点
                var startTaskNodeEntity = flowTaskNodeEntityList.Find(m => m.NodeType == "start");
                var nextTaskNodeIdList = startTaskNodeEntity.NodeNext.Split(",");
                if (nextTaskNodeIdList.FirstOrDefault().Equals("end"))
                {
                    flowTaskEntity.Status = FlowTaskStatusEnum.Adopt;
                    flowTaskEntity.Completion = 100;
                    flowTaskEntity.EndTime = DateTime.Now;
                    flowTaskEntity.ThisStepId = "end";
                    flowTaskEntity.ThisStep = "结束";
                    await _flowTaskRepository.UpdateTask(flowTaskEntity);
                    #region 结束事件
                    var startApproversProperties = flowTaskNodeEntityList.Find(x => x.NodeType.Equals("start")).NodePropertyJson.Deserialize<StartProperties>();
                    if (startApproversProperties.hasEndFunc)
                    {
                        await GetApproverUrl(startApproversProperties.endInterfaceUrl, flowTaskEntity.Id, startTaskNodeEntity.Id);
                    }
                    #endregion

                    #region 流程完成通知发起人
                    await _messageService.SentMessage(new List<string>() { flowTaskEntity.CreatorUserId }, flowTaskEntity.FullName + "【流程审批通过】", "审批通过");
                    #endregion

                    #region 子流程结束回到主流程下一节点
                    if (flowTaskEntity.ParentId != "0")
                    {
                        await InsertSubFlowNextNode(flowTaskEntity);
                    }
                    #endregion
                }
                else
                {
                    #region 流程经办
                    //任务流程当前节点名
                    var ThisStepList = new List<string>();
                    //任务流程当前完成度
                    var CompletionList = new List<int>();
                    foreach (var item in nextTaskNodeIdList)
                    {
                        var nextTaskNodeEntity = flowTaskNodeEntityList.Find(m => m.NodeCode.Equals(item));
                        var approverPropertiers = nextTaskNodeEntity.NodePropertyJson.Deserialize<ApproversProperties>();
                        if (nextTaskNodeEntity.NodeType.Equals("subFlow"))
                        {
                            var childTaskPro = nextTaskNodeEntity.NodePropertyJson.Deserialize<ChildTaskProperties>();
                            var childTaskCrUserList = await GetSubFlowCreator(childTaskPro, flowTaskEntity.CreatorUserId);
                            var childFormData = await GetSubFlowFormData(childTaskPro, formData.ToJson(), isSysTable);
                            childTaskPro.childTaskId = await CreateSubProcesses(childTaskPro.flowId, childFormData, flowTaskEntity.Id, childTaskCrUserList, isSysTable);
                            childTaskPro.formData = formData.ToJson();
                            nextTaskNodeEntity.NodePropertyJson = childTaskPro.ToJson();
                            //将子流程id保存到主流程的子流程节点属性上
                            await _flowTaskRepository.UpdateTaskNode(nextTaskNodeEntity);
                        }
                        else
                        {
                            await AddFlowTaskOperatorEntityByAssigneeType(flowTaskOperatorEntityList, flowTaskNodeEntityList, startTaskNodeEntity, nextTaskNodeEntity, flowTaskEntity.CreatorUserId, formData.Serialize(), 0);
                        }
                        ThisStepList.Add(nextTaskNodeEntity.NodeName);
                        CompletionList.Add(approverPropertiers.progress.ToInt());
                    }
                    await _flowTaskRepository.CreateTaskOperator(flowTaskOperatorEntityList);
                    #endregion

                    #region 更新流程任务
                    flowTaskEntity.ThisStepId = startTaskNodeEntity.NodeNext;
                    flowTaskEntity.ThisStep = string.Join(",", ThisStepList);
                    flowTaskEntity.Completion = CompletionList.Min();
                    await _flowTaskRepository.UpdateTask(flowTaskEntity);
                    #endregion
                }

                #region 流程经办记录
                FlowTaskOperatorRecordEntity flowTaskOperatorRecordEntity = new FlowTaskOperatorRecordEntity();
                flowTaskOperatorRecordEntity.HandleId = _userManager.UserId;
                flowTaskOperatorRecordEntity.HandleTime = DateTime.Now;
                flowTaskOperatorRecordEntity.HandleStatus = 2;
                flowTaskOperatorRecordEntity.NodeName = "开始";
                flowTaskOperatorRecordEntity.TaskId = flowTaskEntity.Id;
                flowTaskOperatorRecordEntity.Status = 0;
                await _flowTaskRepository.CreateTaskOperatorRecord(flowTaskOperatorRecordEntity);
                #endregion

                #region 开始事件
                var startProperties = startTaskNodeEntity.NodePropertyJson.Deserialize<StartProperties>();
                if (startProperties.hasInitFunc)
                {
                    await GetApproverUrl(startProperties.initInterfaceUrl, startTaskNodeEntity.TaskId, startTaskNodeEntity.Id);
                }
                #endregion

                #region 消息提醒
                Dictionary<string, object> messageDic = new Dictionary<string, object>();
                messageDic["type"] = FlowMessageEnum.wait;
                messageDic["id"] = flowTaskEntity.Id;
                //审核提醒
                foreach (var item in flowTaskOperatorEntityList)
                {
                    var messageType = flowTaskNodeEntityList.Find(x => x.Id == item.TaskNodeId)
                        .NodePropertyJson.Deserialize<ApproversProperties>().messageType;
                    SendNodeMessage(messageType, flowTaskEntity.FullName + "【审核】", new List<string>() { item.HandleId }, messageDic.Serialize());
                }
                #endregion
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.Sugar.RollbackTran();
                _logger.LogInformation("提交日志:" + ex.Message + ",错误详情:" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 审批(同意)
        /// </summary>
        /// <param name="flowTaskEntity"></param>
        /// <param name="flowTaskOperatorEntity"></param>
        /// <param name="flowHandleModel"></param>
        /// <param name="formType">表单类型</param>
        /// <returns></returns>
        [NonAction]
        public async Task Audit(FlowTaskEntity flowTaskEntity, FlowTaskOperatorEntity flowTaskOperatorEntity, FlowHandleModel flowHandleModel, int formType)
        {
            try
            {
                DbScoped.SugarScope.BeginTran();
                //流程所有节点
                List<FlowTaskNodeEntity> flowTaskNodeEntityList = (await _flowTaskRepository.GetTaskNodeList(flowTaskEntity.Id)).FindAll(x => x.State == "0");
                //当前节点
                FlowTaskNodeEntity flowTaskNodeEntity = flowTaskNodeEntityList.Find(m => m.Id == flowTaskOperatorEntity.TaskNodeId);
                //当前节点属性
                ApproversProperties approversProperties = flowTaskNodeEntity.NodePropertyJson.Deserialize<ApproversProperties>();
                //当前节点所有审批人
                var thisFlowTaskOperatorEntityList = (await _flowTaskRepository.GetTaskOperatorList(flowTaskNodeEntity.TaskId))
                    .FindAll(x => x.TaskNodeId == flowTaskNodeEntity.Id && x.State == "0");
                //下一节点流程经办
                List<FlowTaskOperatorEntity> flowTaskOperatorEntityList = new List<FlowTaskOperatorEntity>();
                //流程抄送
                List<FlowTaskCirculateEntity> flowTaskCirculateEntityList = new List<FlowTaskCirculateEntity>();
                //表单数据
                var formData = formType == 2 ? flowHandleModel.formData.Serialize().Deserialize<JObject>()["data"].ToString() : flowHandleModel.formData.Serialize();

                if (flowTaskOperatorEntity.Id.IsNotEmptyOrNull())
                {
                    #region 更新当前经办数据
                    await UpdateFlowTaskOperator(flowTaskOperatorEntity, thisFlowTaskOperatorEntityList, approversProperties, 1, flowHandleModel.freeApproverUserId);
                    #endregion

                    #region 更新当前抄送
                    GetflowTaskCirculateEntityList(approversProperties, flowTaskOperatorEntity, flowTaskCirculateEntityList, flowHandleModel.copyIds);
                    await _flowTaskRepository.CreateTaskCirculate(flowTaskCirculateEntityList);
                    #endregion
                }

                #region 更新经办记录
                await CreateOperatorRecode(flowTaskOperatorEntity, flowHandleModel, 1);
                #endregion

                #region 更新下一节点经办
                if (flowHandleModel.freeApproverUserId.IsNotEmptyOrNull())
                {
                    //加签审批人
                    var freeApproverOperatorEntity = new FlowTaskOperatorEntity()
                    {
                        Id = YitIdHelper.NextId().ToString(),
                        ParentId = flowTaskOperatorEntity.Id,
                        HandleType = "7",
                        HandleId = flowHandleModel.freeApproverUserId,
                        NodeCode = flowTaskOperatorEntity.NodeCode,
                        NodeName = flowTaskOperatorEntity.NodeName,
                        Description = flowTaskOperatorEntity.Description,
                        CreatorTime = DateTime.Now,
                        TaskNodeId = flowTaskOperatorEntity.TaskNodeId,
                        TaskId = flowTaskOperatorEntity.TaskId,
                        Type = flowTaskOperatorEntity.Type,
                        State = flowTaskOperatorEntity.State,
                        Completion = 0
                    };
                    await _flowTaskRepository.CreateTaskOperator(freeApproverOperatorEntity);
                    //当前审批人state改为1
                    flowTaskOperatorEntity.State = "1";
                    await _flowTaskRepository.UpdateTaskOperator(flowTaskOperatorEntity);

                    #region 流转记录
                    var flowTaskOperatorRecordEntity = new FlowTaskOperatorRecordEntity();
                    flowTaskOperatorRecordEntity.HandleOpinion = flowHandleModel.handleOpinion;
                    flowTaskOperatorRecordEntity.HandleId = _userManager.UserId;
                    flowTaskOperatorRecordEntity.HandleTime = DateTime.Now;
                    flowTaskOperatorRecordEntity.HandleStatus = 6;
                    flowTaskOperatorRecordEntity.NodeName = flowTaskOperatorEntity.NodeName;
                    flowTaskOperatorRecordEntity.TaskId = flowTaskOperatorEntity.TaskId;
                    flowTaskOperatorRecordEntity.TaskNodeId = flowTaskOperatorEntity.TaskNodeId;
                    flowTaskOperatorRecordEntity.TaskOperatorId = flowTaskOperatorEntity.Id;
                    flowTaskOperatorRecordEntity.Status = 0;
                    flowTaskOperatorRecordEntity.OperatorId = flowHandleModel.freeApproverUserId;
                    await _flowTaskRepository.CreateTaskOperatorRecord(flowTaskOperatorRecordEntity);
                    #endregion
                }
                else
                {
                    await CreateNextFlowTaskOperator(flowTaskNodeEntityList, flowTaskNodeEntity, approversProperties,
                                            thisFlowTaskOperatorEntityList, 1, flowTaskEntity, flowHandleModel.freeApproverUserId,
                                            flowTaskOperatorEntityList, formData, flowHandleModel, formType);
                }
                #endregion

                #region 更新节点
                await _flowTaskRepository.UpdateTaskNode(flowTaskNodeEntity);
                #endregion

                #region 更新任务
                if (flowTaskNodeEntity.Completion > 0)
                {
                    #region 审批事件
                    if (approversProperties.hasApproverFunc)
                    {
                        await GetApproverUrl(approversProperties.approverInterfaceUrl, flowTaskEntity.Id, flowTaskNodeEntity.Id, 1);
                    }
                    #endregion
                    if (flowTaskEntity.Status == FlowTaskStatusEnum.Adopt)
                    {
                        #region 结束事件
                        var startApproversProperties = flowTaskNodeEntityList.Find(x => x.NodeType.Equals("start")).NodePropertyJson.Deserialize<StartProperties>();
                        if (startApproversProperties.hasEndFunc)
                        {
                            await GetApproverUrl(startApproversProperties.endInterfaceUrl, flowTaskEntity.Id, flowTaskNodeEntity.Id);
                        }
                        #endregion

                        #region 流程完成通知发起人
                        await _messageService.SentMessage(new List<string>() { flowTaskEntity.CreatorUserId }, flowTaskEntity.FullName + "【流程审批通过】", "审批通过");
                        #endregion

                        #region 子流程结束回到主流程下一节点
                        if (flowTaskEntity.ParentId != "0")
                        {
                            await InsertSubFlowNextNode(flowTaskEntity);
                        }
                        #endregion
                    }
                    await _flowTaskRepository.UpdateTask(flowTaskEntity);
                }
                #endregion

                #region 消息提醒
                if (flowTaskNodeEntity.Completion > 0)
                {
                    Dictionary<string, object> message = new Dictionary<string, object>();
                    message["id"] = flowTaskEntity.Id;
                    var userIdList = new List<string>();
                    var title = "";
                    //审核提醒
                    message["type"] = FlowMessageEnum.wait;
                    title = flowTaskEntity.FullName + "【审核】";
                    foreach (var item in flowTaskOperatorEntityList)
                    {
                        var messageType = flowTaskNodeEntityList.Find(x => x.Id == item.TaskNodeId)
                            .NodePropertyJson.Deserialize<ApproversProperties>().messageType;
                        SendNodeMessage(messageType, title, new List<string>() { item.HandleId }, message.Serialize());
                    }
                    //抄送提醒
                    message["type"] = FlowMessageEnum.circulate;
                    title = flowTaskEntity.FullName + "【抄送】";
                    userIdList = flowTaskCirculateEntityList.Select(x => x.ObjectId).ToList();
                    SendNodeMessage(approversProperties.messageType, title, userIdList, message.Serialize());
                    //创建者消息提醒
                    message["type"] = FlowMessageEnum.me;
                    title = string.Format("{0}【{1}审核通过】", flowTaskEntity.FullName, await _usersService.GetUserName(flowTaskOperatorEntity.HandleId));
                    userIdList.Clear();
                    userIdList.Add(flowTaskEntity.CreatorUserId);
                    SendNodeMessage(approversProperties.messageType, title, userIdList, message.Serialize());
                }
                #endregion
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.Sugar.RollbackTran();
            }


        }

        /// <summary>
        /// 审批(拒绝)
        /// </summary>
        /// <param name="flowTaskEntity"></param>
        /// <param name="flowTaskOperatorEntity"></param>
        /// <param name="flowHandleModel"></param>
        /// <param name="formType"></param>
        /// <returns></returns>
        [NonAction]
        public async Task Reject(FlowTaskEntity flowTaskEntity, FlowTaskOperatorEntity flowTaskOperatorEntity, FlowHandleModel flowHandleModel, int formType)
        {
            try
            {
                DbScoped.SugarScope.BeginTran();
                //流程所有节点
                List<FlowTaskNodeEntity> flowTaskNodeEntityList = (await _flowTaskRepository.GetTaskNodeList(flowTaskEntity.Id)).FindAll(x => x.State == "0");
                //当前节点
                FlowTaskNodeEntity flowTaskNodeEntity = flowTaskNodeEntityList.Find(m => m.Id == flowTaskOperatorEntity.TaskNodeId);
                //当前节点属性
                ApproversProperties approversProperties = flowTaskNodeEntity.NodePropertyJson.Deserialize<ApproversProperties>();
                //当前节点所有审批人
                var thisFlowTaskOperatorEntityList = (await _flowTaskRepository.GetTaskOperatorList(flowTaskNodeEntity.TaskId)).FindAll(x => x.TaskNodeId == flowTaskNodeEntity.Id && x.State == "0");
                //表单数据
                var formData = formType == 2 ? (flowHandleModel.formData.Serialize().Deserialize<JObject>())["data"].ToString() : flowHandleModel.formData.Serialize();
                //驳回节点流程经办
                List<FlowTaskOperatorEntity> flowTaskOperatorEntityList = new List<FlowTaskOperatorEntity>();
                #region 更新当前经办数据
                await UpdateFlowTaskOperator(flowTaskOperatorEntity, thisFlowTaskOperatorEntityList, approversProperties, 0, flowHandleModel.freeApproverUserId);
                #endregion

                #region 自定义抄送
                var flowTaskCirculateEntityList = new List<FlowTaskCirculateEntity>();
                GetflowTaskCirculateEntityList(approversProperties, flowTaskOperatorEntity, flowTaskCirculateEntityList, flowHandleModel.copyIds, 0);
                await _flowTaskRepository.CreateTaskCirculate(flowTaskCirculateEntityList);
                #endregion

                #region 更新驳回经办
                await CreateNextFlowTaskOperator(flowTaskNodeEntityList, flowTaskNodeEntity, approversProperties,
                    thisFlowTaskOperatorEntityList, 0, flowTaskEntity, flowHandleModel.freeApproverUserId,
                    flowTaskOperatorEntityList, formData,flowHandleModel,formType);
                #endregion

                #region 更新流程任务
                if (flowTaskEntity.Status == FlowTaskStatusEnum.Reject)
                {
                    await _flowTaskRepository.UpdateTask(flowTaskEntity);
                    await _flowTaskRepository.DeleteFlowTaskAllData(flowTaskEntity.Id);
                }
                else
                {
                    await _flowTaskRepository.UpdateTask(flowTaskEntity);
                    await _flowTaskRepository.CreateTaskOperator(flowTaskOperatorEntityList);
                }
                #endregion

                #region 更新经办记录
                await CreateOperatorRecode(flowTaskOperatorEntity, flowHandleModel, 0);
                #endregion

                if (approversProperties.hasApproverRejectFunc)
                {
                    await GetApproverUrl(approversProperties.approverRejectInterfaceUrl, flowTaskNodeEntity.TaskId, flowTaskNodeEntity.Id);
                }

                #region 消息提醒
                Dictionary<string, object> message = new Dictionary<string, object>();
                message["id"] = flowTaskEntity.Id;
                var userIdList = new List<string>();
                var title = "";

                //审核提醒
                message["type"] = FlowMessageEnum.wait;
                title = flowTaskEntity.FullName + "【审核】";
                foreach (var item in flowTaskOperatorEntityList)
                {
                    var messageType = flowTaskNodeEntityList.Find(x => x.Id == item.TaskNodeId)
                        .NodePropertyJson.Deserialize<ApproversProperties>().messageType;
                    SendNodeMessage(messageType, flowTaskEntity.FullName + "【审核】", new List<string>() { item.HandleId }, message.Serialize());
                }
                //抄送提醒
                message["type"] = FlowMessageEnum.circulate;
                title = flowTaskEntity.FullName + "【抄送】";
                userIdList = flowTaskCirculateEntityList.Select(x => x.ObjectId).ToList();
                SendNodeMessage(approversProperties.messageType, title, userIdList, message.Serialize());
                //创建者消息提醒
                message["type"] = FlowMessageEnum.me;
                title = string.Format("{0}【{1}审核拒绝】", flowTaskEntity.FullName, await _usersService.GetUserName(flowTaskOperatorEntity.HandleId));
                userIdList.Clear();
                userIdList.Add(flowTaskEntity.CreatorUserId);
                SendNodeMessage(approversProperties.messageType, title, userIdList, message.Serialize());
                #endregion
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.Sugar.RollbackTran();
            }
        }

        /// <summary>
        /// 审批(撤回)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flowHandleModel"></param>
        /// <returns></returns>
        [NonAction]
        public async Task Recall(string id, FlowHandleModel flowHandleModel)
        {
            try
            {
                DbScoped.SugarScope.BeginTran();
                //撤回经办记录
                var flowTaskOperatorRecordEntity = await _flowTaskRepository.GetTaskOperatorRecordInfo(id);
                //撤回经办
                var flowTaskOperatorEntity = await _flowTaskRepository.GetTaskOperatorInfo(flowTaskOperatorRecordEntity.TaskOperatorId);
                //撤回节点
                var flowTaskNodeEntity = await _flowTaskRepository.GetTaskNodeInfo(flowTaskOperatorRecordEntity.TaskNodeId);
                //撤回任务
                var flowTaskEntity = await _flowTaskRepository.GetTaskInfo(flowTaskOperatorRecordEntity.TaskId);
                //所有节点
                var flowTaskNodeEntityList = (await _flowTaskRepository.GetTaskNodeList(flowTaskOperatorRecordEntity.TaskId)).FindAll(x => x.State == "0");
                //所有经办
                var flowTaskOperatorEntityList = (await _flowTaskRepository.GetTaskOperatorList(flowTaskOperatorRecordEntity.TaskId)).FindAll(x => x.State == "0");
                //撤回节点属性
                var recallNodeProperties = flowTaskNodeEntity.NodePropertyJson.Deserialize<ApproversProperties>();
                #region 撤回判断
                //拒绝不撤回
                if (flowTaskOperatorEntity.HandleStatus == 0)
                    throw JNPFException.Oh(ErrorCode.WF0010);
                //任务待审状态才能撤回
                if (!(flowTaskEntity.EnabledMark == 1 && flowTaskEntity.Status == 1))
                    throw JNPFException.Oh(ErrorCode.WF0011);
                //撤回节点下一节点已操作
                var recallNextOperatorList = flowTaskOperatorEntityList.FindAll(x => flowTaskNodeEntity.NodeNext.Contains(x.NodeCode));
                if (recallNextOperatorList.FindAll(x => x.Completion == 1).Count > 0)
                    throw JNPFException.Oh(ErrorCode.WF0011);
                #endregion

                #region 经办修改
                var delOperatorRecordIds = new List<string>();
                //加签人
                var upOperatorList = await GetOperator(flowTaskOperatorEntity.Id, new List<FlowTaskOperatorEntity>());

                flowTaskOperatorEntity.HandleStatus = null;
                flowTaskOperatorEntity.HandleTime = null;
                flowTaskOperatorEntity.Completion = 0;
                flowTaskOperatorEntity.State = "0";
                upOperatorList.Add(flowTaskOperatorEntity);

                foreach (var item in upOperatorList)
                {
                    var operatorRecord = await _flowTaskRepository.GetTaskOperatorRecordInfo(item.TaskId, item.TaskNodeId, item.Id);
                    if (operatorRecord.IsNotEmptyOrNull())
                    {
                        delOperatorRecordIds.Add(operatorRecord.Id);
                    }
                }
                //撤回节点是否完成
                if (flowTaskNodeEntity.Completion == 1)
                {
                    //撤回节点下一节点经办删除
                    await _flowTaskRepository.DeleteTaskOperator(recallNextOperatorList.Select(x => x.Id).ToList());
                    //或签经办全部撤回，会签撤回未处理的经办
                    //撤回节点未审批的经办
                    var notHanleOperatorList = flowTaskOperatorEntityList.FindAll(x => x.TaskNodeId == flowTaskOperatorRecordEntity.TaskNodeId && x.HandleStatus == null
                     && x.HandleTime == null);
                    foreach (var item in notHanleOperatorList)
                    {
                        item.Completion = 0;
                    }
                    upOperatorList = upOperatorList.Union(notHanleOperatorList).ToList();

                    #region 更新任务流程
                    var recallNodeList = flowTaskNodeEntityList.FindAll(x => x.SortCode == flowTaskNodeEntity.SortCode);
                    flowTaskEntity.ThisStep = string.Join(",", recallNodeList.Select(x => x.NodeName).ToList());
                    flowTaskEntity.ThisStepId = string.Join(",", recallNodeList.Select(x => x.NodeCode).ToList());
                    flowTaskEntity.Completion = flowTaskNodeEntity.NodePropertyJson.Deserialize<ApproversProperties>().progress.ToInt();
                    flowTaskEntity.Status = FlowTaskStatusEnum.Handle;
                    await _flowTaskRepository.UpdateTask(flowTaskEntity);
                    #endregion
                }
                #endregion
                await _flowTaskRepository.UpdateTaskOperator(upOperatorList);

                #region 删除经办记录
                delOperatorRecordIds.Add(flowTaskOperatorRecordEntity.Id);
                await _flowTaskRepository.DeleteTaskOperatorRecord(delOperatorRecordIds);
                #endregion

                #region 撤回记录
                flowTaskOperatorRecordEntity.HandleId = _userManager.UserId;
                flowTaskOperatorRecordEntity.HandleOpinion = flowHandleModel.handleOpinion;
                flowTaskOperatorRecordEntity.HandleTime = DateTime.Now;
                flowTaskOperatorRecordEntity.HandleStatus = 3;
                flowTaskOperatorRecordEntity.NodeName = flowTaskNodeEntity.NodeName;
                flowTaskOperatorRecordEntity.TaskId = flowTaskEntity.Id;
                flowTaskOperatorRecordEntity.TaskNodeId = flowTaskOperatorRecordEntity.TaskNodeId;
                flowTaskOperatorRecordEntity.TaskOperatorId = flowTaskOperatorRecordEntity.Id;
                flowTaskOperatorRecordEntity.Status = 0;
                await _flowTaskRepository.CreateTaskOperatorRecord(flowTaskOperatorRecordEntity);
                #endregion

                #region 撤回事件
                if (recallNodeProperties.hasRecallFunc)
                {
                    await GetApproverUrl(recallNodeProperties.recallInterfaceUrl, flowTaskNodeEntity.TaskId, flowTaskNodeEntity.Id);
                }
                #endregion
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.Sugar.RollbackTran();
                throw JNPFException.Oh(ErrorCode.WF0005);
            }
        }

        /// <summary>
        /// 流程撤回
        /// </summary>
        /// <param name="flowTaskEntity">流程实例</param>
        /// <param name="flowHandleModel">流程经办</param>
        [NonAction]
        public async Task Revoke(FlowTaskEntity flowTaskEntity, string flowHandleModel)
        {
            try
            {
                DbScoped.SugarScope.BeginTran();
                var starProperty = (await _flowTaskRepository.GetTaskNodeList(flowTaskEntity.Id))
                    .Find(x => x.NodeType == "start" && x.State == "0").NodePropertyJson.Deserialize<StartProperties>();
                #region 撤回数据
                await _flowTaskRepository.DeleteFlowTaskAllData(flowTaskEntity.Id);
                #endregion

                #region 更新实例
                flowTaskEntity.ThisStepId = "";
                flowTaskEntity.ThisStep = "开始";
                flowTaskEntity.Completion = 0;
                flowTaskEntity.Status = FlowTaskStatusEnum.Revoke;
                flowTaskEntity.StartTime = null;
                flowTaskEntity.EndTime = null;
                await _flowTaskRepository.UpdateTask(flowTaskEntity);
                #endregion

                #region 撤回记录
                FlowTaskOperatorRecordEntity flowTaskOperatorRecordEntity = new FlowTaskOperatorRecordEntity();
                flowTaskOperatorRecordEntity.HandleOpinion = flowHandleModel;
                flowTaskOperatorRecordEntity.HandleId = _userManager.UserId;
                flowTaskOperatorRecordEntity.HandleTime = DateTime.Now;
                flowTaskOperatorRecordEntity.HandleStatus = 3;
                flowTaskOperatorRecordEntity.NodeName = "开始";
                flowTaskOperatorRecordEntity.TaskId = flowTaskEntity.Id;
                flowTaskOperatorRecordEntity.Status = 0;
                await _flowTaskRepository.CreateTaskOperatorRecord(flowTaskOperatorRecordEntity);
                #endregion

                #region 撤回子流程任务
                var childTask = (await _flowTaskRepository.GetTaskList()).FindAll(x => flowTaskEntity.Id == x.ParentId);
                foreach (var item in childTask)
                {
                    if (item.Status == 1)
                    {
                        await this.Revoke(item, flowHandleModel);
                    }
                    await _flowTaskRepository.DeleteTask(item);
                }
                #endregion

                #region 撤回事件
                if (starProperty.hasFlowRecallFunc)
                {
                    await GetApproverUrl(starProperty.flowRecallInterfaceUrl, flowTaskEntity.Id, "");
                }
                #endregion
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.Sugar.RollbackTran();
            }
        }

        /// <summary>
        /// 终止
        /// </summary>
        /// <param name="flowTaskEntity">流程实例</param>
        /// <param name="flowHandleModel">流程经办</param>
        [NonAction]
        public async Task Cancel(FlowTaskEntity flowTaskEntity, FlowHandleModel flowHandleModel)
        {
            try
            {
                DbScoped.SugarScope.BeginTran();

                #region 更新实例
                flowTaskEntity.Status = FlowTaskStatusEnum.Cancel;
                flowTaskEntity.EndTime = DateTime.Now;
                await _flowTaskRepository.UpdateTask(flowTaskEntity);
                #endregion

                #region 作废记录
                FlowTaskOperatorRecordEntity flowTaskOperatorRecordEntity = new FlowTaskOperatorRecordEntity();
                flowTaskOperatorRecordEntity.HandleOpinion = flowHandleModel.handleOpinion;
                flowTaskOperatorRecordEntity.HandleId = _userManager.UserId;
                flowTaskOperatorRecordEntity.HandleTime = DateTime.Now;
                flowTaskOperatorRecordEntity.HandleStatus = 4;
                flowTaskOperatorRecordEntity.NodeName = flowTaskEntity.ThisStep;
                flowTaskOperatorRecordEntity.TaskId = flowTaskEntity.Id;
                flowTaskOperatorRecordEntity.Status = 0;
                await _flowTaskRepository.CreateTaskOperatorRecord(flowTaskOperatorRecordEntity);
                #endregion
                DbScoped.SugarScope.CommitTran();

            }
            catch (Exception ex)
            {
                DbScoped.Sugar.RollbackTran();
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 指派
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flowHandleModel"></param>
        /// <returns></returns>
        [NonAction]
        public async Task Assigned(string id, FlowHandleModel flowHandleModel)
        {
            try
            {
                DbScoped.SugarScope.BeginTran();
                var flowOperatorEntityList = (await _flowTaskRepository.GetTaskOperatorList(id)).FindAll(x => x.State == "0" && x.NodeCode == flowHandleModel.nodeCode);
                await _flowTaskRepository.DeleteTaskOperator(flowOperatorEntityList.Select(x => x.Id).ToList());
                var entity = new FlowTaskOperatorEntity()
                {
                    Id = YitIdHelper.NextId().ToString(),
                    HandleId = flowHandleModel.freeApproverUserId,
                    HandleType = flowOperatorEntityList.FirstOrDefault().HandleType,
                    NodeCode = flowOperatorEntityList.FirstOrDefault().NodeCode,
                    NodeName = flowOperatorEntityList.FirstOrDefault().NodeName,
                    CreatorTime = DateTime.Now,
                    TaskId = flowOperatorEntityList.FirstOrDefault().TaskId,
                    TaskNodeId = flowOperatorEntityList.FirstOrDefault().TaskNodeId,
                    Type = flowOperatorEntityList.FirstOrDefault().Type,
                    Completion = 0,
                    State = "0"
                };
                var isOk = await _flowTaskRepository.CreateTaskOperator(entity);
                if (isOk < 1)
                    throw JNPFException.Oh(ErrorCode.WF0008);

                #region 流转记录
                var flowTaskOperatorRecordEntity = new FlowTaskOperatorRecordEntity();
                flowTaskOperatorRecordEntity.HandleOpinion = flowHandleModel.handleOpinion;
                flowTaskOperatorRecordEntity.HandleId = _userManager.UserId;
                flowTaskOperatorRecordEntity.HandleTime = DateTime.Now;
                flowTaskOperatorRecordEntity.HandleStatus = 5;
                flowTaskOperatorRecordEntity.NodeName = entity.NodeName;
                flowTaskOperatorRecordEntity.TaskId = entity.TaskId;
                flowTaskOperatorRecordEntity.Status = 0;
                flowTaskOperatorRecordEntity.OperatorId = flowHandleModel.freeApproverUserId;
                await _flowTaskRepository.CreateTaskOperatorRecord(flowTaskOperatorRecordEntity);
                #endregion

                #region 消息通知
                var flowTaskEntity = await _flowTaskRepository.GetTaskInfo(id);
                var nodeEntity = (await _flowTaskRepository.GetTaskNodeList(id)).Find(x => x.NodeCode == flowHandleModel.nodeCode && x.State == "0");
                var nodeProperties = nodeEntity.NodePropertyJson.Deserialize<ApproversProperties>();
                Dictionary<string, object> message = new Dictionary<string, object>();
                message["id"] = id;
                message["type"] = FlowMessageEnum.wait;
                var userIdList = new List<string>();
                var title = flowTaskEntity.FullName + "【审核】";
                SendNodeMessage(nodeProperties.messageType, title, new List<string>() { flowHandleModel.freeApproverUserId }, message.Serialize());
                #endregion
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.Sugar.RollbackTran();
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 转办
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flowHandleModel"></param>
        /// <returns></returns>
        [NonAction]
        public async Task Transfer(string id, FlowHandleModel flowHandleModel)
        {
            try
            {
                DbScoped.SugarScope.BeginTran();
                var flowOperatorEntity = await _flowTaskRepository.GetTaskOperatorInfo(id);
                if (flowOperatorEntity == null)
                    throw JNPFException.Oh(ErrorCode.COM1005);
                flowOperatorEntity.HandleId = flowHandleModel.freeApproverUserId;
                var isOk = await _flowTaskRepository.UpdateTaskOperator(flowOperatorEntity);
                if (isOk < 1)
                    throw JNPFException.Oh(ErrorCode.WF0007);

                #region 流转记录
                var flowTaskOperatorRecordEntity = new FlowTaskOperatorRecordEntity();
                flowTaskOperatorRecordEntity.HandleOpinion = flowHandleModel.handleOpinion;
                flowTaskOperatorRecordEntity.HandleId = _userManager.UserId;
                flowTaskOperatorRecordEntity.HandleTime = DateTime.Now;
                flowTaskOperatorRecordEntity.HandleStatus = 7;
                flowTaskOperatorRecordEntity.NodeName = flowOperatorEntity.NodeName;
                flowTaskOperatorRecordEntity.TaskId = flowOperatorEntity.TaskId;
                flowTaskOperatorRecordEntity.Status = 0;
                flowTaskOperatorRecordEntity.OperatorId = flowHandleModel.freeApproverUserId;
                await _flowTaskRepository.CreateTaskOperatorRecord(flowTaskOperatorRecordEntity);
                #endregion
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.Sugar.RollbackTran();
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// 审批事前操作
        /// </summary>
        /// <param name="flowEngineEntity"></param>
        /// <param name="flowTaskEntity"></param>
        /// <param name="flowHandleModel"></param>
        /// <returns></returns>
        [NonAction]
        public async Task ApproveBefore(FlowEngineEntity flowEngineEntity, FlowTaskEntity flowTaskEntity, FlowHandleModel flowHandleModel)
        {
            try
            {
                if (flowEngineEntity.FormType == 2)
                {
                    var data = (flowHandleModel.formData.Serialize().Deserialize<JObject>())["data"].ToString().Deserialize<JObject>();
                    var devData = (flowHandleModel.formData.Serialize().Deserialize<JObject>())["data"].ToString();
                    var devEntity = await _visualDevServce.GetInfoById(flowEngineEntity.Id);
                    var upInput = new VisualDevModelDataUpInput() { id = flowTaskEntity.Id, data = devData, status = 1 };
                    await Save(flowTaskEntity.Id, flowTaskEntity.FlowId, flowTaskEntity.ProcessId, flowTaskEntity.FullName, flowTaskEntity.FlowUrgent, flowTaskEntity.EnCode, data, 1, 1, false, "0", null, devEntity.IsNotEmptyOrNull());
                    if (devEntity.IsNotEmptyOrNull())
                    {
                        await _runService.Update(flowTaskEntity.Id, devEntity, upInput);
                    }
                }
                else
                {
                    flowTaskEntity.FlowFormContentJson = flowHandleModel.formData.ToJson();
                    await _flowTaskRepository.UpdateTask(flowTaskEntity);
                    GetSysTableFromService(flowHandleModel.enCode, flowHandleModel.formData, flowTaskEntity.Id, 0);
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region PrivateMethod

        #region 流程模板解析
        /// <summary>
        /// 递归获取流程模板数组
        /// </summary>
        /// <param name="template">流程模板</param>
        /// <param name="templateList">流程模板数组</param>
        private void GetFlowTemplateList(FlowTemplateJsonModel template, List<FlowTemplateJsonModel> templateList)
        {
            if (template.IsNotEmptyOrNull())
            {
                var haschildNode = template.childNode.IsNotEmptyOrNull();
                var hasconditionNodes = template.conditionNodes.IsNotEmptyOrNull() && template.conditionNodes.Count > 0;

                templateList.Add(template);

                if (hasconditionNodes)
                {
                    foreach (var conditionNode in template.conditionNodes)
                    {
                        GetFlowTemplateList(conditionNode, templateList);
                    }
                }
                if (haschildNode)
                {
                    GetFlowTemplateList(template.childNode, templateList);
                }
            }
        }

        /// <summary>
        /// 递归获取流程模板最外层childNode中所有nodeid
        /// </summary>
        /// <param name="template"></param>
        /// <param name="childNodeIdList"></param>
        private void GetChildNodeIdList(FlowTemplateJsonModel template, List<string> childNodeIdList)
        {
            if (template.IsNotEmptyOrNull())
            {
                if (template.childNode.IsNotEmptyOrNull())
                {
                    childNodeIdList.Add(template.childNode.nodeId);
                    GetChildNodeIdList(template.childNode, childNodeIdList);
                }
            }
        }

        /// <summary>
        /// 递归审批模板获取所有节点
        /// </summary>
        /// <param name="template">当前审批流程json</param>
        /// <param name="nodeList">流程节点数组</param>
        /// <param name="templateList">流程模板数组</param>
        private void GetFlowTemplateAll(FlowTemplateJsonModel template, List<TaskNodeModel> nodeList, List<FlowTemplateJsonModel> templateList, List<string> childNodeIdList)
        {
            if (template.IsNotEmptyOrNull())
            {
                var taskNodeModel = template.Adapt<TaskNodeModel>();
                taskNodeModel.propertyJson = GetPropertyByType(template.type, template.properties);
                var haschildNode = template.childNode.IsNotEmptyOrNull();
                var hasconditionNodes = template.conditionNodes.IsNotEmptyOrNull() && template.conditionNodes.Count > 0;
                List<string> nextNodeIdList = new List<string> { "" };
                if (templateList.Count > 1)
                {
                    nextNodeIdList = GetNextNodeIdList(templateList, template, childNodeIdList);
                }
                taskNodeModel.nextNodeId = string.Join(',', nextNodeIdList.ToArray());
                nodeList.Add(taskNodeModel);

                if (hasconditionNodes)
                {
                    foreach (var conditionNode in template.conditionNodes)
                    {
                        GetFlowTemplateAll(conditionNode, nodeList, templateList, childNodeIdList);
                    }
                }
                if (haschildNode)
                {
                    GetFlowTemplateAll(template.childNode, nodeList, templateList, childNodeIdList);
                }
            }
        }

        /// <summary>
        /// 根据类型获取不同属性对象
        /// </summary>
        /// <param name="type">属性类型</param>
        /// <param name="jobj">数据</param>
        /// <returns></returns>
        private dynamic GetPropertyByType(string type, JObject jobj)
        {
            switch (type)
            {
                case "approver":
                    return jobj.ToObject<ApproversProperties>();
                case "timer":
                    return jobj.ToObject<TimerProperties>();
                case "start":
                    return jobj.ToObject<StartProperties>();
                case "condition":
                    return jobj.ToObject<ConditionProperties>();
                case "subFlow":
                    return jobj.ToObject<ChildTaskProperties>();
                default:
                    return jobj;
            }
        }

        /// <summary>
        /// 获取当前模板的下一节点
        /// 下一节点数据来源：conditionNodes和childnode (conditionNodes优先级大于childnode)
        /// conditionNodes非空：下一节点则为conditionNodes数组中所有nodeID
        /// conditionNodes非空childNode非空：下一节点则为childNode的nodeId
        /// conditionNodes空childNode空则为最终节点(两种情况：当前模板属于conditionNodes的最终节点或childNode的最终节点)
        /// conditionNodes的最终节点:下一节点为与conditionNodes同级的childNode的nodeid,没有则继续递归，直到最外层的childNode
        /// childNode的最终节点直接为""
        /// </summary>
        /// <param name="templateList">模板数组</param>
        /// <param name="template">当前模板</param>
        /// <param name="childNodeIdList">最外层childnode的nodeid集合</param>
        /// <returns></returns>
        private List<string> GetNextNodeIdList(List<FlowTemplateJsonModel> templateList, FlowTemplateJsonModel template, List<string> childNodeIdList)
        {
            List<string> nextNodeIdList = new List<string>();
            if (template.conditionNodes.IsNotEmptyOrNull() && template.conditionNodes.Count > 0)
            {
                nextNodeIdList = template.conditionNodes.Select(x => x.nodeId).ToList();
            }
            else
            {
                if (template.childNode.IsNotEmptyOrNull())
                {
                    nextNodeIdList.Add(template.childNode.nodeId);
                }
                else
                {
                    //判断是否是最外层的节点
                    if (childNodeIdList.Contains(template.nodeId))
                    {
                        nextNodeIdList.Add("");
                    }
                    else
                    {
                        //conditionNodes中最终节点
                        nextNodeIdList.Add(GetChildId(templateList, template, childNodeIdList));
                    }
                }
            }
            return nextNodeIdList;
        }

        /// <summary>
        /// 递归获取conditionNodes最终节点下一节点
        /// </summary>
        /// <param name="templateList">流程模板数组</param>
        /// <param name="template">当前模板</param>
        /// <param name="childNodeIdList">最外层childNode的节点数据</param>
        /// <returns></returns>
        private string GetChildId(List<FlowTemplateJsonModel> templateList, FlowTemplateJsonModel template, List<string> childNodeIdList)
        {
            var prevModel = new FlowTemplateJsonModel();
            if (template.prevId.IsNotEmptyOrNull())
            {
                prevModel = templateList.Find(x => x.nodeId.Equals(template.prevId));
                if (prevModel.childNode.IsNotEmptyOrNull() && prevModel.childNode.nodeId != template.nodeId)
                {
                    return prevModel.childNode.nodeId;
                }
                if (childNodeIdList.Contains(prevModel.nodeId))
                {
                    return prevModel.childNode.IsNullOrEmpty() ? "" : prevModel.childNode.nodeId;
                }
                else
                {
                    return GetChildId(templateList, prevModel, childNodeIdList);
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 删除条件节点
        /// 将条件的上非条件的节点的nextnode替换成当前条件的nextnode
        /// </summary>
        /// <param name="taskNodeModelList">所有节点数据</param>
        /// <param name="formDataJson">填写表单数据</param>
        /// <param name="jnpfKey"></param>
        /// <param name="keyList"></param>
        /// <returns></returns>
        private void DeleteConditionTaskNodeModel(List<TaskNodeModel> taskNodeModelList, string formDataJson, Dictionary<string, string> jnpfKey, Dictionary<string, object> keyList)
        {
            var conditionTaskNodeModelList = taskNodeModelList.FindAll(x => x.type.Equals("condition"));
            //条件的默认情况判断（同层条件的父节点是一样的，只要非默认的匹配成功则不需要走默认的）
            var isDefault = new List<string>();
            foreach (var item in conditionTaskNodeModelList)
            {
                //条件节点的父节点且为非条件的节点
                var upTaskNodeModel = taskNodeModelList.Where(x => x.nodeId == item.upNodeId).FirstOrDefault();
                if (upTaskNodeModel.type.Equals("condition"))
                {
                    upTaskNodeModel = GetUpTaskNodeModelIsNotCondition(taskNodeModelList, upTaskNodeModel);
                }
                if (!item.propertyJson.isDefault && ConditionNodeJudge(formDataJson, item.propertyJson, jnpfKey, keyList))
                {
                    upTaskNodeModel.nextNodeId = item.nextNodeId;
                    isDefault.Add(item.upNodeId);
                }
                else
                {
                    if (!isDefault.Contains(item.upNodeId) && item.propertyJson.isDefault)
                    {
                        upTaskNodeModel.nextNodeId = item.nextNodeId;
                    }
                }

            }
            taskNodeModelList.RemoveAll(x => "condition".Equals(x.type));
        }

        /// <summary>
        /// 向上递获取非条件的节点
        /// </summary>
        /// <param name="taskNodeModelList">所有节点数据</param>
        /// <param name="taskNodeModel">当前节点</param>
        /// <returns></returns>
        private TaskNodeModel GetUpTaskNodeModelIsNotCondition(List<TaskNodeModel> taskNodeModelList, TaskNodeModel taskNodeModel)
        {
            var preTaskNodeModel = taskNodeModelList.Find(x => x.nodeId == taskNodeModel.upNodeId);
            if (preTaskNodeModel.type.Equals("condition"))
            {
                return GetUpTaskNodeModelIsNotCondition(taskNodeModelList, preTaskNodeModel);
            }
            else
            {
                return preTaskNodeModel;
            }
        }

        /// <summary>
        /// 条件判断
        /// </summary>
        /// <param name="formDataJson">表单填写数据</param>
        /// <param name="conditionPropertie">条件属性</param>
        /// <param name="jnpfKey"></param>
        /// <param name="keyList"></param>
        /// <returns></returns>
        private bool ConditionNodeJudge(string formDataJson, ConditionProperties conditionPropertie, Dictionary<string, string> jnpfKey, Dictionary<string, object> keyList)
        {
            try
            {
                bool flag = false;
                StringBuilder expression = new StringBuilder();
                var formData = formDataJson.Deserialize<JObject>();
                int i = 0;
                foreach (ConditionsModel flowNodeWhereModel in conditionPropertie.conditions)
                {
                    var logic = flowNodeWhereModel.logic;
                    var formValue = formData.ContainsKey(flowNodeWhereModel.field) ? formData[flowNodeWhereModel.field].ToString() : "";
                    var symbol = flowNodeWhereModel.symbol.Equals("==") ? "=" : flowNodeWhereModel.symbol;
                    var value = flowNodeWhereModel.filedValue;
                    var jnpfkey = jnpfKey.ContainsKey(flowNodeWhereModel.field) ? jnpfKey[flowNodeWhereModel.field].ToString() : null;
                    if (jnpfKey.IsNotEmptyOrNull())
                    {
                        var condFiledValue = value.Trim();
                        //下拉和单选
                        if ("select".Equals(jnpfkey) || "radio".Equals(jnpfkey))
                        {
                            List<Dictionary<string, string>> dataList = keyList[flowNodeWhereModel.field].Serialize().Deserialize<List<Dictionary<string, string>>>();
                            Dictionary<string, string> data = dataList.Find(x => x["id"].ToString().Equals(condFiledValue) || x["fullName"].ToString().Equals(condFiledValue));
                            if (data != null)
                            {
                                value = data["id"].ToString();
                            }
                        }
                        //公司和部门
                        if ("comSelect".Equals(jnpfkey) || "depSelect".Equals(jnpfkey))
                        {
                            List<OrganizeEntity> dataList = keyList[flowNodeWhereModel.field].Serialize().Deserialize<List<OrganizeEntity>>();
                            OrganizeEntity organize = dataList.Find(x => x.Id.Equals(condFiledValue) || x.FullName.Equals(condFiledValue));
                            if (organize != null)
                            {
                                value = organize.Id;
                            }
                        }
                        //岗位
                        if ("posSelect".Equals(jnpfkey))
                        {
                            List<PositionEntity> dataList = keyList[flowNodeWhereModel.field].Serialize().Deserialize<List<PositionEntity>>();
                            PositionEntity position = dataList.Find(x => x.Id.Equals(condFiledValue) || x.FullName.Equals(condFiledValue));
                            if (position != null)
                            {
                                value = position.Id;
                            }
                        }
                        //字典
                        if ("dicSelect".Equals(jnpfkey))
                        {
                            List<DictionaryTypeEntity> dictypeList = keyList[flowNodeWhereModel.field].Serialize().Deserialize<List<DictionaryTypeEntity>>();
                            DictionaryTypeEntity dic = dictypeList.Find(x => x.Id.Equals(formValue));
                            if (dic != null)
                            {
                                value = dic.Id;
                            }
                        }
                        //用户
                        if ("userSelect".Equals(jnpfkey))
                        {
                            List<UserEntity> dataList = keyList[flowNodeWhereModel.field].Serialize().Deserialize<List<UserEntity>>();
                            UserEntity user = dataList.Find(x => x.Id.Equals(condFiledValue) || x.RealName.Equals(condFiledValue) || x.Account.Equals(condFiledValue));
                            if (user != null)
                            {
                                value = user.Id;
                            }
                        }
                    }
                    expression.Append(string.Format("{0}{1}{2}", formValue, symbol, value));
                    if (!string.IsNullOrEmpty(logic))
                    {
                        if (i != conditionPropertie.conditions.Count - 1)
                        {
                            expression.Append(" " + logic.Replace("&&", " and ").Replace("||", " or ") + " ");
                        }
                    }
                    i++;
                }
                flag = (bool)new DataTable().Compute(expression.ToString(), "");
                return flag;
            }
            catch (Exception e)
            {
                return false;
                throw new Exception(e.Message);

            }
        }

        /// <summary>
        /// 删除定时器和空节点
        /// </summary>
        /// <param name="flowTaskNodeEntityList"></param>
        private void DeleteEmptyOrTimerTaskNode(List<FlowTaskNodeEntity> flowTaskNodeEntityList)
        {
            var emptyTaskNodeList = flowTaskNodeEntityList.FindAll(x => "empty".Equals(x.NodeType));
            var timerTaskNodeList = flowTaskNodeEntityList.FindAll(x => "timer".Equals(x.NodeType));
            foreach (var item in emptyTaskNodeList)
            {
                //下-节点为empty类型节点的节点集合
                var taskNodeList = flowTaskNodeEntityList.FindAll(x => x.NodeNext.Contains(item.NodeCode));
                //替换节点
                foreach (var taskNode in taskNodeList)
                {
                    var flowTaskNodeEntity = flowTaskNodeEntityList.Where(x => x.NodeCode == taskNode.NodeCode).FirstOrDefault();
                    flowTaskNodeEntity.NodeNext = item.NodeNext;
                }
            }
            foreach (var item in timerTaskNodeList)
            {
                //下一节点为Timer类型节点的节点集合
                var taskNodeList = flowTaskNodeEntityList.FindAll(x => x.NodeNext.Contains(item.NodeCode));
                //Timer类型节点的下节点集合
                var nextTaskNodeList = flowTaskNodeEntityList.FindAll(x => item.NodeNext.Contains(x.NodeCode));
                //保存定时器节点的上节点编码到属性中
                var timerProperties = item.NodePropertyJson.Deserialize<TimerProperties>();
                timerProperties.upNodeCode = string.Join(",", taskNodeList.Select(x => x.NodeCode).ToArray());
                item.NodePropertyJson = timerProperties.Serialize();
                //上节点替换NodeNext
                foreach (var taskNode in taskNodeList)
                {
                    var flowTaskNodeEntity = flowTaskNodeEntityList.Where(x => x.NodeCode == taskNode.NodeCode).FirstOrDefault();
                    flowTaskNodeEntity.NodeNext = item.NodeNext;
                }
                //下节点添加定时器属性
                nextTaskNodeList.ForEach(nextNode =>
                {
                    var flowTaskNodeEntity = flowTaskNodeEntityList.Where(x => x.NodeCode == nextNode.NodeCode).FirstOrDefault();
                    if (flowTaskNodeEntity.NodeType.Equals("approver"))
                    {
                        var properties = flowTaskNodeEntity.NodePropertyJson.Deserialize<ApproversProperties>();
                        properties.timerList.Add(item.NodePropertyJson.Deserialize<TimerProperties>());
                        flowTaskNodeEntity.NodePropertyJson = properties.Serialize();
                    }
                });
            }
            flowTaskNodeEntityList.RemoveAll(x => "empty".Equals(x.NodeType));
            flowTaskNodeEntityList.RemoveAll(x => "timer".Equals(x.NodeType));
        }

        /// <summary>
        /// 封装属性key和保存list
        /// </summary>
        /// <param name="fieLdsModelList">控件数据集合</param>
        /// <param name="jnpfKey"></param>
        /// <param name="keyList"></param>
        private async Task tempJson(List<FieldsModel> fieLdsModelList, Dictionary<string, string> jnpfKey, Dictionary<string, object> keyList)
        {
            List<DictionaryDataEntity> dictionaryDataList = await _dictionaryDataService.GetList();
            foreach (var fieLdsModel in fieLdsModelList)
            {
                string model = fieLdsModel.__vModel__;
                ConfigModel config = fieLdsModel.__config__;
                string key = config.jnpfKey;
                if (model.IsNotEmptyOrNull())
                {
                    jnpfKey.Add(model, key);
                }
                if ("select".Equals(key) || "checkbox".Equals(key) || "radio".Equals(key))
                {
                    string type = config.dataType;
                    List<Dictionary<string, string>> optionslList = new List<Dictionary<string, string>>();
                    string fullName = config.props.label;
                    string value = config.props.value;
                    if ("dictionary".Equals(type))
                    {
                        string dictionaryType = config.dictionaryType;
                        List<DictionaryDataEntity> dicList = dictionaryDataList.FindAll(x => x.DictionaryTypeId.Equals(dictionaryType));
                        foreach (DictionaryDataEntity dataEntity in dicList)
                        {
                            Dictionary<string, string> optionsModel = new Dictionary<string, string>();
                            optionsModel.Add("id", dataEntity.Id);
                            optionsModel.Add("fullName", dataEntity.FullName);
                            optionslList.Add(optionsModel);
                        }
                    }
                    else if ("static".Equals(type))
                    {
                        List<Dictionary<string, object>> staticList = fieLdsModel.__slot__.options;
                        foreach (Dictionary<string, object> options in staticList)
                        {
                            Dictionary<string, string> optionsModel = new Dictionary<string, string>();
                            optionsModel.Add("id", options["id"].ToString());
                            optionsModel.Add("fullName", options["fullName"].ToString());
                            optionslList.Add(optionsModel);
                        }
                    }
                    else if ("dynamic".Equals(type))
                    {
                        string dynId = config.propsUrl;
                        //查询外部接口
                        Dictionary<string, object> dynamicMap = new Dictionary<string, object>();
                        dynamicMap.Add("data", await _dataInterfaceService.GetData(dynId));
                        if (dynamicMap["data"] != null)
                        {
                            List<Dictionary<string, object>> dataList = dynamicMap["data"].Serialize().Deserialize<List<Dictionary<string, object>>>();
                            foreach (Dictionary<string, object> options in dataList)
                            {
                                Dictionary<string, string> optionsModel = new Dictionary<string, string>();
                                optionsModel.Add("id", options[value].ToString());
                                optionsModel.Add("fullName", options[fullName].ToString());
                                optionslList.Add(optionsModel);
                            }
                        }
                    }
                    keyList.Add(model, optionslList);
                }
            }
        }
        #endregion

        #region 审批人员
        /// <summary>
        /// 根据类型获取审批人
        /// </summary>
        /// <param name="flowTaskOperatorEntityList">审批人集合</param>
        /// <param name="flowTaskNodeEntitieList">所有节点</param>
        /// <param name="nextFlowTaskNodeEntity">下个审批节点数据</param>
        /// <param name="flowTaskNodeEntity">当前审批节点数据</param>
        /// <param name="creatorUserId">发起人</param>
        /// <param name="fromData">表单数据</param>
        /// <param name="type">操作标识（0：提交，1：审批）</param>
        private async Task AddFlowTaskOperatorEntityByAssigneeType(List<FlowTaskOperatorEntity> flowTaskOperatorEntityList, List<FlowTaskNodeEntity> flowTaskNodeEntitieList, FlowTaskNodeEntity flowTaskNodeEntity, FlowTaskNodeEntity nextFlowTaskNodeEntity, string creatorUserId, string fromData, int type = 1)
        {
            try
            {
                if (!nextFlowTaskNodeEntity.NodeType.Equals("subFlow"))
                {
                    var approverPropertiers = nextFlowTaskNodeEntity.NodePropertyJson.Deserialize<ApproversProperties>();
                    FlowTaskOperatorEntity flowTaskOperatorEntity = new FlowTaskOperatorEntity();
                    flowTaskOperatorEntity.Id = YitIdHelper.NextId().ToString();
                    flowTaskOperatorEntity.HandleType = approverPropertiers.assigneeType.ToString();
                    flowTaskOperatorEntity.NodeCode = nextFlowTaskNodeEntity.NodeCode;
                    flowTaskOperatorEntity.NodeName = nextFlowTaskNodeEntity.NodeName;
                    flowTaskOperatorEntity.TaskNodeId = nextFlowTaskNodeEntity.Id;
                    flowTaskOperatorEntity.TaskId = nextFlowTaskNodeEntity.TaskId;
                    flowTaskOperatorEntity.CreatorTime = DateTime.Now;
                    flowTaskOperatorEntity.Completion = 0;
                    flowTaskOperatorEntity.State = "0";
                    flowTaskOperatorEntity.Description = GetTimerDate(approverPropertiers, flowTaskNodeEntity.NodeCode);
                    flowTaskOperatorEntity.Type = approverPropertiers.assigneeType.ToString();
                    //创建人信息
                    var creatorUser = _usersService.GetInfoByUserId(creatorUserId);
                    var userList = (await _usersService.GetList()).Select(x => x.Id).ToList();
                    if (nextFlowTaskNodeEntity.NodeCode != "end")
                    {
                        switch (approverPropertiers.assigneeType)
                        {
                            //发起者主管
                            case (int)FlowTaskOperatorEnum.LaunchCharge:
                                flowTaskOperatorEntity.HandleId = type == 0 ? await GetManagerByLevel(_userManager.User.ManagerId, (int)approverPropertiers.managerLevel) : await GetManagerByLevel(creatorUser.ManagerId, (int)approverPropertiers.managerLevel);
                                if (flowTaskOperatorEntity.HandleId.IsNullOrEmpty())
                                {
                                    flowTaskOperatorEntity.HandleId = "admin";
                                }
                                flowTaskOperatorEntityList.Add(flowTaskOperatorEntity);
                                break;
                            //部门主管
                            case (int)FlowTaskOperatorEnum.DepartmentCharge:
                                var organizeEntity = type == 0 ? await _organizeService.GetInfoById(_userManager.User.OrganizeId) : await _organizeService.GetInfoById(creatorUser.OrganizeId);
                                if (organizeEntity.ManagerId.IsNullOrEmpty())
                                {
                                    organizeEntity.ManagerId = "admin";
                                }
                                flowTaskOperatorEntity.HandleId = organizeEntity.ManagerId;
                                flowTaskOperatorEntityList.Add(flowTaskOperatorEntity);
                                break;
                            //发起者本人
                            case (int)FlowTaskOperatorEnum.InitiatorMe:
                                flowTaskOperatorEntity.HandleId = creatorUserId;
                                flowTaskOperatorEntityList.Add(flowTaskOperatorEntity);
                                break;
                            //之前某个节点的审批人(提交时下个节点是环节就跳过，审批则看环节节点是否是当前节点的上级)
                            case (int)FlowTaskOperatorEnum.LinkApprover:
                                if (type == 1 && !IsUpNode(approverPropertiers.nodeId, flowTaskNodeEntitieList, (long)nextFlowTaskNodeEntity.SortCode))
                                {
                                    //环节节点所有经办人(过滤掉加签人)
                                    var handleIds = (await _flowTaskRepository.GetTaskOperatorRecordList(flowTaskNodeEntity.TaskId))
                                        .FindAll(x => x.NodeCode.IsNotEmptyOrNull() && x.NodeCode.Equals(approverPropertiers.nodeId)
                                        && x.HandleStatus == 1 && 0 == x.Status).Where(x => HasFreeApprover(x.TaskOperatorId).Result)
                                        .Select(x => x.HandleId).Distinct().ToList();
                                    foreach (var item in handleIds)
                                    {
                                        var entity = flowTaskOperatorEntity.Adapt<FlowTaskOperatorEntity>();
                                        entity.Id = YitIdHelper.NextId().ToString();
                                        entity.HandleId = item;
                                        entity.HandleType = "8";
                                        entity.Type = "8";
                                        flowTaskOperatorEntityList.Add(entity);
                                    }
                                }
                                else
                                {
                                    var entity = flowTaskOperatorEntity.Adapt<FlowTaskOperatorEntity>();
                                    entity.HandleId = "admin";
                                    entity.HandleType = "8";
                                    entity.Type = "8";
                                    flowTaskOperatorEntityList.Add(entity);
                                }
                                break;
                            //表单值审批人
                            case (int)FlowTaskOperatorEnum.VariableApprover:
                                var jObj = fromData.Deserialize<JObject>();
                                var field = jObj.ContainsKey(approverPropertiers.formField) ? jObj[approverPropertiers.formField].ToString() : "admin";
                                var filedList = field.Split(",").ToList();
                                filedList = userList.Intersect(filedList).ToList();
                                if (filedList.Count == 0)
                                {
                                    flowTaskOperatorEntity.HandleId = "admin";
                                    flowTaskOperatorEntityList.Add(flowTaskOperatorEntity);
                                }
                                else
                                {
                                    foreach (var item in filedList)
                                    {
                                        var entity = flowTaskOperatorEntity.Adapt<FlowTaskOperatorEntity>();
                                        entity.Id = YitIdHelper.NextId().ToString();
                                        entity.HandleId = item;
                                        flowTaskOperatorEntityList.Add(entity);
                                    }
                                }
                                break;
                            //接口审批人(接口结构为{"code":200,"data":{"handleId":"admin"},"msg":""})
                            case (int)FlowTaskOperatorEnum.ServiceApprover:
                                var data = await approverPropertiers.getUserUrl.SetHeaders(new { Authorization = _userManager.ToKen }).GetAsStringAsync();
                                var result = JSON.Deserialize<RESTfulResult<object>>(data);
                                if (result.IsNotEmptyOrNull())
                                {
                                    var resultJobj = result.data.ToObeject<JObject>();
                                    if (result.code == 200)
                                    {
                                        var handleId = resultJobj["handleId"].IsNotEmptyOrNull() ? resultJobj["handleId"].ToString() : "admin";
                                        var handleIdList = userList.Intersect(handleId.Split(",").ToList()).ToList();
                                        if (handleIdList.Count == 0)
                                        {
                                            var entity = flowTaskOperatorEntity.Adapt<FlowTaskOperatorEntity>();
                                            entity.Id = YitIdHelper.NextId().ToString();
                                            entity.HandleId = "admin";
                                            flowTaskOperatorEntityList.Add(entity);
                                        }
                                        else
                                        {
                                            foreach (var item in handleIdList)
                                            {
                                                var entity = flowTaskOperatorEntity.Adapt<FlowTaskOperatorEntity>();
                                                entity.Id = YitIdHelper.NextId().ToString();
                                                entity.HandleId = item;
                                                flowTaskOperatorEntityList.Add(entity);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var entity = flowTaskOperatorEntity.Adapt<FlowTaskOperatorEntity>();
                                    entity.Id = YitIdHelper.NextId().ToString();
                                    flowTaskOperatorEntity.HandleId = "admin";
                                    flowTaskOperatorEntityList.Add(flowTaskOperatorEntity);
                                }
                                break;
                            default:
                                GetAppointApprover(flowTaskOperatorEntityList, flowTaskOperatorEntity, approverPropertiers);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw JNPFException.Oh(ErrorCode.WF0013);
            }
        }

        /// <summary>
        /// 判断经办记录人是否加签且加签是否完成
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<bool> HasFreeApprover(string id)
        {
            var entityList = await GetOperator(id, new List<FlowTaskOperatorEntity>());
            if (entityList.Count == 0)
            {
                return true;
            }
            else
            {
                return entityList.FindAll(x => x.HandleStatus.IsEmpty() || x.HandleStatus == 0).Count() == 0;
            }
        }

        /// <summary>
        /// 指定用户或岗位(审批人)
        /// </summary>
        /// <param name="flowTaskOperatorEntityList">当前节点所有经办</param>
        /// <param name="flowTaskOperatorEntity">当前经办</param>
        /// <param name="approverPropertiers">当前节点属性</param>
        private void GetAppointApprover(List<FlowTaskOperatorEntity> flowTaskOperatorEntityList, FlowTaskOperatorEntity flowTaskOperatorEntity, ApproversProperties approverPropertiers)
        {
            var approverUserList = new List<string>();
            var userList = GetUserDefined(approverPropertiers.approvers, approverPropertiers.approverRole, approverPropertiers.approverPos);
            approverUserList = approverUserList.Union(userList).ToList();
            if (approverUserList.Count == 0)
            {
                var entity = flowTaskOperatorEntity.Adapt<FlowTaskOperatorEntity>();
                entity.Id = YitIdHelper.NextId().ToString();
                entity.HandleId = "admin";
                flowTaskOperatorEntityList.Add(entity);
            }
            else
            {
                foreach (var item in approverUserList.Distinct())
                {
                    var entity = flowTaskOperatorEntity.Adapt<FlowTaskOperatorEntity>();
                    entity.Id = YitIdHelper.NextId().ToString();
                    entity.HandleId = item;
                    flowTaskOperatorEntityList.Add(entity);
                }
            }
        }

        /// <summary>
        /// 获取抄送人
        /// </summary>
        /// <param name="approverPropertiers">节点属性</param>
        /// <param name="flowTaskOperatorEntity">经办</param>
        /// <param name="flowTaskCirculateEntityList">抄送list</param>
        /// <param name="copyIds">自定义抄送</param>
        /// <param name="hanlderState">审批状态</param>
        private void GetflowTaskCirculateEntityList(ApproversProperties approverPropertiers, FlowTaskOperatorEntity flowTaskOperatorEntity, List<FlowTaskCirculateEntity> flowTaskCirculateEntityList, string copyIds, int hanlderState = 1)
        {
            var circulateUserList = copyIds.Split(",").ToList();
            #region 抄送人
            if (hanlderState == 1)
            {
                var userList = GetUserDefined(approverPropertiers.circulateUser, approverPropertiers.circulateRole, approverPropertiers.circulatePosition);
                circulateUserList = circulateUserList.Union(userList).ToList();
            }

            foreach (var item in circulateUserList.Distinct())
            {
                flowTaskCirculateEntityList.Add(new FlowTaskCirculateEntity()
                {
                    Id = YitIdHelper.NextId().ToString(),
                    ObjectType = flowTaskOperatorEntity.HandleType,
                    ObjectId = item,
                    NodeCode = flowTaskOperatorEntity.NodeCode,
                    NodeName = flowTaskOperatorEntity.NodeName,
                    TaskNodeId = flowTaskOperatorEntity.TaskNodeId,
                    TaskId = flowTaskOperatorEntity.TaskId,
                    CreatorTime = DateTime.Now,
                });
            }
            #endregion
        }

        /// <summary>
        /// 获取自定义人员
        /// </summary>
        /// <param name="userList">指定人</param>
        /// <param name="roleList">指定角色</param>
        /// <param name="posList">指定岗位</param>
        /// <returns></returns>
        private List<string> GetUserDefined(List<string> userList, List<string> roleList, List<string> posList)
        {
            if (posList.IsNotEmptyOrNull() && posList.Count > 0)
            {
                foreach (var item in posList)
                {
                    var userPosition = _userRelationService.GetUserId("Position", item);
                    userList = userList.Union(userPosition).ToList();
                }
            }
            if (roleList.IsNotEmptyOrNull() && roleList.Count > 0)
            {
                foreach (var item in roleList)
                {
                    var userRole = _userRelationService.GetUserId("Role", item);
                    userList = userList.Union(userRole).ToList();
                }
            }
            return userList;
        }

        /// <summary>
        /// 获取自定义人员名称
        /// </summary>
        /// <param name="userList"></param>
        /// <param name="roleList"></param>
        /// <param name="posList"></param>
        /// <param name="userNameList"></param>
        /// <returns></returns>
        private async Task GetUserNameDefined(List<string> userList, List<string> roleList, List<string> posList, List<string> userNameList)
        {
            if (userList.IsNotEmptyOrNull() && userList.Count > 0)
            {
                foreach (var item in userList)
                {
                    var approversUser = await _usersService.GetUserName(item);
                    if (approversUser.IsNotEmptyOrNull() && !userNameList.Contains(approversUser))
                    {
                        userNameList.Add(approversUser);
                    }
                }
            }
            if (posList.IsNotEmptyOrNull() && posList.Count > 0)
            {
                foreach (var item in posList)
                {
                    var userPosition = _userRelationService.GetUserId("Position", item);
                    foreach (var item1 in userPosition)
                    {
                        userNameList.Add(await _usersService.GetUserName(item1));
                    }
                }
            }
            if (roleList.IsNotEmptyOrNull() && roleList.Count > 0)
            {
                foreach (var item in roleList)
                {
                    var userRole = _userRelationService.GetUserId("Role", item);
                    foreach (var item1 in userRole)
                    {
                        userNameList.Add(await _usersService.GetUserName(item1));
                    }
                }
            }
        }

        /// <summary>
        /// 获取子流程发起人
        /// </summary>
        /// <param name="childTaskProperties">子流程属性</param>
        /// <param name="creatorUser">主流程发起人</param>
        /// <returns></returns>
        private async Task<List<string>> GetSubFlowCreator(ChildTaskProperties childTaskProperties, string creatorUser)
        {
            var crUserList = new List<string>();
            var userEntity = _usersService.GetInfoByUserId(creatorUser);
            switch (childTaskProperties.initiateType)
            {
                case 1:
                    var userList = GetUserDefined(childTaskProperties.initiator, childTaskProperties.initiateRole, childTaskProperties.initiatePos);
                    crUserList = crUserList.Union(userList).ToList();
                    break;
                case 2:
                    var organizeEntity = await _organizeService.GetInfoById(userEntity.OrganizeId);
                    if (organizeEntity.ManagerId.IsNullOrEmpty())
                    {
                        organizeEntity.ManagerId = "admin";
                    }
                    crUserList.Add(organizeEntity.ManagerId);
                    break;
                case 3:
                    var crDirector = await GetManagerByLevel(userEntity.ManagerId, childTaskProperties.managerLevel);
                    if (crDirector.IsNullOrEmpty())
                    {
                        crDirector = "admin";
                    }
                    crUserList.Add(crDirector);
                    break;
                case 4:
                    crUserList.Add(creatorUser);
                    break;
                default:
                    break;
            }
            if (crUserList.Count == 0)
            {
                crUserList.Add("admin");
            }
            return crUserList.Distinct().ToList();
        }

        /// <summary>
        /// 获取级别主管
        /// </summary>
        /// <param name="managerId">主管id</param>
        /// <param name="level">级别</param>
        /// <returns></returns>
        private async Task<string> GetManagerByLevel(string managerId, int level)
        {
            --level;
            if (level == 0)
            {
                return managerId;
            }
            else
            {
                var manager = (await _usersService.GetList()).Find(x => x.Id.Equals(managerId));
                return manager.IsNullOrEmpty() ? "" : await GetManagerByLevel(manager.ManagerId, level);
            }
        }

        /// <summary>
        /// 获取审批人名称
        /// </summary>
        /// <param name="flowTaskNodeModel">当前节点</param>
        /// <param name="flowTaskEntity">任务</param>
        /// <param name="formData">表单数据</param>
        /// <param name="flowTaskNodeEntities">所有节点</param>
        /// <returns></returns>
        private async Task<string> GetApproverUserName(FlowTaskNodeModel flowTaskNodeModel, FlowTaskEntity flowTaskEntity, string formData, List<FlowTaskNodeEntity> flowTaskNodeEntities)
        {
            var userNameList = new List<string>();
            var creatorUser = (await _usersService.GetList()).Find(x => x.Id == flowTaskEntity.CreatorUserId);
            if (flowTaskNodeModel.nodeType.Equals("start"))
            {
                var userName = await _usersService.GetUserName(creatorUser.Id);
                userNameList.Add(userName);
            }
            else if (flowTaskNodeModel.nodeType.Equals("subFlow"))
            {
                var subFlowProperties = flowTaskNodeModel.nodePropertyJson.Deserialize<ChildTaskProperties>();
                var userIdList = await GetSubFlowCreator(subFlowProperties, creatorUser.Id);
                await GetUserNameDefined(userIdList, null, null, userNameList);
            }
            else
            {
                var approverProperties = flowTaskNodeModel.nodePropertyJson.Deserialize<ApproversProperties>();
                switch (approverProperties.assigneeType)
                {
                    //发起者主管
                    case (int)FlowTaskOperatorEnum.LaunchCharge:
                        var managerId = await GetManagerByLevel(creatorUser.ManagerId, (int)approverProperties.managerLevel);
                        var manager = (await _usersService.GetList()).Find(x => x.Id == managerId);
                        if (manager != null)
                        {
                            userNameList.Add(await _usersService.GetUserName(manager.Id));
                        }
                        break;
                    //部门主管
                    case (int)FlowTaskOperatorEnum.DepartmentCharge:
                        var organize = await _organizeService.GetInfoById(creatorUser.OrganizeId);
                        if (organize != null)
                        {
                            userNameList.Add(await _usersService.GetUserName(organize.ManagerId));
                        }
                        break;
                    //发起者本人
                    case (int)FlowTaskOperatorEnum.InitiatorMe:
                        var userName = await _usersService.GetUserName(creatorUser.Id);
                        userNameList.Add(userName);
                        break;
                    //环节
                    case (int)FlowTaskOperatorEnum.LinkApprover:
                        if (!IsUpNode(approverProperties.nodeId, flowTaskNodeEntities, (long)flowTaskNodeModel.sortCode))
                        {
                            //环节节点所有经办人
                            var handleIds = (await _flowTaskRepository.GetTaskOperatorRecordList(flowTaskNodeModel.taskId)).
                                FindAll(x => x.NodeCode.IsNotEmptyOrNull() && x.NodeCode.Equals(approverProperties.nodeId)
                                && x.HandleStatus == 1 && x.Status == 0).Where(x => HasFreeApprover(x.TaskOperatorId).Result).Select(x => x.HandleId).Distinct().ToList();
                            foreach (var item in handleIds)
                            {
                                var linkUserName = await _usersService.GetUserName(item);
                                userNameList.Add(linkUserName);
                            }
                        }
                        break;
                    //变量
                    case (int)FlowTaskOperatorEnum.VariableApprover:
                        var jObj = formData.Deserialize<JObject>();
                        var fieldList = jObj[approverProperties.formField].ToString().Split(",");
                        foreach (var item in fieldList)
                        {
                            var variableUserName = await _usersService.GetUserName(item);
                            if (variableUserName.IsEmpty())
                            {
                                userNameList.Add(await _usersService.GetUserName("admin"));
                            }
                            else
                            {
                                userNameList.Add(variableUserName);
                            }
                        }
                        break;
                    //服务
                    case (int)FlowTaskOperatorEnum.ServiceApprover:
                        try
                        {
                            var data = await approverProperties.getUserUrl.SetHeaders(new { Authorization = _userManager.ToKen }).GetAsStringAsync();
                            var result = JSON.Deserialize<RESTfulResult<object>>(data);
                            if (result.IsNotEmptyOrNull())
                            {
                                var resultJobj = result.data.ToObeject<JObject>();
                                if (result.code == 200)
                                {
                                    var handleId = resultJobj["handleId"].IsNotEmptyOrNull() ? resultJobj["handleId"].ToString() : "";
                                    foreach (var item in handleId.Split(","))
                                    {
                                        var serviceUserName = await _usersService.GetUserName(item);
                                        if (serviceUserName.IsNotEmptyOrNull())
                                        {
                                            userNameList.Add(serviceUserName);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {

                            break;
                        }
                        break;
                    default:
                        await GetUserNameDefined(approverProperties.approvers, approverProperties.approverRole, approverProperties.approverPos, userNameList);
                        break;
                }
                if (userNameList.Count == 0 && approverProperties.assigneeType != 5)
                {
                    userNameList.Add(await _usersService.GetUserName("admin"));
                }
            }
            return string.Join(",", userNameList.Distinct());
        }

        /// <summary>
        /// 获取定时器节点定时结束时间
        /// </summary>
        /// <param name="approverPropertiers">定时器节点属性</param>
        /// <param name="nodeCode">定时器节点编码</param>
        /// <returns></returns>
        private string GetTimerDate(ApproversProperties approverPropertiers, string nodeCode)
        {
            var nowTime = DateTime.Now;
            if (approverPropertiers.timerList.Count > 0)
            {
                string upNodeStr = string.Join(",", approverPropertiers.timerList.Select(x => x.upNodeCode).ToArray());
                if (upNodeStr.Contains(nodeCode))
                {
                    foreach (var item in approverPropertiers.timerList)
                    {
                        var result = DateTime.Now.AddDays(item.day).AddHours(item.hour).AddMinutes(item.minute).AddSeconds(item.second);
                        if (result > nowTime)
                        {
                            nowTime = result;
                        }
                    }
                    return nowTime.ToString();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 判断是否为上级节点
        /// </summary>
        /// <param name="nodeId">上级节点编码</param>
        /// <param name="flowTaskNodeEntitieList">所有节点</param>
        /// <param name="sortCode">当前节点排序码</param>
        /// <returns></returns>
        private bool IsUpNode(string nodeId, List<FlowTaskNodeEntity> flowTaskNodeEntitieList, long sortCode)
        {
            var upflowTaskNodeEntity = flowTaskNodeEntitieList.Find(x => x.NodeCode.Equals(nodeId) && x.SortCode < sortCode);
            return upflowTaskNodeEntity.IsNullOrEmpty();
        }
        #endregion

        #region 节点处理
        /// <summary>
        /// 判断分流节点是否完成
        /// (因为分流节点最终只能是一个 所以只需判断下一节点中的其中一个的上节点完成情况)
        /// </summary>
        /// <param name="flowTaskNodeEntityList">所有节点</param>
        /// <param name="nextNodeCode">下一个节点编码</param>
        /// <param name="flowTaskNodeEntity">当前节点</param>
        /// <returns></returns>
        private bool IsShuntNodeCompletion(List<FlowTaskNodeEntity> flowTaskNodeEntityList, string nextNodeCode, FlowTaskNodeEntity flowTaskNodeEntity)
        {
            var shuntNodeCodeList = flowTaskNodeEntityList.FindAll(x => x.NodeNext.IsNotEmptyOrNull() &&
            x.NodeCode != flowTaskNodeEntity.NodeCode && x.NodeNext.Contains(nextNodeCode) && x.Completion == 0);
            return shuntNodeCodeList.Count == 0;
        }

        /// <summary>
        /// 替换任务当前节点
        /// </summary>
        /// <param name="flowTaskNodeEntityList">所有节点</param>
        /// <param name="nextNodeCodeList">替换数据</param>
        /// <param name="thisStepId">源数据</param>
        /// <returns></returns>
        private string GetThisStepId(List<FlowTaskNodeEntity> flowTaskNodeEntityList, List<string> nextNodeCodeList, string thisStepId)
        {
            var replaceNodeCodeList = new List<string>();
            nextNodeCodeList.ForEach(item =>
            {
                var nodeCode = new List<string>();
                var nodeEntityList = flowTaskNodeEntityList.FindAll(x => x.NodeNext.IsNotEmptyOrNull() && x.NodeNext.Contains(item));
                nodeCode = nodeEntityList.Select(x => x.NodeCode).ToList();
                replaceNodeCodeList = replaceNodeCodeList.Union(nodeCode).ToList();
            });
            var thisNodeList = thisStepId.Split(",").ToList();
            //去除当前审批节点并添加下个节点
            var list = thisNodeList.Except(replaceNodeCodeList).Union(nextNodeCodeList);
            return string.Join(",", list.ToArray());
        }

        /// <summary>
        /// 根据当前节点编码获取节点名称
        /// </summary>
        /// <param name="flowTaskNodeEntityList"></param>
        /// <param name="thisStepId"></param>
        /// <returns></returns>
        private string GetThisStep(List<FlowTaskNodeEntity> flowTaskNodeEntityList, string thisStepId)
        {
            var ids = thisStepId.Split(",").ToList();
            var nextNodeNameList = new List<string>();
            foreach (var item in ids)
            {
                var name = flowTaskNodeEntityList.Find(x => x.NodeCode.Equals(item)).NodeName;
                nextNodeNameList.Add(name);
            }
            return string.Join(",", nextNodeNameList);
        }

        /// <summary>
        /// 获取驳回节点
        /// </summary>
        /// <param name="flowTaskNodeEntityList">所有节点</param>
        /// <param name="flowTaskNodeEntity">当前节点</param>
        /// <param name="approversProperties">当前节点属性</param>
        /// <returns></returns>
        private List<FlowTaskNodeEntity> GetRejectFlowTaskOperatorEntity(List<FlowTaskNodeEntity> flowTaskNodeEntityList, FlowTaskNodeEntity flowTaskNodeEntity, ApproversProperties approversProperties)
        {
            //驳回节点
            var upflowTaskNodeEntityList = new List<FlowTaskNodeEntity>();
            if (flowTaskNodeEntity.NodeUp == "1")
            {
                upflowTaskNodeEntityList = flowTaskNodeEntityList.FindAll(x => x.NodeNext.IsNotEmptyOrNull() && x.NodeNext.Contains(flowTaskNodeEntity.NodeCode));
            }
            else
            {
                var upflowTaskNodeEntity = flowTaskNodeEntityList.Find(x => x.NodeCode == approversProperties.rejectStep);
                upflowTaskNodeEntityList = flowTaskNodeEntityList.FindAll(x => x.SortCode == upflowTaskNodeEntity.SortCode);
            }
            return upflowTaskNodeEntityList;
        }

        /// <summary>
        /// 修改节点完成状态
        /// </summary>
        /// <param name="taskNodeList"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private async Task RejectUpdateTaskNode(List<FlowTaskNodeEntity> taskNodeList, int state)
        {
            foreach (var item in taskNodeList)
            {
                item.Completion = state;
                await _flowTaskRepository.UpdateTaskNode(item);
            }
        }

        /// <summary>
        /// 作废节点
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private async Task InvalidNode(List<FlowTaskNodeEntity> nodeList, string state)
        {
            foreach (var item in nodeList)
            {
                item.State = state;
                await _flowTaskRepository.UpdateTaskNode(item);
            }
        }

        /// <summary>
        /// 处理并保存节点
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        private async Task CreateNode(List<FlowTaskNodeEntity> entitys)
        {
            var startNodes = entitys.FindAll(x => x.NodeType.Equals("start"));
            if (startNodes.Count > 0)
            {
                var startNode = startNodes[0].NodeCode;
                long num = 0L;
                long maxNum = 0L;
                var max = new List<long>();
                var _treeList = new List<FlowTaskNodeEntity>();
                NodeList(entitys, startNode, _treeList, num, max);
                max.Sort();
                if (max.Count > 0)
                {
                    maxNum = max[max.Count - 1];
                }
                var nodeNext = "end";
                foreach (var item in entitys)
                {
                    var type = item.NodeType;
                    var node = _treeList.Find(x => x.NodeCode.Equals(item.NodeCode));
                    if (item.NodeNext.IsEmpty())
                    {
                        item.NodeNext = nodeNext;
                    }
                    if (node.IsNotEmptyOrNull())
                    {
                        item.SortCode = node.SortCode;
                        item.State = "0";
                        if (item.NodeNext.IsEmpty())
                        {
                            item.NodeNext = nodeNext;
                        }
                    }
                }
                entitys.Add(new FlowTaskNodeEntity()
                {
                    Id = YitIdHelper.NextId().ToString(),
                    NodeCode = nodeNext,
                    NodeName = "结束",
                    Completion = 0,
                    CreatorTime = DateTime.Now,
                    SortCode = maxNum + 1,
                    TaskId = _treeList[0].TaskId,
                    NodePropertyJson = startNodes[0].NodePropertyJson,
                    NodeType = "endround",
                    State = "0"
                });
                await _flowTaskRepository.CreateTaskNode(entitys);
            }
        }

        /// <summary>
        /// 递归获取经过的节点
        /// </summary>
        /// <param name="dataAll"></param>
        /// <param name="nodeCode"></param>
        /// <param name="_treeList"></param>
        /// <param name="num"></param>
        /// <param name="max"></param>
        private void NodeList(List<FlowTaskNodeEntity> dataAll, string nodeCode, List<FlowTaskNodeEntity> _treeList, long num, List<long> max)
        {
            num++;
            max.Add(num);
            var thisEntity = dataAll.FindAll(x => x.NodeCode.Contains(nodeCode));
            foreach (var item in thisEntity)
            {
                item.SortCode = num;
                item.State = "0";
                _treeList.Add(item);
                foreach (var nodeNext in item.NodeNext.Split(","))
                {
                    long nums = _treeList.FindAll(x => x.NodeCode.Equals(nodeNext)).Count;
                    if (nodeNext.IsNotEmptyOrNull() && nums == 0)
                    {
                        NodeList(dataAll, nodeNext, _treeList, num, max);
                    }
                }
            }
        }

        /// <summary>
        /// 判断驳回节点是否存在子流程
        /// </summary>
        /// <param name="flowTaskOperatorEntity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> IsSubFlowUpNode(FlowTaskOperatorEntity flowTaskOperatorEntity)
        {
            var nodeList = await _flowTaskRepository.GetTaskNodeList(flowTaskOperatorEntity.TaskId);
            var nodeInfo = await _flowTaskRepository.GetTaskNodeInfo(flowTaskOperatorEntity.TaskNodeId);
            if (nodeInfo.NodeUp=="0")
            {
                return false;
            }
            else
            {
                var rejectNodeList = GetRejectFlowTaskOperatorEntity(nodeList, nodeInfo, nodeInfo.NodePropertyJson.Deserialize<ApproversProperties>());
                return rejectNodeList.Any(x => x.NodeType.Equals("subFlow"));
            }
        }
        #endregion

        #region 经办处理
        /// <summary>
        /// 根据不同节点类型修改经办数据
        /// </summary>
        /// <param name="flowTaskOperatorEntity">当前经办</param>
        /// <param name="thisFlowTaskOperatorEntityList">当前节点所有经办</param>
        /// <param name="aspproversProperties">当前节点属性</param>
        /// <param name="handleStatus">审批类型（0：拒绝，1：同意）</param>
        /// <param name="freeApprover">加签人</param>
        /// <returns></returns>
        private async Task UpdateFlowTaskOperator(FlowTaskOperatorEntity flowTaskOperatorEntity, List<FlowTaskOperatorEntity> thisFlowTaskOperatorEntityList, ApproversProperties aspproversProperties, int handleStatus, string freeApprover)
        {
            //当前用户委托人id
            List<string> delegateUserIdList = await _flowDelegateService.GetDelegateUserId(_userManager.UserId);
            if (aspproversProperties.counterSign == 0)
            {
                if (freeApprover.IsNullOrEmpty())
                {
                    //未审批经办
                    var notCompletion = GetNotCompletion(thisFlowTaskOperatorEntityList);
                    await _flowTaskRepository.UpdateTaskOperator(notCompletion);
                }
            }
            else
            {
                var deleIndex = (await GetDelegateOperator(flowTaskOperatorEntity.TaskId, flowTaskOperatorEntity.TaskNodeId, delegateUserIdList, handleStatus)).Count;
                if (IsAchievebilProportion(thisFlowTaskOperatorEntityList, handleStatus, (int)aspproversProperties.countersignRatio, deleIndex, freeApprover.IsEmpty()))
                {
                    //未审批经办
                    var notCompletion = GetNotCompletion(thisFlowTaskOperatorEntityList);
                    await _flowTaskRepository.UpdateTaskOperator(notCompletion);
                }
            }
            await UpdateThisOperator(flowTaskOperatorEntity, handleStatus, delegateUserIdList);
        }

        /// <summary>
        /// 根据当前审批节点插入下一节点经办
        /// </summary>
        /// <param name="flowTaskNodeEntityList">所有节点</param>
        /// <param name="flowTaskNodeEntity">当前节点</param>
        /// <param name="approversProperties">当前节点属性</param>
        /// <param name="thisFlowTaskOperatorEntityList">当前节点所有经办</param>
        /// <param name="handleStatus">审批状态</param>
        /// <param name="flowTaskEntity">流程任务</param>
        /// <param name="freeApproverUserId">加签人</param>
        /// <param name="nextFlowTaskOperatorEntityList">经办数据</param>
        /// <param name="fromData">表单数据</param>
        /// <returns></returns>
        private async Task CreateNextFlowTaskOperator(List<FlowTaskNodeEntity> flowTaskNodeEntityList, FlowTaskNodeEntity flowTaskNodeEntity,
            ApproversProperties approversProperties, List<FlowTaskOperatorEntity> thisFlowTaskOperatorEntityList, int handleStatus, FlowTaskEntity flowTaskEntity, string freeApproverUserId,
            List<FlowTaskOperatorEntity> nextFlowTaskOperatorEntityList, string fromData, FlowHandleModel flowHandleModel, int formType)
        {
            try
            {
                var nextNodeCodeList = new List<string>();
                var nextNodeCompletion = new List<int>();
                var isInsert = false;
                //下个节点集合
                List<FlowTaskNodeEntity> nextNodeEntity = flowTaskNodeEntityList.FindAll(m => flowTaskNodeEntity.NodeNext.Contains(m.NodeCode));
                //当前用户委托人id
                List<string> delegateUserIdList = await _flowDelegateService.GetDelegateUserId(_userManager.UserId);
                var deleIndex = (await GetDelegateOperator(flowTaskNodeEntity.TaskId, flowTaskNodeEntity.Id, delegateUserIdList, handleStatus)).Count;
                if (handleStatus == 0)
                {
                    if (approversProperties.counterSign == 0)
                    {
                        await GetNextOperatorByNo(flowTaskNodeEntity, flowTaskEntity, flowTaskNodeEntityList, approversProperties, nextFlowTaskOperatorEntityList, fromData);
                    }
                    else
                    {
                        if (IsAchievebilProportion(thisFlowTaskOperatorEntityList, handleStatus, (int)approversProperties.countersignRatio, deleIndex, freeApproverUserId.IsEmpty()))
                        {
                            await GetNextOperatorByNo(flowTaskNodeEntity, flowTaskEntity, flowTaskNodeEntityList, approversProperties, nextFlowTaskOperatorEntityList, fromData);
                        }
                    }
                }
                else
                {
                    foreach (var item in nextNodeEntity)
                    {
                        if (approversProperties.counterSign == 0)
                        {
                            isInsert = true;
                            await GetNextOperatorByYes(flowTaskNodeEntity, item, nextNodeCodeList, nextNodeCompletion, nextFlowTaskOperatorEntityList, flowTaskNodeEntityList, flowTaskEntity, fromData);
                        }
                        else
                        {
                            if (IsAchievebilProportion(thisFlowTaskOperatorEntityList, handleStatus, (int)approversProperties.countersignRatio, deleIndex, freeApproverUserId.IsEmpty()))
                            {
                                isInsert = true;
                                await GetNextOperatorByYes(flowTaskNodeEntity, item, nextNodeCodeList, nextNodeCompletion, nextFlowTaskOperatorEntityList, flowTaskNodeEntityList, flowTaskEntity, fromData);
                            }
                        }
                    }

                    await InsertNextNodeData(flowTaskNodeEntityList, nextNodeEntity, flowTaskEntity,
                        nextFlowTaskOperatorEntityList, nextNodeCodeList, nextNodeCompletion, flowTaskNodeEntity,
                        fromData, isInsert,flowHandleModel,formType);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据驳回节点修改经办
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private async Task RejectUpdateTaskOperator(string taskId)
        {
            var flowTaskOperatorEntityList = await _flowTaskRepository.GetTaskOperatorList(taskId);
            flowTaskOperatorEntityList.ForEach(x => { x.State = "-1"; });
            await _flowTaskRepository.UpdateTaskOperator(flowTaskOperatorEntityList);
        }

        /// <summary>
        /// 获取委托经办数据
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskNodeId"></param>
        /// <param name="userId"></param>
        /// <param name="handleStatus"></param>
        /// <returns></returns>
        private async Task<List<FlowTaskOperatorEntity>> GetDelegateOperator(string taskId, string taskNodeId, List<string> userId, int handleStatus)
        {
            try
            {
                var entityList = (await _flowTaskRepository.GetTaskOperatorList(taskId)).FindAll(x => x.TaskNodeId == taskNodeId);
                var upEntityList = new List<FlowTaskOperatorEntity>();
                foreach (var item in userId)
                {
                    var upEntity = entityList.Find(x => x.HandleId == item);
                    if (upEntity.IsNotEmptyOrNull())
                    {
                        upEntity.HandleStatus = handleStatus;
                        upEntity.Completion = 1;
                        upEntity.HandleTime = DateTime.Now;
                        upEntityList.Add(upEntity);
                    }
                }
                return upEntityList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 修改当前经办以及所属委托经办
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="handleStatus"></param>
        /// <param name="delegateUserIdList"></param>
        /// <returns></returns>
        private async Task UpdateThisOperator(FlowTaskOperatorEntity entity, int handleStatus, List<string> delegateUserIdList)
        {
            entity.HandleStatus = handleStatus;
            entity.Completion = 1;
            entity.HandleTime = DateTime.Now;
            var upList = await GetDelegateOperator(entity.TaskId, entity.TaskNodeId, delegateUserIdList, handleStatus);
            upList.Add(entity);
            await _flowTaskRepository.UpdateTaskOperator(upList);
        }

        /// <summary>
        /// 获取未审经办并修改完成状态
        /// </summary>
        /// <param name="thisFlowTaskOperatorEntityList"></param>
        /// <returns></returns>
        private List<FlowTaskOperatorEntity> GetNotCompletion(List<FlowTaskOperatorEntity> thisFlowTaskOperatorEntityList)
        {
            var notCompletion = thisFlowTaskOperatorEntityList.FindAll(x => x.Completion == 0);
            notCompletion.ForEach(item =>
            {
                item.Completion = 1;
            });
            return notCompletion;
        }

        /// <summary>
        /// 获取驳回节点经办
        /// </summary>
        /// <param name="flowTaskNodeEntity">当前节点</param>
        /// <param name="flowTaskEntity">当前任务</param>
        /// <param name="flowTaskNodeEntityList">所有节点</param>
        /// <param name="approversProperties">当前节点属性</param>
        /// <param name="nextFlowTaskOperatorEntityList">下个节点存储list</param>
        /// <param name="fromData">表单数据</param>
        /// <returns></returns>
        private async Task GetNextOperatorByNo(FlowTaskNodeEntity flowTaskNodeEntity, FlowTaskEntity flowTaskEntity, List<FlowTaskNodeEntity> flowTaskNodeEntityList, ApproversProperties approversProperties, List<FlowTaskOperatorEntity> nextFlowTaskOperatorEntityList, string fromData)
        {
            if (flowTaskNodeEntity.NodeUp == "0")
            {
                flowTaskEntity.ThisStepId = flowTaskNodeEntityList.Find(x => x.NodeType.Equals("start")).NodeCode;
                flowTaskEntity.ThisStep = "开始";
                flowTaskEntity.Completion = 0;
                flowTaskEntity.Status = FlowTaskStatusEnum.Reject;
            }
            else
            {
                //当上节点为开始节点，当前审批节点NodeUp为0；所以不需做判断
                var upflowTaskNodeEntityList = GetRejectFlowTaskOperatorEntity(flowTaskNodeEntityList, flowTaskNodeEntity, approversProperties);
                if (upflowTaskNodeEntityList.Count == 0)
                {
                    throw new Exception("当前流程不经过该驳回节点!");
                }
                foreach (var item in upflowTaskNodeEntityList)
                {
                    await AddFlowTaskOperatorEntityByAssigneeType(nextFlowTaskOperatorEntityList, flowTaskNodeEntityList, flowTaskNodeEntity, item, flowTaskEntity.CreatorUserId, fromData);
                }
                flowTaskEntity.ThisStep = string.Join(",", upflowTaskNodeEntityList.Select(x => x.NodeName).ToArray());
                flowTaskEntity.ThisStepId = string.Join(",", upflowTaskNodeEntityList.Select(x => x.NodeCode).ToArray());
                flowTaskEntity.Completion = upflowTaskNodeEntityList.Select(x => x.NodePropertyJson.Deserialize<ApproversProperties>().progress.ToInt()).ToList().Min();
                //修改驳回节点完成状态
                var rejectTaskNodeEntityList = flowTaskNodeEntityList.FindAll(x => x.SortCode >= upflowTaskNodeEntityList[0].SortCode);
                await RejectUpdateTaskNode(rejectTaskNodeEntityList, 0);
                //删除驳回节点经办记录
                var rejectNodeIds = rejectTaskNodeEntityList.Select(x => x.Id).ToArray();
                var rejectRecodeList = _flowTaskRepository.GetTaskOperatorRecordList(flowTaskEntity.Id, rejectNodeIds).Select(x => x.Id).ToList();
                await _flowTaskRepository.DeleteTaskOperatorRecord(rejectRecodeList);
            }
        }

        /// <summary>
        /// 获取同意节点经办
        /// </summary>
        /// <param name="flowTaskNodeEntity">当前节点</param>
        /// <param name="nextNode">下个节点</param>
        /// <param name="nextNodeCodeList">下个节点编码list</param>
        /// <param name="nextNodeCompletion">下个节点完成度list</param>
        /// <param name="nextFlowTaskOperatorEntityList">下个节点经办list</param>
        /// <param name="flowTaskNodeEntityList">所有接地那</param>
        /// <param name="flowTaskEntity">当前任务</param>
        /// <param name="fromData">表单数据</param>
        /// <returns></returns>
        private async Task GetNextOperatorByYes(FlowTaskNodeEntity flowTaskNodeEntity, FlowTaskNodeEntity nextNode, List<string> nextNodeCodeList, List<int> nextNodeCompletion, List<FlowTaskOperatorEntity> nextFlowTaskOperatorEntityList, List<FlowTaskNodeEntity> flowTaskNodeEntityList, FlowTaskEntity flowTaskEntity, string fromData)
        {
            flowTaskNodeEntity.Completion = 1;
            nextNodeCodeList.Add(nextNode.NodeCode);
            nextNodeCompletion.Add(nextNode.NodePropertyJson.Deserialize<ApproversProperties>().progress.ToInt());
            await AddFlowTaskOperatorEntityByAssigneeType(nextFlowTaskOperatorEntityList, flowTaskNodeEntityList, flowTaskNodeEntity, nextNode, flowTaskEntity.CreatorUserId, fromData);
        }

        /// <summary>
        /// 递归获取加签人
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flowTaskOperatorEntities"></param>
        /// <returns></returns>
        private async Task<List<FlowTaskOperatorEntity>> GetOperator(string id, List<FlowTaskOperatorEntity> flowTaskOperatorEntities)
        {
            var childEntity = await _flowTaskRepository.GetTaskOperatorInfoByParentId(id);
            if (childEntity.IsNotEmptyOrNull())
            {
                childEntity.State = "-1";
                flowTaskOperatorEntities.Add(childEntity);
                return await GetOperator(childEntity.Id, flowTaskOperatorEntities);
            }
            else
            {
                return flowTaskOperatorEntities;
            }
        }
        #endregion

        #region 子流程处理
        /// <summary>
        /// 创建子流程任务
        /// </summary>
        /// <param name="flowId">子流程</param>
        /// <param name="formData">表单数据</param>
        /// <param name="parentId">子任务父id</param>
        /// <param name="childTaskCrUsers">子任务创建人</param>
        /// <param name="isSysTable">是否系统表单</param>
        private async Task<List<string>> CreateSubProcesses(string flowId, object formData, string parentId, List<string> childTaskCrUsers, bool isSysTable)
        {
            var childFLowEngine = await _flowEngineService.GetInfo(flowId);
            var childTaskId = new List<string>();
            foreach (var item in childTaskCrUsers)
            {
                var prossId = isSysTable ? YitIdHelper.NextId().ToString() : null;
                var childTaskEntity = await this.Save(null, flowId, prossId, null, 0, null, formData, 1, 0, isSysTable, parentId, item);
                childTaskId.Add(childTaskEntity.Id);
                if (isSysTable)
                {
                    GetSysTableFromService(childFLowEngine.EnCode, formData, childTaskEntity.Id, 1);
                }
            }
            return childTaskId;
        }

        /// <summary>
        /// 获取子流程继承父流程的表单数据
        /// </summary>
        /// <param name="childTaskProperties"></param>
        /// <param name="formData"></param>
        /// <param name="isSysTable"></param>
        /// <returns></returns>
        private async Task<object> GetSubFlowFormData(ChildTaskProperties childTaskProperties, string formData, bool isSysTable)
        {
            var childFlowEngine = await _flowEngineService.GetInfo(childTaskProperties.flowId);
            //表单模板list
            List<FieldsModel> fieldsModelList = childFlowEngine.FormTemplateJson.Deserialize<FormDataModel>().fields;
            //剔除布局控件
            fieldsModelList = _runService.TemplateDataConversion(fieldsModelList);
            var parentDic = formData.ToObject().ToObject<Dictionary<string, object>>();
            var childDic = new Dictionary<string, object>();
            foreach (var item in fieldsModelList)
            {
                childDic[item.__vModel__] = "";
            }
            foreach (var item in childTaskProperties.assignList)
            {
                childDic[item.childField] = parentDic.ContainsKey(item.parentField) ? parentDic[item.parentField] : null;
            }
            if (isSysTable)
            {
                childDic["flowId"] = childTaskProperties.flowId;
            }
            return childDic;
        }

        /// <summary>
        /// 插入子流程
        /// </summary>
        /// <param name="childFlowTaskEntity">子流程</param>
        /// <returns></returns>
        private async Task InsertSubFlowNextNode(FlowTaskEntity childFlowTaskEntity)
        {
            try
            {
                //所有子流程(不包括当前流程)
                var childFlowTaskAll = (await _flowTaskRepository.GetTaskList()).FindAll(x => x.ParentId == childFlowTaskEntity.ParentId && x.Id != childFlowTaskEntity.Id);
                //已完成的子流程
                var completeChildFlow = childFlowTaskAll.FindAll(x => x.Status == FlowTaskStatusEnum.Adopt);
                //父流程
                var parentFlowTask = await _flowTaskRepository.GetTaskInfo(childFlowTaskEntity.ParentId);
                if (childFlowTaskAll.Count == completeChildFlow.Count)
                {
                    var parentSubFlowNode = (await _flowTaskRepository.GetTaskNodeList(parentFlowTask.Id)).Find(x => x.NodeType.Equals("subFlow") && x.Completion == 0 && x.NodePropertyJson.Deserialize<ChildTaskProperties>().childTaskId.Contains(childFlowTaskEntity.Id));
                    var subFlowOperator = parentSubFlowNode.Adapt<FlowTaskOperatorEntity>();
                    subFlowOperator.Id = null;
                    subFlowOperator.TaskNodeId = parentSubFlowNode.Id;
                    var formType = (await _flowEngineService.GetInfo(parentFlowTask.FlowId)).FormType;
                    var childData = parentSubFlowNode.NodePropertyJson.ToObject<ChildTaskProperties>().formData;
                    var handleModel = new FlowHandleModel();
                    if (formType == 2)
                    {
                        var dic = new Dictionary<string, object>();
                        dic.Add("data", childData);
                        handleModel.formData = dic;
                    }
                    else
                    {
                        handleModel.formData = childData.ToObject();
                    }
                    await this.Audit(parentFlowTask, subFlowOperator, handleModel, (int)formType);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        /// <summary>
        /// 处理填写数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private async Task<FlowTaskInfoOutput> GetFlowDynamicDataManage(FlowTaskEntity entity)
        {
            try
            {
                var flowEngineEntity = await _flowEngineService.GetInfo(entity.FlowId);
                var flowEngineTablesModelList = flowEngineEntity.Tables.Deserialize<List<FlowEngineTablesModel>>();
                FlowTaskInfoOutput output = entity.Adapt<FlowTaskInfoOutput>();
                var visualDevEntity = flowEngineEntity.Adapt<VisualDevEntity>();
                visualDevEntity.FormData = flowEngineEntity.FormTemplateJson;
                if (flowEngineTablesModelList.Count > 0)
                {
                    var outData = await _runService.GetHaveTableInfo(entity.Id, visualDevEntity);
                    output.data = outData;
                }
                else
                {
                    //真实表单数据
                    Dictionary<string, object> formDataDic = entity.FlowFormContentJson.ToObject<Dictionary<string, object>>();
                    var data = await _runService.GetIsNoTableInfo(visualDevEntity, entity.FlowFormContentJson);
                    output.data = data;
                }
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 添加经办记录
        /// </summary>
        /// <param name="flowTaskOperatorEntity">当前经办</param>
        /// <param name="flowHandleModel">审批数据</param>
        /// <param name="hanldState"></param>
        /// <returns></returns>
        private async Task CreateOperatorRecode(FlowTaskOperatorEntity flowTaskOperatorEntity, FlowHandleModel flowHandleModel, int hanldState)
        {
            FlowTaskOperatorRecordEntity flowTaskOperatorRecordEntity = new FlowTaskOperatorRecordEntity();
            flowTaskOperatorRecordEntity.HandleOpinion = flowHandleModel.handleOpinion;
            flowTaskOperatorRecordEntity.HandleId = _userManager.UserId;
            flowTaskOperatorRecordEntity.HandleTime = DateTime.Now;
            flowTaskOperatorRecordEntity.HandleStatus = hanldState;
            flowTaskOperatorRecordEntity.NodeCode = flowTaskOperatorEntity.NodeCode;
            flowTaskOperatorRecordEntity.NodeName = flowTaskOperatorEntity.NodeName;
            flowTaskOperatorRecordEntity.TaskOperatorId = flowTaskOperatorEntity.Id;
            flowTaskOperatorRecordEntity.TaskNodeId = flowTaskOperatorEntity.TaskNodeId;
            flowTaskOperatorRecordEntity.TaskId = flowTaskOperatorEntity.TaskId;
            flowTaskOperatorRecordEntity.SignImg = flowHandleModel.signImg;
            flowTaskOperatorRecordEntity.Status = flowTaskOperatorEntity.HandleType == "7" ? 1 : 0;
            await _flowTaskRepository.CreateTaskOperatorRecord(flowTaskOperatorRecordEntity);
        }

        /// <summary>
        /// 验证有效状态
        /// </summary>
        /// <param name="status">状态编码</param>
        /// <returns></returns>
        private bool CheckStatus(int? status)
        {
            if (status == FlowTaskStatusEnum.Draft || status == FlowTaskStatusEnum.Reject || status == FlowTaskStatusEnum.Revoke)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断会签人数是否达到会签比例
        /// </summary>
        /// <param name="thisFlowTaskOperatorEntityList">当前节点所有审批人(已剔除加签人)</param>
        /// <param name="handStatus">审批状态</param>
        /// <param name="index">比例</param>
        /// <param name="delCount">委托人数</param>
        /// <param name="hasFreeApprover">是否有加签(true：没有，flase：有)</param>
        /// <returns></returns>
        private bool IsAchievebilProportion(List<FlowTaskOperatorEntity> thisFlowTaskOperatorEntityList, int handStatus, int index, int delCount, bool hasFreeApprover)
        {
            if (handStatus == 0)
                index = 100 - index;
            //完成人数（加上当前审批人）
            var comIndex = thisFlowTaskOperatorEntityList.FindAll(x => x.HandleStatus == handStatus && x.Completion == 1 && x.State == "0").Count.ToDouble() + delCount;
            if (hasFreeApprover)
            {
                comIndex = comIndex + 1;
            }
            //完成比例
            var comProportion = (comIndex / thisFlowTaskOperatorEntityList.Count.ToDouble() * 100).ToInt();
            return comProportion >= index;
        }

        /// <summary>
        /// 是否插入下个节点数据
        /// </summary>
        /// <param name="allNodeList">所有节点</param>
        /// <param name="nextNodeList">下个节点</param>
        /// <param name="flowTaskEntity">任务</param>
        /// <param name="nextOperatorList">下个节点经办数据</param>
        /// <param name="nextNodeCodeList">下个节点编码</param>
        /// <param name="nextNodeCompletion">下个节点进度</param>
        /// <param name="flowTaskNodeEntity">当前节点</param>
        /// <param name="formData">表单数据</param>
        /// <returns></returns>
        private async Task InsertNextNodeData(List<FlowTaskNodeEntity> allNodeList, List<FlowTaskNodeEntity> nextNodeList,
            FlowTaskEntity flowTaskEntity, List<FlowTaskOperatorEntity> nextOperatorList, List<string> nextNodeCodeList,
            List<int> nextNodeCompletion, FlowTaskNodeEntity flowTaskNodeEntity, string formData, bool isInsert, 
            FlowHandleModel flowHandleModel, int formType)
        {
            //下一节点是分流必定有审批人
            if (nextNodeList.Count > 1)
            {
                flowTaskEntity.ThisStepId = GetThisStepId(allNodeList, nextNodeCodeList, flowTaskEntity.ThisStepId);
                flowTaskEntity.ThisStep = GetThisStep(allNodeList, flowTaskEntity.ThisStepId);
                flowTaskEntity.Completion = nextNodeCompletion.Count == 0 ? flowTaskEntity.Completion : nextNodeCompletion.Min();
                await _flowTaskRepository.CreateTaskOperator(nextOperatorList);
                //await CreateFlowTimeTask(nextNodeList, nextOperatorList, flowTaskEntity, flowHandleModel, formType);
            }
            else
            {
                //判断当前节点在不在分流当中且是否为分流的最后审批节点
                var nextNodeEntity = nextNodeList.FirstOrDefault();
                var isShuntNodeCompletion = IsShuntNodeCompletion(allNodeList, nextNodeEntity.NodeCode, flowTaskNodeEntity);
                if (nextOperatorList.Count > 0)
                {
                    if (isShuntNodeCompletion)
                    {
                        flowTaskEntity.ThisStepId = GetThisStepId(allNodeList, nextNodeCodeList, flowTaskEntity.ThisStepId);
                        flowTaskEntity.ThisStep = GetThisStep(allNodeList, flowTaskEntity.ThisStepId);
                        flowTaskEntity.Completion = nextNodeCompletion.Count == 0 ? flowTaskEntity.Completion : nextNodeCompletion.Min();
                        await _flowTaskRepository.CreateTaskOperator(nextOperatorList);
                        //await CreateFlowTimeTask(nextNodeList, nextOperatorList, flowTaskEntity, flowHandleModel, formType);
                    }
                    else
                    {
                        var thisStepIds = flowTaskEntity.ThisStepId.Split(",").ToList();
                        thisStepIds.Remove(flowTaskNodeEntity.NodeCode);

                        flowTaskEntity.ThisStepId = string.Join(",", thisStepIds.ToArray());
                        flowTaskEntity.ThisStep = GetThisStep(allNodeList, flowTaskEntity.ThisStepId);
                    }
                }
                else
                {
                    //下一节点没有审批人(1.当前会签节点没结束，2.结束节点，3.子流程)
                    if (isShuntNodeCompletion)
                    {
                        var isLastEndNode = allNodeList.FindAll(x =>
                    x.NodeNext.IsNotEmptyOrNull() && x.NodeNext.Equals("end")
                    && !x.NodeCode.Equals(flowTaskNodeEntity.NodeCode) && x.Completion == 0).Count == 0;
                        //下一节点是子流程
                        if (nextNodeEntity.NodeType.Equals("subFlow") && isInsert)
                        {
                            flowTaskEntity.ThisStepId = GetThisStepId(allNodeList, nextNodeCodeList, flowTaskEntity.ThisStepId);
                            flowTaskEntity.ThisStep = GetThisStep(allNodeList, flowTaskEntity.ThisStepId);
                            flowTaskEntity.Completion = nextNodeCompletion.Count == 0 ? flowTaskEntity.Completion : nextNodeCompletion.Min();
                            var childTaskPro = nextNodeEntity.NodePropertyJson.Deserialize<ChildTaskProperties>();
                            var childTaskCrUserList = await GetSubFlowCreator(childTaskPro, flowTaskEntity.CreatorUserId);
                            var childFLowEngine = await _flowEngineService.GetInfo(childTaskPro.flowId);
                            var isSysTable = childFLowEngine.FormType == 1;
                            var childFormData = await GetSubFlowFormData(childTaskPro, formData, isSysTable);
                            childTaskPro.childTaskId = await CreateSubProcesses(childTaskPro.flowId, childFormData, flowTaskEntity.Id, childTaskCrUserList, isSysTable);
                            childTaskPro.formData = formData;
                            nextNodeEntity.NodePropertyJson = childTaskPro.ToJson();
                            //将子流程id保存到主流程的子流程节点属性上
                            await _flowTaskRepository.UpdateTaskNode(nextNodeEntity);
                        }
                        else if (nextNodeEntity.NodeCode.Equals("end") && isLastEndNode)
                        {
                            flowTaskEntity.Status = FlowTaskStatusEnum.Adopt;
                            flowTaskEntity.Completion = 100;
                            flowTaskEntity.EndTime = DateTime.Now;
                            flowTaskEntity.ThisStepId = "end";
                            flowTaskEntity.ThisStep = "结束";
                        }
                        else
                        {
                            flowTaskEntity.ThisStepId = GetThisStepId(allNodeList, nextNodeCodeList, flowTaskEntity.ThisStepId);
                            flowTaskEntity.ThisStep = GetThisStep(allNodeList, flowTaskEntity.ThisStepId);
                            flowTaskEntity.Completion = nextNodeCompletion.Count == 0 ? flowTaskEntity.Completion : nextNodeCompletion.Min();
                        }
                    }
                    else
                    {
                        var thisStepIds = flowTaskEntity.ThisStepId.Split(",").ToList();
                        thisStepIds.Remove(flowTaskNodeEntity.NodeCode);

                        flowTaskEntity.ThisStepId = string.Join(",", thisStepIds.ToArray());
                        flowTaskEntity.ThisStep = GetThisStep(allNodeList, flowTaskEntity.ThisStepId);
                    }
                }
            }
        }

        /// <summary>
        /// 是否为本地接口拼接本地地址
        /// </summary>
        /// <param name="url"></param>
        /// <param name="taskId"></param>
        /// <param name="taskNodeId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private async Task GetApproverUrl(string url, string taskId, string taskNodeId, int state = 0)
        {
            var faceUrl = "";
            if (url.Substring(0, 4).ToLower().Equals("http"))
            {
                faceUrl = string.Format(url + "?taskld={0}&taskNodeId={1}&handleStatus={2}", taskId, taskNodeId, state);
            }
            else
            {
                var host = App.HttpContext.Request.Headers["Host"].ToString();
                faceUrl = string.Format(host + url + "?taskld={0}&taskNodeId={1}&handleStatus={2}", taskId, taskNodeId, state);
            }
            await faceUrl.SetHeaders(new { Authorization = _userManager.ToKen }).GetAsStringAsync();
        }

        /// <summary>
        /// 获取超时间隔时间
        /// </summary>
        /// <param name="timeOutConfig"></param>
        /// <returns></returns>
        private double GetInterval(TimeOutConfig timeOutConfig)
        {
            switch (timeOutConfig.type)
            {
                case "day":
                    return timeOutConfig.quantity * 86400000;
                case "hour":
                    return timeOutConfig.quantity * 3600000;
                case "minute":
                    return timeOutConfig.quantity * 60000;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 工作流定时任务
        /// </summary>
        /// <param name="nextNodeList">下个节点集合</param>
        /// <param name="nextOperatorList">下个节点所有审批人集合</param>
        /// <param name="flowTaskEntity">当前任务</param>
        /// <returns></returns>
        private async Task CreateFlowTimeTask(List<FlowTaskNodeEntity> nextNodeList,List<FlowTaskOperatorEntity> nextOperatorList
            , FlowTaskEntity flowTaskEntity, FlowHandleModel flowHandleModel,int fromType)
        {
            foreach (var item in nextNodeList)
            {
                var nextNodePro = item.NodePropertyJson.Deserialize<ApproversProperties>();
                var qaa = nextOperatorList.FindAll(x => x.TaskNodeId==item.Id);

                foreach (var item1 in nextOperatorList)
                {
                    if (nextNodePro.timeoutConfig.on)
                    {
                        var interval = GetInterval(nextNodePro.timeoutConfig);
                        Action<SpareTimer, long> action = async (timer, count) =>
                        {
                            //每个节点找到他下面审批人
                            if (nextNodePro.timeoutConfig.handler == 1)
                            {
                                await Audit(flowTaskEntity,item1, flowHandleModel, fromType);
                            }
                            else
                            {
                                await Reject(flowTaskEntity, item1, flowHandleModel, fromType);
                            }
                        };
                        if (interval>0)
                        {
                            SpareTime.DoOnce(interval, action, item1.Id, null, true, executeType: SpareTimeExecuteTypes.Parallel);
                        }
                    }
                }
                
            }
        }

        #region 消息推送
        /// <summary>
        /// 消息推送
        /// </summary>
        /// <param name="typeList">推送方式</param>
        /// <param name="titile">标题</param>
        /// <param name="userList">接收用户</param>
        /// <param name="context">内容</param>
        private async Task SendNodeMessage(List<string> typeList, string titile, List<string> userList, string context)
        {
            try
            {
                var sysconfig = await _sysConfigService.GetInfo();
                if (typeList.IsNotEmptyOrNull())
                {
                    foreach (var item in typeList)
                    {
                        if (item.Equals("1"))
                        {
                            await _messageService.SentMessage(userList, titile, context);
                        }
                        if (item.Equals("2"))
                        {
                            EmailSend(titile, userList, context, sysconfig);
                        }
                        if (item.Equals("3"))
                        {
                            SmsSend(titile, userList, context, sysconfig);
                        }
                        if (item.Equals("4"))
                        {
                            var dingIds = await _synThirdInfoService.GetThirdIdList(userList, 2, 3);
                            var dingMsg = new { msgtype = "text", text = new { content = titile } }.ToJson();
                            DingWorkMsgModel dingWorkMsgModel = new DingWorkMsgModel()
                            {
                                toUsers = string.Join(",", dingIds),
                                agentId = sysconfig.dingAgentId,
                                msg = dingMsg
                            };
                            new Ding(sysconfig.dingSynAppKey, sysconfig.dingSynAppSecret).SendWorkMsg(dingWorkMsgModel);
                        }
                        if (item.Equals("5"))
                        {
                            var qyIds = await _synThirdInfoService.GetThirdIdList(userList, 1, 3);
                            new WeChat(sysconfig.qyhCorpId, sysconfig.qyhAgentSecret).SendText(sysconfig.qyhAgentId, titile, string.Join(",", qyIds));
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// 邮箱
        /// </summary>
        /// <param name="titile"></param>
        /// <param name="userList"></param>
        /// <param name="context"></param>
        /// <param name="sysconfig"></param>
        /// <returns></returns>
        private void EmailSend(string titile, List<string> userList, string context, SysConfigOutput sysconfig)
        {
            var emailList = new List<string>();
            foreach (var item in userList)
            {
                var user = _usersService.GetInfoByUserId(item);
                if (user.IsNotEmptyOrNull() && user.Email.IsNotEmptyOrNull())
                {
                    emailList.Add(user.Email);
                }
            }
            var mailModel = new MailModel();
            mailModel.To = string.Join(",", emailList);
            mailModel.Subject = titile;
            mailModel.BodyText = HttpUtility.HtmlDecode(context);
            Mail.Send(
                new MailAccount
                {
                    AccountName = sysconfig.emailSenderName,
                    Account = sysconfig.emailAccount,
                    Password = sysconfig.emailPassword,
                    SMTPHost = sysconfig.emailSmtpHost,
                    SMTPPort = sysconfig.emailSmtpPort.ToInt()
                }, mailModel);
        }

        /// <summary>
        /// 短信
        /// </summary>
        /// <param name="titile"></param>
        /// <param name="userList"></param>
        /// <param name="context"></param>
        private void SmsSend(string titile, List<string> userList, string context, SysConfigOutput sysconfig)
        {
            var telList = new List<string>();
            foreach (var item in userList)
            {
                var user = _usersService.GetInfoByUserId(item);
                if (user.IsNotEmptyOrNull() && user.MobilePhone.IsNotEmptyOrNull())
                {
                    telList.Add("+86" + user.MobilePhone);
                }
            }
            var smsModel = new SmsModel()
            {
                keyId = sysconfig.smsKeyId,
                keySecret = sysconfig.smsKeySecret,
                signName = sysconfig.smsSignName,
                appId = sysconfig.smsAppId,
                templateId = sysconfig.smsTemplateId,
                region = "ap-guangzhou",
                mobileAli = string.Join(",", telList),
                mobileTx = telList.ToArray(),
                templateParamAli = new { title = titile, status = "0" }.ToJson(),
                templateParamTx = new string[] { "12345" }
            };
            if (sysconfig.smsCompany.Equals("2"))
            {
                Sms.SendSmsByTencent(smsModel);
            }
            else
            {
                Sms.SendSmsByAli(smsModel);
            }

        }
        #endregion
        #endregion

        #region 系统表单
        /// <summary>
        /// 系统表单操作
        /// </summary>
        /// <param name="enCode"></param>
        /// <param name="data"></param>
        /// <param name="id"></param>
        /// <param name="type"></param>
        private void GetSysTableFromService(string enCode, object data, string id, int type)
        {
            Scoped.Create((_, scope) =>
            {
                switch (enCode.ToLower())
                {
                    case "applybanquet":
                        var ApplyBanquet = App.GetService<IApplyBanquetService>();
                        ApplyBanquet.Save(id, data, type);
                        break;
                    case "quotationapproval":
                        var QuotationApproval = App.GetService<IQuotationApprovalService>();
                        QuotationApproval.Save(id, data, type);
                        break;
                    case "paydistribution":
                        var PayDistribution = App.GetService<IPayDistributionService>();
                        PayDistribution.Save(id, data, type);
                        break;
                    case "contractapprovalsheet":
                        var ContractApprovalSheet = App.GetService<IContractApprovalSheetService>();
                        ContractApprovalSheet.Save(id, data, type);
                        break;
                    case "salesorder":
                        var SalesOrder = App.GetService<ISalesOrderService>();
                        SalesOrder.Save(id, data, type);
                        break;
                    case "paymentapply":
                        var PaymentApply = App.GetService<IPaymentApplyService>();
                        PaymentApply.Save(id, data, type);
                        break;
                    case "finishedproduct":
                        var FinishedProduct = App.GetService<IFinishedProductService>();
                        FinishedProduct.Save(id, data, type);
                        break;
                    case "archivalborrow":
                        var ArchivalBorrow = App.GetService<IArchivalBorrowService>();
                        ArchivalBorrow.Save(id, data, type);
                        break;
                    case "applydelivergoods":
                        var ApplyDeliverGoods = App.GetService<IApplyDeliverGoodsService>();
                        ApplyDeliverGoods.Save(id, data, type);
                        break;
                    case "applymeeting":
                        var ApplyMeeting = App.GetService<IApplyMeetingService>();
                        ApplyMeeting.Save(id, data, type);
                        break;
                    case "articleswarehous":
                        var ArticlesWarehous = App.GetService<IArticlesWarehousService>();
                        ArticlesWarehous.Save(id, data, type);
                        break;
                    case "batchpack":
                        var BatchPack = App.GetService<IBatchPackService>();
                        BatchPack.Save(id, data, type);
                        break;
                    case "batchtable":
                        var BatchTable = App.GetService<IBatchTableService>();
                        BatchTable.Save(id, data, type);
                        break;
                    case "conbilling":
                        var ConBilling = App.GetService<IConBillingService>();
                        ConBilling.Save(id, data, type);
                        break;
                    case "contractapproval":
                        var ContractApproval = App.GetService<IContractApprovalService>();
                        ContractApproval.Save(id, data, type);
                        break;
                    case "debitbill":
                        var DebitBill = App.GetService<IDebitBillService>();
                        DebitBill.Save(id, data, type);
                        break;
                    case "documentapproval":
                        var DocumentApproval = App.GetService<IDocumentApprovalService>();
                        DocumentApproval.Save(id, data, type);
                        break;
                    case "documentsigning":
                        var DocumentSigning = App.GetService<IDocumentSigningService>();
                        DocumentSigning.Save(id, data, type);
                        break;
                    case "expenseexpenditure":
                        var ExpenseExpenditure = App.GetService<IExpenseExpenditureService>();
                        ExpenseExpenditure.Save(id, data, type);
                        break;
                    case "incomerecognition":
                        var IncomeRecognition = App.GetService<IIncomeRecognitionService>();
                        IncomeRecognition.Save(id, data, type);
                        break;
                    case "leaveapply":
                        var LeaveApply = App.GetService<ILeaveApplyService>();
                        LeaveApply.Save(id, data, type);
                        break;
                    case "letterservice":
                        var LetterService = App.GetService<ILetterServiceService>();
                        LetterService.Save(id, data, type);
                        break;
                    case "materialrequisition":
                        var MaterialRequisition = App.GetService<IMaterialRequisitionService>();
                        MaterialRequisition.Save(id, data, type);
                        break;
                    case "monthlyreport":
                        var MonthlyReport = App.GetService<IMonthlyReportService>();
                        MonthlyReport.Save(id, data, type);
                        break;
                    case "officesupplies":
                        var OfficeSupplies = App.GetService<IOfficeSuppliesService>();
                        OfficeSupplies.Save(id, data, type);
                        break;
                    case "outboundorder":
                        var OutboundOrder = App.GetService<IOutboundOrderService>();
                        OutboundOrder.Save(id, data, type);
                        break;
                    case "outgoingapply":
                        var OutgoingApply = App.GetService<IOutgoingApplyService>();
                        OutgoingApply.Save(id, data, type);
                        break;
                    case "postbatchtab":
                        var PostBatchTab = App.GetService<IPostBatchTabService>();
                        PostBatchTab.Save(id, data, type);
                        break;
                    case "procurementmaterial":
                        var ProcurementMaterial = App.GetService<IProcurementMaterialService>();
                        ProcurementMaterial.Save(id, data, type);
                        break;
                    case "purchaselist":
                        var PurchaseList = App.GetService<IPurchaseListService>();
                        PurchaseList.Save(id, data, type);
                        break;
                    case "receiptprocessing":
                        var ReceiptProcessing = App.GetService<IReceiptProcessingService>();
                        ReceiptProcessing.Save(id, data, type);
                        break;
                    case "receiptsign":
                        var ReceiptSign = App.GetService<IReceiptSignService>();
                        ReceiptSign.Save(id, data, type);
                        break;
                    case "rewardpunishment":
                        var RewardPunishment = App.GetService<IRewardPunishmentService>();
                        RewardPunishment.Save(id, data, type);
                        break;
                    case "salessupport":
                        var SalesSupport = App.GetService<ISalesSupportService>();
                        SalesSupport.Save(id, data, type);
                        break;
                    case "staffovertime":
                        var StaffOvertime = App.GetService<IStaffOvertimeService>();
                        StaffOvertime.Save(id, data, type);
                        break;
                    case "supplementcard":
                        var SupplementCard = App.GetService<ISupplementCardService>();
                        SupplementCard.Save(id, data, type);
                        break;
                    case "travelapply":
                        var TravelApply = App.GetService<ITravelApplyService>();
                        TravelApply.Save(id, data, type);
                        break;
                    case "travelreimbursement":
                        var TravelReimbursement = App.GetService<ITravelReimbursementService>();
                        TravelReimbursement.Save(id, data, type);
                        break;
                    case "vehicleapply":
                        var VehicleApply = App.GetService<IVehicleApplyService>();
                        VehicleApply.Save(id, data, type);
                        break;
                    case "violationhandling":
                        var ViolationHandling = App.GetService<IViolationHandlingService>();
                        ViolationHandling.Save(id, data, type);
                        break;
                    case "warehousereceipt":
                        var WarehouseReceipt = App.GetService<IWarehouseReceiptService>();
                        WarehouseReceipt.Save(id, data, type);
                        break;
                    case "workcontactsheet":
                        var WorkContactSheet = App.GetService<IWorkContactSheetService>();
                        WorkContactSheet.Save(id, data, type);
                        break;
                    default:
                        break;
                }
            });
        }
        #endregion
    }
}
