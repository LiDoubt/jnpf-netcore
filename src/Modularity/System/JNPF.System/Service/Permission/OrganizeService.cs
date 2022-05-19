using JNPF.Common.Const;
using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Filter;
using JNPF.Common.Util;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.JsonSerialization;
using JNPF.System.Entitys.Dto.Permission.Organize;
using JNPF.System.Entitys.Model.Permission.Organize;
using JNPF.System.Entitys.Model.Permission.User;
using JNPF.System.Entitys.Permission;
using JNPF.System.Interfaces.Permission;
using JNPF.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JNPF.System.Service.Permission
{
    /// <summary>
    /// 机构管理
    /// 组织架构：公司》部门》岗位》用户
    /// 版 本：V3.2.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021.06.07 
    /// </summary>
    [ApiDescriptionSettings(Tag = "Permission", Name = "Organize", Order = 165)]
    [Route("api/permission/[controller]")]
    public class OrganizeService : IOrganizeService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<OrganizeEntity> _organizeRepository;
        private readonly ISqlSugarRepository<PositionEntity> _positionRepository;
        private readonly ISqlSugarRepository<UserEntity> _userRepository;
        private readonly ISysConfigService _sysConfigService;
        private readonly ISynThirdInfoService _synThirdInfoService;

        private readonly HttpContext _httpContext;

        /// <summary>
        /// 初始化一个<see cref="OrganizeService"/>类型的新实例
        /// </summary>
        public OrganizeService(ISqlSugarRepository<OrganizeEntity> organizeRepository, ISqlSugarRepository<PositionEntity> positionRepository, ISqlSugarRepository<UserEntity> userRepository, ISysConfigService sysConfigService, ISynThirdInfoService synThirdInfoService)
        {
            _organizeRepository = organizeRepository;
            _positionRepository = positionRepository;
            _userRepository = userRepository;
            _sysConfigService = sysConfigService;
            _synThirdInfoService = synThirdInfoService;
            _httpContext = App.HttpContext;
        }

        #region GET

        /// <summary>
        /// 获取机构列表
        /// </summary>
        /// <param name="input">关键字参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] KeywordInput input)
        {
            var data = await _organizeRepository.Where(t => t.Category.Equals("company") && t.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
            if (!string.IsNullOrEmpty(input.keyword))
            {
                data = data.TreeWhere(t => t.FullName.Contains(input.keyword) || t.EnCode.Contains(input.keyword), t => t.Id, t => t.ParentId);
            }
            var treeList = data.Adapt<List<OrganizeListOutput>>();
            return new { list = treeList.ToTree("-1") };
        }

        /// <summary>
        /// 获取下拉框
        /// </summary>
        /// <returns></returns>
        [HttpGet("Selector/{id}")]
        public async Task<dynamic> GetSelector(string id)
        {
            var data = await _organizeRepository.Where(t => t.Category.Equals("company") && t.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
            if (!"0".Equals(id))
            {
                var info = data.Find(it => it.Id == id);
                data.Remove(info);
            }
            var treeList = data.Adapt<List<OrganizeListOutput>>();
            treeList.ForEach(item =>
            {
                if (item.category.Equals("company"))
                {
                    item.icon = "icon-ym icon-ym-tree-organization3";
                }
            });
            return new { list = treeList.ToTree("-1") };
        }

        /// <summary>
        /// 获取树形
        /// </summary>
        /// <returns></returns>
        [HttpGet("Tree")]
        public async Task<dynamic> GetTree()
        {
            var data = await _organizeRepository.Where(t => t.Category.Equals("company") && t.DeleteMark == null).ToListAsync();
            var treeList = data.Adapt<List<OrganizeTreeOutput>>();
            return new { list = treeList.ToTree("-1") };
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var entity = await _organizeRepository.SingleAsync(p => p.Id == id);
            var output = entity.Adapt<OrganizeInfoOutput>();
            return output;
        }

        #endregion

        #region POST

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] OrganizeCrInput input)
        {
            var user = new UserInfo();
            await Scoped.Create(async (_, scope) =>
            {
                var services = scope.ServiceProvider;

                var _userManager = App.GetService<IUserManager>(services);

                user = await _userManager.GetUserInfo();
            });
            if (!user.dataScope.Any(it => it.organizeId == input.parentId && it.Add == true) && !user.isAdministrator)
            {
                throw JNPFException.Oh(ErrorCode.D1013);
            }
            if (await _organizeRepository.AnyAsync(o => o.EnCode == input.enCode && o.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D2008);
            if (await _organizeRepository.AnyAsync(o => o.ParentId == input.parentId && o.FullName == input.fullName && o.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D2009);
            var entity = input.Adapt<OrganizeEntity>();
            entity.Category = "company";
            entity.PropertyJson = JSON.Serialize(input.propertyJson);
            var isOk = await _organizeRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();
            _ = isOk ?? throw JNPFException.Oh(ErrorCode.D2012);

            #region 第三方同步
            var sysConfig = await _sysConfigService.GetInfo();
            var orgList = new List<OrganizeListOutput>();
            orgList.Add(entity.Adapt<OrganizeListOutput>());
            if (sysConfig.dingSynIsSynOrg == 1)
            {
                await _synThirdInfoService.SynDep(2, 1, sysConfig, orgList);
            }
            if (sysConfig.qyhIsSynOrg == 1)
            {
                await _synThirdInfoService.SynDep(1, 1, sysConfig, orgList);
            }
            #endregion
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] OrganizeUpInput input)
        {
            var user = new UserInfo();
            await Scoped.Create(async (_, scope) =>
            {
                var services = scope.ServiceProvider;

                var _userManager = App.GetService<IUserManager>(services);

                user = await _userManager.GetUserInfo();
            });
            var oldEntity = await _organizeRepository.SingleAsync(it => it.Id == id);
            if (oldEntity.ParentId != input.parentId && !user.dataScope.Any(it => it.organizeId == oldEntity.ParentId && it.Edit == true) && !user.isAdministrator)
                throw JNPFException.Oh(ErrorCode.D1013);
            if (!user.dataScope.Any(it => it.organizeId == id && it.Edit == true) && !user.isAdministrator)            
                throw JNPFException.Oh(ErrorCode.D1013);
            if (input.parentId.Equals(id))
                throw JNPFException.Oh(ErrorCode.D2001);
            //父id不能为自己的子节点
            var childIdListById = await GetChildIdListWithSelfById(id);
            if (childIdListById.Contains(input.parentId))
                throw JNPFException.Oh(ErrorCode.D2001);
            if (await _organizeRepository.AnyAsync(o => o.EnCode == input.enCode && o.Id != id && o.DeleteMark == null && o.Id != id))
                throw JNPFException.Oh(ErrorCode.D2008);
            if (await _organizeRepository.AnyAsync(o => o.ParentId == input.parentId && o.FullName == input.fullName && o.Id != id && o.DeleteMark == null && o.Id != id))
                throw JNPFException.Oh(ErrorCode.D2009);
            var entity = input.Adapt<OrganizeEntity>();
            entity.PropertyJson = JSON.Serialize(input.propertyJson);
            var isOK = await _organizeRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOK > 0))
                throw JNPFException.Oh(ErrorCode.D2010);

            #region 第三方同步
            var sysConfig = await _sysConfigService.GetInfo();
            var orgList = new List<OrganizeListOutput>();
            var synEntity = _organizeRepository.FirstOrDefault(x => x.Id == id);
            orgList.Add(synEntity.Adapt<OrganizeListOutput>());
            if (sysConfig.dingSynIsSynOrg == 1)
            {
                await _synThirdInfoService.SynDep(2, 1, sysConfig, orgList);
            }
            if (sysConfig.qyhIsSynOrg == 1)
            {
                await _synThirdInfoService.SynDep(1, 1, sysConfig, orgList);
            }
            #endregion
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var user = new UserInfo();
            await Scoped.Create(async (_, scope) =>
            {
                var services = scope.ServiceProvider;

                var _userManager = App.GetService<IUserManager>(services);

                user = await _userManager.GetUserInfo();
            });
            if (!user.dataScope.Any(it => it.organizeId == id && it.Delete == true) && !user.isAdministrator)
            {
                throw JNPFException.Oh(ErrorCode.D1013);
            }
            //该机构下有机构，则不能删
            if (await _organizeRepository.AnyAsync(o => o.ParentId.Equals(id) && o.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D2005);
            //该机构下有岗位，则不能删
            if (await _positionRepository.AnyAsync(p => p.OrganizeId.Equals(id) && p.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D2006);
            //该机构下有用户，则不能删
            if (await _userRepository.AnyAsync(u => SqlFunc.ToString(u.OrganizeId).Equals(id) && u.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D2004);
            var entity = await _organizeRepository.SingleAsync(o => o.Id == id && o.DeleteMark == null);
            _ = entity ?? throw JNPFException.Oh(ErrorCode.D2002);
            var isOk = await _organizeRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
            if (!(isOk > 0))
                throw JNPFException.Oh(ErrorCode.D2012);

            #region 第三方同步
            var sysConfig = await _sysConfigService.GetInfo();
            if (sysConfig.dingSynIsSynOrg == 1)
            {
                await _synThirdInfoService.DelSynData(2, 1, sysConfig, id);
            }
            if (sysConfig.qyhIsSynOrg == 1)
            {
                await _synThirdInfoService.DelSynData(1, 1, sysConfig, id);
            }
            #endregion
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/State")]
        public async Task UpdateState(string id)
        {
            var user = new UserInfo();
            await Scoped.Create(async (_, scope) =>
            {
                var services = scope.ServiceProvider;

                var _userManager = App.GetService<IUserManager>(services);

                user = await _userManager.GetUserInfo();
            });
            if (!user.dataScope.Any(it => it.organizeId == id && it.Edit == true) && !user.isAdministrator)
            {
                throw JNPFException.Oh(ErrorCode.D1013);
            }

            if (!await _organizeRepository.AnyAsync(u => u.Id == id && u.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D2002);

            var isOk = await _organizeRepository.Context.Updateable<OrganizeEntity>().SetColumns(it => new OrganizeEntity()
            {
                EnabledMark = SqlFunc.IIF(it.EnabledMark == 1, 0, 1),
                LastModifyUserId = user.userId,
                LastModifyTime = SqlFunc.GetDate()
            }).Where(it => it.Id == id).ExecuteCommandAsync();

            if (!(isOk > 0))
                throw JNPFException.Oh(ErrorCode.D2011);
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 是否机构主管
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> GetIsManagerByUserId(string userId)
        {
            return await _organizeRepository.AnyAsync(o => o.EnabledMark.Equals(1) && o.DeleteMark == null && o.ManagerId == userId);
        }

        /// <summary>
        /// 获取机构列表(其他服务使用)
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<OrganizeEntity>> GetListAsync()
        {
            return await _organizeRepository.Where(t => t.EnabledMark.Equals(1) && t.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
        }

        /// <summary>
        /// 获取公司列表(其他服务使用)
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<OrganizeEntity>> GetCompanyListAsync()
        {
            return await _organizeRepository.Where(t => t.Category.Equals("company") && t.EnabledMark.Equals(1) && t.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
        }

        /// <summary>
        /// 下属机构
        /// </summary>
        /// <param name="organizeId">机构ID</param>
        /// <param name="isAdmin">是否管理员</param>
        /// <returns></returns>
        [NonAction]
        public async Task<string[]> GetSubsidiary(string organizeId, bool isAdmin)
        {
            var data = await _organizeRepository.Where(o => o.DeleteMark == null && o.EnabledMark.Equals(1)).OrderBy(o => o.SortCode).ToListAsync();
            if (!isAdmin)
            {
                data = data.TreeChildNode(organizeId, t => t.Id, t => t.ParentId);
            }
            return data.Select(m => m.Id).ToArray();
        }

        /// <summary>
        /// 下属机构
        /// </summary>
        /// <param name="organizeId">机构ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<string>> GetSubsidiary(string organizeId)
        {
            var data = await _organizeRepository.Where(o => o.DeleteMark == null && o.EnabledMark.Equals(1)).OrderBy(o => o.SortCode).ToListAsync();
            data = data.TreeChildNode(organizeId, t => t.Id, t => t.ParentId);
            return data.Select(m => m.Id).ToList();
        }

        /// <summary>
        /// 根据节点Id获取所有子节点Id集合，包含自己
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<string>> GetChildIdListWithSelfById(string id)
        {
            var childIdList = await _organizeRepository.Where(u => u.ParentId.Contains(id) && u.DeleteMark == null).Select(u => u.Id).ToListAsync();
            childIdList.Add(id);
            return childIdList;
        }

        /// <summary>
        /// 获取机构成员列表
        /// </summary>
        /// <param name="organizeId">机构ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<OrganizeMemberListOutput>> GetOrganizeMemberList(string organizeId)
        {
            var output = new List<OrganizeMemberListOutput>();
            if (organizeId.Equals("0"))
            {
                var data = await _organizeRepository.Where(o => o.ParentId.Equals("-1") && o.DeleteMark == null && o.EnabledMark.Equals(1)).OrderBy(o => o.SortCode).ToListAsync();
                data.ForEach(o =>
                {
                    output.Add(new OrganizeMemberListOutput
                    {
                        id = o.Id,
                        fullName = o.FullName,
                        enabledMark = o.EnabledMark,
                        type = o.Category,
                        icon = "icon-ym icon-ym-tree-organization3",
                        hasChildren = true,
                        isLeaf = false
                    });
                });
            }
            else
            {
                var userList = await _userRepository.Where(u => SqlFunc.ToString(u.OrganizeId).Equals(organizeId) && u.EnabledMark.Equals(1) && u.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
                userList.ForEach(u =>
                {
                    output.Add(new OrganizeMemberListOutput()
                    {
                        id = u.Id,
                        fullName = u.RealName + "/" + u.Account,
                        enabledMark = u.EnabledMark,
                        type = "user",
                        icon = "icon-ym icon-ym-tree-user2",
                        hasChildren = false,
                        isLeaf = true
                    });
                });
                var departmentList = await _organizeRepository.Where(o => o.ParentId.Equals(organizeId) && o.DeleteMark == null && o.EnabledMark.Equals(1)).OrderBy(o => o.SortCode).ToListAsync();
                departmentList.ForEach(o =>
                {
                    output.Add(new OrganizeMemberListOutput()
                    {
                        id = o.Id,
                        fullName = o.FullName,
                        enabledMark = o.EnabledMark,
                        type = o.Category,
                        icon = "icon-ym icon-ym-tree-department1",
                        hasChildren = true,
                        isLeaf = false
                    });
                });

            }
            return output;
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<OrganizeEntity> GetInfoById(string Id)
        {
            return await _organizeRepository.SingleAsync(p => p.Id == Id);
        }

        #endregion
    }
}