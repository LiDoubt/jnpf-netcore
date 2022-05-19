using JNPF.Common.Enum;
using JNPF.Common.Filter;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.System.Entitys.Dto.System.Province;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.System.Core.Service.Province
{
    /// <summary>
    /// 行政区划
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "Area", Order = 206)]
    [Route("api/system/[controller]")]
    public class ProvinceService : IProvinceService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ProvinceEntity> _provinceRepository;

        /// <summary>
        /// 初始化一个<see cref="ProvinceService"/>类型的新实例
        /// </summary>
        /// <param name="provinceRepository"></param>
        public ProvinceService(ISqlSugarRepository<ProvinceEntity> provinceRepository)
        {
            _provinceRepository = provinceRepository;
        }

        #region GET

        /// <summary>
        /// 获取行政区划列表
        /// </summary>
        /// <param name="nodeid">节点Id</param>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("{nodeId}")]
        public async Task<dynamic> GetList([FromQuery] KeywordInput input, string nodeid)
        {
            var data = await _provinceRepository.Entities.Where(m => m.ParentId == nodeid && m.DeleteMark == null).WhereIF(!string.IsNullOrEmpty(input.keyword), t => t.EnCode.Contains(input.keyword) || t.FullName.Contains(input.keyword)).OrderBy(o => o.SortCode).ToListAsync();
            var output = data.Adapt<List<ProvinceListOutput>>();
            foreach (var item in output)
            {
                var flag = await GetExistsLeaf(item.id);
                item.isLeaf = flag;
                item.hasChildren = !flag;
            }
            return new { list = output };
        }

        /// <summary>
        /// 获取行政区划下拉框数据(异步)
        /// </summary>
        /// <param name="id">当前Id</param>
        /// <returns></returns>
        [HttpGet("{id}/Selector/{areaId}")]
        public async Task<dynamic> GetSelector(string id,string areaId)
        {
            var data = (await GetList(id)).FindAll(x=>x.EnabledMark==1);
            if (!areaId.Equals("0"))
            {
                data.RemoveAll(x => x.Id == areaId);
            }
            var output = data.Adapt<List<ProvinceSelectorOutput>>();
            foreach (var item in output)
            {
                item.isLeaf = await GetExistsLeaf(item.id);
            }
            return new { list = output };
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpGet("{id}/Info")]
        public async Task<dynamic> GetInfo(string id)
        {
            var data = (await _provinceRepository.FirstOrDefaultAsync(m => m.Id == id && m.DeleteMark == null)).Adapt<ProvinceInfoOutput>();
            return data;
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
            var entity = await _provinceRepository.FirstOrDefaultAsync(m => m.Id == id && m.DeleteMark == null);
            if (entity == null || (await _provinceRepository.Entities.Where(m => m.ParentId == id && m.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync()).Count > 0)
                throw JNPFException.Oh(ErrorCode.D1007);
            var isOk = await _provinceRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ProvinceCrInput input)
        {
            if (await _provinceRepository.AnyAsync(x => x.EnCode == input.enCode && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<ProvinceEntity>();
            var isOk = await _provinceRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
            if (isOk <= 0) throw JNPFException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ProvinceUpInput input)
        {
            if (await _provinceRepository.AnyAsync(x => x.Id != id && x.EnCode == input.enCode && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<ProvinceEntity>();
            var isOk = await _provinceRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 0)
                throw JNPFException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 更新行政区划状态
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/State")]
        public async Task ActionsState(string id)
        {
            var entity = await _provinceRepository.FirstOrDefaultAsync(m => m.Id == id && m.DeleteMark == null);
            entity.EnabledMark = entity.EnabledMark == 0 ? 1 : 0;
            var isOk = await _provinceRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 0)
                throw JNPFException.Oh(ErrorCode.COM1003);
        }

        #endregion

        #region PulicMethod

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ProvinceEntity>> GetList()
        {
            var data = await _provinceRepository.Entities.OrderBy(o => o.SortCode).ToListAsync();
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ProvinceEntity>> GetList(string id)
        {
            return await _provinceRepository.Entities.Where(m => m.ParentId == id && m.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
        }

        #endregion

        #region PrivateMethod

        /// <summary>
        /// 是否存在子节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<bool> GetExistsLeaf(string id)
        {
            return (await _provinceRepository.Where(m => m.ParentId == id && m.DeleteMark == null).CountAsync()) > 0 ? false : true;
        }

        

        #endregion
    }
}