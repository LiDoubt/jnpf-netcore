using JNPF.Common.Configuration;
using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Filter;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.LinqBuilder;
using JNPF.System.Entitys.Dto.System.DbBackup;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace JNPF.System.Core.Service.DbBackup
{
    /// <summary>
    /// 数据备份
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "DataBackup", Order = 207)]
    [Route("api/system/[controller]")]
    public class DbBackupService : IDbBackupService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<DbBackupEntity> _dbBackupRepository;
        private readonly IUserManager _userManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbBackupRepository"></param>
        /// <param name="userManager"></param>
        public DbBackupService(ISqlSugarRepository<DbBackupEntity> dbBackupRepository, IUserManager userManager)
        {
            _dbBackupRepository = dbBackupRepository;
            _userManager = userManager;
        }

        #region GET
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] PageInputBase input)
        {
            var queryWhere = LinqExpression.And<DbBackupEntity>();
            if (!string.IsNullOrEmpty(input.keyword))
                queryWhere = queryWhere.And(m => m.FileName.Contains(input.keyword) || m.FilePath.Contains(input.keyword));
            var list = await _dbBackupRepository.Entities.Where(queryWhere).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<DbBackupListOutput>()
            {
                list = list.list.Adapt<List<DbBackupListOutput>>(),
                pagination = list.pagination
            };
            return PageResult<DbBackupListOutput>.SqlSugarPageResult(pageList);
        }
        #endregion

        #region POST
        /// <summary>
        /// 创建备份(不支持跨库备份)
        /// </summary>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create()
        {
            await DbBackup();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _dbBackupRepository.FirstOrDefaultAsync(m => m.Id == id && m.DeleteMark == null);
            if (entity == null)
                throw JNPFException.Oh(ErrorCode.COM1005);
            await _dbBackupRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 备份数据
        /// </summary>
        private async Task DbBackup()
        {
            var fileName = YitIdHelper.NextId().ToString() + ".bak";
            var filePath = FileVariable.DataBackupFilePath + fileName;
            //备份数据
            var dataBase = $"{App.Configuration["ConnectionStrings:DBName"]}";
            _dbBackupRepository.Context.DbMaintenance.BackupDataBase(dataBase, filePath);
            //备份记录
            DbBackupEntity entity = new DbBackupEntity();
            entity.FileName = fileName;
            entity.FilePath = "/api/Common/Download?encryption=" + _userManager.UserId + "|" + fileName + "|dataBackup";
            entity.FileSize = new FileInfo(filePath).Length.ToString();
            await _dbBackupRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync(); ;
        }

        /// <summary>
        /// 还原
        /// </summary>
        /// <param name="disk">路径</param>
        private void DbRestore(string disk)
        {
            var dataBase = $"{App.Configuration["ConnectionStrings:DBName"]}";
            _dbBackupRepository.Context.DbMaintenance.CreateDatabase(dataBase, disk);
        }
        #endregion
    }
}
