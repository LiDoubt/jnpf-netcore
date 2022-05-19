using Microsoft.AspNetCore.SignalR;
using System;

namespace JNPF.InstantMessaging
{
    /// <summary>
    /// 即时通信静态类
    /// </summary>
    public static class IM
    {
        /// <summary>
        /// 获取集线器实例
        /// </summary>
        /// <typeparam name="THub"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static IHubContext<THub> GetHub<THub>(IServiceProvider serviceProvider = default)
            where THub : Hub
        {
            return App.GetService<IHubContext<THub>>(serviceProvider);
        }

        /// <summary>
        /// 获取强类型集线器实例
        /// </summary>
        /// <typeparam name="THub"></typeparam>
        /// <typeparam name="TStronglyTyped"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static IHubContext<THub, TStronglyTyped> GetHub<THub, TStronglyTyped>(IServiceProvider serviceProvider = default)
            where THub : Hub<TStronglyTyped>
            where TStronglyTyped : class
        {
            return App.GetService<IHubContext<THub, TStronglyTyped>>(serviceProvider);
        }
    }
}