using JNPF.Common.Configuration;
using JNPF.Common.Const;
using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Filter;
using JNPF.Common.Helper;
using JNPF.Common.Util;
using JNPF.DataEncryption;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.System.Entitys.Dto.Permission.Organize;
using JNPF.System.Entitys.Dto.Permission.Position;
using JNPF.System.Entitys.Dto.Permission.Role;
using JNPF.System.Entitys.Dto.Permission.User;
using JNPF.System.Entitys.Dto.Permission.UserRelation;
using JNPF.System.Entitys.Model.Permission.User;
using JNPF.System.Entitys.Permission;
using JNPF.System.Interfaces.Permission;
using JNPF.System.Interfaces.System;
using JNPF.VisualDev.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UAParser;

namespace JNPF.System.Service.Permission
{
    /// <summary>
    ///  业务实现：用户信息
    /// </summary>
    [ApiDescriptionSettings(Tag = "Permission", Name = "Users", Order = 163)]
    [Route("api/permission/[controller]")]
    public class UsersService : IUsersService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<UserEntity> _userRepository;  // 用户表仓储
        private readonly ISqlSugarRepository<PositionEntity> _positionRepository;
        private readonly ISqlSugarRepository<RoleEntity> _roleRepository;
        private readonly IOrganizeService _organizeService; // 机构表仓储
        private readonly IDepartmentService _departmentService;
        private readonly IUserRelationService _userRelationService; // 用户关系表服务
        private readonly ISysConfigService _sysConfigService; //系统配置仓储
        private readonly ISynThirdInfoService _synThirdInfoService;
        private readonly ISysCacheService _sysCacheService;

        private readonly HttpContext _httpContext;

        /// <summary>
        /// 初始化一个<see cref="UsersService"/>类型的新实例
        /// </summary>
        public UsersService(ISqlSugarRepository<UserEntity> userRepository, ISqlSugarRepository<PositionEntity> positionRepository, ISqlSugarRepository<RoleEntity> roleRepository, IOrganizeService organizeService, IUserRelationService userRelationService, ISysConfigService sysConfigService, ISysCacheService sysCacheService, ISynThirdInfoService synThirdInfoService, IDepartmentService departmentService)
        {
            _userRepository = userRepository;
            _positionRepository = positionRepository;
            _roleRepository = roleRepository;
            _organizeService = organizeService;
            _userRelationService = userRelationService;
            _sysCacheService = sysCacheService;
            _sysConfigService = sysConfigService;
            _httpContext = App.HttpContext;
            _synThirdInfoService = synThirdInfoService;
            _departmentService = departmentService;
        }

        #region GET

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] UserListQuery input)
        {
            //当前请求用户ID
            var userId = _httpContext.User.FindFirst(ClaimConst.CLAINM_USERID)?.Value;
            var tenantId = _httpContext.User.FindFirst(ClaimConst.TENANT_ID)?.Value;
            var pageInput = input.Adapt<PageInputBase>();
            var organizeIds = new List<string>();
            if (!input.organizeId.IsNullOrEmpty())
                organizeIds = await GetSubOrganizeIds(input.organizeId);
            var data = await _userRepository.Context.Queryable<UserEntity, OrganizeEntity>((a, b) => new JoinQueryInfos(JoinType.Left, b.Id == SqlFunc.ToString(a.OrganizeId)))
                .Select((a, b) => new UserListOutput { id = a.Id, account = a.Account, realName = a.RealName, creatorTime = a.CreatorTime, department = b.FullName, description = a.Description, enabledMark = a.EnabledMark, gender = a.Gender, mobilePhone = a.MobilePhone, positionId = a.PositionId, position = "", roleId = a.RoleId, roleName = "", sortCode = a.SortCode, deleteMark = a.DeleteMark, organizeId = a.OrganizeId })
                .MergeTable()
                .Mapper((output) =>
                {
                    //判断岗位缓存是否存在
                    var positionKey = CommonConst.CACHE_KEY_POSITION + tenantId + "_" + userId;
                    var PositionCacheList = new List<PositionCacheListOutput>();
                    if (_sysCacheService.Exists(positionKey))
                    {
                        PositionCacheList = _sysCacheService.GetPositionList(tenantId + "_" + userId);
                    }
                    else
                    {
                        var positionList = _positionRepository.Context.Queryable<PositionEntity>().ToList();
                        var cacheList = positionList.Adapt<List<PositionCacheListOutput>>();
                        PositionCacheList = cacheList;
                        _sysCacheService.SetPositionList(tenantId + "_" + userId, cacheList, TimeSpan.FromMinutes(10));
                    }
                    var PositionId = string.IsNullOrEmpty(output.positionId) ? null : output.positionId.Split(',').ToList();
                    if (PositionId != null)
                    {
                        var positionData = PositionCacheList.Where(p => PositionId.Contains(p.id)).ToList();
                        output.position = string.Join(",", positionData.Select(s => s.fullName).ToArray());
                    }
                })
                .Mapper((output) =>
                {
                    var roleKey = CommonConst.CACHE_KEY_ROLE + tenantId + "_" + userId;
                    var PositionCacheList = new List<RoleCacheListOutput>();
                    if (_sysCacheService.Exists(roleKey))
                    {
                        PositionCacheList = _sysCacheService.GetRoleList(tenantId + "_" + userId);
                    }
                    else
                    {
                        var positionList = _roleRepository.Context.Queryable<RoleEntity>().ToList();
                        var cacheList = positionList.Adapt<List<RoleCacheListOutput>>();
                        PositionCacheList = cacheList;
                        _sysCacheService.SetRoleList(tenantId + "_" + userId, cacheList, TimeSpan.FromMinutes(10));
                    }
                    var RoleId = output.roleId == null ? null : output.roleId.Split(',').ToList();
                    if (RoleId != null)
                    {
                        var roleData = PositionCacheList.Where(r => RoleId.Contains(r.id)).ToList();
                        output.roleName = string.Join(",", roleData.Select(s => s.fullName).ToArray());
                    }
                })
                //组织机构
                .WhereIF(organizeIds.Any(), u => organizeIds.Contains(SqlFunc.ToString(u.organizeId)))
                .WhereIF(!pageInput.keyword.IsNullOrEmpty(), u => u.account.Contains(pageInput.keyword) || u.realName.Contains(pageInput.keyword))
                .Where(a => a.deleteMark == null)
                .OrderBy(a => a.sortCode).OrderBy(a => a.creatorTime, OrderByType.Desc)
                .ToPagedListAsync(pageInput.currentPage, pageInput.pageSize);
            return PageResult<UserListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 获取全部用户
        /// </summary>
        /// <returns></returns>
        [HttpGet("All")]
        public async Task<dynamic> GetUserAllList()
        {
            var list = await _userRepository.Context.Queryable<UserEntity, OrganizeEntity>((a, b) => new JoinQueryInfos(JoinType.Left, b.Id == SqlFunc.ToString(a.OrganizeId)))
                .Select((a, b) => new UserListAllOutput
                {
                    id = a.Id,
                    account = a.Account,
                    realName = a.RealName,
                    headIcon = SqlFunc.MergeString("/api/File/Image/userAvatar/", a.HeadIcon),
                    gender = a.Gender,
                    department = b.FullName,
                    sortCode = a.SortCode,
                    quickQuery = a.QuickQuery,
                    enabledMark = a.EnabledMark,
                    deleteMark = a.DeleteMark
                })
                .MergeTable().Where(p => p.enabledMark.Equals(1) && p.deleteMark == null).OrderBy(p => p.sortCode).ToListAsync();
            return list;
        }

        /// <summary>
        /// 获取IM用户列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("ImUser")]
        public async Task<dynamic> GetImUserList([FromQuery] PageInputBase input)
        {
            var list = await _userRepository.Context.Queryable<UserEntity, OrganizeEntity>((a, b) => new JoinQueryInfos(JoinType.Left, b.Id == SqlFunc.ToString(a.OrganizeId)))
                .Select((a, b) => new IMUserListOutput { id = a.Id, account = a.Account, realName = a.RealName, headIcon = SqlFunc.MergeString("/api/File/Image/userAvatar/", a.HeadIcon), department = b.FullName, sortCode = a.SortCode, enabledMark = a.EnabledMark, deleteMark = a.DeleteMark })
                .MergeTable().WhereIF(!input.keyword.IsNullOrEmpty(), u => u.account.Contains(input.keyword) || u.realName.Contains(input.keyword)).Where(p => p.enabledMark.Equals(1) && p.deleteMark == null).OrderBy(p => p.sortCode).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<IMUserListOutput>.SqlSugarPageResult(list);
        }

        /// <summary>
        /// 获取下拉框（公司+部门+用户）
        /// </summary>
        /// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> GetSelector()
        {
            var organizeList = await _organizeService.GetListAsync();
            var userList = await _userRepository.Where(t => t.EnabledMark.Equals(1) && t.DeleteMark == null).ToListAsync();
            var organizeTreeList = organizeList.Adapt<List<UserSelectorOutput>>();
            var treeList = userList.Adapt<List<UserSelectorOutput>>();
            treeList = treeList.Concat(organizeTreeList).ToList();
            return new { list = treeList.ToTree("-1") };
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var entity = await _userRepository.FirstOrDefaultAsync(u => u.Id == id);
            var output = entity.Adapt<UserInfoOutput>();
            return output;
        }

        #endregion

        #region POST

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetUserList")]
        public async Task<dynamic> GetUserList([FromBody] UserRelationInput input)
        {
            var data = await _userRepository.Context.Queryable<UserEntity>().In(it => it.Id, input.userId.ToArray()).Select(it => new { id = it.Id, fullName = SqlFunc.MergeString(it.RealName, "/", it.Account), enabledMark = it.EnabledMark, deleteMark = it.DeleteMark, sortCode = it.SortCode }).MergeTable().Select<UserRelationListOutput>().Where(it => it.enabledMark.Equals(1) && it.deleteMark == null).OrderBy(it => it.sortCode).ToListAsync();
            return new { list = data };
        }

        /// <summary>
        /// 获取机构成员列表
        /// </summary>
        /// <param name="organizeId">机构ID</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("ImUser/Selector/{organizeId}")]
        public async Task<dynamic> GetOrganizeMemberList(string organizeId, [FromBody] KeywordInput input)
        {
            var output = new List<OrganizeMemberListOutput>();
            if (!input.keyword.IsNullOrEmpty())
            {
                output = await _userRepository.Entities.Select(u =>
                new OrganizeMemberListOutput
                {
                    id = u.Id,
                    Account = u.Account,
                    RealName = u.RealName,
                    fullName = SqlFunc.MergeString(u.RealName, "/", u.Account),
                    enabledMark = u.EnabledMark,
                    icon = "icon-ym icon-ym-tree-user2",
                    isLeaf = true,
                    hasChildren = false,
                    type = "user",
                    DeleteMark = u.DeleteMark,
                    SortCode = u.SortCode
                }).MergeTable()
                    .WhereIF(!input.keyword.IsNullOrEmpty(),
                    u => u.Account.Contains(input.keyword) || u.RealName.Contains(input.keyword))
                    .Where(u => u.enabledMark.Equals(1) && u.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
            }
            else
            {
                output = await _organizeService.GetOrganizeMemberList(organizeId);
            }
            return new { list = output };
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] UserCrInput input)
        {
            var user = new UserInfo();
            await Scoped.Create(async (_, scope) =>
            {
                var services = scope.ServiceProvider;

                var _userManager = App.GetService<IUserManager>(services);

                user = await _userManager.GetUserInfo();
            });
            if (!user.dataScope.Any(it => it.organizeId == input.organizeId && it.Add == true) && !user.isAdministrator)
            {
                throw JNPFException.Oh(ErrorCode.D1013);
            }

            var isExist = await _userRepository.AnyAsync(u => u.Account == input.account && u.DeleteMark == null);
            if (isExist) throw JNPFException.Oh(ErrorCode.D1003);
            var entity = input.Adapt<UserEntity>();

            #region 用户表单

            entity.IsAdministrator = 0;
            entity.EntryDate = input.entryDate.IsNullOrEmpty() ? DateTime.Now : Ext.GetDateTime(input.entryDate.ToString());
            entity.Birthday = input.birthday.IsNullOrEmpty() ? DateTime.Now : Ext.GetDateTime(input.birthday.ToString());
            entity.QuickQuery = PinyinUtil.PinyinString(input.realName);
            entity.Secretkey = Guid.NewGuid().ToString();
            entity.Password = MD5Encryption.Encrypt(MD5Encryption.Encrypt(CommonConst.DEFAULT_PASSWORD) + entity.Secretkey);
            var headIcon = input.headIcon.Split('/').ToList().Last();
            if (string.IsNullOrEmpty(headIcon))
                headIcon = "001.png";
            entity.HeadIcon = headIcon;

            #endregion

            try
            {
                //开启事务
                _userRepository.Ado.BeginTran();

                //新增用户记录
                var newEntity = await _userRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();

                //将临时文件迁移至正式文件
                FileHelper.MoveFile(FileVariable.TemporaryFilePath + headIcon, FileVariable.UserAvatarFilePath + headIcon);

                var userRelationList = new List<UserRelationEntity>();
                var positionList = _userRelationService.CreateByPosition(newEntity.Id, newEntity.PositionId);
                var roleList = _userRelationService.CreateByRole(newEntity.Id, newEntity.RoleId);
                userRelationList.AddRange(positionList);
                userRelationList.AddRange(roleList);

                if (userRelationList.Count > 0)
                {
                    //批量新增用户关系
                    await _userRelationService.Create(userRelationList);
                }

                _userRepository.Ado.CommitTran();
            }
            catch (Exception)
            {
                _userRepository.Ado.RollbackTran();
                throw JNPFException.Oh(ErrorCode.D5001);
            }

            #region 第三方同步
            var sysConfig = await _sysConfigService.GetInfo();
            var userList = new List<UserEntity>();
            userList.Add(entity);
            if (sysConfig.dingSynIsSynUser == 1)
            {
                await _synThirdInfoService.SynUser(2, 3, sysConfig, userList);
            }
            if (sysConfig.qyhIsSynUser == 1)
            {
                await _synThirdInfoService.SynUser(1, 3, sysConfig, userList);
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
            var entity = await _userRepository.FirstOrDefaultAsync(u => u.Id == id && u.DeleteMark == null);
            if (!user.dataScope.Any(it => it.organizeId == entity.OrganizeId && it.Delete == true) && !user.isAdministrator)
            {
                throw JNPFException.Oh(ErrorCode.D1013);
            }
            var userId = _httpContext.User.FindFirst(ClaimConst.CLAINM_USERID)?.Value;
            var depManagerId = await _organizeService.GetIsManagerByUserId(id);
            if (depManagerId)
                throw JNPFException.Oh(ErrorCode.D2003);
            _ = entity ?? throw JNPFException.Oh(ErrorCode.D5002);
            if (entity.IsAdministrator == (int)AccountType.Administrator)
                throw JNPFException.Oh(ErrorCode.D1014);
            if (entity.Id == userId)
                throw JNPFException.Oh(ErrorCode.D1001);
            try
            {
                //开启事务
                _userRepository.Ado.BeginTran();

                //用户软删除
                await _userRepository.Context.Updateable(entity).IgnoreColumns(it => new { it.CreatorTime, it.CreatorUserId, it.LastModifyTime, it.LastModifyUserId }).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();

                //直接删除用户关系表相关相关数据
                await _userRelationService.Delete(id);

                _userRepository.Ado.CommitTran();
            }
            catch (Exception)
            {
                _userRepository.Ado.RollbackTran();
                throw;
            }
            #region 第三方同步
            var sysConfig = await _sysConfigService.GetInfo();
            if (sysConfig.dingSynIsSynUser == 1)
            {
                await _synThirdInfoService.DelSynData(2, 3, sysConfig, id);
            }
            if (sysConfig.qyhIsSynUser == 1)
            {
                await _synThirdInfoService.DelSynData(1, 3, sysConfig, id);
            }
            #endregion
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] UserUpInput input)
        {
            var user = new UserInfo();
            await Scoped.Create(async (_, scope) =>
            {
                var services = scope.ServiceProvider;

                var _userManager = App.GetService<IUserManager>(services);

                user = await _userManager.GetUserInfo();
            });
            var oldUserEntity = await _userRepository.SingleAsync(it => it.Id == id);
            //旧数据
            if (!user.dataScope.Any(it => it.organizeId == oldUserEntity.OrganizeId && it.Edit == true) && !user.isAdministrator)
            {
                throw JNPFException.Oh(ErrorCode.D1013);
            }
            //新数据
            if (!user.dataScope.Any(it => it.organizeId == input.organizeId && it.Edit == true) && !user.isAdministrator)
            {
                throw JNPFException.Oh(ErrorCode.D1013);
            }
            // 排除自己并且判断与其他是否相同
            var isExist = await _userRepository.AnyAsync(u => u.Account == input.account && u.DeleteMark == null && u.Id != id);
            if (isExist) throw JNPFException.Oh(ErrorCode.D1003);
            if (id == input.managerId) throw JNPFException.Oh(ErrorCode.D1021);
            var entity = input.Adapt<UserEntity>();
            entity.QuickQuery = PinyinUtil.PinyinString(input.realName);
            var headIcon = input.headIcon.Split('/').ToList().Last();
            entity.HeadIcon = headIcon;
            entity.LastModifyTime = DateTime.Now;
            entity.LastModifyUserId = user.userId;
            try
            {
                //开启事务
                _userRepository.Ado.BeginTran();

                //更新用户记录
                var newEntity = await _userRepository.Context.Updateable(entity).UpdateColumns(it => new
                {
                    it.Account,
                    it.RealName,
                    it.QuickQuery,
                    it.Gender,
                    it.Email,
                    it.OrganizeId,
                    it.ManagerId,
                    it.PositionId,
                    it.RoleId,
                    it.SortCode,
                    it.EnabledMark,
                    it.Description,
                    it.HeadIcon,
                    it.Nation,
                    it.NativePlace,
                    it.EntryDate,
                    it.CertificatesType,
                    it.CertificatesNumber,
                    it.Education,
                    it.UrgentContacts,
                    it.UrgentTelePhone,
                    it.PostalAddress,
                    it.MobilePhone,
                    it.Birthday,
                    it.TelePhone,
                    it.Landline,
                    it.LastModifyTime,
                    it.LastModifyUserId
                }).ExecuteCommandAsync();

                //将临时文件迁移至正式文件
                FileHelper.MoveFile(FileVariable.TemporaryFilePath + headIcon, FileVariable.UserAvatarFilePath + headIcon);

                //直接删除用户关系表相关相关数据
                await _userRelationService.Delete(id);

                var userRelationList = new List<UserRelationEntity>();
                var positionList = _userRelationService.CreateByPosition(id, entity.PositionId);
                var roleList = _userRelationService.CreateByRole(id, entity.RoleId);
                userRelationList.AddRange(positionList);
                userRelationList.AddRange(roleList);
                if (userRelationList.Count > 0)
                {
                    //批量新增用户关系
                    await _userRelationService.Create(userRelationList);
                }

                _userRepository.Ado.CommitTran();
            }
            catch (Exception)
            {
                FileHelper.MoveFile(FileVariable.UserAvatarFilePath + headIcon, FileVariable.TemporaryFilePath + headIcon);
                _userRepository.Ado.RollbackTran();
                throw JNPFException.Oh(ErrorCode.D5004);
            }

            #region 第三方同步
            var sysConfig = await _sysConfigService.GetInfo();
            var userList = new List<UserEntity>();
            userList.Add(entity);
            if (sysConfig.dingSynIsSynUser == 1)
            {
                await _synThirdInfoService.SynUser(2, 3, sysConfig, userList);
            }
            if (sysConfig.qyhIsSynUser == 1)
            {
                await _synThirdInfoService.SynUser(1, 3, sysConfig, userList);
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
            var entity = await _userRepository.SingleAsync(it => it.Id == id);
            if (!user.dataScope.Any(it => it.organizeId == entity.OrganizeId && it.Edit == true) && !user.isAdministrator)
            {
                throw JNPFException.Oh(ErrorCode.D1013);
            }

            if (!await _userRepository.AnyAsync(u => u.Id == id && u.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D1002);

            var isOk = await _userRepository.Context.Updateable<UserEntity>().SetColumns(it => new UserEntity()
            {
                EnabledMark = SqlFunc.IIF(it.EnabledMark == 1, 0, 1),
                LastModifyUserId = user.userId,
                LastModifyTime = SqlFunc.GetDate()
            }).Where(it => it.Id == id).ExecuteCommandAsync();

            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.D5005);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("{id}/Actions/ResetPassword")]
        public async Task ResetPassword(string id, [FromBody] UserResetPasswordInput input)
        {
            var user = new UserInfo();
            await Scoped.Create(async (_, scope) =>
            {
                var services = scope.ServiceProvider;

                var _userManager = App.GetService<IUserManager>(services);

                user = await _userManager.GetUserInfo();
            });
            var entity = await _userRepository.FirstOrDefaultAsync(u => u.Id == id && u.DeleteMark == null);
            if (!user.dataScope.Any(it => it.organizeId == entity.OrganizeId && it.Edit == true) && !user.isAdministrator)
            {
                throw JNPFException.Oh(ErrorCode.D1013);
            }

            if (!input.userPassword.Equals(input.validatePassword))
                throw JNPFException.Oh(ErrorCode.D5006);

            _ = entity ?? throw JNPFException.Oh(ErrorCode.D1002);

            var password = MD5Encryption.Encrypt(input.userPassword + entity.Secretkey);

            var isOk = await _userRepository.Context.Updateable<UserEntity>().SetColumns(it => new UserEntity()
            {
                Password = password,
                ChangePasswordDate = SqlFunc.GetDate(),
                LastModifyUserId = user.userId,
                LastModifyTime = SqlFunc.GetDate()
            }).Where(it => it.Id == id).ExecuteCommandAsync();

            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.D5005);

            //强制将用户提掉线
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 获取用户信息 根据用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [NonAction]
        public UserEntity GetInfoByUserId(string userId)
        {
            return _userRepository.FirstOrDefault(u => u.Id == userId && u.DeleteMark == null);
        }

        /// <summary>
        /// 获取用户信息 根据用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<UserEntity> GetInfoByUserIdAsync(string userId)
        {
            return await _userRepository.FirstOrDefaultAsync(u => u.Id == userId && u.DeleteMark == null);
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<UserEntity>> GetList()
        {
            return await _userRepository.Entities.Where(u => u.DeleteMark == null).ToListAsync();
        }

        /// <summary>
        /// 获取用户信息 根据用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="tenantId">租户ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<UserInfo> GetUserInfo(string userId, string tenantId)
        {
            var clent = Parser.GetDefault().Parse(_httpContext.Request.Headers["User-Agent"]);
            var ipAddress = _httpContext.GetRemoteIpAddressToIPv4();
            var ipAddressName = await NetUtil.GetLocation(ipAddress);
            var defaultPortalId = string.Empty;
            var userDataScope = new List<UserDataScope>();
            await Scoped.Create(async (_, scope) =>
              {
                  var services = scope.ServiceProvider;

                  var _portalService = App.GetService<IPortalService>(services);
                  var _organizeAdministratorService = App.GetService<IOrganizeAdministratorService>(services);
                  userDataScope = await _organizeAdministratorService.GetUserDataScope(userId);
                  defaultPortalId = await _portalService.GetDefault();
              });
            var sysConfigInfo = await _sysConfigService.GetInfo("SysConfig", "tokentimeout");
            var data = await _userRepository.Context.Queryable<UserEntity, OrganizeEntity>((a, b) => new JoinQueryInfos(JoinType.Left, b.Id == SqlFunc.ToString(a.OrganizeId))).Where(a => a.Id == userId)
                .Select((a, b) => new UserInfo
                {
                    userId = a.Id,
                    headIcon = SqlFunc.MergeString("/api/File/Image/userAvatar/", a.HeadIcon),
                    userAccount = a.Account,
                    userName = a.RealName,
                    gender = SqlFunc.ToInt32(a.Gender),
                    organizeId = a.OrganizeId,
                    organizeName = b.FullName,
                    managerId = a.ManagerId,
                    isAdministrator = SqlFunc.IIF(a.IsAdministrator == 1, true, false),
                    portalId = SqlFunc.IIF(a.PortalId == null, defaultPortalId, a.PortalId),
                    positionId = a.PositionId,
                    roleId = a.RoleId,
                    prevLoginTime = a.PrevLogTime,
                    prevLoginIPAddress = a.PrevLogIP
                }).FirstAsync();
            data.loginTime = DateTime.Now;
            data.loginIPAddress = ipAddress;
            data.loginIPAddressName = ipAddressName;
            data.prevLoginIPAddressName = await NetUtil.GetLocation(data.prevLoginIPAddress);
            data.loginPlatForm = clent.String;
            data.subsidiary = await _organizeService.GetSubsidiary(data.organizeId, data.isAdministrator);
            data.subordinates = await this.GetSubordinates(userId);
            data.positionIds = data.positionId == null ? null : await GetPosition(data.positionId);
            data.roleIds = data.roleId == null ? null : data.roleId.Split(',').ToArray();
            data.dataScope = userDataScope;
            //根据系统配置过期时间自动过期
            await _sysCacheService.SetUserInfo(tenantId + "_" + userId, data, TimeSpan.FromMinutes(sysConfigInfo.Value.ToDouble()));

            return data;
        }

        /// <summary>
        /// 获取用户信息 根据用户账户
        /// </summary>
        /// <param name="account">用户账户</param>
        /// <returns></returns>
        [NonAction]
        public async Task<UserEntity> GetInfoByAccount(string account)
        {
            return await _userRepository.FirstOrDefaultAsync(u => u.Account == account && u.DeleteMark == null);
        }

        /// <summary>
        /// 获取用户信息 根据登录信息
        /// </summary>
        /// <param name="account">用户账户</param>
        /// <param name="password">用户密码</param>
        /// <returns></returns>
        [NonAction]
        public async Task<UserEntity> GetInfoByLogin(string account, string password)
        {
            return await _userRepository.FirstOrDefaultAsync(u => u.Account == account && u.Password == password && u.DeleteMark == null);
        }

        /// <summary>
        /// 根据用户姓名获取用户ID
        /// </summary>
        /// <param name="realName">用户姓名</param>
        /// <returns></returns>
        [NonAction]
        public async Task<string> GetUserIdByRealName(string realName)
        {
            return (await _userRepository.FirstOrDefaultAsync(u => u.RealName == realName && u.DeleteMark == null)).Id;
        }

        /// <summary>
        /// 获取下属
        /// </summary>
        /// <param name="managerId">主管Id</param>
        /// <returns></returns>
        [NonAction]
        public async Task<string[]> GetSubordinates(string managerId)
        {
            List<string> data = new List<string>();
            var userIds = await _userRepository.Where(m => m.ManagerId == managerId && m.DeleteMark == null).OrderBy(o => o.SortCode).Select(m => m.Id).ToListAsync();
            data.AddRange(userIds);
            data.AddRange(await GetInfiniteSubordinats(userIds.ToArray()));
            return data.ToArray();
        }


        private async Task<List<string>> GetInfiniteSubordinats(string[] parentIds)
        {
            List<string> data = new List<string>();
            if (parentIds.ToList().Count > 0)
            {
                var userIds = await _userRepository.Context.Queryable<UserEntity>().In(it => it.ManagerId, parentIds).Where(it => it.DeleteMark == null).OrderBy(it => it.SortCode).Select(it => it.Id).ToListAsync();
                data.AddRange(userIds);
                data.AddRange(await GetInfiniteSubordinats(userIds.ToArray()));
            }
            return data;

        }

        /// <summary>
        /// 获取下属
        /// </summary>
        /// <param name="managerId">主管Id</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<string>> GetSubordinatesAsync(string managerId)
        {
            return await _userRepository.Where(m => m.ManagerId == managerId && m.DeleteMark == null).OrderBy(o => o.SortCode).Select(s => s.Id).ToListAsync();
        }

        /// <summary>
        /// 下属机构
        /// </summary>
        /// <param name="organizeId">机构ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<string>> GetSubOrganizeIds(string organizeId)
        {
            var data = await _organizeService.GetListAsync();
            data = data.TreeChildNode(organizeId, t => t.Id, t => t.ParentId);
            return data.Select(m => m.Id).ToList();
        }

        /// <summary>
        /// 获取下属
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<string>> GetSubordinateId(string userId)
        {
            var data = await _userRepository.Where(u => u.ManagerId == userId && u.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
            return data.Select(m => m.Id).ToList();
        }

        /// <summary>
        /// 是否存在机构用户
        /// </summary>
        /// <param name="organizeId">机构ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> ExistOrganizeUser(string organizeId)
        {
            return await _userRepository.AnyAsync(u => u.OrganizeId.Equals(organizeId) && u.DeleteMark == null);
        }

        /// <summary>
        /// 获取用户名
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<string> GetUserName(string userId)
        {
            var entity = await _userRepository.FirstOrDefaultAsync(x => x.Id == userId && x.DeleteMark == null);
            if (entity.IsNullOrEmpty())
                return "";
            return entity.RealName + "/" + entity.Account;
        }

        /// <summary>
        /// 获取当前用户岗位信息
        /// </summary>
        /// <param name="PositionIds"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<PositionInfo>> GetPosition(string PositionIds)
        {
            var ids = PositionIds.Split(",");
            return await _positionRepository.Entities.In(it => it.Id, ids).Select(it => new { id = it.Id, name = it.FullName }).MergeTable().Select<PositionInfo>().ToListAsync();
        }
        #endregion
    }
}