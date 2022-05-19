using JNPF.Dependency;
using JNPF.RemoteRequest;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 远程请求服务拓展类
    /// </summary>
    [SuppressSniffer]
    public static class RemoteRequestServiceCollectionExtensions
    {
        /// <summary>
        /// 注册远程请求
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <param name="inludeDefaultHttpClient">是否包含默认客户端</param>
        /// <returns></returns>
        public static IServiceCollection AddRemoteRequest(this IServiceCollection services, Action<IServiceCollection> configure = null, bool inludeDefaultHttpClient = true)
        {
            // 注册远程请求代理接口
            services.AddScopedDispatchProxyForInterface<HttpDispatchProxy, IHttpDispatchProxy>();

            // 注册默认请求客户端
            if (inludeDefaultHttpClient) services.AddHttpClient();

            // 注册其他客户端
            configure?.Invoke(services);

            return services;
        }
    }
}