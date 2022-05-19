using JNPF.Message.Entitys;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.Message.Interfaces.Message
{
    /// <summary>
    /// 系统消息
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="type">类型(1:公告，2：im消息)</param>
        /// <returns></returns>
        Task<List<MessageEntity>> GetList(int type);

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        Task<MessageEntity> GetInfo(string id);

        /// <summary>
        /// 默认公告(app)
        /// </summary>
        /// <returns></returns>
        public string GetInfoDefaultNotice();

        /// <summary>
        /// 默认消息(app)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetInfoDefaultMessage(string userId);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        Task<int> Delete(MessageEntity entity);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        Task<int> Create(MessageEntity entity);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="receiveEntityList">收件用户</param>
        Task<int> Create(MessageEntity entity, List<MessageReceiveEntity> receiveEntityList);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        Task<int> Update(MessageEntity entity);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="receiveEntityList">收件用户</param>
        Task<int> Update(MessageEntity entity, List<MessageReceiveEntity> receiveEntityList);

        /// <summary>
        /// 消息已读（单条）
        /// </summary>
        /// <param name="userId">当前用户</param>
        /// <param name="messageId">消息主键</param>
        Task MessageRead(string userId, string messageId);

        /// <summary>
        /// 消息已读（全部）
        /// </summary>
        /// <param name="userId">当前用户</param>
        Task MessageRead(string userId);

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="userId">当前用户</param>
        /// <param name="messageIds">消息Id</param>
        Task<int> DeleteRecord(string userId, List<string> messageIds);

        /// <summary>
        /// 获取未读数量（含 通知公告、系统消息）
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        Task<int> GetUnreadCount(string userId);

        /// <summary>
        /// 获取公告未读数量
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        int GetUnreadNoticeCount(string userId);

        /// <summary>
        /// 获取消息未读数量
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        int GetUnreadMessageCount(string userId);

        /// <summary>
        /// 消息发送
        /// </summary>
        /// <param name="toUserIds"></param>
        /// <param name="title"></param>
        /// <param name="bodyText"></param>
        /// <returns></returns>
        Task SentMessage(List<string> toUserIds, string title, string bodyText = null);
    }
}
