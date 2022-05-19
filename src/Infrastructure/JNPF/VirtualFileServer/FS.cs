using JNPF.Dependency;
using Microsoft.Extensions.FileProviders;
using System;
using System.Reflection;

namespace JNPF.VirtualFileServer
{
    /// <summary>
    /// 虚拟文件服务静态类
    /// </summary>
    [SuppressSniffer]
    public static class FS
    {
        /// <summary>
        /// 获取物理文件提供器
        /// </summary>
        /// <param name="root"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static IFileProvider GetPhysicalFileProvider(string root, IServiceProvider serviceProvider = default)
        {
            return GetFileProvider(FileProviderTypes.Physical, root, serviceProvider);
        }

        /// <summary>
        /// 获取嵌入资源文件提供器
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static IFileProvider GetEmbeddedFileProvider(Assembly assembly, IServiceProvider serviceProvider = default)
        {
            return GetFileProvider(FileProviderTypes.Embedded, assembly, serviceProvider);
        }

        /// <summary>
        /// 文件提供器
        /// </summary>
        /// <param name="fileProviderTypes"></param>
        /// <param name="args"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static IFileProvider GetFileProvider(FileProviderTypes fileProviderTypes, object args, IServiceProvider serviceProvider = default)
        {
            var fileProviderResolve = App.GetService<Func<FileProviderTypes, object, IFileProvider>>(serviceProvider ?? App.RootServices);
            return fileProviderResolve(fileProviderTypes, args);
        }
    }
}