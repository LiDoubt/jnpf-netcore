using JNPF.Authorization;
using JNPF.Common.Core.Manager;
using JNPF.DataEncryption;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.API.Core
{
    /// <summary>
    /// jwt处理程序
    /// </summary>
    public class JwtHandler : AppAuthorizeHandler
    {
        /// <summary>
        /// 重写 Handler 添加自动刷新
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task HandleAsync(AuthorizationHandlerContext context)
        {
            // 自动刷新Token
            if (JWTEncryption.AutoRefreshToken(context, context.GetCurrentHttpContext()))
            {
                await AuthorizeHandleAsync(context);
            }
            else context.Fail(); // 授权失败
        }

        /// <summary>
        /// 授权判断逻辑，授权通过返回 true，否则返回 false
        /// </summary>
        /// <param name="context"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public override async Task<bool> PipelineAsync(AuthorizationHandlerContext context, DefaultHttpContext httpContext)
        {
            // 此处已经自动验证 Jwt Token的有效性了，无需手动验证
            return await CheckAuthorzieAsync(httpContext);
        }

        /// <summary>
        /// 检查权限
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private static async Task<bool> CheckAuthorzieAsync(DefaultHttpContext httpContext)
        {
            // 管理员跳过判断
            var userManager = App.GetService<IUserManager>();
            if (userManager.IsAdministrator) return true;

            // 路由名称
            var routeName = httpContext.Request.Path.Value.Substring(1).Replace("/", ":");

            // 默认路由(获取登录用户信息)
            var defalutRoute = new List<string>()
            {
                "api:oauth:CurrentUser"
            };

            if (defalutRoute.Contains(routeName)) return true;

            // 获取用户权限集合（按钮或API接口）
            //var permissionList = await App.GetService<ISysMenuService>().GetLoginPermissionList(userManager.UserId);

            // 检查授权
            //return permissionList.Contains(routeName);
            return true;
        }
    }
}
