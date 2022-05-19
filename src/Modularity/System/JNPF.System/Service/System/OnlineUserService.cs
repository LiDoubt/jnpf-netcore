using JNPF.Common.Const;
using JNPF.Common.Extension;
using JNPF.Common.Filter;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.Message.Entitys.Dto.IM;
using JNPF.Message.Entitys.Model.IM;
using JNPF.Message.Interfaces;
using JNPF.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.System.Service.System
{
    /// <summary>
    ///  业务实现：在线用户
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "OnlineUser", Order = 176)]
    [Route("api/system/[controller]")]
    public class OnlineUserService : IDynamicApiController, ITransient
    {
        private readonly IImReplyService _imReplyService;
        private readonly ISysCacheService _sysCacheService;
        private readonly HttpContext _httpContext;

        /// <summary>
        /// 初始化一个<see cref="OnlineUserService"/>类型的新实例
        /// </summary>
        public OnlineUserService(ISysCacheService sysCacheService, IImReplyService imReplyService)
        {
            _imReplyService = imReplyService;
            _sysCacheService = sysCacheService;
            _httpContext = App.HttpContext;
        }

        /// <summary>
        /// 获取在线用户列表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public dynamic GetList([FromQuery] KeywordInput input)
        {
            var tenantId = _httpContext.User.FindFirst(ClaimConst.TENANT_ID)?.Value;
            var userOnlineList = new List<UserOnlineModel>();
            var onlineKey = CommonConst.CACHE_KEY_ONLINE_USER + $"{tenantId}";
            if (_sysCacheService.Exists(onlineKey))
            {
                userOnlineList = _sysCacheService.GetOnlineUserList(tenantId);
                if (!input.keyword.IsNullOrEmpty())
                    userOnlineList = userOnlineList.FindAll(x => x.userName.Contains(input.keyword));
            }
            return userOnlineList.Adapt<List<OnlineUserListOutput>>();
        }

        /// <summary>
        /// 强制下线
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public void ForcedOffline(string id)
        {
            var tenantId = _httpContext.User.FindFirst(ClaimConst.TENANT_ID)?.Value;
            var list = _sysCacheService.GetOnlineUserList(tenantId);
            var user = list.Find(it => it.tenantId == tenantId && it.userId == id);
            if (user != null)
            {
                _imReplyService.ForcedOffline(user.connectionId);
                _sysCacheService.DelOnlineUser(tenantId + "_" + user.userId);
            }
        }
    }
}
