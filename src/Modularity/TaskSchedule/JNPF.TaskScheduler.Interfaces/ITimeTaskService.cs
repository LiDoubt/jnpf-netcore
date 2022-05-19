using JNPF.TaskScheduler.Entitys.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.TaskScheduler.Interfaces.TaskScheduler
{
    /// <summary>
    /// 定时任务
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface ITimeTaskService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        Task<List<TimeTaskEntity>> GetList();

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tenantDic">租户信息</param>
        /// <returns></returns>
        Task<TimeTaskEntity> GetInfo(string id, Dictionary<string, string> tenantDic = null);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Delete(TimeTaskEntity entity);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<TimeTaskEntity> Create(TimeTaskEntity entity);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Update(TimeTaskEntity entity, Dictionary<string, string> tenantDic = null);

        /// <summary>
        /// 执行记录
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> CreateTaskLog(TimeTaskLogEntity entity, Dictionary<string, string> tenantDic = null);

        /// <summary>
        /// 启动自启动任务
        /// </summary>
        void StartTimerJob();
    }
}
