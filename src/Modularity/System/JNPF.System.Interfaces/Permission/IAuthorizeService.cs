using JNPF.System.Entitys.Permission;
using JNPF.System.Entitys.System;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces.Permission
{
    /// <summary>
    /// 业务契约：操作权限
    /// </summary>
    public interface IAuthorizeService
    {
        /// <summary>
        /// 当前用户模块权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="isAdmin">是否超管</param>
        /// <returns></returns>
        Task<List<ModuleEntity>> GetCurrentUserModuleAuthorize(string userId, bool isAdmin);

        /// <summary>
        /// 当前用户模块按钮权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="isAdmin">是否超管</param>
        /// <returns></returns>
        Task<List<ModuleButtonEntity>> GetCurrentUserButtonAuthorize(string userId, bool isAdmin);

        /// <summary>
        /// 当前用户模块列权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="isAdmin">是否超管</param>
        /// <returns></returns>
        Task<List<ModuleColumnEntity>> GetCurrentUserColumnAuthorize(string userId, bool isAdmin);

        /// <summary>
        /// 当前用户模块权限资源
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="isAdmin">是否超管</param>
        /// <returns></returns>
        Task<List<ModuleDataAuthorizeSchemeEntity>> GetCurrentUserResourceAuthorize(string userId, bool isAdmin);

        /// <summary>
        /// 获取权限项ids
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <param name="itemType">项类型</param>
        /// <returns></returns>
        Task<List<string>> GetAuthorizeItemIds(string roleId, string itemType);

        /// <summary>
        /// 是否存在权限资源
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<bool> GetIsExistModuleDataAuthorizeScheme(string[] ids);

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="objectId">对象主键</param>
        /// <returns></returns>
        Task<List<AuthorizeEntity>> GetAuthorizeListByObjectId(string objectId);

        /// <summary>
        /// 获取数据条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        Task<List<IConditionalModel>> GetConditionAsync<T>(string moduleId) where T : new();

        /// <summary>
        /// 获取数据条件(在线开发专用)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        List<IConditionalModel> GetCondition<T>(string moduleId) where T : new();
    }
}
