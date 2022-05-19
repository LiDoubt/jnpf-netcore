using JNPF;
using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Filter;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.JsonSerialization;
using JNPF.LinqBuilder;
using JNPF.RemoteRequest.Extensions;
using JNPF.System.Interfaces.System;
using JNPF.TaskScheduler;
using JNPF.TaskScheduler.Entitys.Dto.TaskScheduler;
using JNPF.TaskScheduler.Entitys.Entity;
using JNPF.TaskScheduler.Entitys.Model;
using JNPF.TaskScheduler.Interfaces.TaskScheduler;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.TaskScheduler.TaskScheduler
{
    /// <summary>
    /// 定时任务
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "TaskScheduler", Name = "scheduletask", Order = 220)]
    [Route("api/[controller]")]
    public class TimeTaskService : ITimeTaskService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<TimeTaskEntity> _timeTaskRepository;
        private readonly ISqlSugarRepository<TimeTaskLogEntity> _timeTaskLogRepository;
        private readonly IDbLinkService _dblikServer;
        private readonly IDataBaseService _dataBaseService;
        private readonly SqlSugarScope db;
        private readonly IUserManager _userManager;
        private readonly ISpareTimeListener _spareTimeListener;

        /// <summary>
        /// 初始化一个<see cref="TimeTaskService"/>类型的新实例
        /// </summary>
        public TimeTaskService(ISqlSugarRepository<TimeTaskEntity> timeTaskRepository,
            ISqlSugarRepository<TimeTaskLogEntity> timeTaskLogRepository,
            IUserManager userManager, IDbLinkService dblikServer, IDataBaseService dataBaseService, ISpareTimeListener spareTimeListener)
        {
            _timeTaskRepository = timeTaskRepository;
            _timeTaskLogRepository = timeTaskLogRepository;
            db = timeTaskRepository.Context;
            _userManager = userManager;
            _dblikServer = dblikServer;
            _dataBaseService = dataBaseService;
            _spareTimeListener = spareTimeListener;
        }

        #region Get
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] PageInputBase input)
        {
            var queryWhere = LinqExpression.And<TimeTaskEntity>().And(x => x.DeleteMark == null);
            if (!string.IsNullOrEmpty(input.keyword))
                queryWhere = queryWhere.And(m => m.FullName.Contains(input.keyword) || m.EnCode.Contains(input.keyword));
            var list = await _timeTaskRepository.Entities.Where(queryWhere).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<TimeTaskListOutput>()
            {
                list = list.list.Adapt<List<TimeTaskListOutput>>(),
                pagination = list.pagination
            };
            return PageResult<TimeTaskListOutput>.SqlSugarPageResult(pageList);
        }

        /// <summary>
        /// 列表（执行记录）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <param name="id">任务Id</param>
        /// <returns></returns>
        [HttpGet("{id}/TaskLog")]
        public async Task<dynamic> GetTaskLogList([FromQuery] TaskLogInput input, string id)
        {
            var whereLambda = LinqExpression.And<TimeTaskLogEntity>().And(x => x.TaskId == id);
            if (input.runResult.IsNotEmptyOrNull())
            {
                whereLambda = whereLambda.And(x => x.RunResult == input.runResult);
            }
            if (!input.startTime.IsNullOrEmpty() && !input.endTime.IsNullOrEmpty())
            {
                var start = Ext.GetDateTime(input.startTime.ToString());
                var end = Ext.GetDateTime(input.endTime.ToString());
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                whereLambda = whereLambda.And(x => SqlFunc.Between(x.RunTime, start, end));
            }
            var list = await _timeTaskLogRepository.Entities.Where(whereLambda).OrderBy(x => x.RunTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<TimeTaskTaskLogListOutput>()
            {
                list = list.list.Adapt<List<TimeTaskTaskLogListOutput>>(),
                pagination = list.pagination
            };
            return PageResult<TimeTaskTaskLogListOutput>.SqlSugarPageResult(pageList);
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpGet("Info/{id}")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            var data = await GetInfo(id);
            var output = data.Adapt<TimeTaskInfoOutput>();
            return output;
        }
        #endregion

        #region Post
        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] TimeTaskCrInput input)
        {
            if (await _timeTaskRepository.AnyAsync(x => x.EnCode == input.enCode && x.DeleteMark == null) || await _timeTaskRepository.AnyAsync(x => x.FullName == input.fullName && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.COM1004);
            var comtentModel = input.executeContent.Deserialize<ContentModel>();
            var entity = input.Adapt<TimeTaskEntity>();
            entity.ExecuteCycleJson = comtentModel.cron;
            var result = await Create(entity);
            _ = result ?? throw JNPFException.Oh(ErrorCode.COM1000);

            // 添加到任务调度里
            AddTimerJob(result);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] TimeTaskUpInput input)
        {
            if (await _timeTaskRepository.AnyAsync(x => x.Id != id && x.EnCode == input.enCode && x.DeleteMark == null) || await _timeTaskRepository.AnyAsync(x => x.Id != id && x.FullName == input.fullName && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.COM1004);
            var entityOld = await GetInfo(id);
            // 先从调度器里取消
            SpareTime.Cancel(entityOld.Id);
            var entityNew = input.Adapt<TimeTaskEntity>();
            var isOk = await Update(entityNew);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1001);
            var comtentModel = entityNew.ExecuteContent.Deserialize<ContentModel>();
            // 再添加到任务调度里
            if (entityOld.EnabledMark == 1)
            {
                AddTimerJob(entityNew);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await GetInfo(id);
            if (entity == null)
                throw JNPFException.Oh(ErrorCode.COM1005);
            var isOk = await Delete(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1002);
            // 从调度器里取消
            SpareTime.Cancel(entity.Id);
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/Stop")]
        public async Task Stop(string id)
        {
            var entity = await GetInfo(id);
            entity.EnabledMark = 0;
            var isOk = await Update(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1003);
            SpareTime.Stop(entity.Id);
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/Enable")]
        public async Task Enable(string id)
        {
            var entity = await GetInfo(id);
            entity.EnabledMark = 1;
            var isOk = await Update(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1003);
            if (SpareTime.AnyTask(id))
            {
                SpareTime.Start(entity.Id);
            }
            else
            {
                AddTimerJob(entity);
            }

        }
        #endregion

        #region PublicMethod

        /// <summary>
        /// 新增定时任务
        /// </summary>
        /// <param name="input"></param>
        [NonAction]
        public void AddTimerJob(TimeTaskEntity input)
        {
            input.ExecuteCycleJson = input.ExecuteContent.Deserialize<ContentModel>().cron;
            Action<SpareTimer, long> action = async (timer, count) =>
            {
                var msg = await PerformJob(input.ExecuteType, input.ExecuteContent.Deserialize<ContentModel>(), input.Id, null);

                var nextRunTime = ((DateTimeOffset)SpareTime.GetCronNextOccurrence(input.ExecuteCycleJson)).DateTime;
                DbScoped.SugarScope.Updateable<TimeTaskEntity>().SetColumns(x => new TimeTaskEntity()
                {
                    RunCount = x.RunCount + 1,
                    LastRunTime = DateTime.Now,
                    NextRunTime = nextRunTime,
                    LastModifyUserId = "admin",
                    LastModifyTime = DateTime.Now
                }
                    ).Where(x => x.Id == input.Id).ExecuteCommand();
                var status = msg.IsNullOrEmpty() ? 2 : 3;
                var spareTimerExecuter = new SpareTimerExecuter(timer, status, msg);
                await _spareTimeListener.OnListener(spareTimerExecuter);
            };
            //创建定时任务
            SpareTime.Do(input.ExecuteCycleJson, action, input.Id, input.Description, true, executeType: SpareTimeExecuteTypes.Parallel);
        }

        /// <summary>
        /// 启动自启动任务
        /// </summary>
        [NonAction]
        public void StartTimerJob()
        {
            var list = _timeTaskRepository.Entities.Where(x => x.DeleteMark == null && x.EnabledMark.Equals(1)).ToList();
            //查询数据库现有开启的定时任务列表
            list.ForEach(AddTimerJob);
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tenantDic"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<TimeTaskEntity> GetInfo(string id, Dictionary<string, string> tenantDic = null)
        {
            return await _timeTaskRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<TimeTaskEntity>> GetList()
        {
            return await _timeTaskRepository.Entities.Where(x => x.DeleteMark == null).ToListAsync();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<TimeTaskEntity> Create(TimeTaskEntity entity)
        {
            return await _timeTaskRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();
        }

        /// <summary>
        /// 新增日志
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tenantDic"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> CreateTaskLog(TimeTaskLogEntity entity, Dictionary<string, string> tenantDic = null)
        {
            return await _timeTaskLogRepository.InsertAsync(entity);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(TimeTaskEntity entity)
        {
            return await _timeTaskRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tenantDic"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(TimeTaskEntity entity, Dictionary<string, string> tenantDic = null)
        {
            return await _timeTaskRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }
        #endregion

        #region PrivateMethod

        /// <summary>
        /// 根据类型执行任务
        /// </summary>
        /// <param name="type"></param>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <param name="tenantDic"></param>
        private async Task<string> PerformJob(string type, ContentModel model, string id, Dictionary<string, string> tenantDic)
        {
            var msg = "";
            if (type == "1")
            {
                msg = await Connector(model, id);
            }
            else
            {
                msg = Storage(model, id, tenantDic);
            }
            return msg;
        }

        /// <summary>
        /// 接口
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        private async Task<string> Connector(ContentModel model, string id)
        {
            try
            {
                var parameters = new Dictionary<string, object>();
                var headersString = App.HttpContext != null && _userManager != null ? string.Format("{\"{0}\":\"{1}\"}", "Authorization", _userManager.ToKen) : null;
                var headers = string.IsNullOrEmpty(headersString)
                                ? null
                                : JSON.Deserialize<Dictionary<string, string>>(headersString);
                foreach (var item in model.storedParameter)
                {
                    parameters.Add(item.Key, item.Value);
                }
                switch (model.interfaceType.ToLower())
                {
                    case "get":
                        await model.interfaceUrl.SetHeaders(headers).SetQueries(parameters).GetAsStringAsync();
                        break;
                    case "post":
                        await model.interfaceUrl.SetHeaders(headers).SetBody(parameters).PostAsStringAsync();
                        break;
                    default:
                        break;
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 存储过程
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <param name="tenantDic"></param>
        public string Storage(ContentModel model, string id, Dictionary<string, string> tenantDic)
        {
            try
            {
                List<SugarParameter> parameters = new List<SugarParameter>();
                foreach (var item in model.storedParameter)
                {
                    SugarParameter sugarParameter = new SugarParameter("@" + item.Key, item.Value);
                    parameters.Add(sugarParameter);
                }
                if (!model.database.Equals("0"))
                {
                    if (!db.IsAnyConnection(model.database))
                    {
                        var link = _dblikServer.GetInfo(model.database).Result;
                        _dataBaseService.AddConnection(link);
                    }
                    var otherDB = db.GetConnection(model.database);
                    otherDB.Ado.UseStoredProcedure().GetDataTable(model.stored, parameters);
                }
                else
                {
                    db.Ado.UseStoredProcedure().GetDataTable(model.stored, parameters);
                }
                return "";

            }
            catch (Exception ex)
            {
                return ex.Message;

            }
        }


        #endregion
    }
}
