using JNPF.Common.Configuration;
using JNPF.Common.Const;
using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Util;
using JNPF.DataEncryption;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.EventBridge;
using JNPF.FriendlyException;
using JNPF.JsonSerialization;
using JNPF.OAuth.Service.Dto;
using JNPF.RemoteRequest.Extensions;
using JNPF.System.Entitys.Dto.Permission.User;
using JNPF.System.Entitys.Dto.System.SysLog;
using JNPF.System.Entitys.Permission;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.Permission;
using JNPF.System.Interfaces.System;
using JNPF.UnifyResult;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UAParser;
using Yitter.IdGenerator;

namespace JNPF.OAuth.Service
{
    /// <summary>
    /// 业务实现：身份认证模块
    /// </summary>
    [ApiDescriptionSettings(Tag = "OAuth", Name = "OAuth", Order = 160)]
    [Route("api/[controller]")]
    public class OAuthService : IDynamicApiController, ITransient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUsersService _userService; // 用户表仓储
        private readonly ISysConfigService _sysConfigService; //系统配置仓储
        private readonly IModuleService _moduleService;//功能模块
        private readonly IModuleColumnService _columnService; //功能列
        private readonly IModuleButtonService _moduleButtonService;//功能按钮
        private readonly IModuleFormService _formService;//表单
        private readonly IModuleDataAuthorizeSchemeService _moduleDataAuthorizeSchemeService;
        private readonly IUserManager _userManager; // 用户管理
        private readonly ISysCacheService _sysCacheService;

        private readonly SqlSugarScope _db;
        private readonly ITenant _tenant;

        /// <summary>
        /// 初始化一个<see cref="OAuthService"/>类型的新实例
        /// </summary>
        public OAuthService(IUsersService userService,
            IHttpContextAccessor httpContextAccessor,
            ISysConfigService sysConfigService,
            IModuleService moduleService,
            IModuleColumnService columnService,
            IModuleButtonService moduleButtonService,
            IModuleFormService formService,
            IModuleDataAuthorizeSchemeService moduleDataAuthorizeSchemeService,
            IUserManager userManager,
            ISysCacheService sysCacheService,
            ISqlSugarClient db)
        {
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _sysConfigService = sysConfigService;
            _moduleService = moduleService;
            _columnService = columnService;
            _formService = formService;
            _moduleButtonService = moduleButtonService;
            _moduleDataAuthorizeSchemeService = moduleDataAuthorizeSchemeService;
            _userManager = userManager;
            _sysCacheService = sysCacheService;
            _db = (SqlSugarScope)db;
            _tenant = (ITenant)_db;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="input">登录输入参数</param>
        /// <returns></returns>
        [HttpPost("Login")]
        [AllowAnonymous]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<LoginOutput> Login([FromForm] LoginInput input)
        {
            string tenantDbName = App.Configuration["ConnectionStrings:DBName"];
            string tenantId = App.Configuration["ConnectionStrings:ConfigId"];
            string tenantAccout = string.Empty;
            if (KeyVariable.MultiTenancy)
            {
                //分割账号
                var tenantAccount = input.account.Split('@');
                tenantId = tenantAccount.FirstOrDefault();
                if (tenantAccount.Length == 1)
                    input.account = "admin";
                else
                    input.account = tenantAccount[1];
                tenantAccout = input.account;

                var interFace = App.Configuration["JNPF_App:MultiTenancyDBInterFace"] + tenantId;
                var response = await interFace.GetAsStringAsync();
                var data = JSON.Deserialize<RESTfulResult<TenantInterFaceOutput>>(response);
                if (data == null)
                    throw JNPFException.Oh(ErrorCode.D1024);
                else if (data.data == null)
                    throw JNPFException.Oh(ErrorCode.D1023);
                else if (data.data.dotnet == null)
                    throw JNPFException.Oh(ErrorCode.D1025);
                else
                    tenantDbName = data.data.dotnet;

                _tenant.AddConnection(new ConnectionConfig()
                {
                    DbType = (DbType)Enum.Parse(typeof(DbType), App.Configuration["ConnectionStrings:DBType"]),
                    ConfigId = tenantId,//设置库的唯一标识
                    IsAutoCloseConnection = true,
                    ConnectionString = string.Format($"{App.Configuration["ConnectionStrings:DefaultConnection"]}", tenantDbName)
                });
                _tenant.ChangeDatabase(tenantId);
            }
            //根据用户账号获取用户秘钥
            var user = await _userService.GetInfoByAccount(input.account);
            _ = user ?? throw JNPFException.Oh(ErrorCode.D5002);

            //获取加密后的密码
            var encryptPasswod = MD5Encryption.Encrypt(input.password + user.Secretkey);

            var userAnyPwd = await _userService.GetInfoByLogin(input.account, encryptPasswod);
            _ = userAnyPwd ?? throw JNPFException.Oh(ErrorCode.D1000);

            // 验证账号是否未被激活
            if (user.EnabledMark == null)
                throw JNPFException.Oh(ErrorCode.D1018);
            // 验证账号是否被禁用
            if (user.EnabledMark == 0)
                throw JNPFException.Oh(ErrorCode.D1019);
            // 验证账号是否被删除
            if (user.DeleteMark == 1)
                throw JNPFException.Oh(ErrorCode.D1017);
            // app权限验证
            if (NetUtil.isMobileBrowser && user.IsAdministrator == 0 && !ExistRoleByApp(user.RoleId))
                throw JNPFException.Oh(ErrorCode.D1022);

            //登录成功时 判断单点登录信息


            //token过期时间
            var tokenTimeout = await _sysConfigService.GetInfo("SysConfig", "tokentimeout");

            var accessToken = string.Empty;
            // 生成Token令牌
            if (KeyVariable.MultiTenancy)
            {
                accessToken = JWTEncryption.Encrypt(new Dictionary<string, object>
                {
                    { ClaimConst.CLAINM_USERID, userAnyPwd.Id },
                    { ClaimConst.CLAINM_ACCOUNT, userAnyPwd.Account },
                    { ClaimConst.CLAINM_REALNAME, userAnyPwd.RealName },
                    { ClaimConst.CLAINM_ADMINISTRATOR, userAnyPwd.IsAdministrator },
                    { ClaimConst.TENANT_ID, tenantId },
                    { ClaimConst.TENANT_DB_NAME, tenantDbName }
                }, long.Parse(tokenTimeout.Value));
            }
            else
            {
                accessToken = JWTEncryption.Encrypt(new Dictionary<string, object>
                {
                    { ClaimConst.CLAINM_USERID, userAnyPwd.Id },
                    { ClaimConst.CLAINM_ACCOUNT, userAnyPwd.Account },
                    { ClaimConst.CLAINM_REALNAME, userAnyPwd.RealName },
                    { ClaimConst.CLAINM_ADMINISTRATOR, userAnyPwd.IsAdministrator },
                    { ClaimConst.TENANT_ID, tenantId },
                    { ClaimConst.TENANT_DB_NAME, tenantDbName }
                }, long.Parse(tokenTimeout.Value));
            }
            var httpContext = _httpContextAccessor.HttpContext;

            // 设置Swagger自动登录
            httpContext.SigninToSwagger(accessToken);

            // 生成刷新Token令牌
            var refreshToken = JWTEncryption.GenerateRefreshToken(accessToken, 30);

            // 设置刷新Token令牌
            httpContext.Response.Headers["x-access-token"] = refreshToken;

            var ip = httpContext.GetRemoteIpAddressToIPv4();

            // 修改用户登录信息
            await Event.EmitAsync("User:UpdateUserLoginInfo", new UserEventDealWithInput
            {
                tenantId = tenantId,
                tenantDbName = tenantDbName,
                entity = new UserEntity()
                {
                    Id = user.Id,
                    FirstLogIP = user.FirstLogIP ?? ip,
                    FirstLogTime = user.FirstLogTime ?? DateTime.Now,
                    PrevLogTime = user.LastLogTime,
                    PrevLogIP = user.LastLogIP,
                    LastLogTime = DateTime.Now,
                    LastLogIP = ip,
                    LogSuccessCount = user.LogSuccessCount + 1
                }
            });

            //登录时间
            var clent = Parser.GetDefault().Parse(httpContext.Request.Headers["User-Agent"]);

            // 增加登录日志
            await Event.EmitAsync("Log:CreateVisLog", new LogEventBridgeCrInput
            {
                tenantId = tenantId,
                tenantDbName = tenantDbName,
                entity = new SysLogEntity
                {
                    Id = YitIdHelper.NextId().ToString(),
                    UserId = user.Id,
                    UserName = user.RealName,
                    Category = 1,
                    IPAddress = ip,
                    Abstracts = "登录成功",
                    PlatForm = clent.String,
                    CreatorTime = DateTime.Now
                }
            });

            return new LoginOutput()
            {
                theme = user.Theme == null ? "classic" : user.Theme,
                token = "Bearer " + accessToken
            };
        }

        /// <summary>
        /// 锁屏解锁登录
        /// </summary>
        /// <param name="input">登录输入参数</param>
        /// <returns></returns>
        [HttpPost("LockScreen")]
        public async Task LockScreen([Required] LoginInput input)
        {
            //根据用户账号获取用户秘钥
            var secretkey = (await _userService.GetInfoByAccount(input.account)).Secretkey;

            //获取加密后的密码
            var encryptPasswod = MD5Encryption.Encrypt(input.password + secretkey);

            var user = await _userService.GetInfoByLogin(input.account, encryptPasswod);
            _ = user ?? throw JNPFException.Oh(ErrorCode.D1000);
        }

        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("CurrentUser")]
        public async Task<CurrentUserOutput> GetCurrentUser()
        {
            var user = await _userManager.GetUserInfo();
            var userId = user.userId;
            var userContext = App.User;
            var httpContext = _httpContextAccessor.HttpContext;
            var tenantId = userContext?.FindFirstValue(ClaimConst.TENANT_ID);
            var tenantDbName = userContext?.FindFirstValue(ClaimConst.TENANT_DB_NAME);

            var loginOutput = new CurrentUserOutput();
            loginOutput.userInfo = user;

            //菜单
            loginOutput.menuList = await _moduleService.GetUserTreeModuleList(_userManager.IsAdministrator, userId);

            var currentUserModel = new CurrentUserModelOutput();
            currentUserModel.moduleList = await _moduleService.GetUserModueList(_userManager.IsAdministrator, userId);
            currentUserModel.buttonList = await _moduleButtonService.GetUserModuleButtonList(_userManager.IsAdministrator, userId);
            currentUserModel.columnList = await _columnService.GetUserModuleColumnList(_userManager.IsAdministrator, userId);
            currentUserModel.formList = await _formService.GetUserModuleFormList(_userManager.IsAdministrator, userId);
            currentUserModel.resourceList = await _moduleDataAuthorizeSchemeService.GetResourceList(_userManager.IsAdministrator, userId);

            //权限信息
            var permissionList = new List<PermissionModel>();
            currentUserModel.moduleList.ForEach(menu =>
            {
                var permissionModel = new PermissionModel();
                permissionModel.modelId = menu.id;
                permissionModel.moduleName = menu.fullName;
                permissionModel.button = currentUserModel.buttonList.FindAll(t => t.moduleId.Equals(menu.id)).Adapt<List<AuthorizeModuleButtonModel>>();
                permissionModel.column = currentUserModel.columnList.FindAll(t => t.moduleId.Equals(menu.id)).Adapt<List<AuthorizeModuleColumnModel>>();
                permissionModel.form= currentUserModel.formList.FindAll(t => t.moduleId.Equals(menu.id)).Adapt<List<AuthorizeModuleFormModel>>();
                permissionModel.resource = currentUserModel.resourceList.FindAll(t => t.moduleId.Equals(menu.id)).Adapt<List<AuthorizeModuleResourceModel>>();
                permissionList.Add(permissionModel);
            });

            //await _sysCacheService.SetAsync(CommonConst.CACHE_KEY_PERMISSION + "");
            loginOutput.permissionList = permissionList;

            return loginOutput;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        [HttpGet("Logout")]
        public async Task Logout()
        {

            var httpContext = _httpContextAccessor.HttpContext;
            httpContext.SignoutToSwagger();

            var user = _userManager.User;
            await _sysCacheService.DelUserInfo(_userManager.TenantId + "_" + user.Id);
            var clent = Parser.GetDefault().Parse(httpContext.Request.Headers["User-Agent"]);
            var userContext = App.User;
            var tenantId = userContext?.FindFirstValue(ClaimConst.TENANT_ID);
            var tenantDbName = userContext?.FindFirstValue(ClaimConst.TENANT_DB_NAME);
            //清除IM中的webSocket
            var list = _sysCacheService.GetOnlineUserList(tenantId);
            var onlineUser = list.Find(it => it.tenantId == tenantId && it.userId == user.Id);
            list.RemoveAll((x) => x.connectionId == onlineUser.connectionId);
            _sysCacheService.SetOnlineUserList(tenantId, list);

            //// 增加退出日记
            //Event.Emit("Log:CreateVisLog", new LogEventBridgeCrInput
            //{
            //    tenantId = tenantId,
            //    tenantDbName = tenantDbName,
            //    entity = new SysLogEntity
            //    {
            //        Id = YitIdHelper.NextId().ToString(),
            //        UserId = user.Id,
            //        UserName = user.RealName,
            //        Category = 1,
            //        IPAddress = httpContext.GetRemoteIpAddressToIPv4(),
            //        Abstracts = "退出成功",
            //        PlatForm = clent.String,
            //        CreatorTime = DateTime.Now
            //    }
            //});
        }

        #region PrivateMethod
        /// <summary>
        /// 判断app用户角色是否存在且有效
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        private bool ExistRoleByApp(string roleIds)
        {
            if (roleIds.IsEmpty())
            {
                return false;
            }
            var roleIdList1 = roleIds.Split(",").ToList();
            var roleIdList2 = _db.Queryable<RoleEntity>().Where(x => x.DeleteMark == null && x.EnabledMark == 1).Select(x => x.Id).ToList();
            return roleIdList1.Intersect(roleIdList2).ToList().Count > 0;
        }
        #endregion
    }
}
