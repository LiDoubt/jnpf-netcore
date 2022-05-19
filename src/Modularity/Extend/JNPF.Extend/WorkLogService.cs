using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Filter;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.Extend.Entitys;
using JNPF.Extend.Entitys.Dto.WoekLog;
using JNPF.Extend.Entitys.Dto.WorkLog;
using JNPF.FriendlyException;
using JNPF.System.Interfaces.Permission;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace JNPF.Extend
{
    /// <summary>
    /// 工作日志
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "Extend", Name = "WorkLog", Order = 600)]
    [Route("api/extend/[controller]")]
    public class WorkLogService : IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<WorkLogEntity> _workLogRepository;
        private readonly IUsersService _usersService;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope db;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="workLogRepository"></param>
        /// <param name="usersService"></param>
        /// <param name="userManager"></param>
        public WorkLogService(ISqlSugarRepository<WorkLogEntity> workLogRepository, IUsersService usersService, IUserManager userManager)
        {
            _workLogRepository = workLogRepository;
            _usersService = usersService;
            _userManager = userManager;
            db = workLogRepository.Context;
        }

        #region Get
        /// <summary>
        /// 列表(我发出的)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Send")]
        public async Task<dynamic> GetSendList([FromQuery] PageInputBase input)
        {
            var list = await _workLogRepository.Entities.Where(x => x.CreatorUserId == _userManager.UserId && x.DeleteMark == null).WhereIF(input.keyword.IsNotEmptyOrNull(), m => m.Title.Contains(input.keyword) || m.Description.Contains(input.keyword)).OrderBy(t => t.CreatorTime,OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<WorkLogListOutput>()
            {
                list = list.list.Adapt<List<WorkLogListOutput>>(),
                pagination = list.pagination
            };
            return PageResult<WorkLogListOutput>.SqlSugarPageResult(pageList);
        }

        /// <summary>
        /// 列表(我收到的)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Receive")]
        public async Task<dynamic> GetReceiveList([FromQuery] PageInputBase input)
        {
            var list = await db.Queryable<WorkLogEntity, WorkLogShareEntity>(
                (a, b) => new JoinQueryInfos(JoinType.Left, a.Id == b.WorkLogId))
                .Where((a, b) => a.DeleteMark == null && b.ShareUserId == _userManager.UserId)
                .WhereIF(input.keyword.IsNotEmptyOrNull(), a => a.Title.Contains(input.keyword))
                .Select(a => new WorkLogListOutput() { 
                    id=a.Id,
                    title=a.Title,
                    question=a.Question,
                    creatorTime=a.CreatorTime,
                    todayContent=a.TodayContent,
                    tomorrowContent=a.TomorrowContent,
                    toUserId=a.ToUserId
                }).MergeTable().OrderBy(a => a.creatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<WorkLogListOutput>.SqlSugarPageResult(list);
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _workLogRepository.FirstOrDefaultAsync(x=>x.Id==id&&x.DeleteMark==null)).Adapt<WorkLogInfoOutput>();
            output.userIds = output.toUserId;
            output.toUserId =await _usersService.GetUserName(output.toUserId);
            return output;
        }
        #endregion

        #region Post
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Creater([FromBody] WorkLogCrInput input)
        {
            try
            {
                var entity = input.Adapt<WorkLogEntity>();
                entity.Id= YitIdHelper.NextId().ToString();
                List<WorkLogShareEntity> workLogShareList = new List<WorkLogShareEntity>();
                var toUserIds = entity.ToUserId.Split(',');
                foreach (var userId in toUserIds)
                {
                    var workLogShare = new WorkLogShareEntity();
                    workLogShare.Id = YitIdHelper.NextId().ToString();
                    workLogShare.ShareTime = DateTime.Now;
                    workLogShare.WorkLogId = entity.Id;
                    workLogShare.ShareUserId = userId;
                    workLogShareList.Add(workLogShare);
                }
                db.BeginTran();
                db.Insertable(workLogShareList).ExecuteCommand();
                var isOk = await _workLogRepository.Context.Insertable(entity).CallEntityMethod(m => m.Create()).ExecuteCommandAsync();
                if (isOk < 1)
                    throw JNPFException.Oh(ErrorCode.COM1000);
                db.CommitTran();
            }
            catch (Exception)
            {
                db.RollbackTran();
                throw JNPFException.Oh(ErrorCode.COM1000);
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] WorkLogUpInput input)
        {
            try
            {
                var entity = input.Adapt<WorkLogEntity>();
                List<WorkLogShareEntity> workLogShareList = new List<WorkLogShareEntity>();
                var toUserIds = entity.ToUserId.Split(',');
                foreach (var userId in toUserIds)
                {
                    var workLogShare = new WorkLogShareEntity();
                    workLogShare.Id = YitIdHelper.NextId().ToString();
                    workLogShare.ShareTime = DateTime.Now;
                    workLogShare.WorkLogId = entity.Id;
                    workLogShare.ShareUserId = userId;
                    workLogShareList.Add(workLogShare);
                }
                db.BeginTran();
                db.Deleteable(workLogShareList).ExecuteCommand(); 
                db.Insertable(workLogShareList).ExecuteCommand(); 
                var isOk = await _workLogRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
                if (isOk < 1)
                    throw JNPFException.Oh(ErrorCode.COM1001);
                db.CommitTran();
            }
            catch (Exception)
            {
                db.RollbackTran();
                throw JNPFException.Oh(ErrorCode.COM1001);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            try
            {
                var entity = await _workLogRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
                if (entity == null)
                    throw JNPFException.Oh(ErrorCode.COM1005);
                db.BeginTran();
                db.Deleteable<WorkLogShareEntity>(x => x.WorkLogId == id).ExecuteCommand();
                var isOk = await _workLogRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
                if (isOk < 1)
                    throw JNPFException.Oh(ErrorCode.COM1002);
                db.CommitTran();
            }
            catch (Exception)
            {
                db.RollbackTran();
                throw JNPFException.Oh(ErrorCode.COM1002);
            }
        }
        #endregion
    }
}
