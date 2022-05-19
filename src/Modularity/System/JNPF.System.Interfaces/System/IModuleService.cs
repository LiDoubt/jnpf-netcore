using JNPF.System.Entitys.System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces.System
{
    /// <summary>
    /// 菜单管理
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IModuleService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        Task<List<ModuleEntity>> GetList();

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        Task<ModuleEntity> GetInfo(string id);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        Task<int> Delete(ModuleEntity entity);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Create(ModuleEntity entity);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Update(ModuleEntity entity);

        /// <summary>
        /// 获取用户菜单树
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<dynamic> GetUserTreeModuleList(bool isAdmin, string userId);

        /// <summary>
        /// 获取用户菜单列表
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<dynamic> GetUserModueList(bool isAdmin, string userId);
    }
}
