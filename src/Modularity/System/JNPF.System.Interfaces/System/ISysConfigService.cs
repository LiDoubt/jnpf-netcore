using JNPF.System.Entitys.Dto.System.SysConfig;
using JNPF.System.Entitys.System;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces.System
{
    /// <summary>
    /// 系统配置
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface ISysConfigService
    {
        /// <summary>
        /// 系统配置信息
        /// </summary>
        /// <param name="category">分类</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<SysConfigEntity> GetInfo(string category, string key);

        /// <summary>
        /// 获取系统配置
        /// </summary>
        /// <returns></returns>
        Task<SysConfigOutput> GetInfo();
    }
}
