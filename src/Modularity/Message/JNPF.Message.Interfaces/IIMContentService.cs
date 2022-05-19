using JNPF.Common.Filter;
using JNPF.Message.Entitys;
using JNPF.Message.Entitys.Model.IM;
using System.Collections.Generic;

namespace JNPF.Message.Interfaces.Message
{
    /// <summary>
    /// 聊天内容
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IIMContentService
    {
        /// <summary>
        /// 获取消息列表
        /// </summary>
        /// <param name="sendUserId"></param>
        /// <param name="receiveUserId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        dynamic GetMessageList(string sendUserId, string receiveUserId, PageInputBase input);

        /// <summary>
        /// 获取未读消息
        /// </summary>
        /// <param name="receiveUserId"></param>
        /// <returns></returns>
        List<IMUnreadNumModel> GetUnreadList(string receiveUserId);

        /// <summary>
        /// 获取未读消息
        /// </summary>
        /// <param name="receiveUserId">接收者</param>
        /// <returns></returns>
        int GetUnreadCount(string receiveUserId);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sendUserId">发送者</param>
        /// <param name="receiveUserId">接收者</param>
        /// <param name="message">消息内容</param>
        /// <param name="messageType">消息类型</param>
        /// <returns></returns>
        int SendMessage(string sendUserId, string receiveUserId, string message, string messageType);

        /// <summary>
        /// 已读消息
        /// </summary>
        /// <param name="sendUserId">发送者</param>
        /// <param name="receiveUserId">接收者</param>
        int ReadMessage(string sendUserId, string receiveUserId);
    }
}
