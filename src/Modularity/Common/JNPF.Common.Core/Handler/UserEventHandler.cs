using JNPF.Common.Configuration;
using JNPF.EventBridge;
using JNPF.System.Entitys.Dto.Permission.User;
using JNPF.System.Entitys.Permission;
using SqlSugar;
using System;

namespace JNPF.Common.Core.Handler
{
    /// <summary>
    /// 用户事件处理
    /// </summary>
    [EventHandler]
    public class UserEventHandler : IEventHandler
    {
        private readonly ISqlSugarRepository<UserEntity> _userRepository;
        private readonly SqlSugarScope _db;
        private readonly ITenant _tenant;

        public UserEventHandler(ISqlSugarRepository<UserEntity> userRepository)
        {
            _userRepository = userRepository;
            _db = _userRepository.Context;
            _tenant = _db;
        }

        /// <summary>
        /// 修改用户登录信息
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="payload"></param>
        [EventMessage]
        public void UpdateUserLoginInfo(EventMessage<UserEventDealWithInput> eventPayload)
        {
            UserEventDealWithInput payload = eventPayload.Payload;
            if (KeyVariable.MultiTenancy)
            {
                _tenant.AddConnection(new ConnectionConfig()
                {
                    DbType = SqlSugar.DbType.SqlServer,
                    ConfigId = payload.tenantId,//设置库的唯一标识
                    IsAutoCloseConnection = true,
                    ConnectionString = string.Format($"{App.Configuration["ConnectionStrings:DefaultConnection"]}", payload.tenantDbName)
                });
                _tenant.ChangeDatabase(payload.tenantId);
            }
            _db.Updateable(payload.entity).UpdateColumns(m => new { m.FirstLogIP, m.FirstLogTime, m.PrevLogTime, m.PrevLogIP, m.LastLogTime, m.LastLogIP, m.LogSuccessCount }).ExecuteCommand();
        }
    }
}
