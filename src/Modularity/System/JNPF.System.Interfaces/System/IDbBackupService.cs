using JNPF.Common.Filter;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces.System
{
    /// <summary>
    /// 数据备份
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IDbBackupService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        Task<dynamic> GetList(PageInputBase input);
        /// <summary>
        /// 创建备份
        /// </summary>
        /// <returns></returns>
        Task Create();
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        Task Delete(string id);
    }
}
