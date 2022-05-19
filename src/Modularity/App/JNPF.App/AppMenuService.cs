using JNPF.Apps.Entitys.Dto;
using JNPF.Apps.Interfaces;
using JNPF.Common.Util;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.Apps
{
    /// <summary>
    /// App菜单
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "App", Name = "Menu", Order = 800)]
    [Route("api/App/[controller]")]
    public class AppMenuService : IDynamicApiController, ITransient
    {
        private readonly IAppDataService _appDataService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appDataService"></param>
        public AppMenuService(IAppDataService appDataService)
        {
            _appDataService = appDataService;
        }

        #region Get
        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList()
        {
            var list = (await _appDataService.GetAppMenuList()).Adapt<List<AppMenuListOutput>>();
            var output = list.ToTree("-1");
            return new { list = output };
        }
        #endregion
    }
}
