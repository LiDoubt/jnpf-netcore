using JNPF.Dependency;
using JNPF.Message.Entitys.Model.IM;
using System.Collections.Generic;
using System.Linq;

namespace JNPF.Message.Extensions
{
    /// <summary>
    /// WebSocket客户端集合
    /// </summary>
    [SuppressSniffer]
    public class WebSocketClientCollection
    { 
        /// <summary>
        /// 在线用户
        /// </summary>
        public static List<WebSocketClient> _clients { get; set; } = new List<WebSocketClient>();

        /// <summary>
        /// locker
        /// </summary>
        public static readonly object locker = new object();

        /// <summary>
        /// 添加集合
        /// </summary>
        /// <param name="client"></param>
        public static void Add(WebSocketClient client)
        {
            _clients.Add(client);
        }

        /// <summary>
        /// 移除集合
        /// </summary>
        /// <param name="client"></param>
        public static void Remove(WebSocketClient client)
        {
            _clients.Remove(client);
        }

        /// <summary>
        /// 获取WebSocket客户端
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public static WebSocketClient Get(string clientId)
        {
            var client = _clients.FirstOrDefault(c => c.ConnectionId == clientId);
            return client;
        }

        /// <summary>
        /// 获取WebSocket客户端
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static WebSocketClient GetUser(string userId)
        {
            var client = _clients.FirstOrDefault(c => c.UserId == userId);
            return client;
        }
    }
}
