using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Filter;
using JNPF.Common.Util;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.System.Entitys.Dto.Permission.Role;
using JNPF.System.Entitys.Permission;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.Permission;
using JNPF.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JNPF.System.Service.Permission
{
    /// <summary>
    /// 业务实现：角色信息
    /// </summary>
    [ApiDescriptionSettings(Tag = "Permission", Name = "Role", Order = 167)]
    [Route("api/permission/[controller]")]
    public class RoleService : IRoleService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<RoleEntity> _roleRepository;  // 角色表仓储
        private readonly ISqlSugarRepository<UserRelationEntity> _userRelationRepository;
        private readonly IAuthorizeService _authorizeService;
        private readonly ISqlSugarRepository<DictionaryDataEntity> _dictionaryDataRepository;// 数据字典表仓储
        private readonly ISysCacheService _sysCacheService;
        private readonly IUserManager _userManager;

        /// <summary>
        /// 初始化一个<see cref="RoleService"/>类型的新实例
        /// </summary>
        public RoleService(ISqlSugarRepository<RoleEntity> roleRepository, ISqlSugarRepository<DictionaryDataEntity> dictionaryDataRepository, ISqlSugarRepository<UserRelationEntity> userRelationRepository, IAuthorizeService authorizeService, ISysCacheService sysCacheService, IUserManager userManager)
        {
            _roleRepository = roleRepository;
            _authorizeService = authorizeService;
            _userRelationRepository = userRelationRepository;
            _dictionaryDataRepository = dictionaryDataRepository;
            _sysCacheService = sysCacheService;
            _userManager = userManager;
        }

        #region GET

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] KeywordInput input)
        {
            var roleList = await _roleRepository.Entities.Select(r => new { Id = r.Id, ParentId = r.Type, Type = "role", EnCode = r.EnCode, FullName = r.FullName, Description = r.Description, EnabledMark = r.EnabledMark, CreatorTime = r.CreatorTime, DeleteMark = r.DeleteMark, SortCode = r.SortCode })
                     .MergeTable().Select<RoleListOutput>()
                     .WhereIF(!string.IsNullOrEmpty(input.keyword), r => r.fullName.Contains(input.keyword) || r.enCode.Contains(input.keyword))
                     .Where(r => r.deleteMark == null)
                     .OrderBy(o => o.sortCode)
                     .ToListAsync();
            var parentIds = roleList.Select(x => x.parentId).ToList().Distinct();
            var treeList = await _dictionaryDataRepository.Where(d => parentIds.Contains(d.EnCode) && d.DeleteMark == null && d.EnabledMark.Equals(1))
                .Select(d => new { Id = d.EnCode, ParentId = "0", EnCode = "", FullName = d.FullName, SortCode = d.SortCode })
                .MergeTable().Select<RoleListOutput>().OrderBy(o => o.sortCode).ToListAsync();
            return new { list = treeList.Union(roleList).ToList().ToTree("0") };
        }

        /// <summary>
        /// 获取下拉框(类型+角色)
        /// </summary>
        /// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> GetSelector()
        {
            var roleList = await _roleRepository.Entities.Select(r => new { Id = r.Id, ParentId = r.Type, Type = "role", EnCode = r.EnCode, FullName = r.FullName, EnabledMark = r.EnabledMark, DeleteMark = r.DeleteMark, SortCode = r.SortCode })
                    .MergeTable().Select<RoleSelectorOutput>()
                    .Where(r => r.deleteMark == null && r.enabledMark.Equals(1))
                    .OrderBy(o => o.sortCode)
                    .ToListAsync();
            var parentIds = roleList.Select(x => x.parentId).ToList().Distinct();
            var treeList = await _dictionaryDataRepository.Where(d => parentIds.Contains(d.EnCode) && d.DeleteMark == null && d.EnabledMark.Equals(1))
                .Select(d => new { Id = d.EnCode, Type = "", ParentId = "0", EnCode = "", FullName = d.FullName, SortCode = d.SortCode })
                .MergeTable().Select<RoleSelectorOutput>().OrderBy(o => o.sortCode).ToListAsync();
            return new { list = treeList.Union(roleList).ToList().ToTree("0") };
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var entity = await _roleRepository.FirstOrDefaultAsync(r => r.Id == id);
            var output = entity.Adapt<RoleInfoOutput>();
            return output;
        }

        #endregion

        #region POST

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        public async Task Create([FromBody] RoleCrInput input)
        {
            if (await _roleRepository.AnyAsync(r => r.EnCode == input.enCode && r.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D1600);
            if (await _roleRepository.AnyAsync(r => r.FullName == input.fullName && r.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D1601);
            var entity = input.Adapt<RoleEntity>();
            var isOk = await _roleRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();
            _ = isOk ?? throw JNPFException.Oh(ErrorCode.D1602);
            await _sysCacheService.DelRole(_userManager.UserId);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _roleRepository.FirstOrDefaultAsync(r => r.Id == id && r.DeleteMark == null);
            _ = entity ?? throw JNPFException.Oh(ErrorCode.D1608);
            //角色下有数据权限不能删
            var items = await _authorizeService.GetAuthorizeItemIds(entity.Id, "resource");
            if (await _authorizeService.GetIsExistModuleDataAuthorizeScheme(items.ToArray()))
                throw JNPFException.Oh(ErrorCode.D1603);
            //角色下有按钮不能删除
            items = await _authorizeService.GetAuthorizeItemIds(entity.Id, "button");
            if (await _authorizeService.GetIsExistModuleDataAuthorizeScheme(items.ToArray()))
                throw JNPFException.Oh(ErrorCode.D1604);
            //角色下有列不能删除
            items = await _authorizeService.GetAuthorizeItemIds(entity.Id, "column");
            if (await _authorizeService.GetIsExistModuleDataAuthorizeScheme(items.ToArray()))
                throw JNPFException.Oh(ErrorCode.D1605);
            //角色下有菜单不能删
            items = await _authorizeService.GetAuthorizeItemIds(entity.Id, "module");
            if (await _authorizeService.GetIsExistModuleDataAuthorizeScheme(items.ToArray()))
                throw JNPFException.Oh(ErrorCode.D1606);
            //角色下有用户不能删
            if (await _userRelationRepository.AnyAsync(u => u.ObjectType == "Role" && u.ObjectId == id))
                throw JNPFException.Oh(ErrorCode.D1607);
            var isOk = await _roleRepository.Context.Updateable<RoleEntity>().SetColumns(it => new RoleEntity()
            {
                DeleteMark = 1,
                DeleteTime = SqlFunc.GetDate(),
                DeleteUserId = _userManager.UserId
            }).Where(it => it.Id == id && it.DeleteMark == null).ExecuteCommandAsync();
            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.D1609);
            await _sysCacheService.DelRole(_userManager.UserId);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] RoleUpInput input)
        {
            if (await _roleRepository.AnyAsync(r => r.EnCode == input.enCode && r.DeleteMark == null && r.Id != id))
                throw JNPFException.Oh(ErrorCode.D1600);
            if (await _roleRepository.AnyAsync(r => r.FullName == input.fullName && r.DeleteMark == null && r.Id != id))
                throw JNPFException.Oh(ErrorCode.D1601);
            var entity = input.Adapt<RoleEntity>();
            var isOk = await _roleRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.D1610);
            await _sysCacheService.DelRole(_userManager.UserId);
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/State")]
        public async Task UpdateState(string id)
        {
            if (!await _roleRepository.AnyAsync(r => r.Id == id && r.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D1608);

            var isOk = await _roleRepository.Context.Updateable<RoleEntity>().SetColumns(it => new RoleEntity()
            {
                EnabledMark = SqlFunc.IIF(it.EnabledMark == 1, 0, 1),
                LastModifyUserId = _userManager.UserId,
                LastModifyTime = SqlFunc.GetDate()
            }).Where(it => it.Id == id).ExecuteCommandAsync();

            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.D1610);
            await _sysCacheService.DelRole(_userManager.UserId);
        }

        #endregion

        #region PublicMethod
        /// <summary>
        /// 名称
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [NonAction]
        public string GetName(string ids)
        {
            if (ids.IsNullOrEmpty())
            {
                return "";
            }
            var idList = ids.Split(",").ToList();
            var nameList = new List<string>();
            var roleList = _roleRepository.Entities.Where(x => x.DeleteMark == null && x.EnabledMark == 1).ToList();
            foreach (var item in idList)
            {
                var info = roleList.Find(x => x.Id == item);
                if (info != null && info.FullName.IsNotEmptyOrNull())
                {
                    nameList.Add(info.FullName);
                }
            }
            var name = string.Join(",", nameList);
            return name;
        }
        #endregion
    }
}
