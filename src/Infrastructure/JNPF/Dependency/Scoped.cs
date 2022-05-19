﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace JNPF.Dependency
{
    /// <summary>
    /// 创建作用域静态类
    /// </summary>
    [SuppressSniffer]
    public static partial class Scoped
    {
        /// <summary>
        /// 创建一个作用域范围
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="scopeFactory"></param>
        public static void Create(Action<IServiceScopeFactory, IServiceScope> handler, IServiceScopeFactory scopeFactory = default)
        {
            Create(async (fac, scope) =>
            {
                handler(fac, scope);
                await Task.CompletedTask;
            }, scopeFactory).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 创建一个作用域范围
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="scopeFactory"></param>
        public static async Task Create(Func<IServiceScopeFactory, IServiceScope, Task> handler, IServiceScopeFactory scopeFactory = default)
        {
            // 禁止空调用
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            // 创建作用域
            var scoped = CreateScope(scopeFactory);

            try
            {
                // 执行方法
                await handler(scopeFactory, scoped);
            }
            finally
            {
                // 释放
                scoped.Dispose();
            }
        }

        /// <summary>
        /// 创建一个作用域
        /// </summary>
        /// <param name="scopeFactory"></param>
        /// <returns></returns>
        private static IServiceScope CreateScope(IServiceScopeFactory scopeFactory = default)
        {
            // 解析服务作用域工厂
            var scoped = (scopeFactory ?? App.RootServices.GetService<IServiceScopeFactory>()).CreateScope();
            return scoped;
        }
    }
}