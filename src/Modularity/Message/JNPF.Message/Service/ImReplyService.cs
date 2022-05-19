using JNPF.Common.Core.Manager;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.JsonSerialization;
using JNPF.Message.Entitys;
using JNPF.Message.Entitys.Dto.ImReply;
using JNPF.Message.Extensions;
using JNPF.Message.Interfaces;
using JNPF.System.Entitys.Permission;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JNPF.Message.Service
{
    /// <summary>
    /// 消息会话接口
    /// </summary>
    [ApiDescriptionSettings(Tag = "Message", Name = "imreply", Order = 163)]
    [Route("api/message/[controller]")]
    public class ImReplyService : IImReplyService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ImReplyEntity> _imReplyRepository;
        private readonly IUserManager _userManager;
        private readonly SqlSugarScope _db;

        /// <summary>
        /// 初始化一个<see cref="ImReplyService"/>类型的新实例
        /// </summary>
        public ImReplyService(ISqlSugarRepository<ImReplyEntity> imReplyRepository, IUserManager userManager)
        {
            _imReplyRepository = imReplyRepository;
            _userManager = userManager;
            _db = _imReplyRepository.Context;
        }

        /// <summary>
        /// 获取消息会话列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList()
        {
            var userInfo = await _userManager.GetUserInfo();
            var newObjectUserList = new List<ImReplyListOutput>();
            //获取全部聊天对象列表
            var objectList = _db.UnionAll(_db.Queryable<ImReplyEntity>().Where(i => i.ReceiveUserId == userInfo.userId).Select(it => new { userId = it.UserId, latestDate = it.ReceiveTime }).MergeTable().Select<ImReplyObjectIdOutput>(),
                 _db.Queryable<ImReplyEntity>().Where(i => i.UserId == userInfo.userId).Select(it => new { userId = it.ReceiveUserId, latestDate = it.ReceiveTime }).MergeTable().Select<ImReplyObjectIdOutput>()).MergeTable().GroupBy(it => new { it.userId }).Select(it => new { it.userId, latestDate = SqlFunc.AggregateMax(it.latestDate) }).ToList();
            var objectUserList = objectList.Adapt<List<ImReplyListOutput>>();
            if (objectUserList.Count > 0)
            {
                var userList = await _db.Queryable<UserEntity>().In(it => it.Id, objectUserList.Select(it => it.userId).ToArray()).ToListAsync();
                //将用户信息补齐
                userList.ForEach(item =>
                {
                    objectUserList.ForEach(it =>
                    {
                        if (it.userId == item.Id)
                        {
                            it.account = item.Account;
                            it.id = it.userId;
                            it.realName = item.RealName;
                            it.headIcon = "/api/File/Image/userAvatar/" + item.HeadIcon;

                            var imContent = _db.Queryable<IMContentEntity>().Where(i => (i.SendUserId == userInfo.userId && i.ReceiveUserId == it.userId) || (i.SendUserId == it.userId && i.ReceiveUserId == userInfo.userId)).Where(i => i.SendTime.Equals(it.latestDate)).ToList().FirstOrDefault();
                            //获取最信息
                            if (imContent != null)
                            {
                                it.latestMessage = imContent.Content;
                                it.messageType = imContent.ContentType;
                            }
                            it.unreadMessage = _db.Queryable<IMContentEntity>().Where(i => i.SendUserId == it.userId && i.ReceiveUserId == userInfo.userId).Where(i => i.State == 0).Count();
                        }
                    });
                });
            }
            var output = objectUserList.OrderByDescending(x => x.latestDate).ToList();
            return new { list = output };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        [NonAction]
        public void ForcedOffline(string connectionId)
        {
            var onlineUser = WebSocketClientCollection._clients.Find(q => q.ConnectionId == connectionId);
            if (onlineUser != null)
            {
                onlineUser.SendMessageAsync(new { method = "logout", msg = "此账号已在其他地方登陆" }.Serialize());
                WebSocketClientCollection._clients.RemoveAll((x) => x.ConnectionId == connectionId);
            }
        }
    }
}
