using JNPF.DataValidation;
using JNPF.Dependency;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 友好异常服务拓展类
    /// </summary>
    [SuppressSniffer]
    public static class DataValidationServiceCollectionExtensions
    {
        /// <summary>
        /// 添加全局数据验证
        /// </summary>
        /// <typeparam name="TValidationMessageTypeProvider">验证类型消息提供器</typeparam>
        /// <param name="mvcBuilder"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IMvcBuilder AddDataValidation<TValidationMessageTypeProvider>(this IMvcBuilder mvcBuilder, Action<DataValidationServiceOptions> configure = null)
            where TValidationMessageTypeProvider : class, IValidationMessageTypeProvider
        {
            // 添加全局数据验证
            mvcBuilder.Services.AddDataValidation<TValidationMessageTypeProvider>(configure);

            return mvcBuilder;
        }

        /// <summary>
        /// 添加全局数据验证
        /// </summary>
        /// <typeparam name="TValidationMessageTypeProvider">验证类型消息提供器</typeparam>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddDataValidation<TValidationMessageTypeProvider>(this IServiceCollection services, Action<DataValidationServiceOptions> configure = null)
            where TValidationMessageTypeProvider : class, IValidationMessageTypeProvider
        {
            // 添加全局数据验证
            services.AddDataValidation(configure);

            // 单例注册验证消息提供器
            services.AddSingleton<IValidationMessageTypeProvider, TValidationMessageTypeProvider>();

            return services;
        }

        /// <summary>
        /// 添加全局数据验证
        /// </summary>
        /// <param name="mvcBuilder"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IMvcBuilder AddDataValidation(this IMvcBuilder mvcBuilder, Action<DataValidationServiceOptions> configure = null)
        {
            mvcBuilder.Services.AddDataValidation(configure);

            return mvcBuilder;
        }

        /// <summary>
        /// 添加全局数据验证
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddDataValidation(this IServiceCollection services, Action<DataValidationServiceOptions> configure = null)
        {
            // 添加验证配置文件支持
            services.AddConfigurableOptions<ValidationTypeMessageSettingsOptions>();

            // 载入服务配置选项
            var configureOptions = new DataValidationServiceOptions();
            configure?.Invoke(configureOptions);

            // 判断是否启用全局
            if (configureOptions.EnableGlobalDataValidation)
            {
                // 添加自定义验证
                services.Configure<ApiBehaviorOptions>(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });

                // 添加全局数据验证
                services.AddMvcFilter<DataValidationFilter>(options =>
                {
                    // 关闭空引用对象验证
                    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = configureOptions.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes;
                });
            }

            return services;
        }
    }
}