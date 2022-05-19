using JNPF.Common.Core.Manager;
using JNPF.Common.Extension;
using JNPF.Common.Filter;
using JNPF.Dependency;
using JNPF.LinqBuilder;
using JNPF.System.Entitys.Permission;
using JNPF.WorkFlow.Entitys;
using JNPF.WorkFlow.Entitys.Dto.FlowBefore;
using JNPF.WorkFlow.Entitys.Dto.FlowLaunch;
using JNPF.WorkFlow.Entitys.Dto.FlowMonitor;
using JNPF.WorkFlow.Interfaces.FlowTask.Repository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace JNPF.WorkFlow.FlowTask.Repository
{
    /// <summary>
    /// 流程任务
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public class FlowTaskRepository : IFlowTaskRepository, ITransient
    {
        private readonly ISqlSugarRepository<FlowTaskEntity> _flowTaskRepository;
        private readonly ISqlSugarRepository<FlowTaskNodeEntity> _flowTaskNodeRepository;
        private readonly ISqlSugarRepository<FlowTaskOperatorEntity> _flowTaskOperatorRepository;
        private readonly ISqlSugarRepository<FlowTaskOperatorRecordEntity> _flowTaskOperatorRecordRepository;
        private readonly ISqlSugarRepository<FlowTaskCirculateEntity> _flowTaskCirculateRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope db;// 核心对象：拥有完整的SqlSugar全部功能

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flowTaskRepository"></param>
        /// <param name="flowTaskNodeRepository"></param>
        /// <param name="flowTaskOperatorRepository"></param>
        /// <param name="flowTaskOperatorRecordRepository"></param>
        /// <param name="flowTaskCirculateRepository"></param>
        /// <param name="userManager"></param>
        public FlowTaskRepository(ISqlSugarRepository<FlowTaskEntity> flowTaskRepository, ISqlSugarRepository<FlowTaskNodeEntity> flowTaskNodeRepository, ISqlSugarRepository<FlowTaskOperatorEntity> flowTaskOperatorRepository, ISqlSugarRepository<FlowTaskOperatorRecordEntity> flowTaskOperatorRecordRepository, ISqlSugarRepository<FlowTaskCirculateEntity> flowTaskCirculateRepository, IUserManager userManager)
        {
            _flowTaskRepository = flowTaskRepository;
            _flowTaskNodeRepository = flowTaskNodeRepository;
            _flowTaskOperatorRepository = flowTaskOperatorRepository;
            _flowTaskOperatorRecordRepository = flowTaskOperatorRecordRepository;
            _flowTaskCirculateRepository = flowTaskCirculateRepository;
            _userManager = userManager;
            db = _flowTaskRepository.Context;
        }

        /// <summary>
        /// 列表（流程监控）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        public async Task<dynamic> GetMonitorList(FlowMonitorListQuery input)
        {
            var whereLambda = LinqExpression.And<FlowMonitorListOutput>();
            if (!input.startTime.IsNullOrEmpty() && !input.endTime.IsNullOrEmpty())
            {
                var start = Ext.GetDateTime(input.startTime.ToString());
                var end = Ext.GetDateTime(input.endTime.ToString());
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                whereLambda = whereLambda.And(x => SqlFunc.Between(x.creatorTime, start, end));
            }
            if (!input.creatorUserId.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.creatorUserId == input.creatorUserId);
            if (!input.flowCategory.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.flowCategory == input.flowCategory);
            if (!input.creatorUserId.IsNullOrEmpty())
                whereLambda = whereLambda.And(m => m.creatorUserId.Contains(input.creatorUserId));
            if (!input.flowId.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.flowId == input.flowId);
            if (!input.keyword.IsNullOrEmpty())
                whereLambda = whereLambda.And(m => m.enCode.Contains(input.keyword) || m.fullName.Contains(input.keyword));
            var list = await db.Queryable<FlowTaskEntity, FlowEngineEntity, UserEntity>((a, b, c) => new JoinQueryInfos(JoinType.Left, a.FlowId == b.Id, JoinType.Left, a.CreatorUserId == c.Id)).Where(a => a.Status > 0 && a.DeleteMark == null).Select((a, b, c) => new FlowMonitorListOutput()
            {
                completion = a.Completion,
                creatorTime = a.CreatorTime,
                creatorUserId = a.CreatorUserId,
                description = a.Description,
                enCode = a.EnCode,
                flowCategory = a.FlowCategory,
                flowCode = a.FlowCode,
                flowId = a.FlowId,
                flowName = b.FullName,
                formUrgent = a.FlowUrgent,
                formData = b.FormTemplateJson,
                formType = b.FormType,
                fullName = a.FullName,
                id = a.Id,
                processId = a.ProcessId,
                startTime = a.StartTime,
                thisStep = a.ThisStep,
                userName = SqlFunc.MergeString(c.RealName, "/", c.Account),
                status = a.Status
            }).MergeTable().Where(whereLambda).OrderBy(a => a.creatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<FlowMonitorListOutput>()
            {
                list = list.list,
                pagination = list.pagination
            };
            return PageResult<FlowMonitorListOutput>.SqlSugarPageResult(pageList);
        }

        /// <summary>
        /// 列表（我发起的）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        public async Task<dynamic> GetLaunchList(FlowLaunchListQuery input)
        {
            var whereLambda = LinqExpression.And<FlowLaunchListOutput>();
            whereLambda = whereLambda.And(x => x.creatorUserId == _userManager.UserId);
            if (!input.startTime.IsNullOrEmpty() && !input.endTime.IsNullOrEmpty())
            {
                var start = Ext.GetDateTime(input.startTime.ToString());
                var end = Ext.GetDateTime(input.endTime.ToString());
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                whereLambda = whereLambda.And(x => SqlFunc.Between(x.creatorTime, start, end));
            }
            if (!input.flowCategory.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.flowCategory == input.flowCategory);
            if (!input.flowId.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.flowId == input.flowId);
            if (!input.keyword.IsNullOrEmpty())
                whereLambda = whereLambda.And(m => m.enCode.Contains(input.keyword) || m.fullName.Contains(input.keyword));

            var list = await db.Queryable<FlowTaskEntity, FlowEngineEntity, UserEntity>((a, b, c) => new JoinQueryInfos(JoinType.Left, a.FlowId == b.Id, JoinType.Left, a.CreatorUserId == c.Id)).Where((a,b) => a.DeleteMark == null&&b.Type!=1).Select((a, b, c) => new FlowLaunchListOutput()
            {
                completion = a.Completion,
                creatorTime = a.CreatorTime,
                creatorUserId = a.CreatorUserId,
                endTime = a.EndTime,
                description = a.Description,
                enCode = a.EnCode,
                flowCategory = a.FlowCategory,
                flowCode = a.FlowCode,
                flowId = a.FlowId,
                flowName = b.FullName,
                formData = b.FormTemplateJson,
                formType = b.FormType,
                fullName = a.FullName,
                id = a.Id,
                startTime = a.StartTime,
                thisStep = a.ThisStep,
                status = a.Status
            }).MergeTable().Where(whereLambda).OrderBy(a => a.status).OrderBy(a => a.startTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<FlowLaunchListOutput>.SqlSugarPageResult(list);
        }

        /// <summary>
        /// 列表（待我审批）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        public async Task<dynamic> GetWaitList(FlowBeforeListQuery input)
        {
            if (db.CurrentConnectionConfig.DbType==DbType.Oracle)
            {
                return await GetWaitList_Oracle(input);
            }
            else
            {
                var whereLambda = LinqExpression.And<FlowBeforeListOutput>();
                if (!input.startTime.IsNullOrEmpty() && !input.endTime.IsNullOrEmpty())
                {
                    var start = Ext.GetDateTime(input.startTime.ToString());
                    var end = Ext.GetDateTime(input.endTime.ToString());
                    start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                    end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                    whereLambda = whereLambda.And(x => SqlFunc.Between(x.creatorTime, start, end));
                }
                if (!input.flowCategory.IsNullOrEmpty())
                    whereLambda = whereLambda.And(x => x.flowCategory == input.flowCategory);
                if (!input.flowId.IsNullOrEmpty())
                    whereLambda = whereLambda.And(x => x.flowId == input.flowId);
                if (!input.keyword.IsNullOrEmpty())
                    whereLambda = whereLambda.And(m => m.enCode.Contains(input.keyword) || m.fullName.Contains(input.keyword));
                if (!input.creatorUserId.IsNullOrEmpty())
                    whereLambda = whereLambda.And(m => m.creatorUserId.Contains(input.creatorUserId));
                //经办审核
                var list1 = db.Queryable<FlowTaskEntity, FlowTaskOperatorEntity, UserEntity, FlowEngineEntity>((a, b, c, d) =>
                new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId, JoinType.Left, a.CreatorUserId == c.Id, JoinType.Left, a.FlowId == d.Id))
                    .Where((a, b, c) => a.Status == 1 && a.DeleteMark == null && b.Completion == 0 && b.State == "0"
                    && (SqlFunc.ToDate(SqlFunc.ToString(b.Description)) < DateTime.Now || b.Description == null)
                    && b.HandleId == _userManager.UserId)
                    .Select((a, b, c, d) => new FlowBeforeListOutput()
                    {
                        enCode = a.EnCode,
                        creatorUserId = a.CreatorUserId,
                        creatorTime = SqlFunc.IsNullOrEmpty(SqlFunc.ToString(b.Description)) ? b.CreatorTime : SqlFunc.ToDate(SqlFunc.ToString(b.Description)),
                        thisStep = a.ThisStep,
                        thisStepId = b.TaskNodeId,
                        flowCategory = a.FlowCategory,
                        fullName = a.FullName,
                        flowName = a.FlowName,
                        status = a.Status,
                        id = b.Id,
                        userName = SqlFunc.MergeString(c.RealName, "/", c.Account),
                        description = SqlFunc.ToString(a.Description),
                        flowCode = a.FlowCode,
                        flowId = a.FlowId,
                        processId = a.ProcessId,
                        formType = d.FormType,
                        flowUrgent = a.FlowUrgent,
                        startTime = a.CreatorTime,
                        completion = a.Completion,
                    });
                //委托审核
                var list2 = db.Queryable<FlowTaskEntity, FlowTaskOperatorEntity, FlowDelegateEntity, UserEntity, UserEntity, FlowEngineEntity>((a, b, c, d, e, f) =>
                new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId, JoinType.Left, a.FlowId == c.FlowId
                && c.EndTime > DateTime.Now, JoinType.Left, c.CreatorUserId == d.Id, JoinType.Left,
                a.CreatorUserId == e.Id, JoinType.Left, a.FlowId == f.Id))
                    .Where((a, b, c) => a.Status == 1 && a.DeleteMark == null && b.Completion == 0 && b.State == "0"
                    && b.HandleId == c.CreatorUserId && (SqlFunc.ToDate(SqlFunc.ToString(b.Description)) < DateTime.Now || b.Description == null)
                    && c.ToUserId == _userManager.UserId).Select((a, b, c, d, e, f) => new FlowBeforeListOutput()
                    {
                        enCode = a.EnCode,
                        creatorUserId = a.CreatorUserId,
                        creatorTime = SqlFunc.IsNullOrEmpty(SqlFunc.ToString(b.Description)) ? b.CreatorTime : SqlFunc.ToDate(SqlFunc.ToString(b.Description)),
                        thisStep = a.ThisStep,
                        thisStepId = b.TaskNodeId,
                        flowCategory = a.FlowCategory,
                        fullName = SqlFunc.MergeString(a.FullName, "(", d.RealName, "的委托)"),
                        flowName = a.FlowName,
                        status = a.Status,
                        id = b.Id,
                        userName = SqlFunc.MergeString(e.RealName, "/", e.Account),
                        description = SqlFunc.ToString(a.Description),
                        flowCode = a.FlowCode,
                        flowId = a.FlowId,
                        processId = a.ProcessId,
                        formType = f.FormType,
                        flowUrgent = a.FlowUrgent,
                        startTime = a.CreatorTime,
                        completion = a.Completion,
                    });
                var output = await db.UnionAll(list1, list2).Where(whereLambda).MergeTable().OrderBy(x => x.creatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
                return PageResult<FlowBeforeListOutput>.SqlSugarPageResult(output);
            }
        }

        /// <summary>
        /// oracle
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<object> GetWaitList_Oracle(FlowBeforeListQuery input)
        {
            var whereLambda = LinqExpression.And<FlowBeforeListOutput>();
            if (!input.startTime.IsNullOrEmpty() && !input.endTime.IsNullOrEmpty())
            {
                var start = Ext.GetDateTime(input.startTime.ToString());
                var end = Ext.GetDateTime(input.endTime.ToString());
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                whereLambda = whereLambda.And(x => SqlFunc.Between(x.creatorTime, start, end));
            }
            if (!input.flowCategory.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.flowCategory == input.flowCategory);
            if (!input.flowId.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.flowId == input.flowId);
            if (!input.keyword.IsNullOrEmpty())
                whereLambda = whereLambda.And(m => m.enCode.Contains(input.keyword) || m.fullName.Contains(input.keyword));
            if (!input.creatorUserId.IsNullOrEmpty())
                whereLambda = whereLambda.And(m => m.creatorUserId.Contains(input.creatorUserId));
            //经办审核
            var list1 = db.Queryable<FlowTaskEntity, FlowTaskOperatorEntity, UserEntity, FlowEngineEntity>((a, b, c, d) =>
            new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId, JoinType.Left, a.CreatorUserId == c.Id, JoinType.Left, a.FlowId == d.Id))
                .Where((a, b, c) => a.Status == 1 && a.DeleteMark == null && b.Completion == 0 && b.State == "0"
                && (SqlFunc.Oracle_ToDate(SqlFunc.ToString(b.Description), "yyyy-mm-dd hh24:mi:ss") < SqlFunc.GetDate() || b.Description == null)
                && b.HandleId == _userManager.UserId)
                .Select((a, b, c, d) => new FlowBeforeListOutput()
                {
                    enCode = a.EnCode,
                    creatorUserId = a.CreatorUserId,
                    creatorTime = SqlFunc.IsNullOrEmpty(SqlFunc.ToString(b.Description)) ? b.CreatorTime : SqlFunc.Oracle_ToDate(SqlFunc.ToString(b.Description), "yyyy-mm-dd hh24:mi:ss"),
                    thisStep = a.ThisStep,
                    thisStepId = b.TaskNodeId,
                    flowCategory = a.FlowCategory,
                    fullName = a.FullName,
                    flowName = a.FlowName,
                    status = a.Status,
                    id = b.Id,
                    userName = SqlFunc.MergeString(c.RealName, "/", c.Account),
                    description = SqlFunc.ToString(a.Description),
                    flowCode = a.FlowCode,
                    flowId = a.FlowId,
                    processId = a.ProcessId,
                    formType = d.FormType,
                    flowUrgent = a.FlowUrgent,
                    startTime = a.CreatorTime,
                    completion = a.Completion,
                });
            //委托审核
            var list2 = db.Queryable<FlowTaskEntity, FlowTaskOperatorEntity, FlowDelegateEntity, UserEntity, UserEntity, FlowEngineEntity>((a, b, c, d, e, f) =>
            new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId, JoinType.Left, a.FlowId == c.FlowId
            && c.EndTime > SqlFunc.GetDate(), JoinType.Left, c.CreatorUserId == d.Id, JoinType.Left,
            a.CreatorUserId == e.Id, JoinType.Left, a.FlowId == f.Id))
                .Where((a, b, c) => a.Status == 1 && a.DeleteMark == null && b.Completion == 0 && b.State == "0"
                && b.HandleId == c.CreatorUserId && (SqlFunc.Oracle_ToDate(SqlFunc.ToString(b.Description), "yyyy-mm-dd hh24:mi:ss") < SqlFunc.GetDate() || b.Description == null)
                && c.ToUserId == _userManager.UserId).Select((a, b, c, d, e, f) => new FlowBeforeListOutput()
                {
                    enCode = a.EnCode,
                    creatorUserId = a.CreatorUserId,
                    creatorTime = SqlFunc.IsNullOrEmpty(SqlFunc.ToString(b.Description)) ? b.CreatorTime : SqlFunc.Oracle_ToDate(SqlFunc.ToString(b.Description), "yyyy-mm-dd hh24:mi:ss"),
                    thisStep = a.ThisStep,
                    thisStepId = b.TaskNodeId,
                    flowCategory = a.FlowCategory,
                    fullName = SqlFunc.MergeString(a.FullName, "(", d.RealName, "的委托)"),
                    flowName = a.FlowName,
                    status = a.Status,
                    id = b.Id,
                    userName = SqlFunc.MergeString(e.RealName, "/", e.Account),
                    description = SqlFunc.ToString(a.Description),
                    flowCode = a.FlowCode,
                    flowId = a.FlowId,
                    processId = a.ProcessId,
                    formType = f.FormType,
                    flowUrgent = a.FlowUrgent,
                    startTime = a.CreatorTime,
                    completion = a.Completion,
                });
            var output = await db.UnionAll(list1, list2).Where(whereLambda).MergeTable().OrderBy(x => x.creatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<FlowBeforeListOutput>.SqlSugarPageResult(output);
        }

        /// <summary>
        /// 列表（我已审批）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        public async Task<dynamic> GetTrialList(FlowBeforeListQuery input)
        {
            var list = db.Queryable<FlowTaskEntity, FlowTaskOperatorRecordEntity, FlowTaskOperatorEntity,
                UserEntity, UserEntity, FlowEngineEntity>((a, b, c, d, e, f) =>
                new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId, JoinType.Left, b.TaskOperatorId == c.Id,
                JoinType.Left, c.HandleId == d.Id, JoinType.Left, a.CreatorUserId == e.Id,
                JoinType.Left, a.FlowId == f.Id)).Where((a, b, c) => b.HandleStatus < 2 && b.TaskOperatorId != null
                && b.HandleId == _userManager.UserId && b.Status >= 0).Select((a, b, c, d, e, f) => new FlowBeforeListOutput()
                {
                    enCode = a.EnCode,
                    creatorUserId = a.CreatorUserId,
                    creatorTime = b.HandleTime,
                    thisStep = a.ThisStep,
                    thisStepId = b.TaskNodeId,
                    flowCategory = a.FlowCategory,
                    fullName = b.HandleId == c.HandleId || c.Id == null ? a.FullName : SqlFunc.MergeString(a.FullName, "(" , d.RealName, "的委托)"),
                    flowName = a.FlowName,
                    status = b.HandleStatus,
                    id = b.Id,
                    userName = SqlFunc.MergeString(e.RealName, "/", e.Account),
                    description = a.Description,
                    flowCode = a.FlowCode,
                    flowId = a.FlowId,
                    processId = a.ProcessId,
                    formType = f.FormType,
                    flowUrgent = a.FlowUrgent,
                    startTime = a.CreatorTime,
                });
            var whereLambda = LinqExpression.And<FlowBeforeListOutput>();
            if (!input.startTime.IsNullOrEmpty() && !input.endTime.IsNullOrEmpty())
            {
                var start = Ext.GetDateTime(input.startTime.ToString());
                var end = Ext.GetDateTime(input.endTime.ToString());
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                whereLambda = whereLambda.And(x => SqlFunc.Between(x.creatorTime, start, end));
            }
            if (!input.flowCategory.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.flowCategory == input.flowCategory);
            if (!input.flowId.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.flowId == input.flowId);
            if (!input.creatorUserId.IsNullOrEmpty())
                whereLambda = whereLambda.And(m => m.creatorUserId.Contains(input.creatorUserId));
            if (!input.keyword.IsNullOrEmpty())
                whereLambda = whereLambda.And(m => m.enCode.Contains(input.keyword) || m.fullName.Contains(input.keyword));
            var output = await list.MergeTable().Where(whereLambda).OrderBy(x => x.creatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<FlowBeforeListOutput>.SqlSugarPageResult(output);
        }

        /// <summary>
        /// 列表（抄送我的）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        public async Task<dynamic> GetCirculateList(FlowBeforeListQuery input)
        {
            var whereLambda = LinqExpression.And<FlowBeforeListOutput>();
            if (!input.startTime.IsNullOrEmpty() && !input.endTime.IsNullOrEmpty())
            {
                var start = Ext.GetDateTime(input.startTime.ToString());
                var end = Ext.GetDateTime(input.endTime.ToString());
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                whereLambda = whereLambda.And(x => SqlFunc.Between(x.creatorTime, start, end));
            }
            if (!input.flowCategory.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.flowCategory == input.flowCategory);
            if (!input.flowId.IsNullOrEmpty())
                whereLambda = whereLambda.And(x => x.flowId == input.flowId);
            if (!input.creatorUserId.IsNullOrEmpty())
                whereLambda = whereLambda.And(m => m.creatorUserId.Contains(input.creatorUserId));
            if (!input.keyword.IsNullOrEmpty())
                whereLambda = whereLambda.And(m => m.enCode.Contains(input.keyword) || m.fullName.Contains(input.keyword));
            var list = await db.Queryable<FlowTaskEntity, FlowTaskCirculateEntity, UserEntity, FlowEngineEntity>((a, b, c, d) => new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId, JoinType.Left, a.CreatorUserId == c.Id, JoinType.Left, a.FlowId == d.Id)).Where((a, b) => b.ObjectId == _userManager.UserId).Select((a, b, c, d) => new FlowBeforeListOutput()
            {
                enCode = a.EnCode,
                creatorUserId = a.CreatorUserId,
                creatorTime = b.CreatorTime,
                thisStep = a.ThisStep,
                thisStepId = b.TaskNodeId,
                flowCategory = a.FlowCategory,
                fullName = a.FullName,
                flowName = a.FlowName,
                status = a.Status,
                id = b.Id,
                userName = SqlFunc.MergeString(c.RealName, "/", c.Account),
                description = a.Description,
                flowCode = a.FlowCode,
                flowId = a.FlowId,
                processId = a.ProcessId,
                formType = d.FormType,
                flowUrgent = a.FlowUrgent,
                startTime = a.CreatorTime,
            }).MergeTable().Where(whereLambda).OrderBy(x => x.creatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<FlowBeforeListOutput>.SqlSugarPageResult(list);
        }

        /// <summary>
        /// 列表（待我审批）
        /// </summary>
        /// <returns></returns>
        public async Task<List<FlowTaskEntity>> GetWaitList()
        {
            //经办审核
            var list1 = db.Queryable<FlowTaskEntity, FlowTaskOperatorEntity>((a, b) => 
            new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId)).Where((a, b) => a.Status == 1 && a.DeleteMark == null 
            && b.Completion == 0 && b.State == "0" && (SqlFunc.ToDate(SqlFunc.ToString(b.Description)) < DateTime.Now || b.Description == null)
            && b.HandleId == _userManager.UserId).Select((a, b) => new FlowTaskEntity()
            {
                Id = b.Id,
                ParentId = a.ParentId,
                ProcessId = a.ProcessId,
                EnCode = a.EnCode,
                FullName = a.FullName,
                FlowUrgent = a.FlowUrgent,
                FlowId = a.FlowId,
                FlowCode = a.FlowCode,
                FlowName = a.FlowName,
                FlowCategory = a.FlowCategory,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                ThisStep = a.ThisStep,
                ThisStepId = b.TaskNodeId,
                Status = a.Status,
                Completion = a.Completion,
                CreatorTime = b.CreatorTime,
                CreatorUserId = a.CreatorUserId,
                LastModifyTime = a.LastModifyTime,
                LastModifyUserId = a.LastModifyUserId
            });
            //委托审核
            var list2 = db.Queryable<FlowTaskEntity, FlowTaskOperatorEntity, FlowDelegateEntity, UserEntity>((a, b, c, d) => 
            new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId, JoinType.Left, a.FlowId == c.FlowId &&
            c.EndTime > DateTime.Now, JoinType.Left, c.CreatorUserId == d.Id)).Where((a, b, c) =>
            a.Status == 1 && a.DeleteMark == null && b.Completion == 0 && b.State == "0" 
            && (SqlFunc.ToDate(SqlFunc.ToString(b.Description)) < DateTime.Now || b.Description == null)
            && b.HandleId == c.CreatorUserId && c.ToUserId == _userManager.UserId).Select((a, b, c, d) => new FlowTaskEntity()
            {
                Id = b.Id,
                ParentId = a.ParentId,
                ProcessId = a.ProcessId,
                EnCode = a.EnCode,
                FullName = SqlFunc.MergeString(a.FullName, "(", d.RealName, "的委托)"),
                FlowUrgent = a.FlowUrgent,
                FlowId = a.FlowId,
                FlowName = a.FlowName,
                FlowCode = a.FlowCode,
                FlowCategory = a.FlowCategory,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                ThisStep = a.ThisStep,
                ThisStepId = b.TaskNodeId,
                Status = a.Status,
                Completion = a.Completion,
                CreatorTime = b.CreatorTime,
                CreatorUserId = a.CreatorUserId,
                LastModifyTime = a.LastModifyTime,
                LastModifyUserId = a.LastModifyUserId
            });
            var output = await db.UnionAll(list1, list2).MergeTable().ToListAsync();
            return output;
        }

        /// <summary>
        /// 列表（我已审批）
        /// </summary>
        /// <returns></returns>
        public async Task<List<FlowTaskEntity>> GetTrialList()
        {
            var list = await db.Queryable<FlowTaskEntity, FlowTaskOperatorRecordEntity, FlowTaskOperatorEntity, UserEntity>((a, b, c, d) => new JoinQueryInfos(JoinType.Left, a.Id == b.TaskId, JoinType.Left, b.TaskOperatorId == c.Id, JoinType.Left, c.HandleId == d.Id)).Where((a, b, c) => b.HandleStatus < 2 && b.TaskOperatorId != null && b.HandleId == _userManager.UserId).Select((a, b, c, d) => new FlowTaskEntity()
            {
                Id = b.Id,
                ParentId = a.ParentId,
                ProcessId = a.ProcessId,
                EnCode = a.EnCode,
                FullName = b.HandleId == c.HandleId || c.Id == null ? a.FullName : SqlFunc.MergeString(a.FullName, "(", d.RealName, "的委托)"),
                FlowUrgent = a.FlowUrgent,
                FlowId = a.FlowId,
                FlowCode = a.FlowCode,
                FlowName = a.FlowName,
                FlowCategory = a.FlowCategory,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                ThisStep = b.NodeName,
                ThisStepId = c.TaskNodeId,
                Status = b.HandleStatus,
                Completion = a.Completion,
                CreatorTime = b.HandleTime,
                CreatorUserId = a.CreatorUserId,
                LastModifyTime = a.LastModifyTime,
                LastModifyUserId = a.LastModifyUserId
            }).MergeTable().ToListAsync();
            return list;
        }

        /// <summary>
        /// 任务列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<FlowTaskEntity>> GetTaskList()
        {
            return await _flowTaskRepository.Entities.Where(x => x.DeleteMark == null).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToListAsync();
        }

        /// <summary>
        /// 任务列表
        /// </summary>
        /// <param name="flowId">引擎id</param>
        /// <returns></returns>
        public async Task<List<FlowTaskEntity>> GetTaskList(string flowId)
        {
            return await _flowTaskRepository.Entities.Where(x => x.DeleteMark == null && x.FlowId == flowId).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToListAsync();
        }

        /// <summary>
        /// 任务信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FlowTaskEntity> GetTaskInfo(string id)
        {
            return await _flowTaskRepository.FirstOrDefaultAsync(x => x.DeleteMark == null && x.Id == id);
        }

        /// <summary>
        /// 任务信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FlowTaskEntity GetTaskInfoNoAsync(string id)
        {
            return _flowTaskRepository.FirstOrDefault(x => x.DeleteMark == null && x.Id == id);
        }

        /// <summary>
        /// 任务删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> DeleteTask(FlowTaskEntity entity)
        {
            await _flowTaskNodeRepository.DeleteAsync(x => entity.Id == x.TaskId);
            await _flowTaskOperatorRepository.DeleteAsync(x => entity.Id == x.TaskId);
            await _flowTaskOperatorRecordRepository.DeleteAsync(x => entity.Id == x.TaskId);
            await _flowTaskCirculateRepository.DeleteAsync(x => entity.Id == x.TaskId);
            return await _flowTaskRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 任务删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int DeleteTaskNoAsync(FlowTaskEntity entity)
        {
            _flowTaskNodeRepository.Delete(x => entity.Id == x.TaskId);
            _flowTaskOperatorRepository.Delete(x => entity.Id == x.TaskId);
            _flowTaskOperatorRecordRepository.Delete(x => entity.Id == x.TaskId);
            _flowTaskCirculateRepository.Delete(x => entity.Id == x.TaskId);
            return _flowTaskRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommand();
        }

        /// <summary>
        /// 任务创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<FlowTaskEntity> CreateTask(FlowTaskEntity entity)
        {
            return await _flowTaskRepository.Context.Insertable(entity).CallEntityMethod(m => m.Create()).ExecuteReturnEntityAsync();
        }

        /// <summary>
        /// 任务更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> UpdateTask(FlowTaskEntity entity)
        {
            return await _flowTaskRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 判断是否有子流程
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool AnySubFlowTask(string id)
        {
            return _flowTaskRepository.Any(x => x.ParentId == id && x.Status == 0 && x.DeleteMark == null);
        }

        /// <summary>
        /// 节点列表
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<List<FlowTaskNodeEntity>> GetTaskNodeList(string taskId)
        {
            return await _flowTaskNodeRepository.Entities.Where(x => x.TaskId == taskId).OrderBy(x => x.SortCode).ToListAsync();
        }

        /// <summary>
        /// 节点信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FlowTaskNodeEntity> GetTaskNodeInfo(string id)
        {
            return await _flowTaskNodeRepository.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// 节点删除
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<int> DeleteTaskNode(string taskId)
        {
            return await _flowTaskNodeRepository.DeleteAsync(x => x.TaskId == taskId);
        }

        /// <summary>
        /// 节点创建
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public async Task<int> CreateTaskNode(List<FlowTaskNodeEntity> entitys)
        {
            return await _flowTaskNodeRepository.InsertAsync(entitys);
        }

        /// <summary>
        /// 节点更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> UpdateTaskNode(FlowTaskNodeEntity entity)
        {
            return await _flowTaskNodeRepository.UpdateAsync(entity);
        }

        /// <summary>
        /// 经办列表
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<List<FlowTaskOperatorEntity>> GetTaskOperatorList(string taskId)
        {
            return await _flowTaskOperatorRepository.Entities.Where(x => x.TaskId == taskId).OrderBy(x => x.CreatorTime).ToListAsync();
        }

        /// <summary>
        /// 经办列表
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskNodeId"></param>
        /// <returns></returns>
        public async Task<List<FlowTaskOperatorEntity>> GetTaskOperatorList(string taskId, string taskNodeId)
        {
            return await _flowTaskOperatorRepository.Entities.Where(x => x.TaskId == taskId && x.TaskNodeId == taskNodeId).OrderBy(x => x.CreatorTime).ToListAsync();
        }

        /// <summary>
        /// 经办信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FlowTaskOperatorEntity> GetTaskOperatorInfo(string id)
        {
            return await _flowTaskOperatorRepository.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// 经办删除
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<int> DeleteTaskOperator(string taskId)
        {
            return await _flowTaskOperatorRepository.DeleteAsync(x => x.TaskId == taskId);
        }

        /// <summary>
        /// 经办删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<int> DeleteTaskOperator(List<string> ids)
        {
            return await db.Updateable<FlowTaskOperatorEntity>().SetColumns(x => x.State == "-1").Where(x => ids.Contains(x.Id)).ExecuteCommandAsync();
        }

        /// <summary>
        /// 经办创建
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public async Task<int> CreateTaskOperator(List<FlowTaskOperatorEntity> entitys)
        {
            return await _flowTaskOperatorRepository.InsertAsync(entitys);
        }

        /// <summary>
        /// 经办创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> CreateTaskOperator(FlowTaskOperatorEntity entity)
        {
            return await _flowTaskOperatorRepository.InsertAsync(entity);
        }

        /// <summary>
        /// 经办更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> UpdateTaskOperator(FlowTaskOperatorEntity entity)
        {
            return await _flowTaskOperatorRepository.UpdateAsync(entity);
        }

        /// <summary>
        /// 经办更新
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public async Task<int> UpdateTaskOperator(List<FlowTaskOperatorEntity> entitys)
        {
            return await _flowTaskOperatorRepository.UpdateAsync(entitys);
        }

        /// <summary>
        /// 经办记录列表
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<List<FlowTaskOperatorRecordEntity>> GetTaskOperatorRecordList(string taskId)
        {
            return await _flowTaskOperatorRecordRepository.Entities.Where(x => x.TaskId == taskId).OrderBy(o => o.HandleTime).ToListAsync();
        }

        /// <summary>
        /// 经办信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FlowTaskOperatorRecordEntity> GetTaskOperatorRecordInfo(string id)
        {
            return await _flowTaskOperatorRecordRepository.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// 经办信息
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskNodeId"></param>
        /// <param name="taskOperatorId"></param>
        /// <returns></returns>
        public async Task<FlowTaskOperatorRecordEntity> GetTaskOperatorRecordInfo(string taskId, string taskNodeId, string taskOperatorId)
        {
            return await _flowTaskOperatorRecordRepository.FirstOrDefaultAsync(x => x.TaskId == taskId && x.TaskNodeId == taskNodeId && x.TaskOperatorId == taskOperatorId&&x.Status!=-1&&x.HandleStatus<2);
        }

        /// <summary>
        /// 经办创建
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public async Task<int> CreateTaskOperatorRecord(List<FlowTaskOperatorRecordEntity> entitys)
        {
            return await _flowTaskOperatorRecordRepository.InsertAsync(entitys);
        }

        /// <summary>
        /// 经办创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> CreateTaskOperatorRecord(FlowTaskOperatorRecordEntity entity)
        {
            entity.Id = YitIdHelper.NextId().ToString();
            return await _flowTaskOperatorRecordRepository.InsertAsync(entity);
        }

        /// <summary>
        /// 传阅删除
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<int> DeleteTaskCirculate(string taskId)
        {
            return await _flowTaskCirculateRepository.DeleteAsync(x => x.TaskId == taskId);
        }

        /// <summary>
        /// 传阅创建
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public async Task<int> CreateTaskCirculate(List<FlowTaskCirculateEntity> entitys)
        {
            return await _flowTaskCirculateRepository.InsertAsync(entitys);
        }

        /// <summary>
        /// 打回流程删除所有相关数据
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task DeleteFlowTaskAllData(string taskId)
        {
            try
            {
                db.BeginTran();
                await db.Updateable<FlowTaskNodeEntity>().SetColumns(x => x.State == "-2").Where(x => x.TaskId == taskId).ExecuteCommandAsync();
                await db.Updateable<FlowTaskOperatorEntity>().SetColumns(x => x.State == "-1").Where(x => x.TaskId == taskId).ExecuteCommandAsync();
                await db.Updateable<FlowTaskOperatorRecordEntity>().SetColumns(x => x.Status == -1).Where(x => x.TaskId == taskId).ExecuteCommandAsync();
                db.CommitTran();
            }
            catch (Exception)
            {

                db.RollbackTran();
            }
        }

        /// <summary>
        /// 获取允许删除任务列表
        /// </summary>
        /// <param name="ids">id数组</param>
        /// <returns></returns>
        public List<string> GetAllowDeleteFlowTaskList(List<string> ids)
        {
            return _flowTaskRepository.Entities.In(f => f.Id, ids.ToArray()).Where(f => f.Status != 0).Where(f => f.Status != 4).Select(f => f.Id).ToList();
        }

        /// <summary>
        /// 经办记录列表
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskNodeIds"></param>
        /// <returns></returns>
        public List<FlowTaskOperatorRecordEntity> GetTaskOperatorRecordList(string taskId, string[] taskNodeIds)
        {
            return _flowTaskOperatorRecordRepository.Entities.Where(x => x.TaskId == taskId).In(x => x.TaskNodeId, taskNodeIds).OrderBy(x => x.HandleTime).ToList();
        }

        /// <summary>
        /// 经办记录作废
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task DeleteTaskOperatorRecord(List<string> ids)
        {
            await db.Updateable<FlowTaskOperatorRecordEntity>().SetColumns(it => it.Status == -1).Where(x => ids.Contains(x.Id)).ExecuteCommandAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public async Task<FlowTaskOperatorEntity> GetTaskOperatorInfoByParentId(string parentId)
        {
            return await _flowTaskOperatorRepository.FirstOrDefaultAsync(x => x.ParentId == parentId && !x.State.Equals("-1"));
        }
    }
}
