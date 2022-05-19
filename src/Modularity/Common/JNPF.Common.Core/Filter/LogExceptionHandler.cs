using JNPF.Common.Const;
using JNPF.Dependency;
using JNPF.EventBridge;
using JNPF.FriendlyException;
using JNPF.System.Entitys.Dto.System.SysLog;
using JNPF.System.Entitys.System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using UAParser;
using Yitter.IdGenerator;

namespace JNPF.Common.Core.Filter
{
    /// <summary>
    /// 全局异常处理
    /// </summary>
    public class LogExceptionHandler : IGlobalExceptionHandler, ISingleton
    {
        /// <summary>
        /// 异步写入异常日记
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task OnExceptionAsync(ExceptionContext context)
        {
            var userContext = App.User;
            var httpContext = context.HttpContext;
            var httpRequest = httpContext.Request;
            var headers = httpRequest.Headers;
            var clientInfo = Parser.GetDefault().Parse(headers["User-Agent"]);

            if (!httpRequest.Path.Value.Contains("/api/File"))
            {
                Event.Emit("Log:CreateExLog", new LogEventBridgeCrInput
                {
                    tenantId = userContext?.FindFirstValue(ClaimConst.TENANT_ID),
                    tenantDbName = userContext?.FindFirstValue(ClaimConst.TENANT_DB_NAME),
                    entity = new SysLogEntity
                    {
                        Id = YitIdHelper.NextId().ToString(),
                        UserId = userContext?.FindFirstValue(ClaimConst.CLAINM_USERID),
                        UserName = userContext?.FindFirstValue(ClaimConst.CLAINM_REALNAME),
                        Category = 4,
                        IPAddress = httpContext.GetRemoteIpAddressToIPv4(),
                        RequestURL = httpRequest.Path,
                        RequestMethod = httpRequest.Method,
                        Json = context.Exception.Message + "\n" + context.Exception.StackTrace + "\n" + context.Exception.TargetSite.GetParameters().ToString(),
                        PlatForm = clientInfo.String,
                        CreatorTime = DateTime.Now
                    }
                });
            }

            // 写日志文件
            Log.Error(context.Exception.ToString());

            return Task.CompletedTask;
        }
    }
}
