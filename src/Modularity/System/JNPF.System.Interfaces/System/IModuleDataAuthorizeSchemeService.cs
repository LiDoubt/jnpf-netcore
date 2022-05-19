using JNPF.System.Entitys.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces.System
{
    /// <summary>
    /// 数据权限方案
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IModuleDataAuthorizeSchemeService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="moduleId">功能id</param>
        /// <returns></returns>
        Task<List<ModuleDataAuthorizeSchemeEntity>> GetList(string moduleId);

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        Task<List<ModuleDataAuthorizeSchemeEntity>> GetList();

        /// <summary>
        /// 获取按钮信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        Task<ModuleDataAuthorizeSchemeEntity> GetInfo(string id);

        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Create(ModuleDataAuthorizeSchemeEntity entity);

        /// <summary>
        /// 修改按钮
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Update(ModuleDataAuthorizeSchemeEntity entity);
        /// <summary>
        /// 删除按钮
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Delete(ModuleDataAuthorizeSchemeEntity entity);

        /// <summary>
        /// 获取用户数据权限方案
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<dynamic> GetResourceList(bool isAdmin, string userId);
    }
}
