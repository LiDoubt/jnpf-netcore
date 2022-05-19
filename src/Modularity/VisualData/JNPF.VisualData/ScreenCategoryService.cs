using JNPF.Common.Enum;
using JNPF.Common.Filter;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.VisualData.Entity;
using JNPF.VisualData.Entitys.Dto.ScreenCategory;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Linq;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace JNPF.VisualData
{
    /// <summary>
    /// 业务实现：大屏
    /// </summary>
    [ApiDescriptionSettings(Tag = "BladeVisual", Name = "category", Order = 160)]
    [Route("api/blade-visual/[controller]")]
    public class ScreenCategoryService : IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<VisualCategoryEntity> _visualCategoryRepository;

        /// <summary>
        /// 初始化一个<see cref="ScreenCategoryService"/>类型的新实例
        /// </summary>
        public ScreenCategoryService(ISqlSugarRepository<VisualCategoryEntity> visualCategoryRepository)
        {
            _visualCategoryRepository = visualCategoryRepository;
        }

        #region Get

        /// <summary>
        /// 获取大屏分类分页列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("page")]
        public async Task<dynamic> GetPagetList([FromQuery] ScreenCategoryListQueryInput input)
        {
            var data = await _visualCategoryRepository.Entities.Where(v => v.IsDeleted == "0").Select(v => new { id = v.Id, categoryKey = v.CategoryKey, categoryValue = v.CategoryValue, isDeleted = v.IsDeleted }).MergeTable().Select<ScreenCategoryListOutput>().ToPagedListAsync(input.current, input.size);
            return new { current = data.pagination.PageIndex, pages = data.pagination.Total / data.pagination.PageSize, records = data.list, size = data.pagination.PageSize, total = data.pagination.Total };
        }

        /// <summary>
        /// 获取大屏分类列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<dynamic> GetList([FromQuery] ScreenCategoryListQueryInput input)
        {
            var list = await _visualCategoryRepository.Entities.Where(v => v.IsDeleted == "0").Select(v => new { id = v.Id, categoryKey = v.CategoryKey, categoryValue = v.CategoryValue, isDeleted = v.IsDeleted }).MergeTable().Select<ScreenCategoryListOutput>().ToPagedListAsync(input.current, input.size);
            return list.list;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("detail")]
        public async Task<dynamic> GetInfo(string id)
        {
            var entity = await _visualCategoryRepository.SingleAsync(v => v.Id == id);
            var data = entity.Adapt<ScreenCategoryInfoOutput>();
            return data;
        }

        #endregion

        #region Post

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("save")]
        public async Task Create([FromBody] ScreenCategoryCrInput input)
        {
            var isExist = await _visualCategoryRepository.AnyAsync(v => v.CategoryValue == input.categoryValue && v.IsDeleted == "0");
            if (isExist) throw JNPFException.Oh(ErrorCode.D2200);
            var entity = input.Adapt<VisualCategoryEntity>();
            entity.IsDeleted = "0";
            entity.Id = YitIdHelper.NextId().ToString();
            var isOk = await _visualCategoryRepository.Context.Insertable(entity).IgnoreColumns(ignoreNullColumn: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("update")]
        public async Task Update([FromBody] ScreenCategoryUpInput input)
        {
            var isExist = await _visualCategoryRepository.AnyAsync(v => v.CategoryValue == input.categoryValue && v.Id != input.Id && v.IsDeleted == "0");
            if (isExist) throw JNPFException.Oh(ErrorCode.D2200);
            var entity = input.Adapt<VisualCategoryEntity>();
            var isOk = await _visualCategoryRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost("remove")]
        public async Task Delete(string ids)
        {
            var entity = await _visualCategoryRepository.Entities.In(v => v.Id, ids.Split(',').ToArray()).Where(v => v.IsDeleted == "0").ToListAsync();
            _ = entity ?? throw JNPFException.Oh(ErrorCode.COM1005);
            var isOk = await _visualCategoryRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.COM1002);
        }

        #endregion
    }
}
