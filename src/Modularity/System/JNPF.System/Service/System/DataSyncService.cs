using JNPF.Common.Enum;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.System.Entitys.Dto.System.DataSync;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.System;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace JNPF.System.Core.Service.DataSync
{
    /// <summary>
    /// 数据同步
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "DataSync", Order = 209)]
    [Route("api/system/[controller]")]
    public class DataSyncService : IDataSyncService, IDynamicApiController, ITransient
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IDbLinkService _dbLinkService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataBaseService"></param>
        /// <param name="dbLinkService"></param>
        public DataSyncService(IDataBaseService dataBaseService, IDbLinkService dbLinkService)
        {
            _dataBaseService = dataBaseService;
            _dbLinkService = dbLinkService;
        }

        /// <summary>
        /// 同步判断
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<dynamic> Estimate([FromBody] DbSyncActionsExecuteInput input)
        {
            var linkFrom = await _dbLinkService.GetInfo(input.dbConnectionFrom);
            var linkTo = await _dbLinkService.GetInfo(input.dbConnectionTo);
            if (!IsNullDataByTable(linkFrom, input.dbTable))
            {
                //初始表有数据
                return 1;
            }else if(!_dataBaseService.IsAnyTable(linkTo, input.dbTable))
            {
                //目的表不存在
                return 2;
            }else if(IsNullDataByTable(linkTo, input.dbTable))
            {
                //目的表有数据
                return 3;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 执行同步
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPost("Actions/Execute")]
        public async Task Execute([FromBody] DbSyncActionsExecuteInput input)
        {
            var linkFrom = await _dbLinkService.GetInfo(input.dbConnectionFrom);
            var linkTo = await _dbLinkService.GetInfo(input.dbConnectionTo);
            _dataBaseService.SyncTable(linkFrom, linkTo, input.dbTable, input.type);
            var isOk = ImportTableData(linkFrom, linkTo, input.dbTable);
            if (!isOk)
                throw JNPFException.Oh(ErrorCode.COM1006);
        }

        #region PrivateMethod
        /// <summary>
        /// 判断表中是否有数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private bool IsNullDataByTable(DbLinkEntity entity, string table)
        {
            var data = _dataBaseService.GetData(entity, table);
            if (data.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 批量写入
        /// </summary>
        /// <param name="linkFrom">数据库连接 From</param>
        /// <param name="linkTo">数据库连接To</param>
        /// <param name="table"></param>
        private bool ImportTableData(DbLinkEntity linkFrom, DbLinkEntity linkTo, string table)
        {
            try
            {
                //取同步数据
                var syncData = _dataBaseService.GetData(linkFrom, table);
                //插入同步数据
                var isOk = _dataBaseService.SyncData(linkTo, syncData, table);
                return isOk;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}
