using JNPF.Extend.Entitys;
using System.Threading.Tasks;

namespace JNPF.Extend.Interfaces
{
    /// <summary>
    /// 邮件收发
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// 门户未读邮件
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetUnreadList();

        /// <summary>
        /// 信息（配置）
        /// </summary>
        /// <returns></returns>
        Task<EmailConfigEntity> GetConfigInfo();
    }
}
