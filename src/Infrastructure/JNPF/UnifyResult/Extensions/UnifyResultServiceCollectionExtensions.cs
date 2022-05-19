﻿using JNPF.Dependency;
using JNPF.UnifyResult;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 规范化结果服务拓展
    /// </summary>
    [SuppressSniffer]
    public static class UnifyResultServiceCollectionExtensions
    {
        /// <summary>
        /// 添加规范化结果服务
        /// </summary>
        /// <param name="mvcBuilder"></param>
        /// <returns></returns>
        public static IMvcBuilder AddUnifyResult(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.Services.AddUnifyResult<RESTfulResultProvider>();

            return mvcBuilder;
        }

        /// <summary>
        /// 添加规范化结果服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUnifyResult(this IServiceCollection services)
        {
            services.AddUnifyResult<RESTfulResultProvider>();

            return services;
        }

        /// <summary>
        /// 添加规范化结果服务
        /// </summary>
        /// <typeparam name="TUnifyResultProvider"></typeparam>
        /// <param name="mvcBuilder"></param>
        /// <returns></returns>
        public static IMvcBuilder AddUnifyResult<TUnifyResultProvider>(this IMvcBuilder mvcBuilder)
            where TUnifyResultProvider : class, IUnifyResultProvider
        {
            mvcBuilder.Services.AddUnifyResult<TUnifyResultProvider>();

            return mvcBuilder;
        }

        /// <summary>
        /// 添加规范化结果服务
        /// </summary>
        /// <typeparam name="TUnifyResultProvider"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUnifyResult<TUnifyResultProvider>(this IServiceCollection services)
            where TUnifyResultProvider : class, IUnifyResultProvider
        {
            // 添加配置
            services.AddConfigurableOptions<UnifyResultSettingsOptions>();

            // 是否启用规范化结果
            UnifyContext.EnabledUnifyHandler = true;

            // 获取规范化提供器模型
            UnifyContext.RESTfulResultType = typeof(TUnifyResultProvider).GetCustomAttribute<UnifyModelAttribute>().ModelType;

            // 添加规范化提供器
            services.AddSingleton<IUnifyResultProvider, TUnifyResultProvider>();

            // 添加成功规范化结果筛选器
            services.AddMvcFilter<SucceededUnifyResultFilter>();

            return services;
        }
    }
}