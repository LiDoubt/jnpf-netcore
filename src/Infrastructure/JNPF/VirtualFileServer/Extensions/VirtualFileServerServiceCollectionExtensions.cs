﻿using JNPF.Dependency;
using JNPF.VirtualFileServer;
using Microsoft.Extensions.FileProviders;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 虚拟文件服务服务拓展
    /// </summary>
    [SuppressSniffer]
    public static class VirtualFileServerServiceCollectionExtensions
    {
        /// <summary>
        /// 文件提供器系统服务拓展
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddVirtualFileServer(this IServiceCollection services)
        {
            // 解析文件提供器
            services.AddSingleton(provider =>
            {
                static IFileProvider fileProviderResolve(FileProviderTypes fileProviderTypes, object args)
                {
                    // 根据类型创建对应 提供器
                    IFileProvider fileProvider = fileProviderTypes switch
                    {
                        FileProviderTypes.Embedded => new EmbeddedFileProvider(args as Assembly),
                        FileProviderTypes.Physical => new PhysicalFileProvider(args as string),
                        _ => throw new NotSupportedException()
                    };

                    return fileProvider;
                }

                // 转换成委托
                return (Func<FileProviderTypes, object, IFileProvider>)fileProviderResolve;
            });

            return services;
        }
    }
}