using JNPF.System.Entitys.Dto.System.DataSync;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces.System
{
    /// <summary>
    /// 数据同步
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IDataSyncService
    {
        /// <summary>
        /// 执行同步
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        Task Execute(DbSyncActionsExecuteInput input);
    }
}
