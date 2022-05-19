using JNPF.Common.Enum;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.System.Entitys.Dto.System.ComFields;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.System.Service.System
{
    /// <summary>
    /// 常用字段
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "CommonFields", Order = 201)]
    [Route("api/system/[controller]")]
    public class ComFieldsService : IComFieldsService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ComFieldsEntity> _comFieldsRepository;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="comFieldsRepository"></param>
        public ComFieldsService(ISqlSugarRepository<ComFieldsEntity> comFieldsRepository)
        {
            _comFieldsRepository = comFieldsRepository;
        }

        #region Get
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">请求参数</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            var data = await GetInfo(id);
            var output = data.Adapt<ComFieldsInfoOutput>();
            return output;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList_Api()
        {
            var data = await GetList();
            var output = data.Adapt<List<ComFieldsListOutput>>();
            return new { list = output };
        }
        #endregion

        #region Post
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create_Api([FromBody] ComFieldsCrInput input)
        {
            if (await _comFieldsRepository.AnyAsync(x => x.Field.ToLower() == input.field.ToLower() && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<ComFieldsEntity>();
            var isOk = await Create(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">请求参数</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete_Api(string id)
        {
            var entity = await _comFieldsRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
            if (entity == null)
                throw JNPFException.Oh(ErrorCode.COM1005);
            var isOk = await Delete(entity);
            if (isOk < 0)
                throw JNPFException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update_Api(string id, [FromBody] ComFieldsUpInput input)
        {
            if (await _comFieldsRepository.AnyAsync(x =>x.Id!=id&&x.Field.ToLower() == input.field.ToLower() && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<ComFieldsEntity>();
            var isOk = await Update(entity);
            if (isOk < 0)
                throw JNPFException.Oh(ErrorCode.COM1001);
        }
        #endregion

        #region PulicMethod
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ComFieldsEntity>> GetList()
        {
            return await _comFieldsRepository.Entities.Where(x => x.DeleteMark == null).OrderBy(x => x.SortCode).ToListAsync();
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<ComFieldsEntity> GetInfo(string id)
        {
            return await _comFieldsRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(ComFieldsEntity entity)
        {
            return await _comFieldsRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(ComFieldsEntity entity)
        {
            return await _comFieldsRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(ComFieldsEntity entity)
        {
            return await _comFieldsRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }
        #endregion
    }
}
