using JNPF.WorkFlow.Entitys;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.WorkFlow.Interfaces.FLowDelegate
{
    /// <summary>
    /// 流程委托
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IFlowDelegateService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        Task<List<FlowDelegateEntity>> GetList(string userId);

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<FlowDelegateEntity> GetInfo(string id);

        /// <summary>
        /// 所有委托人
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<string>> GetDelegateUserId(string userId);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Delete(FlowDelegateEntity entity);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Create(FlowDelegateEntity entity);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Update(FlowDelegateEntity entity);

    }
}
