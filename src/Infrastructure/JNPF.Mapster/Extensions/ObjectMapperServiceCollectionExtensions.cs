﻿using Mapster;
using MapsterMapper;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Mapster 映射拓展类
    /// </summary>
    public static class ObjectMapperServiceCollectionExtensions
    {
        /// <summary>
        /// 添加 Mapster 映射
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="assemblies">扫描的程序集</param>
        /// <returns></returns>
        public static IServiceCollection AddObjectMapper(this IServiceCollection services, params Assembly[] assemblies)
        {
            // 获取全局映射配置
            var config = TypeAdapterConfig.GlobalSettings;

            // 扫描所有继承  IRegister 接口的对象映射配置
            if (assemblies != null && assemblies.Length > 0) config.Scan(assemblies);

            // 配置默认全局映射（支持覆盖）
            config.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible).PreserveReference(true);
            // 配置默认全局映射（忽略大小写敏感）
            config.Default.NameMatchingStrategy(NameMatchingStrategy.IgnoreCase).PreserveReference(true);

            // 配置支持依赖注入
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            return services;
        }
    }
}
