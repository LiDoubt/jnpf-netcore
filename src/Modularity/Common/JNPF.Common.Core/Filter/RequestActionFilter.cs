using JNPF.Common.Const;
using JNPF.EventBridge;
using JNPF.System.Entitys.Dto.System.SysLog;
using JNPF.System.Entitys.System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using UAParser;
using Yitter.IdGenerator;

namespace JNPF.Common.Core.Filter
{
    /// <summary>
    /// 请求日志拦截
    /// </summary>
    public class RequestActionFilter : IAsyncActionFilter
    {
        /// <summary>
        /// 请求日记写入
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userContext = App.User;
            var httpContext = context.HttpContext;
            var httpRequest = httpContext.Request;

            var sw = new Stopwatch();
            sw.Start();
            var actionContext = await next();
            sw.Stop();

            // 判断是否请求成功（没有异常就是请求成功）
            var isRequestSucceed = actionContext.Exception == null;
            var headers = httpRequest.Headers;
            var clientInfo = Parser.GetDefault().Parse(headers["User-Agent"]);
            var tenantId = userContext?.FindFirstValue(ClaimConst.TENANT_ID);
            var tenantDbName = userContext?.FindFirstValue(ClaimConst.TENANT_DB_NAME);

            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (!httpRequest.Path.Value.Contains("/api/File/"))
            {
                await Event.EmitAsync("Log:CreateOpLog", new LogEventBridgeCrInput
                {
                    tenantId = tenantId,
                    tenantDbName = tenantDbName,
                    entity = new SysLogEntity
                    {
                        Id = YitIdHelper.NextId().ToString(),
                        UserId = userContext?.FindFirstValue(ClaimConst.CLAINM_USERID),
                        UserName = userContext?.FindFirstValue(ClaimConst.CLAINM_REALNAME),
                        Category = 5,
                        IPAddress = httpContext.GetRemoteIpAddressToIPv4(),
                        RequestURL = httpRequest.Path,
                        RequestDuration = (int)sw.ElapsedMilliseconds,
                        RequestMethod = httpRequest.Method,
                        PlatForm = clientInfo.String,
                        CreatorTime = DateTime.Now
                    }
                });
            }
        }
    }
}
