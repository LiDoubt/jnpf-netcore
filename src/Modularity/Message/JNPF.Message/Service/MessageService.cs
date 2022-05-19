using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Filter;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.JsonSerialization;
using JNPF.LinqBuilder;
using JNPF.Message.Entitys;
using JNPF.Message.Entitys.Dto.Message;
using JNPF.Message.Extensions;
using JNPF.Message.Interfaces.Message;
using JNPF.System.Entitys.Permission;
using JNPF.System.Interfaces.Permission;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace JNPF.Message.Service
{
    /// <summary>
    /// 系统消息
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "Message", Name = "message", Order = 240)]
    [Route("api/[controller]")]
    public class MessageService : IMessageService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<MessageEntity> _messageRepository;
        private readonly ISqlSugarRepository<MessageReceiveEntity> _messageReceiveRepository;
        private readonly SqlSugarScope db;
        private readonly IUserManager _userManager;
        private readonly IUsersService _usersService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageRepository"></param>
        /// <param name="messageReceiveRepository"></param>
        /// <param name="usersService"></param>
        /// <param name="userManager"></param>
        public MessageService(ISqlSugarRepository<MessageEntity> messageRepository, ISqlSugarRepository<MessageReceiveEntity> messageReceiveRepository, IUsersService usersService, IUserManager userManager)
        {
            _messageRepository = messageRepository;
            _messageReceiveRepository = messageReceiveRepository;
            db = messageRepository.Context;
            _usersService = usersService;
            _userManager = userManager;
        }

        #region Get

        /// <summary>
        /// 列表（通知公告）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("Notice")]
        public async Task<dynamic> GetNoticeList([FromQuery] PageInputBase input)
        {
            var list = await _messageRepository.Context.Queryable<MessageEntity, UserEntity>((a, b) =>
            new JoinQueryInfos(JoinType.Left, b.Id == a.CreatorUserId)).Select((a, b) =>
            new MessageNoticeOutput
            {
                id = a.Id,
                lastModifyTime = a.LastModifyTime,
                enabledMark = a.EnabledMark,
                creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account),
                title = a.Title,
                deleteMark = SqlFunc.ToString(a.DeleteMark),
                type = a.Type
            }).MergeTable().Where(m => m.type == 1 && m.deleteMark == null)
                .WhereIF(!string.IsNullOrEmpty(input.keyword), m => m.title.Contains(input.keyword))
                .OrderBy(t => t.lastModifyTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<MessageNoticeOutput>.SqlSugarPageResult(list);
        }

        /// <summary>
        /// 列表（通知公告/系统消息/私信消息）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetMessageList([FromQuery] MessageListInput input)
        {
            var list = await _messageRepository.Context.Queryable<MessageEntity, MessageReceiveEntity, UserEntity>(
                (a, b, c) => new JoinQueryInfos(JoinType.Left, a.Id == b.MessageId, JoinType.Left, 
                a.CreatorUserId == c.Id)).Select((a, b, c) => 
                new MessageListOutput{ 
                    id = a.Id, lastModifyTime = a.LastModifyTime, enabledMark = a.EnabledMark, 
                    creatorUser = SqlFunc.MergeString(c.RealName, "/", c.Account), 
                    title = a.Title, deleteMark = a.DeleteMark, type = a.Type, 
                    isRead = b.IsRead, userId=b.UserId }).MergeTable()
                    .Where(m =>m.userId==_userManager.UserId&& m.deleteMark == null)
                    .WhereIF(input.type.IsNotEmptyOrNull(), x => x.type == input.type)
                    .WhereIF(!string.IsNullOrEmpty(input.keyword), m => m.title.Contains(input.keyword))
                    .OrderBy(t => t.lastModifyTime,OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            
            return PageResult<MessageListOutput>.SqlSugarPageResult(list);
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            var data = await _messageRepository.Context.Queryable<MessageEntity, UserEntity>(
                (a, b) => new JoinQueryInfos(JoinType.Left, a.CreatorUserId == b.Id))
               .Select((a, b) => 
               new MessageInfoOutput{ id = a.Id,
                   lastModifyTime = a.LastModifyTime, 
                   creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account),
                   title = a.Title, deleteMark = a.DeleteMark,
                   bodyText = a.BodyText }).MergeTable().
                   Where(m => m.id == id && m.deleteMark == null).FirstAsync();
            return data;
        }

        /// <summary>
        /// 读取消息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpGet("ReadInfo/{id}")]
        public async Task<dynamic> ReadInfo(string id)
        {
            var data = await _messageRepository.Context.Queryable<MessageEntity, UserEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.CreatorUserId == b.Id))
                .Select((a, b) => new MessageReadInfoOutput { 
                    id = a.Id, lastModifyTime = a.LastModifyTime, 
                    creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account), 
                    title = a.Title, deleteMark = a.DeleteMark, 
                    bodyText = a.BodyText }).MergeTable().Where(m => m.id == id && m.deleteMark == null)
                    .OrderBy(t => t.lastModifyTime).FirstAsync();
            if (data != null)
                await MessageRead(id);
            return data;
        }       
        #endregion

        #region Post

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await GetInfo(id);
            var isOk = await Delete(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] MessageCrInput input)
        {
            var entity = input.Adapt<MessageEntity>();
            entity.Type = 1;
            entity.EnabledMark = 0;
            var isOk = await Create(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] MessageUpInput input)
        {
            var entity = input.Adapt<MessageEntity>();
            var isOk = await Update(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 发布公告
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/Release")]
        public async Task Release(string id)
        {
            var entity = await GetInfo(id);
            if (entity != null)
            {
                var userList = await _usersService.GetList();
                entity.ToUserIds = string.Join(",", userList.Select(m => m.Id).ToList());
                //发送
                await SentNotice(entity.ToUserIds.Split(',').ToList(), entity);
            }
        }

        /// <summary>
        /// 全部已读
        /// </summary>
        /// <returns></returns>
        [HttpPost("Actions/ReadAll")]
        public async Task AllRead()
        {
            await MessageRead("");
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="postParam">请求参数</param>
        /// <returns></returns>
        [HttpDelete("Record")]
        public async Task DeleteRecord_Api([FromBody] dynamic postParam)
        {
            string[] ids = postParam.ids.ToString().Split(',');
            var isOk = await DeleteRecord(_userManager.UserId, ids.ToList());
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1002);
        }
        #endregion

        #region PublicMethod
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<MessageEntity>> GetList(int type)
        {
            return await _messageRepository.Entities.Where(m => m.Type == type && m.DeleteMark == null).ToListAsync();
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [NonAction]
        public async Task<MessageEntity> GetInfo(string id)
        {
            return await _messageRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
        }

        /// <summary>
        /// 默认公告(app)
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public string GetInfoDefaultNotice()
        {
            var entity = _messageRepository.Entities.Where(x => x.Type == 1 && x.DeleteMark == null).OrderBy(x=>x.CreatorTime,OrderByType.Desc).First();
            return entity == null ? "" : entity.Title;
        }

        /// <summary>
        /// 默认消息(app)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public string GetInfoDefaultMessage(string userId)
        {
            var entity = db.Queryable<MessageEntity, MessageReceiveEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.Id == b.MessageId))
                .Where((a, b) => a.Type == 2 && a.DeleteMark == null && b.UserId == userId).OrderBy(a => a.CreatorTime,OrderByType.Desc).Select(a => a).First();
            return entity == null ? "" : entity.Title;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        [NonAction]
        public async Task<int> Delete(MessageEntity entity)
        {
            try
            {
                db.BeginTran();
                await _messageReceiveRepository.DeleteAsync(x => x.MessageId == entity.Id);
                var total = await _messageRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
                db.CommitTran();
                return total;
            }
            catch (Exception)
            {
                db.RollbackTran();
                return 0;
            }
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        [NonAction]
        public async Task<int> Create(MessageEntity entity)
        {
            return await _messageRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="receiveEntityList">收件用户</param>
        [NonAction]
        public async Task<int> Create(MessageEntity entity, List<MessageReceiveEntity> receiveEntityList)
        {
            try
            {
                db.BeginTran();
                await _messageReceiveRepository.InsertAsync(receiveEntityList);
                var total = await _messageRepository.Context.Insertable(entity).CallEntityMethod(m => m.Create()).ExecuteCommandAsync();
                db.CommitTran();
                return total;
            }
            catch (Exception ex)
            {
                db.RollbackTran();
                return 0;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        [NonAction]
        public async Task<int> Update(MessageEntity entity)
        {
            return await _messageRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="receiveEntityList">收件用户</param>
        [NonAction]
        public async Task<int> Update(MessageEntity entity, List<MessageReceiveEntity> receiveEntityList)
        {
            try
            {
                db.BeginTran();
                await _messageReceiveRepository.InsertAsync(receiveEntityList);
                var total = await _messageRepository.Context.Updateable(entity).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
                db.CommitTran();
                return total;
            }
            catch (Exception ex)
            {
                db.RollbackTran();
                return 0;
            }
        }

        /// <summary>
        /// 消息已读（单条）
        /// </summary>
        /// <param name="userId">当前用户</param>
        /// <param name="messageId">消息主键</param>
        [NonAction]
        public async Task MessageRead(string userId, string messageId)
        {
            await db.Updateable<MessageReceiveEntity>().SetColumns(it => it.ReadCount == it.ReadCount + 1).SetColumns(x => new MessageReceiveEntity()
            {
                IsRead = 1,
                ReadTime = DateTime.Now
            }).Where(x => x.UserId == userId && x.MessageId == messageId).ExecuteCommandAsync();

        }

        /// <summary>
        /// 消息已读（全部）
        /// </summary>
        /// <param name="id">id</param>
        [NonAction]
        public async Task MessageRead(string id)
        {
            try
            {
                db.BeginTran();
                var whereParams= LinqExpression.And<MessageReceiveEntity>();
                whereParams = whereParams.And(x => x.UserId == _userManager.UserId && x.IsRead == 0);
                if(id.IsNotEmptyOrNull())
                    whereParams = whereParams.And(x => x.MessageId ==id);
                await db.Updateable<MessageReceiveEntity>().SetColumns(it => it.ReadCount == it.ReadCount + 1).SetColumns(x => new MessageReceiveEntity()
                {
                    IsRead = 1,
                    ReadTime = DateTime.Now
                }).Where(whereParams).ExecuteCommandAsync();

                db.CommitTran();
            }
            catch (Exception e)
            {
                db.RollbackTran();
            }
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="userId">当前用户</param>
        /// <param name="messageIds">消息Id</param>
        [NonAction]
        public async Task<int> DeleteRecord(string userId, List<string> messageIds)
        {
            return await _messageReceiveRepository.DeleteAsync(m => m.UserId == userId && messageIds.Contains(m.MessageId));
        }

        /// <summary>
        /// 获取未读数量（含 通知公告、系统消息）
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> GetUnreadCount(string userId)
        {
            return await _messageReceiveRepository.CountAsync(m => m.UserId == userId && m.IsRead == 0);
        }

        /// <summary>
        /// 获取公告未读数量
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        [NonAction]
        public int GetUnreadNoticeCount(string userId)
        {
            return db.Queryable<MessageEntity, MessageReceiveEntity>((m, mr) => new JoinQueryInfos(JoinType.Left, m.Id == mr.MessageId)).Select((m, mr) => new { mr.Id, mr.UserId, mr.IsRead, m.Type, m.DeleteMark }).MergeTable().Where(x => x.Type == 1 && x.DeleteMark == null && x.UserId == userId && x.IsRead == 0).Count();
        }

        /// <summary>
        /// 获取消息未读数量
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        [NonAction]
        public int GetUnreadMessageCount(string userId)
        {
            return db.Queryable<MessageEntity, MessageReceiveEntity>((m, mr) => new JoinQueryInfos(JoinType.Left, m.Id == mr.MessageId)).Select((m, mr) => new { mr.Id, mr.UserId, mr.IsRead, m.Type, m.DeleteMark }).MergeTable().Where(x => x.Type == 2 && x.DeleteMark == null && x.UserId == userId && x.IsRead == 0).Count();
        }

        /// <summary>
        /// 发送公告
        /// </summary>
        /// <param name="toUserIds">发送用户</param>
        /// <param name="entity">消息信息</param>
        [NonAction]
        public async Task SentNotice(List<string> toUserIds, MessageEntity entity)
        {
            try
            {
                entity.EnabledMark = 1;
                List<MessageReceiveEntity> receiveEntityList = new List<MessageReceiveEntity>();
                foreach (var item in toUserIds)
                {
                    MessageReceiveEntity messageReceiveEntity = new MessageReceiveEntity();
                    messageReceiveEntity.Id = YitIdHelper.NextId().ToString();
                    messageReceiveEntity.MessageId = entity.Id;
                    messageReceiveEntity.UserId = item;
                    messageReceiveEntity.IsRead = 0;
                    receiveEntityList.Add(messageReceiveEntity);
                }
                await Update(entity, receiveEntityList);
                //消息推送 - PC端
                foreach (var item in WebSocketClientCollection._clients.FindAll(it=>it.TenantId==_userManager.TenantId))
                {
                    if (toUserIds.Contains(item.UserId))
                    {
                        await item.SendMessageAsync(new { method = "messagePush", userId = _userManager.UserId, toUserId = toUserIds, title = entity.Title, unreadNoticeCount = 1 }.Serialize());
                    }
                }
            }
            catch (Exception ex)
            {
                throw JNPFException.Oh(ErrorCode.D7003);
            }
        }

        /// <summary>
        /// 发送站内消息
        /// </summary>
        /// <param name="toUserIds">发送用户</param>
        /// <param name="title">标题</param>
        /// <param name="bodyText">内容</param>
        [NonAction]
        public async Task SentMessage(List<string> toUserIds, string title, string bodyText = null)
        {
            try
            {
                MessageEntity entity = new MessageEntity();
                entity.Id = YitIdHelper.NextId().ToString();
                entity.Title = title;
                entity.BodyText = bodyText;
                entity.Type = 2;
                entity.LastModifyTime = DateTime.Now;
                entity.LastModifyUserId = _userManager.UserId;
                List<MessageReceiveEntity> receiveEntityList = new List<MessageReceiveEntity>();
                foreach (var item in toUserIds)
                {
                    MessageReceiveEntity messageReceiveEntity = new MessageReceiveEntity();
                    messageReceiveEntity.Id = YitIdHelper.NextId().ToString();
                    messageReceiveEntity.MessageId = entity.Id;
                    messageReceiveEntity.UserId = item;
                    messageReceiveEntity.IsRead = 0;
                    receiveEntityList.Add(messageReceiveEntity);
                }
                await Create(entity, receiveEntityList);
                //消息推送 - PC端
                foreach (var item in WebSocketClientCollection._clients)
                {
                    if (toUserIds.Contains(item.UserId))
                    {
                        await item.SendMessageAsync(new { method = "messagePush", userId = _userManager.UserId, toUserId = toUserIds, title = entity.Title, unreadNoticeCount = 1 }.Serialize());
                    }
                }
                //消息推送 - APP
                // GetuiAppPushHelper.Instance.SendNotice(userInfo.TenantId, toUserIds, "系统消息", entity.F_Title, "2");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion
    }
}
