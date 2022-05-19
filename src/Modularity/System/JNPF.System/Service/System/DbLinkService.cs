using JNPF.Basics.Models.PlatForm.Dtos.DbLink;
using JNPF.Common.Enum;
using JNPF.Common.Filter;
using JNPF.Common.Util;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.System.Entitys.Dto.System.DbLink;
using JNPF.System.Entitys.Entity.System;
using JNPF.System.Entitys.Permission;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace JNPF.System.Core.Service.DbLink
{
    /// <summary>
    /// 数据连接
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "DataSource", Order = 205)]
    [Route("api/system/[controller]")]
    public class DbLinkService : IDbLinkService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<DbLinkEntity> _dbLinkRepository;
        private readonly IDictionaryDataService _dictionaryDataService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbLinkRepository"></param>
        /// <param name="dictionaryDataService"></param>
        public DbLinkService(ISqlSugarRepository<DbLinkEntity> dbLinkRepository, IDictionaryDataService dictionaryDataService)
        {
            _dbLinkRepository = dbLinkRepository;
            _dictionaryDataService = dictionaryDataService;
        }

        #region GET
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList_Api([FromQuery] KeywordInput input)
        {
            var data = await GetList();
            //数据库分类
            var dbTypeList = (await _dictionaryDataService.GetList("dbType")).FindAll(x=>x.EnabledMark==1);
            if (!string.IsNullOrEmpty(input.keyword))
            {
                data = data.FindAll(t => t.fullName.ToLower().Contains(input.keyword.ToLower()) || t.host.ToLower().Contains(input.keyword.ToLower()));
            }
            var result = new List<DbLinkListOutput>();
            result.Add(new DbLinkListOutput()
            {
                id = "-1",
                parentId = "0",
                fullName = "未分类",
                host = "",
                num = data.FindAll(x => x.parentId == null).Count
            });
            foreach (var item in dbTypeList)
            {
                var index = data.FindAll(x => x.dbType.Equals(item.EnCode)).Count;
                if (index>0)
                {
                    result.Add(new DbLinkListOutput()
                    {
                        id = item.Id,
                        fullName = item.FullName,
                        host = "",
                        num = index
                    });
                }
            }
            var treeList = result.Union(data).ToList();
            if (!string.IsNullOrEmpty(input.keyword))
            {
                treeList = treeList.TreeWhere(t => t.fullName.ToLower().Contains(input.keyword.ToLower()) || t.host.ToLower().Contains(input.keyword.ToLower()), t => t.id, t => t.parentId);
            }
            return new { list = treeList.ToTree() };
        }

        /// <summary>
        /// 下拉框列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> GetSelector()
        {
            var data = (await GetList()).FindAll(m => m.enabledMark == 1).Adapt<List<DbLinkSelectorOutput>>();
            //数据库分类
            var dbTypeList = (await _dictionaryDataService.GetList("dbType")).FindAll(x => x.EnabledMark == 1);
            var output = new List<DbLinkSelectorOutput>();
            output.Add(new DbLinkSelectorOutput()
            {
                id = "-1",
                parentId = "0",
                fullName = "未分类",
                num = data.FindAll(x => x.parentId == null).Count
            });
            foreach (var item in dbTypeList)
            {
                var index = data.FindAll(x => x.dbType.Equals(item.EnCode)).Count;
                if (index > 0)
                {
                    output.Add(new DbLinkSelectorOutput()
                    {
                        id = item.Id,
                        fullName = item.FullName
                    });
                }
            }
            var treeList = output.Union(data).ToList().ToTree();
            return new { list = treeList };
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            var data = await GetInfo(id);
            var output = data.Adapt<DbLinkInfoOutput>();
            return output;
        }
        #endregion

        #region POST
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete_Api(string id)
        {
            var entity = await GetInfo(id);
            if (entity == null)
                throw JNPFException.Oh(ErrorCode.COM1005);
            var isOk = await Delete(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create_Api([FromBody] DbLinkCrInput input)
        {
            if (await _dbLinkRepository.AnyAsync(x => x.FullName == input.fullName && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<DbLinkEntity>();
            var isOk = await Create(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update_Api(string id, [FromBody] DbLinkUpInput input)
        {
            if (await _dbLinkRepository.AnyAsync(x => x.Id != id && x.FullName == input.fullName && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<DbLinkEntity>();
            var isOk = await Update(entity);
            if (isOk < 0)
                throw JNPFException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("Actions/Test")]
        public void TestDbConnection([FromBody] DbLinkActionsTestInput input)
        {
            var entity = input.Adapt<DbLinkEntity>();
            entity.Id = input.id.Equals("0") ? YitIdHelper.NextId().ToString() : input.id;
            var flag = TestDbConnection(entity);
            if (!flag)
                throw JNPFException.Oh(ErrorCode.D1507);
        }

        #endregion

        #region PublicMethod
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<DbLinkListOutput>> GetList()
        {
            var list = await _dbLinkRepository.Context.Queryable<DbLinkEntity, UserEntity, UserEntity, DictionaryDataEntity, DictionaryTypeEntity>(
                (a, b, c, d, e) => new JoinQueryInfos(
                    JoinType.Left, a.CreatorUserId == b.Id,
                    JoinType.Left, a.LastModifyUserId == c.Id,
                    JoinType.Left, a.DbType == d.EnCode && d.DeleteMark==null,
                    JoinType.Left, d.DictionaryTypeId == e.Id && e.EnCode == "dbType")).
                    Select((a, b, c, d) => new 
                    {
                        id = a.Id,
                        parentId = d.Id==null?"-1":d.Id,
                        creatorTime = a.CreatorTime,
                        creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account),
                        dbType = a.DbType,
                        enabledMark = a.EnabledMark,
                        fullName = a.FullName,
                        host = a.Host,
                        lastModifyTime = a.LastModifyTime,
                        lastModifyUser = SqlFunc.MergeString(c.RealName, "/", c.Account),
                        port = a.Port.ToString(),
                        sortCode = a.SortCode,
                        deleteMark=a.DeleteMark
                    }).MergeTable().Where(x=>x.deleteMark==null).Distinct().OrderBy(o => o.sortCode).OrderBy(o => o.creatorTime, OrderByType.Desc).Select<DbLinkListOutput>().ToListAsync();
            return list;
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [NonAction]
        public async Task<DbLinkEntity> GetInfo(string id)
        {
            return await _dbLinkRepository.FirstOrDefaultAsync(m => m.Id == id && m.DeleteMark == null);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(DbLinkEntity entity)
        {
            return await _dbLinkRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(DbLinkEntity entity)
        {
            return await _dbLinkRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(DbLinkEntity entity)
        {
            return await _dbLinkRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }
        #endregion

        #region PrivateMethod

        /// <summary>
        /// 测试连接串
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        private bool TestDbConnection(DbLinkEntity entity)
        {
            try
            {
                return App.GetService<IDataBaseService>().IsConnection(entity);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}
