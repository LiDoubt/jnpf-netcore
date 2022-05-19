using JNPF.System.Entitys.Dto.System.SysLog;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces.System
{
    /// <summary>
    /// 系统日志
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface ISysLogService
    {
        /// <summary>
        /// 获取系统日志列表（分页）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <param name="Type">分类</param>
        /// <returns></returns>
        Task<dynamic> GetList(LogListQuery input, int Type);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        Task Delete(LogDelInput input);
    }
}
