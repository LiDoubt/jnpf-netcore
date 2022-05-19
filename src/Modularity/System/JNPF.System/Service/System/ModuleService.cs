using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Util;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.JsonSerialization;
using JNPF.System.Entitys.Dto.System.Module;
using JNPF.System.Entitys.Permission;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.Common;
using JNPF.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JNPF.System.Service.System
{
    /// <summary>
    /// 菜单管理
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "Menu", Order = 212)]
    [Route("api/system/[controller]")]
    public class ModuleService : IModuleService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ModuleEntity> _moduleRepository; //系统功能表仓储
        private readonly ISqlSugarRepository<AuthorizeEntity> _authorizeRepository; //权限操作表仓储
        private readonly ISqlSugarRepository<UserEntity> _userRepository; //用户仓储
        private readonly ISqlSugarRepository<RoleEntity> _roleRepository;
        private readonly IModuleButtonService _moduleButtonService;
        private readonly IModuleColumnService _moduleColumnService;
        private readonly IModuleDataAuthorizeSchemeService _moduleDataAuthorizeSchemeService;
        private readonly IModuleDataAuthorizeSerive _moduleDataAuthorizeSerive;
        private readonly IModuleFormService _moduleFormSerive;
        private readonly IFileService _fileService;
        private readonly SqlSugarScope db;

        /// <summary>
        /// 初始化一个<see cref="ModuleService"/>类型的新实例
        /// </summary>
        /// <param name="moduleRepository"></param>
        /// <param name="authorizeRepository"></param>
        /// <param name="userRepository"></param>
        /// <param name="roleRepository"></param>
        /// <param name="moduleButtonService"></param>
        /// <param name="moduleColumnService"></param>
        /// <param name="moduleDataAuthorizeSchemeService"></param>
        /// <param name="moduleDataAuthorizeSerive"></param>
        /// <param name="fileService"></param>
        public ModuleService(ISqlSugarRepository<ModuleEntity> moduleRepository,
            ISqlSugarRepository<AuthorizeEntity> authorizeRepository,
            ISqlSugarRepository<UserEntity> userRepository,
            ISqlSugarRepository<RoleEntity> roleRepository,
            IModuleButtonService moduleButtonService,
            IModuleColumnService moduleColumnService,
            IModuleDataAuthorizeSchemeService moduleDataAuthorizeSchemeService,
            IModuleDataAuthorizeSerive moduleDataAuthorizeSerive,
            IFileService fileService, IModuleFormService moduleFormSerive)
        {
            _moduleRepository = moduleRepository;
            _authorizeRepository = authorizeRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _moduleButtonService = moduleButtonService;
            _moduleColumnService = moduleColumnService;
            _moduleDataAuthorizeSchemeService = moduleDataAuthorizeSchemeService;
            _moduleDataAuthorizeSerive = moduleDataAuthorizeSerive;
            _fileService = fileService;
            db = _moduleRepository.Context;
            _moduleFormSerive = moduleFormSerive;
        }

        #region GET

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] ModuleListQuery input)
        {
            try
            {
                var data = await GetList();
                if (!string.IsNullOrEmpty(input.category))
                    data = data.FindAll(x => x.Category == input.category);
                if (!string.IsNullOrEmpty(input.keyword))
                    data = data.TreeWhere(t => t.FullName.Contains(input.keyword) || t.EnCode.Contains(input.keyword) || (t.UrlAddress.IsNotEmptyOrNull() && t.UrlAddress.Contains(input.keyword)), t => t.Id, t => t.ParentId);
                var treeList = data.Adapt<List<ModuleListOutput>>();
                return new { list = treeList.ToTree("-1") };
            }
            catch (Exception e)
            {

                throw;
            }
        }

        /// <summary>
        /// 获取菜单下拉框
        /// </summary>
        /// <param name="category">菜单分类（参数有Web,App），默认显示所有分类</param>
        /// <returns></returns>
        [HttpGet("Selector/{id}")]
        public async Task<dynamic> GetSelector(string id, string category)
        {
            var data = await GetList();
            if (!string.IsNullOrEmpty(category))
                data = data.FindAll(x => x.Category == category && x.Type == 1);
            if (!id.Equals("0"))
            {
                data.RemoveAll(x => x.Id == id);
            }
            var treeList = data.Adapt<List<ModuleSelectorOutput>>();
            return new { list = treeList.ToTree("-1") };
        }

        /// <summary>
        /// 获取菜单列表（下拉框）
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet("Selector/All")]
        public async Task<dynamic> GetSelectorAll(string category)
        {
            var data = await GetList();
            if (!string.IsNullOrEmpty(category))
                data = data.FindAll(x => x.Category == category);
            var treeList = data.Adapt<List<ModuleSelectorAllOutput>>();
            return new { list = treeList.ToTree("-1") };
        }

        /// <summary>
        /// 获取菜单信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            var data = await GetInfo(id);
            var output = data.Adapt<ModuleInfoOutput>();
            return output;
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Action/Export")]
        public async Task<dynamic> ActionsExport(string id)
        {
            var data = (await GetInfo(id)).Adapt<ModuleExportInput>();
            data.buttonEntityList = (await _moduleButtonService.GetList(id)).Adapt<List<ButtonEntityListItem>>();
            data.columnEntityList = (await _moduleColumnService.GetList(id)).Adapt<List<ColumnEntityListItem>>();
            data.authorizeEntityList = (await _moduleDataAuthorizeSerive.GetList(id)).Adapt<List<AuthorizeEntityListItem>>();
            data.schemeEntityList = (await _moduleDataAuthorizeSchemeService.GetList(id)).Adapt<List<SchemeEntityListItem>>();
            data.formEntityList = await _moduleFormSerive.GetList(id);
            var jsonStr = data.Serialize();
            return _fileService.Export(jsonStr, data.fullName);
        }
        #endregion

        #region Post

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Creater([FromBody] ModuleCrInput input)
        {
            if (await _moduleRepository.AnyAsync(x => x.EnCode == input.enCode && x.DeleteMark == null) || await _moduleRepository.AnyAsync(x => x.FullName == input.fullName && x.DeleteMark == null && input.parentId == x.ParentId))
                throw JNPFException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<ModuleEntity>();
            //添加字典菜单按钮
            if (entity.Type == 4)
            {
                var btnEntityList = (await _moduleButtonService.GetList()).FindAll(x => x.ModuleId == "-1");
                foreach (var item in btnEntityList)
                {
                    item.ModuleId = entity.Id;
                    await _moduleButtonService.Create(item);
                }
            }
            var isOk = await Create(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] ModuleUpInput input)
        {
            if (await _moduleRepository.AnyAsync(x => x.Id != id && x.EnCode == input.enCode && x.DeleteMark == null) || await _moduleRepository.AnyAsync(x => x.Id != id && x.FullName == input.fullName && x.DeleteMark == null && input.parentId == x.ParentId))
                throw JNPFException.Oh(ErrorCode.COM1004);
            var info = await _moduleRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
            if (info.Type == 1 && info.Type != input.type && await _moduleRepository.AnyAsync(x => x.ParentId == id && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D4008);
            var entity = input.Adapt<ModuleEntity>();
            entity.Id = id;
            var isOk = await Update(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            try
            {
                var entity = await _moduleRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
                if (entity == null || await _moduleRepository.AnyAsync(x => x.ParentId == id && x.DeleteMark == null))
                    throw JNPFException.Oh(ErrorCode.D1007);
                db.BeginTran();
                var isOk = await Delete(entity);
                if (isOk < 1)
                    throw JNPFException.Oh(ErrorCode.COM1002);
                db.CommitTran();
            }
            catch (Exception)
            {
                db.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 更新菜单状态
        /// </summary>
        /// <param name="id">菜单id</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/State")]
        public async Task ActionsState(string id)
        {
            var entity = await _moduleRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
            entity.EnabledMark = entity.EnabledMark == 0 ? 1 : 0;
            var isOk = await Update(entity);
            if (isOk < 0)
                throw JNPFException.Oh(ErrorCode.COM1003);
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("Action/Import")]
        public async Task ActionsImport(IFormFile file)
        {
            var josn = _fileService.Import(file);
            var moduleModel = josn.Deserialize<ModuleExportInput>();
            if (moduleModel == null || moduleModel.linkTarget.IsNullOrEmpty())
                throw JNPFException.Oh(ErrorCode.D3006);
            if (moduleModel.parentId != "-1" && !_moduleRepository.Any(x => x.Id == moduleModel.parentId && x.DeleteMark == null))
            {
                throw JNPFException.Oh(ErrorCode.D3007);
            }
            if (await _moduleRepository.AnyAsync(x => x.EnCode == moduleModel.enCode && x.DeleteMark == null) || await _moduleRepository.AnyAsync(x => x.FullName == moduleModel.fullName && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D4000);
            await ImportData(moduleModel);
        }
        #endregion

        #region PublicMethod

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ModuleEntity>> GetList()
        {
            return await _moduleRepository.Entities.Where(x => x.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [NonAction]
        public async Task<ModuleEntity> GetInfo(string id)
        {
            return await _moduleRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(ModuleEntity entity)
        {
            return await _moduleRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(ModuleEntity entity)
        {
            return await _moduleRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(ModuleEntity entity)
        {
            return await _moduleRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取用户树形模块功能列表
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetUserTreeModuleList(bool isAdmin, string userId)
        {
            var output = new List<ModuleNodeOutput>();
            if (!isAdmin)
            {
                var role = _userRepository.FirstOrDefault(u => u.Id == userId).RoleId;
                if (!string.IsNullOrEmpty(role))
                {
                    var roleArray = role.Split(',');
                    var roleId = await _roleRepository.Entities.In(r => r.Id, roleArray).Where(r => r.EnabledMark.Equals(1) && r.DeleteMark == null).Select(r => r.Id).ToListAsync();
                    var items = await _authorizeRepository.Entities.In(a => a.ObjectId, roleId).Where(a => a.ItemType == "module").Select(a => a.ItemId).ToListAsync();
                    var menus = await _moduleRepository.Entities.In(a => a.Id, items).Where(a => a.EnabledMark == 1 && a.Category.Equals("Web") && a.DeleteMark == null).Select<ModuleEntity>().OrderBy(q => q.ParentId).OrderBy(q => q.SortCode).ToListAsync();
                    output = menus.Adapt<List<ModuleNodeOutput>>();
                }
            }
            else
            {
                var menus = await _moduleRepository.Where(a => a.EnabledMark.Equals("1") && a.Category.Equals("Web") && a.DeleteMark == null).Select<ModuleEntity>().OrderBy(q => q.ParentId).OrderBy(q => q.SortCode).ToListAsync();
                output = menus.Adapt<List<ModuleNodeOutput>>();
            }
            return output.ToTree("-1");
        }

        /// <summary>
        /// 获取用户菜单模块功能列表
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetUserModueList(bool isAdmin, string userId)
        {
            var output = new List<ModuleOutput>();
            if (!isAdmin)
            {
                var role = _userRepository.FirstOrDefault(u => u.Id == userId).RoleId;
                if (!string.IsNullOrEmpty(role))
                {
                    var roleArray = role.Split(',');
                    var roleId = await _roleRepository.Entities.In(r => r.Id, roleArray).Where(r => r.EnabledMark.Equals(1) && r.DeleteMark == null).Select(r => r.Id).ToListAsync();
                    var items = await _authorizeRepository.Entities.In(a => a.ObjectId, roleId).Where(a => a.ItemType == "module").GroupBy(it => new { it.ItemId }).Select(it => new { it.ItemId }).ToListAsync();
                    if (items.Count == 0) return output;
                    output = await _moduleRepository.Entities.In(a => a.Id, items.Select(it => it.ItemId).ToArray()).Where(a => a.EnabledMark == 1 && a.Category.Equals("Web") && a.DeleteMark == null).Select(a => new { Id = a.Id, FullName = a.FullName, SortCode = a.SortCode }).MergeTable().OrderBy(o => o.SortCode).Select<ModuleOutput>().ToListAsync();
                }
            }
            else
            {
                output = await _moduleRepository.Where(a => a.EnabledMark.Equals("1") && a.Category.Equals("Web") && a.DeleteMark == null).Select(a => new { Id = a.Id, FullName = a.FullName, SortCode = a.SortCode }).MergeTable().OrderBy(o => o.SortCode).Select<ModuleOutput>().ToListAsync();
            }

            return output;
        }

        #endregion

        #region PrivateMethod
        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task ImportData(ModuleExportInput data)
        {
            try
            {
                var module = data.Adapt<ModuleEntity>();
                var button = data.buttonEntityList.Adapt<List<ModuleButtonEntity>>();
                var colum = data.buttonEntityList.Adapt<List<ModuleColumnEntity>>();
                var dataAuthorize = data.buttonEntityList.Adapt<List<ModuleDataAuthorizeEntity>>();
                var dataAuthorizeScheme = data.buttonEntityList.Adapt<List<ModuleDataAuthorizeSchemeEntity>>();

                db.BeginTran();
                var storBtn = db.Storageable(button).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await storBtn.AsInsertable.ExecuteCommandAsync(); //执行插入
                await storBtn.AsUpdateable.ExecuteCommandAsync(); //执行更新　

                var storcolum = db.Storageable(colum).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await storcolum.AsInsertable.ExecuteCommandAsync(); //执行插入
                await storcolum.AsUpdateable.ExecuteCommandAsync(); //执行更新

                var storAuthorize = db.Storageable(dataAuthorize).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await storAuthorize.AsInsertable.ExecuteCommandAsync(); //执行插入
                await storAuthorize.AsUpdateable.ExecuteCommandAsync(); //执行更新

                var storAuthorizeScheme = db.Storageable(dataAuthorizeScheme).Saveable().ToStorage();
                await storAuthorizeScheme.AsInsertable.ExecuteCommandAsync(); //执行插入
                await storAuthorizeScheme.AsUpdateable.ExecuteCommandAsync(); //执行更新

                var stroForm = db.Storageable(data.formEntityList).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await stroForm.AsInsertable.ExecuteCommandAsync(); //执行插入
                await stroForm.AsUpdateable.ExecuteCommandAsync(); //执行更新

                var stroModule = db.Storageable(module).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await stroModule.AsInsertable.ExecuteCommandAsync(); //执行插入
                await stroModule.AsUpdateable.ExecuteCommandAsync(); //执行更新
                db.CommitTran();
            }
            catch (Exception ex)
            {
                db.RollbackTran();
                throw JNPFException.Oh(ErrorCode.D3008);
            }
        }
        #endregion
    }
}
