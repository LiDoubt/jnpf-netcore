using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Filter;
using JNPF.Common.Util;
using JNPF.DataEncryption;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.System.Entitys.Dto.Permission.UsersCurrent;
using JNPF.System.Entitys.Model.Permission.UsersCurrent;
using JNPF.System.Entitys.Permission;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.Permission;
using JNPF.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.System.Service.Permission
{
    /// <summary>
    /// 业务实现:个人资料
    /// </summary>
    [ApiDescriptionSettings(Tag = "Permission", Name = "Current", Order = 168)]
    [Route("api/permission/Users/[controller]")]
    public class UsersCurrentService : IUsersCurrentService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<UserEntity> _userRepository;
        private readonly ISqlSugarRepository<PositionEntity> _positionRepository;
        private readonly ISqlSugarRepository<RoleEntity> _roleRepository;
        private readonly ISqlSugarRepository<SysLogEntity> _sysLogRepository;
        private readonly IAuthorizeService _authorizeService;
        private readonly ISysCacheService _sysCacheService;
        private readonly IUserManager _userManager; // 用户管理

        /// <summary>
        /// 初始化一个<see cref="UsersCurrentService"/>类型的新实例
        /// </summary>
        public UsersCurrentService(ISqlSugarRepository<UserEntity> userRepository, ISqlSugarRepository<PositionEntity> positionRepository, ISqlSugarRepository<RoleEntity> roleRepository, ISqlSugarRepository<SysLogEntity> sysLogRepository, IAuthorizeService authorizeService, ISysCacheService sysCacheService, IUserManager userManager)
        {
            _userRepository = userRepository;
            _positionRepository = positionRepository;
            _roleRepository = roleRepository;
            _sysLogRepository = sysLogRepository;
            _authorizeService = authorizeService;
            _sysCacheService = sysCacheService;
            _userManager = userManager;
        }

        #region GET

        /// <summary>
        /// 获取我的下属
        /// </summary>
        /// <returns></returns>
        [HttpGet("Subordinate")]
        public async Task<dynamic> GetSubordinate()
        {
            var userInfo = await _userManager.GetUserInfo();
            var subordinates = userInfo.subordinates.ToList();
            var data = await _userRepository.Context.Queryable<UserEntity, OrganizeEntity>((a, b) => new JoinQueryInfos(JoinType.Left, b.Id == SqlFunc.ToString(a.OrganizeId))).Select((a, b) => new
            {
                Id = a.Id,
                Avatar = SqlFunc.MergeString("/api/File/Image/userAvatar/", a.HeadIcon),
                Department = b.FullName,
                UserName = SqlFunc.MergeString(a.RealName, "/", a.Account),
                DeleteMark = a.DeleteMark,
                EnabledMark = a.EnabledMark,
                SortCode = a.SortCode
            }).MergeTable()
                .WhereIF(subordinates.Any(), u => subordinates.Contains(u.Id))
                .Where(u => u.DeleteMark == null && u.EnabledMark.Equals(1))
                .OrderBy(o => o.SortCode).Select<UsersCurrentSubordinateOutput>()
                .ToListAsync();
            return data;
        }

        /// <summary>
        /// 获取个人资料
        /// </summary>
        /// <returns></returns>
        [HttpGet("BaseInfo")]
        public async Task<dynamic> GetBaseInfo()
        {
            var userId = _userManager.UserId;
            var data = await _userRepository.Context.Queryable<UserEntity, OrganizeEntity, OrganizeEntity, UserEntity>((a, b, c, d) => new JoinQueryInfos(JoinType.Left, b.Id == SqlFunc.ToString(a.OrganizeId), JoinType.Left, c.Id == b.ParentId, JoinType.Left, d.Id == a.ManagerId))
                .Select((a, b, c, d) => new UsersCurrentInfoOutput { id = a.Id, account = a.Account, realName = a.RealName, organize = SqlFunc.IIF(c.FullName == null, b.FullName, SqlFunc.MergeString(b.FullName, " / ", c.FullName)), position = "", positionId = a.PositionId, manager = SqlFunc.IIF(d.Account == null, null, SqlFunc.MergeString(d.RealName, "/", d.Account)), roleId = "", roleIds = a.RoleId, creatorTime = a.CreatorTime, prevLogTime = a.PrevLogTime, signature = a.Signature, gender = a.Gender.ToString(), nation = a.Nation, nativePlace = a.NativePlace, entryDate = a.EntryDate, certificatesType = a.CertificatesType, certificatesNumber = a.CertificatesNumber, education = a.Education, birthday = a.Birthday, telePhone = a.TelePhone, landline = a.Landline, mobilePhone = a.MobilePhone, email = a.Email, urgentContacts = a.UrgentContacts, urgentTelePhone = a.UrgentTelePhone, postalAddress = a.PostalAddress, theme = a.Theme, language = a.Language, avatar = SqlFunc.MergeString("/api/File/Image/userAvatar/", a.HeadIcon) }).MergeTable()
                .Mapper((output) =>
                {
                    var PositionId = output.positionId == null ? null : output.positionId.Split(',').ToList();
                    var positionData = _positionRepository.Where(p => PositionId.Contains(p.Id)).ToList();
                    output.position = string.Join(",", positionData.Select(s => s.FullName).ToArray());
                })
                .Mapper((output) =>
                {
                    var RoleId = output.roleIds == null ? null : output.roleIds.Split(',').ToList();
                    var roleData = _roleRepository.Where(r => RoleId.Contains(r.Id)).ToList();
                    output.roleId = string.Join(",", roleData.Select(s => s.FullName).ToArray());
                })
                .Where(a => a.id == userId).FirstAsync();

            return data;
        }

        /// <summary>
        /// 获取系统权限
        /// </summary>
        /// <returns></returns>
        [HttpGet("Authorize")]
        public async Task<dynamic> GetAuthorize()
        {
            var userId = _userManager.UserId;
            var isAdmin = _userManager.IsAdministrator;
            var output = new UsersCurrentAuthorizeOutput();
            var moduleList = await _authorizeService.GetCurrentUserModuleAuthorize(userId, isAdmin);
            var buttonList = await _authorizeService.GetCurrentUserButtonAuthorize(userId, isAdmin);
            var columnList = await _authorizeService.GetCurrentUserColumnAuthorize(userId, isAdmin);
            var resourceList = await _authorizeService.GetCurrentUserResourceAuthorize(userId, isAdmin);
            if (moduleList.Count != 0)
                output.module = moduleList.Adapt<List<UsersCurrentAuthorizeMoldel>>().ToTree("-1");
            if (buttonList.Count != 0)
            {
                var menuAuthorizeData = new List<UsersCurrentAuthorizeMoldel>();
                var pids = buttonList.Select(m => m.ModuleId).ToList();
                this.GetParentsModuleList(pids, moduleList, ref menuAuthorizeData);
                output.button = menuAuthorizeData.Union(buttonList.Adapt<List<UsersCurrentAuthorizeMoldel>>()).ToList().ToTree("-1");
            }
            if (columnList.Count != 0)
            {
                var menuAuthorizeData = new List<UsersCurrentAuthorizeMoldel>();
                var pids = columnList.Select(m => m.ModuleId).ToList();
                this.GetParentsModuleList(pids, moduleList, ref menuAuthorizeData);
                output.column = menuAuthorizeData.Union(columnList.Adapt<List<UsersCurrentAuthorizeMoldel>>()).ToList().ToTree("-1");
            }
            if (resourceList.Count != 0)
            {
                var resourceData = resourceList.Select(r => new UsersCurrentAuthorizeMoldel
                {
                    id = r.Id,
                    parentId = r.ModuleId,
                    fullName = r.FullName,
                    icon = "icon-ym icon-ym-extend"
                }).ToList();
                var menuAuthorizeData = new List<UsersCurrentAuthorizeMoldel>();
                var pids = resourceList.Select(bt => bt.ModuleId).ToList();
                this.GetParentsModuleList(pids, moduleList, ref menuAuthorizeData);
                output.resource = menuAuthorizeData.Union(resourceData.Adapt<List<UsersCurrentAuthorizeMoldel>>()).ToList().ToTree("-1");
            }
            return output;
        }

        /// <summary>
        /// 获取系统日志
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("SystemLog")]
        public async Task<dynamic> GetSystemLog([FromQuery] UsersCurrentSystemLogQuery input)
        {
            DateTime? startTime = input.startTime != null ? Ext.GetDateTime(input.startTime.ToString()) : null;
            DateTime? endTime = input.endTime != null ? Ext.GetDateTime(input.endTime.ToString()) : null;
            var userId = _userManager.UserId;
            var requestParam = input.Adapt<PageInputBase>();
            var data = await _sysLogRepository.Context.Queryable<SysLogEntity>().Select(a => new UsersCurrentSystemLogOutput { creatorTime = a.CreatorTime, userName = a.UserName, ipaddress = a.IPAddress, moduleName = a.ModuleName, category = a.Category, userId = a.UserId, platForm = a.PlatForm, requestURL = a.RequestURL, requestMethod = a.RequestMethod, requestDuration = a.RequestDuration }).MergeTable()
                .WhereIF(!startTime.IsNullOrEmpty(), s => s.creatorTime >= new DateTime(startTime.ToDate().Year, startTime.ToDate().Month, startTime.ToDate().Day, 0, 0, 0, 0))
                .WhereIF(!endTime.IsNullOrEmpty(), s => s.creatorTime <= new DateTime(endTime.ToDate().Year, endTime.ToDate().Month, endTime.ToDate().Day, 23, 59, 59, 999))
                .WhereIF(!input.keyword.IsNullOrEmpty(), s => s.userName.Contains(input.keyword) || s.ipaddress.Contains(input.keyword) || s.moduleName.Contains(input.keyword))
                .Where(s => s.category == input.category && s.userId == userId).OrderBy(o => o.creatorTime, OrderByType.Desc).ToPagedListAsync(requestParam.currentPage, requestParam.pageSize);
            return PageResult<UsersCurrentSystemLogOutput>.SqlSugarPageResult(data);
        }

        #endregion

        #region Post

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [HttpPost("Actions/ModifyPassword")]
        public async Task ModifyPassword([FromBody] UsersCurrentActionsModifyPasswordInput input)
        {
            var user = _userManager.User;
            if (MD5Encryption.Encrypt(input.oldPassword + user.Secretkey) != user.Password)
                throw JNPFException.Oh(ErrorCode.D5007);
            var imageCode = _sysCacheService.GetCode(input.timestamp);
            if (input.code.ToLower().Equals(imageCode))
            {
                throw JNPFException.Oh(ErrorCode.D5007);
            }
            else
            {
                await _sysCacheService.DelCode(input.timestamp);
                await _sysCacheService.DelUserInfo(user.Id);
            }
            user.Password = MD5Encryption.Encrypt(input.password + user.Secretkey);
            user.ChangePasswordDate = DateTime.Now;
            var isOk = await _userRepository.Context.Updateable(user).UpdateColumns(it => new
            {
                it.Password,
                it.ChangePasswordDate,
                it.LastModifyUserId,
                it.LastModifyTime
            }).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.D5008);
        }

        /// <summary>
        /// 修改个人资料
        /// </summary>
        /// <returns></returns>
        [HttpPut("BaseInfo")]
        public async Task UpdateBaseInfo([FromBody] UsersCurrentInfoUpInput input)
        {
            var user = _userManager.User;
            var userInfo = input.Adapt<UserEntity>();
            userInfo.Id = user.Id;
            userInfo.IsAdministrator = user.IsAdministrator;
            var isOk = await _userRepository.Context.Updateable(userInfo).UpdateColumns(it => new
            {
                it.RealName,
                it.Signature,
                it.Gender,
                it.Nation,
                it.NativePlace,
                it.CertificatesType,
                it.CertificatesNumber,
                it.Education,
                it.Birthday,
                it.TelePhone,
                it.Landline,
                it.MobilePhone,
                it.Email,
                it.UrgentContacts,
                it.UrgentTelePhone,
                it.PostalAddress,
                it.LastModifyUserId,
                it.LastModifyTime
            }).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.D5009);
        }

        /// <summary>
        /// 修改主题
        /// </summary>
        /// <returns></returns>
        [HttpPut("SystemTheme")]
        public async Task UpdateBaseInfo([FromBody] UsersCurrentSysTheme input)
        {
            var user = _userManager.User;
            user.Theme = input.theme;
            var isOk = await _userRepository.Context.Updateable(user).UpdateColumns(it => new
            {
                it.Theme,
                it.LastModifyUserId,
                it.LastModifyTime
            }).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.D5010);
        }

        /// <summary>
        /// 修改语言
        /// </summary>
        /// <returns></returns>
        [HttpPut("SystemLanguage")]
        public async Task UpdateLanguage([FromBody] UsersCurrentSysLanguage input)
        {
            var user = _userManager.User;
            user.Language = input.language;
            var isOk = await _userRepository.Context.Updateable(user).UpdateColumns(it => new
            {
                it.Language,
                it.LastModifyUserId,
                it.LastModifyTime
            }).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.D5011);
        }

        /// <summary>
        /// 修改头像
        /// </summary>
        /// <returns></returns>
        [HttpPut("Avatar/{name}")]
        public async Task UpdateAvatar(string name)
        {
            var user = _userManager.User;
            user.HeadIcon = name;
            var isOk = await _userRepository.Context.Updateable(user).UpdateColumns(it => new
            {
                it.HeadIcon,
                it.LastModifyUserId,
                it.LastModifyTime
            }).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.D5012);
        }

        #endregion

        #region PrivateMethod

        /// <summary>
        /// 过滤菜单权限数据
        /// </summary>
        /// <param name="pids">其他权限数据</param>
        /// <param name="moduleList">勾选菜单权限数据</param>
        /// <param name="output">返回值</param>
        private void GetParentsModuleList(List<string> pids, List<ModuleEntity> moduleList, ref List<UsersCurrentAuthorizeMoldel> output)
        {
            var authorizeModuleData = moduleList.Adapt<List<UsersCurrentAuthorizeMoldel>>();
            foreach (var item in pids)
            {
                this.GteModuleListById(item, authorizeModuleData, output);
            }
            output = output.Distinct().ToList();
        }

        /// <summary>
        /// 根据菜单id递归获取authorizeDataOutputModel的父级菜单
        /// </summary>
        /// <param name="id">菜单id</param>
        /// <param name="authorizeModuleData">选中菜单集合</param>
        /// <param name="output">返回数据</param>
        private void GteModuleListById(string id, List<UsersCurrentAuthorizeMoldel> authorizeModuleData, List<UsersCurrentAuthorizeMoldel> output)
        {
            var data = authorizeModuleData.Find(l => l.id == id);
            if (data != null)
            {
                if (!data.parentId.Equals("-1"))
                {
                    if (!output.Contains(data))
                    {
                        output.Add(data);
                    }
                    GteModuleListById(data.parentId, authorizeModuleData, output);
                }
                else
                {
                    if (!output.Contains(data))
                    {
                        output.Add(data);
                    }
                }
            }
        }

        #endregion
    }
}
