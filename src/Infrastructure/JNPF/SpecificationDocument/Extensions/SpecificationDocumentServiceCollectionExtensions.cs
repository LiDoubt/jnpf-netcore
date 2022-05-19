using JNPF;
using JNPF.Dependency;
using JNPF.SpecificationDocument;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 规范化接口服务拓展类
    /// </summary>
    [SuppressSniffer]
    public static class SpecificationDocumentServiceCollectionExtensions
    {
        /// <summary>
        /// 添加规范化文档服务
        /// </summary>
        /// <param name="mvcBuilder">Mvc 构建器</param>
        /// <param name="configure">自定义配置</param>
        /// <returns>服务集合</returns>
        public static IMvcBuilder AddSpecificationDocuments(this IMvcBuilder mvcBuilder, Action<SpecificationDocumentServiceOptions> configure = default)
        {
            mvcBuilder.Services.AddSpecificationDocuments(configure);

            return mvcBuilder;
        }

        /// <summary>
        /// 添加规范化文档服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configure">自定义配置</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddSpecificationDocuments(this IServiceCollection services, Action<SpecificationDocumentServiceOptions> configure = default)
        {
            // 判断是否启用规范化文档
            if (App.Settings.InjectSpecificationDocument != true) return services;

            // 添加配置
            services.AddConfigurableOptions<SpecificationDocumentSettingsOptions>();

            // 载入服务配置选项
            var configureOptions = new SpecificationDocumentServiceOptions();
            configure?.Invoke(configureOptions);

            // 添加Swagger生成器服务
            services.AddSwaggerGen(options => SpecificationDocumentBuilder.BuildGen(options, configureOptions?.SwaggerGenConfigure));

            // 添加 MiniProfiler 服务
            AddMiniProfiler(services);

            return services;
        }

        /// <summary>
        /// 添加 MiniProfiler 配置
        /// </summary>
        /// <param name="services"></param>
        private static void AddMiniProfiler(IServiceCollection services)
        {
            // 注册MiniProfiler 组件
            if (App.Settings.InjectMiniProfiler != true) return;

            services.AddMiniProfiler(options =>
            {
                options.RouteBasePath = "/index-mini-profiler";
                options.EnableMvcFilterProfiling = false;
                options.EnableMvcViewProfiling = false;
            });
        }
    }
}