using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Serilog;
using System.Threading;
using System.Text;
using UAParser;
using JNPF.Message.Entitys.Model.IM;
using JNPF.Message.Entitys.Dto.IM;
using JNPF.Message.Interfaces.Message;
using System.Linq;
using JNPF.JsonSerialization;
using JNPF.Common.Const;
using JNPF.DataEncryption;
using JNPF.Common.Extension;
using JNPF.Dependency;
using JNPF.Common.Configuration;
using System.IO;
using JNPF.Common.Helper;
using JNPF.Common.Filter;
using JNPF.System.Interfaces.Permission;
using System.Collections.Generic;
using JNPF.System.Interfaces.System;

namespace JNPF.Message.Extensions
{
    /// <summary>
    /// WebSocket中间件
    /// </summary>
    public class WebSocketHandlerMiddleware
    {
        /// <summary>
        /// 下一级管道
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// 缓冲区大小
        /// </summary>
        private const int bufferSize = 1024 * 8;

        /// <summary>
        /// URL地址后缀
        /// </summary>
        private const string routePostfix = "/api/message/websocket";

        /// <summary>
        /// 初始化一个<see cref="WebSocketHandlerMiddleware"/>类型的新实例
        /// </summary>
        public WebSocketHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 调用
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            if (!IsWebSocket(context))
            {
                await _next.Invoke(context);
                return;
            }

            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            while (webSocket.State == WebSocketState.Open)
            {
                string clientId = Guid.NewGuid().ToString();
                var wsClient = new WebSocketClient
                {
                    ConnectionId = clientId,
                    WebSocket = webSocket,
                    LoginIpAddress = App.HttpContext.GetRemoteIpAddressToIPv4(),
                    LoginPlatForm = Parser.GetDefault().Parse(context.Request.Headers["User-Agent"]).String
                };
                try
                {
                    await Handle(wsClient);
                }
                catch (Exception ex)
                {
                    WebSocketClientCollection.Remove(wsClient);
                    Log.Error(ex, "Echo websocket client {0} err.", clientId);
                    await context.Response.WriteAsync("closed");
                    Log.Information($"Websocket client closed.");
                }
            }
        }

        private async Task Handle(WebSocketClient webSocket)
        {
            WebSocketClientCollection.Add(webSocket);
            Log.Information($"Websocket client added.");

            WebSocketReceiveResult result = null;
            do
            {
                var buffer = new ArraySegment<byte>(new byte[bufferSize]);
                result = await webSocket.WebSocket.ReceiveAsync(buffer, CancellationToken.None);
                while (!result.EndOfMessage)
                {
                    result = await webSocket.WebSocket.ReceiveAsync(buffer, default(CancellationToken));
                }
                if (result.MessageType == WebSocketMessageType.Text && !result.CloseStatus.HasValue)
                {
                    var msgString = Encoding.UTF8.GetString(buffer.Array);
                    Log.Information($"Websocket client ReceiveAsync message {msgString}.");
                    var message = msgString.ToObject<MessageInput>();
                    message.sendClientId = webSocket.ConnectionId;
                    MessageRoute(message);
                }
            }
            while (!result.CloseStatus.HasValue);
            WebSocketClientCollection.Remove(webSocket);
            Log.Information($"Websocket client closed.");
        }

        private void MessageRoute(MessageInput message)
        {
            var client = WebSocketClientCollection.Get(message.sendClientId);
            var claims = JWTEncryption.ReadJwtToken(message.token.Replace("Bearer ", "").Replace("bearer ", ""))?.Claims;
            var userId = claims.FirstOrDefault(e => e.Type == ClaimConst.CLAINM_USERID)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                client.SendMessageAsync(new { method = "logout" }.Serialize());
            }

            switch (message.method)
            {
                case "OnConnection":
                    {
                        #region 建立连接

                        var isMobileDevice = message.mobileDevice;
                        //读取 Token，不含验证 
                        client.UserId = userId;
                        client.Account = claims.FirstOrDefault(e => e.Type == ClaimConst.CLAINM_ACCOUNT)?.Value;
                        client.UserName = claims.FirstOrDefault(e => e.Type == ClaimConst.CLAINM_REALNAME)?.Value;
                        client.TenantId = claims.FirstOrDefault(e => e.Type == ClaimConst.TENANT_ID)?.Value;
                        client.LoginTime = string.Format("{0:yyyy-MM-dd HH:mm}", Ext.GetDateTime(claims.FirstOrDefault(e => e.Type == "iat")?.Value + "000"));
                        client.LoginIpAddress = client.LoginIpAddress == "0.0.0.1" ? "127.0.0.1" : client.LoginIpAddress;
                        Scoped.Create((_, scope) =>
                        {
                            var services = scope.ServiceProvider;
                            var _userService = App.GetService<IUsersService>(services);
                            var userEntity = _userService.GetInfoByUserId(userId);
                            if (userEntity != null)
                                client.HeadIcon = "/api/file/Image/userAvatar/" + userEntity.HeadIcon;
                        });

                        //添加在线用户缓存与单体登录
                        Scoped.Create((_, scope) =>
                        {
                            var services = scope.ServiceProvider;
                            var _sysCacheService = App.GetService<ISysCacheService>(services);
                            var list = _sysCacheService.GetOnlineUserList(client.TenantId);
                            if (list == null)
                            {
                                list = new List<UserOnlineModel>();
                            }
                            var user = list.Find(it => it.tenantId == client.TenantId && it.userId == client.UserId);
                            if (user != null)
                            {
                                var onlineUser = WebSocketClientCollection._clients.Find(q => q.ConnectionId == user.connectionId);
                                if (onlineUser != null)
                                {
                                    onlineUser.SendMessageAsync(new { method = "logout", msg = "此账号已在其他地方登陆" }.Serialize());
                                    WebSocketClientCollection._clients.RemoveAll((x) => x.ConnectionId == user.connectionId);
                                }
                                list.RemoveAll((x) => x.connectionId == user.connectionId);
                            }
                            list.Add(new UserOnlineModel()
                            {
                                connectionId = client.ConnectionId,
                                userId = client.UserId,
                                account = client.Account,
                                userName = client.UserName,
                                lastTime = DateTime.Now,
                                lastLoginIp = client.LoginIpAddress,
                                tenantId = client.TenantId,
                                lastLoginPlatForm = client.LoginPlatForm
                            });
                            _sysCacheService.SetOnlineUserList(client.TenantId, list);
                        });

                        lock (WebSocketClientCollection.locker)
                        {
                            var onlineUserList = WebSocketClientCollection._clients.FindAll(q => q.TenantId == client.TenantId && q.IsMobileDevice == isMobileDevice);
                            #region 反馈信息给登录者

                            var onlineUsers = onlineUserList.Select(m => m.UserId).ToList();
                            var webOnlineUsers = onlineUserList.FindAll(q => q.IsMobileDevice == false).Select(m => m.UserId).ToList();

                            //从作用域内获取出IMessageService
                            Scoped.Create((_, scope) =>
                            {
                                var services = scope.ServiceProvider;

                                var _messageService = App.GetService<IMessageService>(services);   // services 传递进去
                                var _IMContentService = App.GetService<IIMContentService>(services);
                                var unreadNums = _IMContentService.GetUnreadList(userId);
                                var unreadNoticeCount = _messageService.GetUnreadNoticeCount(client.UserId);
                                var unreadMessageCount = _messageService.GetUnreadMessageCount(client.UserId);
                                var noticeDefaultText = _messageService.GetInfoDefaultNotice();
                                var messageDefaultText = _messageService.GetInfoDefaultMessage(userId);

                                client.SendMessageAsync(new { method = "initMessage", onlineUsers, unreadNums, unreadNoticeCount, noticeDefaultText, unreadMessageCount, messageDefaultText, dateTime = DateTime.Now }.Serialize());
                            });

                            #endregion

                            #region 通知所有在线用户，有用户在线

                            if (!webOnlineUsers.Contains(userId))
                            {
                                onlineUserList.ForEach(c =>
                                {
                                    c.SendMessageAsync(new { method = "online", userId = userId }.Serialize());
                                });
                            }

                            #endregion
                        }

                        #endregion
                    }
                    break;
                case "SendMessage":
                    {
                        #region 发送消息

                        var toUserId = message.toUserId;
                        var messageType = message.messageType;
                        var messageContent = message.messageContent;
                        var fileName = "";

                        if (messageType == "image")
                        {
                            var directoryPath = FileVariable.IMContentFilePath;
                            if (!Directory.Exists(directoryPath))
                                Directory.CreateDirectory(directoryPath);
                            var imageInput = messageContent.ToObeject<MessagetImageInput>();
                            fileName = imageInput.name;
                            //var filePath = directoryPath + fileName;
                            //FileHelper.MakeThumbnail(filePath, (directoryPath + "T" + fileName), 300, 300, "H", Path.GetExtension(fileName), 0, 0);
                        }
                        var onlineUser = WebSocketClientCollection._clients.FirstOrDefault(q => q.ConnectionId == client.ConnectionId);
                        var onlineToUser = WebSocketClientCollection._clients.FirstOrDefault(q => q.TenantId == onlineUser.TenantId && q.UserId == toUserId && q.IsMobileDevice == onlineUser.IsMobileDevice);
                        //将发送消息对象信息补全
                        var toAccount = string.Empty;
                        var toHeadIcon = string.Empty;
                        var toRealName = string.Empty;
                        if (onlineToUser != null)
                        {
                            toAccount = onlineToUser.Account;
                            toHeadIcon = onlineToUser.HeadIcon;
                            toRealName = onlineToUser.UserName;
                        }
                        else
                        {
                            Scoped.Create((_, scope) =>
                            {
                                var services = scope.ServiceProvider;
                                var _userService = App.GetService<IUsersService>(services);
                                var userEntity = _userService.GetInfoByUserId(userId);
                                toAccount = userEntity.Account;
                                toHeadIcon = "/api/file/Image/userAvatar/" + userEntity.HeadIcon;
                                toRealName = userEntity.RealName;
                            });
                        }
                        if (onlineUser != null)
                        {
                            #region saveMessage

                            if (messageType == "text")
                            {
                                Scoped.Create((_, scope) =>
                                {
                                    var services = scope.ServiceProvider;
                                    var _IMContentService = App.GetService<IIMContentService>(services);
                                    _IMContentService.SendMessage(onlineUser.UserId, toUserId, messageContent.ToString(), messageType);
                                });
                            }
                            else if (messageType == "image")
                            {
                                var imageInput = messageContent.ToObeject<MessagetImageInput>();
                                var toMessage = new { path = "/api/file/Image/IM/" + fileName, width = imageInput.width, height = imageInput.height };
                                Scoped.Create((_, scope) =>
                                {
                                    var services = scope.ServiceProvider;
                                    var _IMContentService = App.GetService<IIMContentService>(services);
                                    _IMContentService.SendMessage(onlineUser.UserId, toUserId, toMessage.ToJson(), messageType);
                                });
                            }
                            else if (messageType == "voice")
                            {
                                //var toMessage = new { path = fileName, length = receivedMessage["messageContent"]["length"].ToString() };
                                //await iMContentBll.SendMessage(onlineUser.UserId, toUserId, toMessage.ToJson(), messageType);
                            }

                            #endregion

                            #region sendMessage                            

                            if (messageType == "text")
                            {
                                client.SendMessageAsync(new { method = "sendMessage", onlineUser.UserId, account = onlineUser.Account, headIcon = onlineUser.HeadIcon, realName = onlineUser.UserName, toAccount, toHeadIcon, messageType, toUserId, toRealName, toMessage = messageContent, dateTime = DateTime.Now, latestDate = DateTime.Now }.Serialize());
                            }
                            else if (messageType == "image")
                            {
                                var imageInput = messageContent.ToObeject<MessagetImageInput>();
                                var toMessage = new { path = "/api/file/Image/IM/" + fileName, width = imageInput.width, height = imageInput.height };
                                client.SendMessageAsync(new { method = "sendMessage", onlineUser.UserId, account = onlineUser.Account, headIcon = onlineUser.HeadIcon, realName = onlineUser.UserName, toAccount, toHeadIcon, messageType, toUserId, toMessage, dateTime = DateTime.Now, latestDate = DateTime.Now }.Serialize());
                            }
                            else if (messageType == "voice")
                            {
                                //var toMessage = new { path = fileName, length = receivedMessage["messageContent"]["length"].ToString() };
                                //client.SendMessageAsync(new { method = "sendMessage", onlineUser.UserId, account = onlineUser.Account, headIcon = onlineUser.HeadIcon, realName = onlineUser.UserName, toAccount, toHeadIcon, messageType, toUserId, toMessage, dateTime = DateTime.Now }.Serialize());
                            }

                            #endregion
                        }
                        if (onlineToUser != null)
                        {
                            #region receiveMessage

                            if (messageType == "text")
                            {
                                onlineToUser.SendMessageAsync(new { method = "receiveMessage", messageType, formUserId = client.UserId, formMessage = messageContent, dateTime = DateTime.Now, latestDate = DateTime.Now, headIcon = onlineUser.HeadIcon, realName = onlineUser.UserName}.Serialize());
                            }
                            else if (messageType == "image")
                            {
                                var imageInput = messageContent.ToObeject<MessagetImageInput>();
                                var formMessage = new { path = "/api/file/Image/IM/" + fileName, width = imageInput.width, height = imageInput.height };
                                onlineToUser.SendMessageAsync(new { method = "receiveMessage", messageType, formUserId = onlineUser.UserId, formMessage, dateTime = DateTime.Now, latestDate = DateTime.Now, headIcon = onlineUser.HeadIcon, realName = onlineUser.UserName }.Serialize());
                            }
                            else if (messageType == "voice")
                            {
                                //var formMessage = new { path = fileName, length = receivedMessage["messageContent"]["length"].ToString() };
                                //SendAsync(onlineToUser.WebSocket, new { method = "receiveMessage", messageType, formUserId = onlineUser.UserId, formMessage, dateTime = DateTime.Now, latestDate = DateTime.Now, headIcon = onlineUser.HeadIcon, realName = onlineUser.UserName }.Serialize());
                            }

                            #endregion
                        }

                        #endregion
                    }
                    break;
                case "UpdateReadMessage":
                    {
                        var fromUserId = message.formUserId;
                        var onlineUser = WebSocketClientCollection._clients.FirstOrDefault(q => q.ConnectionId == client.ConnectionId);
                        //从作用域内获取出IMessageService
                        Scoped.Create((_, scope) =>
                        {
                            var services = scope.ServiceProvider;

                            var _IMContentService = App.GetService<IIMContentService>(services);
                            _IMContentService.ReadMessage(fromUserId, onlineUser.UserId);
                        });
                    }
                    break;
                case "MessageList":
                    {
                        var sendUserId = message.toUserId;                //发送者
                        var receiveUserId = message.formUserId;           //接收者
                        var requestParam = new PageInputBase();
                        requestParam.currentPage = message.currentPage;
                        requestParam.pageSize = message.pageSize;
                        requestParam.sort = message.sord;
                        requestParam.keyword = message.keyword;
                        //从作用域内获取出IMessageService
                        Scoped.Create((_, scope) =>
                        {
                            var services = scope.ServiceProvider;

                            var _IMContentService = App.GetService<IIMContentService>(services);
                            var data = _IMContentService.GetMessageList(sendUserId, receiveUserId, requestParam);
                            var list = data.GetType().GetProperty("list").GetValue(data, null);
                            var pagination = data.GetType().GetProperty("pagination").GetValue(data, null);
                            client.SendMessageAsync(new { method = "messageList", list = list, pagination = pagination }.Serialize());
                        });
                    }
                    break;
                default:
                    break;
            }
        }

        private static bool Base64ToFileAndSave(string fileName, string strInput)
        {
            fileName = fileName.Replace("/", "\\");
            bool bTrue = false;
            try
            {
                byte[] buffer = Convert.FromBase64String(strInput);
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
                bTrue = true;
            }
            catch (Exception ex)
            {
                Log.Error("Base64ToFileAndSave - OnError");
                Log.Error(ex.Message);
                bTrue = false;
            }
            return bTrue;
        }


        /// <summary>
        /// 当前请求是否为WebSocket
        /// </summary>
        /// <param name="context">Http上下文</param>
        /// <returns></returns>
        private bool IsWebSocket(HttpContext context)
        {
            return context.WebSockets.IsWebSocketRequest &&
                context.Request.Path == routePostfix;
        }
    }
}
