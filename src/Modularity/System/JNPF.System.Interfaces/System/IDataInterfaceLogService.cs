using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces.System
{
    /// <summary>
    /// 数据接口日志
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IDataInterfaceLogService
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="id">接口id</param>
        /// <param name="sw">请求时间</param>
        /// <returns></returns>
        Task CreateLog(string id, Stopwatch sw);
    }
}
