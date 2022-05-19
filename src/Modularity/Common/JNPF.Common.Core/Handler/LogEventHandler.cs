using JNPF.Common.Configuration;
using JNPF.EventBridge;
using JNPF.System.Entitys.Dto.System.SysLog;
using JNPF.System.Entitys.System;
using SqlSugar;

namespace JNPF.Common.Core.Handler
{
    /// <summary>
    /// 日记事件处理
    /// </summary>
    [EventHandler]
    public class LogEventHandler : IEventHandler
    {
        private readonly ISqlSugarRepository<SysLogEntity> _sysLogRepository;
        private readonly SqlSugarScope _db;
        private readonly ITenant _tenant;

        /// <summary>
        /// 构造函数
        /// </summary>
        public LogEventHandler(ISqlSugarRepository<SysLogEntity> sysLogRepository)
        {
            _sysLogRepository = sysLogRepository;
            _db = _sysLogRepository.Context;
            _tenant = _db;
        }

        /// <summary>
        /// 请求日志拦截
        /// </summary>
        [EventMessage]
        public void CreateOpLog(EventMessage<LogEventBridgeCrInput> eventPayload)
        {
            LogEventBridgeCrInput log = eventPayload.Payload;
            if (KeyVariable.MultiTenancy)
            {
                if (log.tenantId == null) return;
                _tenant.AddConnection(new ConnectionConfig()
                {
                    DbType = SqlSugar.DbType.SqlServer,
                    ConfigId = log.tenantId,//设置库的唯一标识
                    IsAutoCloseConnection = true,
                    ConnectionString = string.Format($"{App.Configuration["ConnectionStrings:DefaultConnection"]}", log.tenantDbName)
                });
                _tenant.ChangeDatabase(log.tenantId);
            }
            _db.Insertable(log.entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommand();
        }

        /// <summary>
        /// 全局异常处理
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="payload"></param>
        [EventMessage]
        public void CreateExLog(EventMessage<LogEventBridgeCrInput> eventPayload)
        {
            LogEventBridgeCrInput log = eventPayload.Payload;
            if (KeyVariable.MultiTenancy)
            {
                _tenant.AddConnection(new ConnectionConfig()
                {
                    DbType = SqlSugar.DbType.SqlServer,
                    ConfigId = log.tenantId,//设置库的唯一标识
                    IsAutoCloseConnection = true,
                    ConnectionString = string.Format($"{App.Configuration["ConnectionStrings:DefaultConnection"]}", log.tenantDbName)
                });
                _tenant.ChangeDatabase(log.tenantId);
            }
            _db.Insertable(log.entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommand();
        }

        /// <summary>
        /// 登录日记
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="payload"></param>
        [EventMessage]
        public void CreateVisLog(EventMessage<LogEventBridgeCrInput> eventPayload)
        {
            LogEventBridgeCrInput log = eventPayload.Payload;
            if (KeyVariable.MultiTenancy)
            {
                _tenant.AddConnection(new ConnectionConfig()
                {
                    DbType = SqlSugar.DbType.SqlServer,
                    ConfigId = log.tenantId,//设置库的唯一标识
                    IsAutoCloseConnection = true,
                    ConnectionString = string.Format($"{App.Configuration["ConnectionStrings:DefaultConnection"]}", log.tenantDbName)
                });
                _tenant.ChangeDatabase(log.tenantId);
            }
            _db.Insertable(log.entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommand();
        }
    }
}
