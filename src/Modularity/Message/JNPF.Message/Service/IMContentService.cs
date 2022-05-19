using JNPF.Common.Filter;
using JNPF.Dependency;
using JNPF.LinqBuilder;
using JNPF.Message.Entitys;
using JNPF.Message.Entitys.Dto.IM;
using JNPF.Message.Entitys.Model.IM;
using JNPF.Message.Interfaces.Message;
using Mapster;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using Yitter.IdGenerator;

namespace JNPF.Message.Service.Message
{
    /// <summary>
    /// 聊天内容
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public class IMContentService : IIMContentService, ITransient
    {
        private readonly ISqlSugarRepository<IMContentEntity> _IMContentRepository;
        private readonly ISqlSugarRepository<ImReplyEntity> _imReplyRepository;

        /// <summary>
        /// 
        /// </summary>
        public IMContentService(ISqlSugarRepository<IMContentEntity> IMContentRepository, ISqlSugarRepository<ImReplyEntity> imReplyRepository)
        {
            _IMContentRepository = IMContentRepository;
            _imReplyRepository = imReplyRepository;
        }

        /// <summary>
        /// 获取消息列表
        /// </summary>
        /// <param name="input">分页参数</param>
        /// <param name="sendUserId">发送者</param>
        /// <param name="receiveUserId">接收者</param>
        /// <returns></returns>
        public dynamic GetMessageList(string sendUserId, string receiveUserId, PageInputBase input)
        {
            var orderByType = input.sort == "asc" ? OrderByType.Asc : OrderByType.Desc;
            var list = _IMContentRepository.Entities.Select(it => new IMContentListOutput { id = it.Id, sendUserId = it.SendUserId, sendTime = it.SendTime, receiveUserId = it.ReceiveUserId, receiveTime = it.ReceiveTime, content = it.Content, contentType = it.ContentType, state = it.State }).MergeTable()
                .OrderBy(it => it.sendTime, orderByType)
                .WhereIF(!string.IsNullOrEmpty(input.keyword), it => it.content.Contains(input.keyword))
                .Where(i => (i.sendUserId == sendUserId && i.receiveUserId == receiveUserId) || (i.sendUserId == receiveUserId && i.receiveUserId == sendUserId))
                .ToPagedList(input.currentPage, input.pageSize);
            if (input.sort == "desc")
            {
                list.list = list.list.ToList().OrderBy(it => it.sendTime);
            }
            return PageResult<IMContentListOutput>.SqlSugarPageResult(list);
        }

        /// <summary>
        /// 获取未读消息条数
        /// </summary>
        /// <param name="receiveUserId">接收者</param>
        /// <returns></returns>
        public int GetUnreadCount(string receiveUserId)
        {
            return _IMContentRepository.Entities.Where(x => x.State == 0 && x.ReceiveUserId == receiveUserId).ToList().Count;
        }

        /// <summary>
        /// 获取未读消息内容
        /// </summary>
        /// <param name="receiveUserId">接收者</param>
        /// <returns></returns>
        public List<IMUnreadNumModel> GetUnreadList(string receiveUserId)
        {
            var list = _IMContentRepository.Context.Queryable<IMContentEntity>().Where(x => x.ReceiveUserId == receiveUserId).Select(x => new IMContentEntity
            {
                State = SqlFunc.AggregateSum(SqlFunc.IIF(x.State == 0, 1, 0)),
                SendUserId = x.SendUserId,
                ReceiveUserId = x.ReceiveUserId
            }).GroupBy(x => new { x.SendUserId, x.ReceiveUserId }).MergeTable().ToList().Where(x => x.State > 0).ToList();
            var list1 = _IMContentRepository.Entities.Where(x => x.ReceiveUserId == receiveUserId).OrderBy(x => x.SendTime, OrderByType.Desc).ToList();
            var output = list.Adapt<List<IMUnreadNumModel>>();
            foreach (var item in output)
            {
                var entity = list1.FirstOrDefault(x => x.SendUserId == item.sendUserId);
                item.defaultMessage = entity.Content;
                item.defaultMessageType = entity.ContentType;
                item.defaultMessageTime = entity.SendTime.ToString(); ;
            }
            return output;
        }

        /// <summary>
        /// 已读消息
        /// </summary>
        /// <param name="sendUserId"></param>
        /// <param name="receiveUserId"></param>
        /// <returns></returns>
        public int ReadMessage(string sendUserId, string receiveUserId)
        {
            return _IMContentRepository.Context.Updateable<IMContentEntity>().SetColumns(x => x.State == 1).SetColumns(x => x.ReceiveTime == DateTime.Now).Where(x => x.State == 0 && x.SendUserId == sendUserId && x.ReceiveUserId == receiveUserId).ExecuteCommand();
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sendUserId"></param>
        /// <param name="receiveUserId"></param>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public int SendMessage(string sendUserId, string receiveUserId, string message, string messageType)
        {
            IMContentEntity entity = new IMContentEntity();
            entity.Id = YitIdHelper.NextId().ToString();
            entity.SendUserId = sendUserId;
            entity.SendTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            entity.ReceiveUserId = receiveUserId;
            entity.State = 0;
            entity.Content = message;
            entity.ContentType = messageType;

            //写入到会话表中
            var isExist = _imReplyRepository.Any(it => it.UserId == sendUserId && it.ReceiveUserId == receiveUserId);
            if (isExist)
            {
                var imReplyEntity = _imReplyRepository.Single(it => it.UserId == sendUserId && it.ReceiveUserId == receiveUserId);
                imReplyEntity.ReceiveTime = entity.SendTime;
                _imReplyRepository.Context.Updateable(imReplyEntity).ExecuteCommand();
            }
            else
            {
                var imReplyEntity = new ImReplyEntity()
                {
                    Id = YitIdHelper.NextId().ToString(),
                    UserId = sendUserId,
                    ReceiveUserId = receiveUserId,
                    ReceiveTime = entity.SendTime
                };
                _imReplyRepository.Context.Insertable(imReplyEntity).ExecuteCommand();
            }

            return _IMContentRepository.Insert(entity);
        }
    }
}
