using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Filter;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.Extend.Entitys;
using JNPF.Extend.Entitys.Dto.BigData;
using JNPF.FriendlyException;
using JNPF.LinqBuilder;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace JNPF.Extend
{
    /// <summary>
    /// 大数据测试
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "Extend", Name = "BigData", Order = 600)]
    [Route("api/extend/[controller]")]
    public class BigDataService: IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<BigDataEntity> _bigDataRepository;
        private readonly SqlSugarScope db;// 核心对象：拥有完整的SqlSugar全部功能

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bigDataRepository"></param>
        public BigDataService(ISqlSugarRepository<BigDataEntity> bigDataRepository)
        {
            _bigDataRepository = bigDataRepository;
            db = bigDataRepository.Context;
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
            var queryWhere = LinqExpression.And<BigDataEntity>();
            if (!string.IsNullOrEmpty(input.keyword))
                queryWhere = queryWhere.And(m => m.FullName.Contains(input.keyword) || m.EnCode.Contains(input.keyword));
            var list = await _bigDataRepository.Entities.Where(queryWhere).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<BigDataListOutput>()
            {
                list = list.list.Adapt<List<BigDataListOutput>>(),
                pagination = list.pagination
            };
            return PageResult<BigDataListOutput>.SqlSugarPageResult(pageList);
        }
        #endregion

        #region POST
        /// <summary>
        /// 新建
        /// </summary>
        /// <returns></returns>
        [HttpPost("")]
        public void Create()
        {
            var list = _bigDataRepository.Entities.ToList();
            var code = 0;
            if (list.Count>0)
            {
                code = list.Select(x => x.EnCode).ToList().Max().ToInt();
            }
            var index = code == 0 ? 10000001 : code;
            if (index > 11500001)
                throw JNPFException.Oh(ErrorCode.Ex0001);
            List<BigDataEntity> entityList = new List<BigDataEntity>();
            for (int i = 0; i < 1000000; i++)
            {
                entityList.Add(new BigDataEntity
                {
                    Id = YitIdHelper.NextId().ToString(),
                    EnCode = index.ToString(),
                    FullName = "测试大数据" + index,
                    CreatorTime = DateTime.Now,
                });
                index++;
            }
            Blukcopy(entityList);
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 大数据批量插入
        /// </summary>
        /// <param name="entityList"></param>
        private void Blukcopy(List<BigDataEntity> entityList)
        {
            try
            {
                var storageable = db.Storageable(entityList).SplitInsert(x => true).ToStorage();
                switch (db.CurrentConnectionConfig.DbType)
                {
                    case DbType.SqlServer:
                        storageable.AsInsertable.UseSqlServer().ExecuteBulkCopy();
                        break;
                    case DbType.MySql:
                        storageable.AsInsertable.UseMySql().ExecuteBulkCopy();
                        break;
                    case DbType.Oracle:
                        //db.Utilities.PageEach(entityList, 100, pageList =>
                        //{
                        //    storageable.AsInsertable.ExecuteCommand(); //执行插入 
                        //});
                        storageable.AsInsertable.UseOracle().ExecuteBulkCopy();
                        break;
                    default:
                        db.Utilities.PageEach(entityList, 1000, pageList =>
                        {
                            storageable.AsInsertable.ExecuteCommand(); //执行插入 
                        });
                        break;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
        #endregion
    }
}
