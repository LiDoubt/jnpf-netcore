using JNPF.Common.Enum;
using JNPF.Common.Filter;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.JsonSerialization;
using JNPF.System.Entitys.Dto.System.ModuleColumn;
using JNPF.System.Entitys.Permission;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JNPF.System.Service.System
{
    /// <summary>
    /// 功能列表
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "ModuleColumn", Order = 213)]
    [Route("api/system/[controller]")]
    public class ModuleColumnService : IModuleColumnService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ModuleColumnEntity> _moduleColumnRepository; //系统功能按钮表仓储
        private readonly ISqlSugarRepository<AuthorizeEntity> _authorizeRepository; //权限操作表仓储
        private readonly ISqlSugarRepository<UserEntity> _userRepository; //用户仓储
        private readonly ISqlSugarRepository<RoleEntity> _roleRepository;

        /// <summary>
        /// 初始化一个<see cref="ModuleColumnService"/>类型的新实例
        /// </summary>
        /// <param name="moduleColumnRepository"></param>
        /// <param name="authorizeRepository"></param>
        /// <param name="userRepository"></param>
        /// <param name="roleRepository"></param>
        public ModuleColumnService(ISqlSugarRepository<ModuleColumnEntity> moduleColumnRepository, ISqlSugarRepository<AuthorizeEntity> authorizeRepository, ISqlSugarRepository<UserEntity> userRepository, ISqlSugarRepository<RoleEntity> roleRepository)
        {
            _moduleColumnRepository = moduleColumnRepository;
            _authorizeRepository = authorizeRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        #region GET

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="moduleId">功能主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("{moduleId}/Fields")]
        public async Task<dynamic> GetList(string moduleId, [FromQuery] KeywordInput input)
        {
            var list = await GetList(moduleId);
            if (!string.IsNullOrEmpty(input.keyword))
            {
                list = list.FindAll(t => t.EnCode.Contains(input.keyword) || t.FullName.Contains(input.keyword));
            }
            var treeList = list.Adapt<List<ModuleColumnListOutput>>();
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
            var output = data.Adapt<ModuleColumnInfoOutput>();
            return output;
        }
        #endregion

        #region POST
        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ModuleColumnCrInput input)
        {
            var entity = input.Adapt<ModuleColumnEntity>();
            var isOk = await Create(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ModuleColumnUpInput input)
        {
            var entity = input.Adapt<ModuleColumnEntity>();
            var isOk = await Update(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            if (await _moduleColumnRepository.AnyAsync(x => x.ParentId == id && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D1007);
            var entity = await GetInfo(id);
            if (entity == null)
                throw JNPFException.Oh(ErrorCode.COM1005);
            var isOk = await Delete(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 批量新建
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPost("Actions/Batch")]
        public async Task BatchCreate([FromBody] ModuleColumnActionsBatchInput input)
        {
            var entitys = new List<ModuleColumnEntity>();
            foreach (var item in input.columnJson)
            {
                var entity = input.Adapt<ModuleColumnEntity>();
                entity.EnCode = item.enCode;
                entity.FullName = item.fullName;
                entitys.Add(entity);
            }
            var newDic = await _moduleColumnRepository.Context.Insertable(entitys).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();
            _ = newDic ?? throw JNPFException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 更新字段状态
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/State")]
        public async Task ActionsState(string id)
        {
            var entity = await _moduleColumnRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
            entity.EnabledMark = entity.EnabledMark == 0 ? 1 : 0;
            var isOk = await Update(entity);
            if (isOk < 0)
                throw JNPFException.Oh(ErrorCode.COM1003);
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ModuleColumnEntity>> GetList()
        {
            return await _moduleColumnRepository.Entities.Where(x => x.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="moduleId">功能主键</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ModuleColumnEntity>> GetList(string moduleId)
        {
            return await _moduleColumnRepository.Entities.Where(x => x.DeleteMark == null && x.ModuleId == moduleId).OrderBy(o => o.SortCode).OrderBy(x=>x.CreatorTime,OrderByType.Desc).ToListAsync();
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [NonAction]
        public async Task<ModuleColumnEntity> GetInfo(string id)
        {
            return await _moduleColumnRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(ModuleColumnEntity entity)
        {
            return await _moduleColumnRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(ModuleColumnEntity entity)
        {
            return await _moduleColumnRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(ModuleColumnEntity entity)
        {
            return await _moduleColumnRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetUserModuleColumnList(bool isAdmin, string userId)
        {
            var output = new List<ModuleColumnOutput>();
            if (!isAdmin)
            {
                var role = _userRepository.FirstOrDefault(u => u.Id == userId).RoleId;
                if (!string.IsNullOrEmpty(role))
                {
                    var roleArray = role.Split(',');
                    var roleId = await _roleRepository.Entities.In(r => r.Id, roleArray).Where(r => r.EnabledMark.Equals(1) && r.DeleteMark == null).Select(r => r.Id).ToListAsync();
                    var items = await _authorizeRepository.Entities.In(a => a.ObjectId, roleId).Where(a => a.ItemType == "column").GroupBy(it => new { it.ItemId }).Select(it => new { it.ItemId }).ToListAsync();
                    if (items.Count == 0) return output;
                    var columns = await _moduleColumnRepository.Entities.In(a => a.Id, items.Select(it => it.ItemId).ToArray()).Where(a => a.EnabledMark.Equals(1) && a.DeleteMark == null).Select<ModuleColumnEntity>().OrderBy(q => q.SortCode,OrderByType.Asc).ToListAsync();
                    output = columns.Adapt<List<ModuleColumnOutput>>();
                }
            }
            else
            {
                var buttons = await _moduleColumnRepository.Where(a => a.EnabledMark == 1 && a.DeleteMark == null).Select<ModuleColumnEntity>().OrderBy(q => q.SortCode).ToListAsync();
                output = buttons.Adapt<List<ModuleColumnOutput>>();
            }
            return output;
        }

        #endregion
    }
}
