using JNPF.System.Entitys.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.Apps.Interfaces
{
    /// <summary>
    /// App常用数据
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IAppDataService
    {
        /// <summary>
        /// 菜单列表
        /// </summary>
        /// <returns></returns>
        Task<List<ModuleEntity>> GetAppMenuList();
    }
}
