using JNPF.Apps.Entitys.Dto;
using JNPF.Common.Core.Manager;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.System.Interfaces.Permission;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JNPF.Apps
{
    /// <summary>
    /// App用户信息
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "App", Name = "User", Order = 800)]
    [Route("api/App/[controller]")]
    public class AppUserService : IDynamicApiController, ITransient
    {
        private readonly IUsersService _usersService;
        private readonly IDepartmentService _departmentService;
        private readonly IRoleService _roleService;
        private readonly IPositionService _positionService;
        private readonly IUserManager _userManager;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="usersService"></param>
        /// <param name="departmentService"></param>
        /// <param name="roleService"></param>
        /// <param name="positionService"></param>
        /// <param name="userManager"></param>
        public AppUserService(IUsersService usersService, IDepartmentService departmentService, IRoleService roleService, IPositionService positionService, IUserManager userManager)
        {
            _usersService = usersService;
            _departmentService = departmentService;
            _roleService = roleService;
            _positionService = positionService;
            _userManager = userManager;
        }

        #region Get
        /// <summary>
        /// 用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetInfo()
        {
            var userEntity = _usersService.GetInfoByUserId(_userManager.UserId);
            var appUserInfo = userEntity.Adapt<AppUserOutput>();
            appUserInfo.positionIds = userEntity.PositionId == null ? null : await _usersService.GetPosition(userEntity.PositionId);
            appUserInfo.departmentName = _departmentService.GetDepName(userEntity.OrganizeId);
            appUserInfo.organizeId = _departmentService.GetCompanyId(userEntity.OrganizeId);
            appUserInfo.organizeName = _departmentService.GetComName(userEntity.OrganizeId);
            appUserInfo.roleName = _roleService.GetName(userEntity.RoleId);
            return appUserInfo;
        }

        /// <summary>
        /// 用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public dynamic GetInfo(string id)
        {
            var userEntity = _usersService.GetInfoByUserId(id);
            var appUserInfo = userEntity.Adapt<AppUserInfoOutput>();
            appUserInfo.organizeName = _departmentService.GetDepName(userEntity.OrganizeId);
            appUserInfo.positionName = _positionService.GetName(userEntity.PositionId);
            return appUserInfo;
        }
        #endregion
    }
}
