using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Filter;
using JNPF.Common.Util;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.System.Entitys.Dto.System.ModuleButton;
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
    /// 功能按钮
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "ModuleButton", Order = 212)]
    [Route("api/system/[controller]")]
    public class ModuleButtonService : IModuleButtonService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ModuleButtonEntity> _moduleButtonRepository; //系统功能按钮表仓储
        private readonly ISqlSugarRepository<AuthorizeEntity> _authorizeRepository; //权限操作表仓储
        private readonly ISqlSugarRepository<UserEntity> _userRepository; //用户仓储
        private readonly ISqlSugarRepository<RoleEntity> _roleRepository;
        private readonly IUserManager _userManager;

        /// <summary>
        /// 初始化一个<see cref="ModuleButtonService"/>类型的新实例
        /// </summary>
        public ModuleButtonService(ISqlSugarRepository<ModuleButtonEntity> moduleButtonRepository, ISqlSugarRepository<AuthorizeEntity> authorizeRepository, ISqlSugarRepository<UserEntity> userRepository, ISqlSugarRepository<RoleEntity> roleRepository, IUserManager userManager)
        {
            _moduleButtonRepository = moduleButtonRepository;
            _authorizeRepository = authorizeRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userManager = userManager;
        }

        #region GET

        /// <summary>
        /// 获取按钮权限列表
        /// </summary>
        /// <param name="moduleId">功能id</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("{moduleId}/List")]
        public async Task<dynamic> GetList_Api(string moduleId, [FromQuery] KeywordInput input)
        {
            var list = await GetList(moduleId);
            if (!string.IsNullOrEmpty(input.keyword))
            {
                list = list.FindAll(t => t.EnCode.Contains(input.keyword) || t.FullName.Contains(input.keyword));
            }
            var treeList = list.Adapt<List<ModuleButtonListOutput>>();
            return new { list = treeList.ToTree("-1") };
        }

        /// <summary>
        /// 获取按钮权限下拉列表
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        [HttpGet("{moduleId}/Selector")]
        public async Task<dynamic> GetSelector(string moduleId)
        {
            var treeList = (await GetList(moduleId)).Adapt<List<ModuleButtonSelectorOutput>>();
            return new { list = treeList.ToTree("-1") };
        }

        /// <summary>
        /// 获取按钮信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            var data = await GetInfo(id);
            var output = data.Adapt<ModuleButtonInfoOutput>();
            return output;
        }

        #endregion

        #region Post

        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create_Api([FromBody] ModuleButtonCrInput input)
        {
            var entity = input.Adapt<ModuleButtonEntity>();
            if (await _moduleButtonRepository.AnyAsync(x => x.EnCode == input.enCode && x.DeleteMark == null&&x.ModuleId==input.moduleId) 
                || await _moduleButtonRepository.AnyAsync(x => x.FullName == input.fullName && x.DeleteMark == null && x.ModuleId == input.moduleId))
                throw JNPFException.Oh(ErrorCode.COM1004);
            var isOk = await Create(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 修改按钮
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update_Api(string id, [FromBody] ModuleButtonUpInput input)
        {
            var entity = input.Adapt<ModuleButtonEntity>();
            var isOk = await Update(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除按钮
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete_Api(string id)
        {
            if (!await _moduleButtonRepository.AnyAsync(r => r.Id == id && r.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.COM1005);
            if (await _moduleButtonRepository.AnyAsync(x => x.ParentId == id && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D1007);
            var isOk = await _moduleButtonRepository.Context.Updateable<ModuleButtonEntity>().SetColumns(it => new ModuleButtonEntity()
            {
                DeleteMark = 1,
                DeleteUserId = _userManager.UserId,
                DeleteTime = SqlFunc.GetDate()
            }).Where(it => it.Id == id).ExecuteCommandAsync();

            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.COM1003);
        }

        /// <summary>
        /// 更新按钮状态
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/State")]
        public async Task ActionsState(string id)
        {
            if (!await _moduleButtonRepository.AnyAsync(r => r.Id == id && r.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.COM1005);

            var isOk = await _moduleButtonRepository.Context.Updateable<ModuleButtonEntity>().SetColumns(it => new ModuleButtonEntity()
            {
                EnabledMark = SqlFunc.IIF(it.EnabledMark == 1, 0, 1),
                LastModifyUserId = _userManager.UserId,
                LastModifyTime = SqlFunc.GetDate()
            }).Where(it => it.Id == id).ExecuteCommandAsync();

            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.COM1003);
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ModuleButtonEntity>> GetList()
        {
            return await _moduleButtonRepository.Entities.Where(x => x.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="moduleId">功能主键</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ModuleButtonEntity>> GetList(string moduleId)
        {
            return await _moduleButtonRepository.Entities.Where(x => x.DeleteMark == null && x.ModuleId == moduleId).OrderBy(o => o.SortCode).ToListAsync();
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [NonAction]
        public async Task<ModuleButtonEntity> GetInfo(string id)
        {
            return await _moduleButtonRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(ModuleButtonEntity entity)
        {
            return await _moduleButtonRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(ModuleButtonEntity entity)
        {
            return await _moduleButtonRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entitys">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(List<ModuleButtonEntity> entitys)
        {
            return await _moduleButtonRepository.Context.Insertable(entitys).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(ModuleButtonEntity entity)
        {
            return await _moduleButtonRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetUserModuleButtonList(bool isAdmin, string userId)
        {
            var output = new List<ModuleButtonOutput>();
            if (!isAdmin)
            {
                var role = _userRepository.FirstOrDefault(u => u.Id == userId).RoleId;
                if (!string.IsNullOrEmpty(role))
                {
                    var roleArray = role.Split(',');
                    var roleId = await _roleRepository.Entities.In(r => r.Id, roleArray).Where(r => r.EnabledMark.Equals(1) && r.DeleteMark == null).Select(r => r.Id).ToListAsync();
                    var items = await _authorizeRepository.Entities.In(a => a.ObjectId, roleId).Where(a => a.ItemType == "button").Select(a => a.ItemId).ToListAsync();
                    var buttons = await _moduleButtonRepository.Entities.In(a => a.Id, items).Where(a => a.EnabledMark == 1 && a.DeleteMark == null).Select<ModuleButtonEntity>().OrderBy(q => q.SortCode).ToListAsync();
                    output = buttons.Adapt<List<ModuleButtonOutput>>();
                }
            }
            else
            {
                var buttons = await _moduleButtonRepository.Where(a => a.EnabledMark.Equals("1") && a.DeleteMark == null).Select<ModuleButtonEntity>().OrderBy(q => q.SortCode).ToListAsync();
                output = buttons.Adapt<List<ModuleButtonOutput>>();
            }
            return output;
        }

        #endregion
    }
}
