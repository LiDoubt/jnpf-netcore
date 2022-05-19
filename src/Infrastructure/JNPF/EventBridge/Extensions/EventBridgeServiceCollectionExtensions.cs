﻿using JNPF;
using JNPF.Dependency;
using JNPF.EventBridge;
using JNPF.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 事件总线服务拓展
    /// </summary>
    [SuppressSniffer]
    public static class EventBridgeServiceCollectionExtensions
    {
        /// <summary>
        /// 添加事件总线服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEventBridge(this IServiceCollection services)
        {
            return services.AddEventBridge<MemoryEventStoreProvider>();
        }

        /// <summary>
        /// 添加事件总线服务
        /// </summary>
        /// <typeparam name="TEventStoreProvider"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEventBridge<TEventStoreProvider>(this IServiceCollection services)
            where TEventStoreProvider : class, IEventStoreProvider
        {
            // 查找所有事件处理程序
            var eventHandlerTypes = App.EffectiveTypes.Where(u => u.IsClass && !u.IsInterface && !u.IsAbstract && typeof(IEventHandler).IsAssignableFrom(u));
            if (!eventHandlerTypes.Any()) return services;

            // 注册事件存储提供器
            services.AddTransient<IEventStoreProvider, TEventStoreProvider>();

            using var serviceProvider = InternalApp.InternalServices.BuildServiceProvider();

            // 批量注册
            foreach (var type in eventHandlerTypes)
            {
                // 注册事件处理程序
                services.AddTransient(type);
                services.AddTransient(typeof(IEventHandler), type);

                // 触发事件存储提供器 [注册事件] 接口方法
                var eventStoreProvider = serviceProvider.GetService<IEventStoreProvider>();
                eventStoreProvider.RegisterEventHandlerAsync(new EventHandlerMetadata
                {
                    AssemblyName = Reflect.GetAssemblyName(type),
                    Category = Event.GetEventHandlerCategory(type),
                    TypeFullName = type.FullName,
                    CreatedTime = DateTimeOffset.UtcNow
                }).GetAwaiter().GetResult();
            }

            // 注册事件处理程序委托
            services.TryAddTransient(provider =>
            {
                IEventHandler eventHandlerResolve(EventMessageMetadata eventIdMetadata)
                {
                    // 加载类型程序集
                    var eventHandlerType = Reflect.GetType(eventIdMetadata.AssemblyName, eventIdMetadata.TypeFullName);
                    return provider.GetService(eventHandlerType) as IEventHandler;
                }
                return (Func<EventMessageMetadata, IEventHandler>)eventHandlerResolve;
            });

            return services;
        }
    }
}