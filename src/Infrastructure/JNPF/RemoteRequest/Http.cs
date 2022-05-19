using JNPF.Dependency;
using System;

namespace JNPF.RemoteRequest
{
    /// <summary>
    /// 远程请求静态类
    /// </summary>
    [SuppressSniffer]
    public static class Http
    {
        /// <summary>
        /// 获取远程请求代理
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns>IHttpDispatchProxy</returns>
        public static THttpDispatchProxy GetHttpProxy<THttpDispatchProxy>(IServiceProvider serviceProvider = default)
            where THttpDispatchProxy : class, IHttpDispatchProxy
        {
            return App.GetService<THttpDispatchProxy>(serviceProvider);
        }
    }
}