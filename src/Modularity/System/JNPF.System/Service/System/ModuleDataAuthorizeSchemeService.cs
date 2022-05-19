using JNPF.Common.Enum;
using JNPF.Common.Filter;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.System.Entitys.Dto.System.ModuleDataAuthorizeScheme;
using JNPF.System.Entitys.Permission;
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
    /// 数据权限
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "ModuleDataAuthorizeScheme", Order = 214)]
    [Route("api/system/[controller]")]
    public class ModuleDataAuthorizeSchemeService : IModuleDataAuthorizeSchemeService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ModuleDataAuthorizeSchemeEntity> _moduleDataAuthorizeSchemeRepository; //系统功能数据计划表仓储
        private readonly ISqlSugarRepository<AuthorizeEntity> _authorizeRepository; //权限操作表仓储
        private readonly ISqlSugarRepository<UserEntity> _userRepository; //用户仓储
        private readonly ISqlSugarRepository<RoleEntity> _roleRepository;

        /// <summary>
        /// 初始化一个<see cref="ModuleDataAuthorizeSchemeService"/>类型的新实例
        /// </summary>
        /// <param name="moduleDataAuthorizeSchemeRepository"></param>
        /// <param name="authorizeRepository"></param>
        /// <param name="userRepository"></param>
        /// <param name="roleRepository"></param>
        public ModuleDataAuthorizeSchemeService(ISqlSugarRepository<ModuleDataAuthorizeSchemeEntity> moduleDataAuthorizeSchemeRepository, ISqlSugarRepository<AuthorizeEntity> authorizeRepository, ISqlSugarRepository<UserEntity> userRepository, ISqlSugarRepository<RoleEntity> roleRepository)
        {
            _moduleDataAuthorizeSchemeRepository = moduleDataAuthorizeSchemeRepository;
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
        [HttpGet("{moduleId}/List")]
        public async Task<dynamic> GetList(string moduleId, [FromQuery] KeywordInput input)
        {
            var list = await GetList(moduleId);
            if (!string.IsNullOrEmpty(input.keyword))
            {
                list = list.FindAll(t => t.EnCode.Contains(input.keyword) || t.FullName.Contains(input.keyword));
            }
            var treeList = list.Adapt<List<ModuleDataAuthorizeSchemeListOutput>>();
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
            var output = data.Adapt<ModuleDataAuthorizeSchemeInfoOutput>();
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
        public async Task Delete(string id)
        {
            var entity = await GetInfo(id);
            if (entity == null)
                throw JNPFException.Oh(ErrorCode.COM1005);
            var isOk = await Delete(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] ModuleDataAuthorizeSchemeCrInput input)
        {
            var entity = input.Adapt<ModuleDataAuthorizeSchemeEntity>();
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
        public async Task Update(string id, [FromBody] ModuleDataAuthorizeSchemeUpInput input)
        {
            var entity = input.Adapt<ModuleDataAuthorizeSchemeEntity>();
            var isOk = await Update(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1001);
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ModuleDataAuthorizeSchemeEntity>> GetList()
        {
            return await _moduleDataAuthorizeSchemeRepository.Entities.Where(x => x.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="moduleId">功能主键</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ModuleDataAuthorizeSchemeEntity>> GetList(string moduleId)
        {
            return await _moduleDataAuthorizeSchemeRepository.Entities.Where(x => x.DeleteMark == null && x.ModuleId == moduleId).OrderBy(o => o.SortCode).ToListAsync();
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [NonAction]
        public async Task<ModuleDataAuthorizeSchemeEntity> GetInfo(string id)
        {
            return await _moduleDataAuthorizeSchemeRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(ModuleDataAuthorizeSchemeEntity entity)
        {
            return await _moduleDataAuthorizeSchemeRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(ModuleDataAuthorizeSchemeEntity entity)
        {
            return await _moduleDataAuthorizeSchemeRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(ModuleDataAuthorizeSchemeEntity entity)
        {
            return await _moduleDataAuthorizeSchemeRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取用户资源列表
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetResourceList(bool isAdmin, string userId)
        {
            var output = new List<ModuleDataAuthorizeSchemeOutput>();
            if (!isAdmin)
            {
                var role = _userRepository.FirstOrDefault(u => u.Id == userId).RoleId;
                if (!string.IsNullOrEmpty(role))
                {
                    var roleArray = role.Split(',');
                    var roleId = await _roleRepository.Entities.In(r => r.Id, roleArray).Where(r => r.EnabledMark.Equals(1) && r.DeleteMark == null).Select(r => r.Id).ToListAsync();
                    var items = await _authorizeRepository.Entities.In(a => a.ObjectId, roleId).Where(a => a.ItemType == "resource").Select(a => a.ItemId).ToListAsync();
                    var buttons = await _moduleDataAuthorizeSchemeRepository.Entities.In(a => a.Id, items).Where(a => a.EnabledMark == 1 && a.DeleteMark == null).Select<ModuleDataAuthorizeSchemeEntity>().OrderBy(q => q.SortCode).ToListAsync();
                    output = buttons.Adapt<List<ModuleDataAuthorizeSchemeOutput>>();
                }
            }
            else
            {
                var buttons = await _moduleDataAuthorizeSchemeRepository.Where(a => a.EnabledMark == 1 && a.DeleteMark == null).Select<ModuleDataAuthorizeSchemeEntity>().OrderBy(q => q.SortCode).ToListAsync();
                output = buttons.Adapt<List<ModuleDataAuthorizeSchemeOutput>>();
            }
            return output;
        }

        #endregion
    }
}
