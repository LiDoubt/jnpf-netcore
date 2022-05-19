using JNPF.Common.Enum;
using JNPF.Common.Filter;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.JsonSerialization;
using JNPF.System.Entitys.Dto.System.Map;
using JNPF.System.Entitys.Permission;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.System.Core.Service.Map
{
    /// <summary>
    /// 地图管理
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "DataMap", Order = 208)]
    [Route("api/system/[controller]")]
    public class MapService: IMapService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<MapEntity> _mapRepository;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapRepository"></param>
        public MapService(ISqlSugarRepository<MapEntity> mapRepository)
        {
            _mapRepository = mapRepository;
        }

        #region GET
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns>返回列表</returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] PageInputBase input)
        {
            var list = await _mapRepository.Context.Queryable<MapEntity, UserEntity>((a, b) => new JoinQueryInfos(JoinType.Left, b.Id == a.CreatorUserId)).Select((a, b) => new { CreatorTime = a.CreatorTime, CreatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account), EnCode = a.EnCode, EnabledMark = a.EnabledMark, FullName = a.FullName, Id = a.Id, SortCode = a.SortCode,DeleteMark=a.DeleteMark }).MergeTable().Where(x=>x.DeleteMark==null).WhereIF(!string.IsNullOrEmpty(input.keyword), m => m.FullName.Contains(input.keyword) || m.EnCode.Contains(input.keyword)).OrderBy(t => t.SortCode).Select<MapListOutput>().ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<MapListOutput>()
            {
                list = list.list,
                pagination = list.pagination
            };
            return PageResult<MapListOutput>.SqlSugarPageResult(pageList);
        }

        /// <summary>
        /// 下拉列表
        /// </summary>
        [HttpGet("Selector")]
        public async Task<dynamic> GetSelector()
        {
            var data = await _mapRepository.Entities.Where(x=>x.DeleteMark==null).OrderBy(o => o.SortCode).ToListAsync();
            var output = data.Adapt<List<MapSelectorOutput>>();
            return new { list = output };
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns>返回对象</returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var data = (await _mapRepository.FirstOrDefaultAsync(x => x.Id == id&&x.DeleteMark==null)).Adapt<MapInfoOutput>();
            return data;
        }

        /// <summary>
        /// 获取地图数据
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>返回列表</returns>
        [HttpGet("{id}/Data")]
        public async Task<object> GetMapData(string id)
        {
            var entity = await _mapRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
            if (entity != null)
            {
                return JSON.Deserialize<object>(entity.Data);
            }
            return new { data = new List<object>() };
        }
        #endregion

        #region POST
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _mapRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
            if (entity == null)
                throw JNPFException.Oh(ErrorCode.D1002);
            var isOk = await _mapRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
            if (isOk < 0)
                throw JNPFException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] MapCrInput input)
        {
            var entity = input.Adapt<MapEntity>();
            var newDic = await _mapRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();
            _ = newDic ?? throw JNPFException.Oh(ErrorCode.D1507);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] MapUpInput input)
        {
            var entity = input.Adapt<MapEntity>();
            var isOk = await _mapRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 0)
                throw JNPFException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/State")]
        public async Task ActionsState(string id)
        {
            var entity = await _mapRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
            entity.EnabledMark = entity.EnabledMark == 1 ? 0 : 1;
            var isOk = await _mapRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
            if (isOk < 0)
                throw JNPFException.Oh(ErrorCode.D1506);
        }
        #endregion
    }
}
