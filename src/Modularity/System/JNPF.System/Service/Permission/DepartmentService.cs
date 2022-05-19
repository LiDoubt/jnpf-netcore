using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Filter;
using JNPF.Common.Util;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.System.Entitys.Dto.Permission.Department;
using JNPF.System.Entitys.Dto.Permission.Organize;
using JNPF.System.Entitys.Model.Permission.User;
using JNPF.System.Entitys.Permission;
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
    /// 业务实现：部门管理
    /// </summary>
    [ApiDescriptionSettings(Tag = "Permission", Name = "Organize", Order = 166)]
    [Route("api/permission/[controller]")]
    public class DepartmentService : IDepartmentService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<OrganizeEntity> _departmentRepository;
        private readonly ISqlSugarRepository<PositionEntity> _positionRepository;
        private readonly IOrganizeService _organizeService;
        private readonly ISqlSugarRepository<UserEntity> _userRepository;
        private readonly SqlSugarScope db;// 核心对象：拥有完整的SqlSugar全部功能
        private readonly ISysConfigService _sysConfigService;
        private readonly ISynThirdInfoService _synThirdInfoService;
        /// <summary>
        /// 初始化一个<see cref="DepartmentService"/>类型的新实例
        /// </summary>
        /// <param name="departmentRepository"></param>
        /// <param name="positionRepository"></param>
        /// <param name="userRepository"></param>
        /// <param name="organizeService"></param>
        /// <param name="sysConfigService"></param>
        /// <param name="synThirdInfoService"></param>
        public DepartmentService(ISqlSugarRepository<OrganizeEntity> departmentRepository, ISqlSugarRepository<PositionEntity> positionRepository, ISqlSugarRepository<UserEntity> userRepository, IOrganizeService organizeService, ISysConfigService sysConfigService, ISynThirdInfoService synThirdInfoService)
        {
            _departmentRepository = departmentRepository;
            _positionRepository = positionRepository;
            _userRepository = userRepository;
            _organizeService = organizeService;
            db = departmentRepository.Context;
            _sysConfigService = sysConfigService;
            _synThirdInfoService = synThirdInfoService;
        }

        #region GET

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("{companyId}/Department")]
        public async Task<dynamic> GetList(string companyId, [FromQuery] KeywordInput input)
        {
            var data = new List<DepartmentListOutput>();
            //全部部门数据
            var departmentAllList = await db.Queryable<OrganizeEntity, UserEntity>((a, b) => new JoinQueryInfos(JoinType.Left, b.Id == a.ManagerId)).Select((a, b) => new { Id = a.Id, ParentId = a.ParentId, FullName = a.FullName, EnCode = a.EnCode, Description = a.Description, EnabledMark = a.EnabledMark, CreatorTime = a.CreatorTime, Manager = SqlFunc.MergeString(b.RealName, "/", b.Account), SortCode = a.SortCode, Category = a.Category, DeleteMark = a.DeleteMark }).MergeTable().Where(t => t.Category.Equals("department") && t.DeleteMark == null).OrderBy(o => o.SortCode).Select<DepartmentListOutput>().ToListAsync();
            //当前公司部门
            var departmentList = await _departmentRepository.Entities.WhereIF(!string.IsNullOrEmpty(input.keyword), d => d.FullName.Contains(input.keyword) || d.EnCode.Contains(input.keyword)).Where(t => t.ParentId.Equals(companyId) && t.Category.Equals("department") && t.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
            departmentList.ForEach(item =>
            {
                item.ParentId = "0";
                data.AddRange(departmentAllList.TreeChildNode(item.Id, t => t.id, t => t.parentId));
            });
            return new { list = data };
        }

        /// <summary>
        /// 获取下拉框
        /// </summary>
        /// <returns></returns>
        [HttpGet("Department/Selector/{id}")]
        public async Task<dynamic> GetSelector(string id)
        {
            var data = await _departmentRepository.Where(t => t.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
            if (!"0".Equals(id))
            {
                var info = data.Find(it => it.Id == id);
                data.Remove(info);
            }
            var treeList = data.Adapt<List<DepartmentSelectorOutput>>();
            treeList.ForEach(item =>
            {
                if (item.type.Equals("company"))
                {
                    item.icon = "icon-ym icon-ym-tree-organization3";
                }
            });
            return new { list = treeList.ToTree("-1") };
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("Department/{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var entity = await _departmentRepository.SingleAsync(d => d.Id == id);
            var output = entity.Adapt<DepartmentInfoOutput>();
            return output;
        }

        #endregion

        #region POST

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("Department")]
        public async Task Create([FromBody] DepartmentCrInput input)
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
            if (await _departmentRepository.AnyAsync(o => o.ParentId == input.parentId && o.EnCode == input.enCode && o.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D2014);
            if (await _departmentRepository.AnyAsync(o => o.ParentId == input.parentId && o.FullName == input.fullName && o.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D2019);
            var entity = input.Adapt<OrganizeEntity>();
            entity.Category = "department";
            var newEntity = await _departmentRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();
            _ = newEntity ?? throw JNPFException.Oh(ErrorCode.D2015);

            #region 第三方同步
            var sysConfig = await _sysConfigService.GetInfo();
            var orgList = new List<OrganizeListOutput>();
            orgList.Add(entity.Adapt<OrganizeListOutput>());
            if (sysConfig.dingSynIsSynOrg == 1)
            {
                await _synThirdInfoService.SynDep(2, 2, sysConfig, orgList);
            }
            if (sysConfig.qyhIsSynOrg == 1)
            {
                await _synThirdInfoService.SynDep(1, 2, sysConfig, orgList);
            }
            #endregion
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("Department/{id}")]
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
            if (await _departmentRepository.AnyAsync(o => o.ParentId.Equals(id) && o.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D2005);
            //该机构下有岗位，则不能删
            if (await _positionRepository.AnyAsync(p => p.OrganizeId.Equals(id) && p.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D2006);
            //该机构下有用户，则不能删
            if (await _userRepository.AnyAsync(u => SqlFunc.ToString(u.OrganizeId).Equals(id) && u.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D2004);
            var entity = await _departmentRepository.SingleAsync(o => o.Id == id && o.DeleteMark == null);
            _ = entity ?? throw JNPFException.Oh(ErrorCode.D2002);
            var isOK = await _departmentRepository.Context.Updateable(entity).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
            if (!(isOK > 0))
                throw JNPFException.Oh(ErrorCode.D2017);
            #region 第三方数据删除
            var sysConfig = await _sysConfigService.GetInfo();
            if (sysConfig.dingSynIsSynOrg == 1)
            {
                await _synThirdInfoService.DelSynData(2, 2, sysConfig, id);
            }
            if (sysConfig.qyhIsSynOrg == 1)
            {
                await _synThirdInfoService.DelSynData(1, 2, sysConfig, id);
            }
            #endregion
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("Department/{id}")]
        public async Task Update(string id, [FromBody] DepartmentUpInput input)
        {
            var user = new UserInfo();
            await Scoped.Create(async (_, scope) =>
            {
                var services = scope.ServiceProvider;

                var _userManager = App.GetService<IUserManager>(services);

                user = await _userManager.GetUserInfo();
            });
            var oldEntity = await _departmentRepository.SingleAsync(it => it.Id == id);
            if (oldEntity.ParentId != input.parentId && !user.dataScope.Any(it => it.organizeId == oldEntity.ParentId && it.Edit == true) && !user.isAdministrator)
                throw JNPFException.Oh(ErrorCode.D1013);
            if (!user.dataScope.Any(it => it.organizeId == id && it.Edit == true) && !user.isAdministrator)
                throw JNPFException.Oh(ErrorCode.D1013);
            if (input.parentId.Equals(id))
                throw JNPFException.Oh(ErrorCode.D2001);
            //父id不能为自己的子节点
            var childIdListById = await _organizeService.GetChildIdListWithSelfById(id);
            if (childIdListById.Contains(input.parentId))
                throw JNPFException.Oh(ErrorCode.D2001);
            if (await _departmentRepository.AnyAsync(o => o.ParentId == input.parentId && o.EnCode == input.enCode && o.Id != id && o.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D2014);
            if (await _departmentRepository.AnyAsync(o => o.ParentId == input.parentId && o.FullName == input.fullName && o.Id != id && o.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D2019);
            var entity = input.Adapt<OrganizeEntity>();
            var isOK = await _departmentRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOK > 0))
                throw JNPFException.Oh(ErrorCode.D2018);

            #region 第三方同步
            var sysConfig = await _sysConfigService.GetInfo();
            var orgList = new List<OrganizeListOutput>();
            var synEntity = _departmentRepository.FirstOrDefault(x => x.Id == id);
            orgList.Add(synEntity.Adapt<OrganizeListOutput>());
            if (sysConfig.dingSynIsSynOrg == 1)
            {
                await _synThirdInfoService.SynDep(2, 2, sysConfig, orgList);
            }
            if (sysConfig.qyhIsSynOrg == 1)
            {
                await _synThirdInfoService.SynDep(1, 2, sysConfig, orgList);
            }
            #endregion
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpPut("Department/{id}/Actions/State")]
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
            var entity = await _departmentRepository.FirstOrDefaultAsync(o => o.Id == id);
            _ = entity.EnabledMark == 1 ? 0 : 1;
            var isOk = await _departmentRepository.Context.Updateable(entity).UpdateColumns(o => new { o.EnabledMark }).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0))
                throw JNPFException.Oh(ErrorCode.D2016);
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 获取部门列表(其他服务使用)
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<OrganizeEntity>> GetListAsync()
        {
            return await _departmentRepository.Where(t => t.Category.Equals("department") && t.EnabledMark.Equals(1) && t.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
        }

        /// <summary>
        /// 部门名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public string GetDepName(string id)
        {
            var entity = _departmentRepository.FirstOrDefault(x => x.Id == id && x.Category == "department" && x.EnabledMark.Equals(1) && x.DeleteMark == null);
            var name = entity == null ? "" : entity.FullName;
            return name;
        }

        /// <summary>
        /// 公司名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public string GetComName(string id)
        {
            var name = "";
            var entity = _departmentRepository.FirstOrDefault(x => x.Id == id && x.EnabledMark.Equals(1) && x.DeleteMark == null);
            if (entity == null)
            {
                return name;
            }
            else
            {
                if (entity.Category == "company")
                {
                    return entity.FullName;
                }
                else
                {
                    var pEntity = _departmentRepository.FirstOrDefault(x => x.Id == entity.ParentId && x.EnabledMark.Equals(1) && x.DeleteMark == null);
                    return GetComName(pEntity.Id);
                }
            }
        }

        /// <summary>
        /// 公司id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public string GetCompanyId(string id)
        {
            var entity = _departmentRepository.FirstOrDefault(x => x.Id == id && x.EnabledMark.Equals(1) && x.DeleteMark == null);
            if (entity == null)
            {
                return "";
            }
            else
            {
                if (entity.Category == "company")
                {
                    return entity.Id;
                }
                else
                {
                    var pEntity = _departmentRepository.FirstOrDefault(x => x.Id == entity.ParentId && x.EnabledMark.Equals(1) && x.DeleteMark == null);
                    return GetCompanyId(pEntity.Id);
                }
            }
        }

        #endregion
    }
}