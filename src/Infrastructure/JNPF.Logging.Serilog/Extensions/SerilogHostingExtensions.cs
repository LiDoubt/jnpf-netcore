﻿using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Text;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Serilog 日志拓展
    /// </summary>
    public static class SerilogHostingExtensions
    {
        /// <summary>
        /// 添加默认日志拓展
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="configAction"></param>
        /// <returns>IWebHostBuilder</returns>
        public static IWebHostBuilder UseSerilogDefault(this IWebHostBuilder hostBuilder, Action<LoggerConfiguration> configAction = default)
        {
            hostBuilder.UseSerilog((context, configuration) =>
            {
                // 加载配置文件
                var config = configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext();

                if (configAction != null) configAction.Invoke(config);
                else
                {
                    var hasWriteTo = context.Configuration["Serilog:WriteTo:0:Name"];
                    if (hasWriteTo == null)
                    {
                        config.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                          .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "logs", "application.log"), LogEventLevel.Information, rollingInterval: RollingInterval.Day, retainedFileCountLimit: null, encoding: Encoding.UTF8);
                    }
                }
            });

            return hostBuilder;
        }

        /// <summary>
        /// 添加默认日志拓展
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IHostBuilder UseSerilogDefault(this IHostBuilder builder, Action<LoggerConfiguration> configAction = default)
        {
            builder.UseSerilog((context, configuration) =>
            {
                // 加载配置文件
                var config = configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext();

                if (configAction != null) configAction.Invoke(config);
                else
                {
                    var hasWriteTo = context.Configuration["Serilog:WriteTo:0:Name"];
                    if (hasWriteTo == null)
                    {
                        config.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                          .WriteTo.File(Path.Combine("logs", "application.log"), LogEventLevel.Information, rollingInterval: RollingInterval.Day, retainedFileCountLimit: null, encoding: Encoding.UTF8);
                    }
                }
            });

            return builder;
        }
    }
}
