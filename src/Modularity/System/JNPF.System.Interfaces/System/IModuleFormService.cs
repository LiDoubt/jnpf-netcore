using JNPF.System.Entitys.System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces.System
{
    /// <summary>
    /// 表单权限
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IModuleFormService
    {
        /// <summary>
        /// 表单权限列表
        /// </summary>
        /// <param name="moduleId">功能id</param>
        /// <returns></returns>
        Task<List<ModuleFormEntity>> GetList(string moduleId);

        /// <summary>
        /// 获取用户功能列表
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<dynamic> GetUserModuleFormList(bool isAdmin, string userId);
    }
}
