using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.VisualDev.Interfaces
{
    /// <summary>
    /// 业务契约：门户设计
    /// </summary>
    public interface IPortalService
    {
        /// <summary>
        /// 获取默认门户
        /// </summary>
        /// <returns></returns>
        Task<string> GetDefault();
    }
}
