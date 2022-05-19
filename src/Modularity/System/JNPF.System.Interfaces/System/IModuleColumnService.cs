using JNPF.System.Entitys.Dto.System.ModuleColumn;
using JNPF.System.Entitys.System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces.System
{
    /// <summary>
    /// 功能列表
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IModuleColumnService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="moduleId">功能id</param>
        /// <returns></returns>
        Task<List<ModuleColumnEntity>> GetList(string moduleId);

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        Task<List<ModuleColumnEntity>> GetList();

        /// <summary>
        /// 获取按钮信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        Task<ModuleColumnEntity> GetInfo(string id);

        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Create(ModuleColumnEntity entity);

        /// <summary>
        /// 修改按钮
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Update(ModuleColumnEntity entity);
        /// <summary>
        /// 删除按钮
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Delete(ModuleColumnEntity entity);

        /// <summary>
        /// 批量新建
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        Task BatchCreate(ModuleColumnActionsBatchInput input);

        /// <summary>
        /// 更新字段状态
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        Task ActionsState(string id);

        /// <summary>
        /// 获取用户功能列表
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<dynamic> GetUserModuleColumnList(bool isAdmin, string userId);
    }
}
