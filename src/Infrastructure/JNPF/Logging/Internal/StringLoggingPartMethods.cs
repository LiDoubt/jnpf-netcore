﻿using Microsoft.Extensions.Logging;

namespace JNPF.Logging
{
    /// <summary>
    /// 构建字符串日志部分类
    /// </summary>
    public sealed partial class StringLoggingPart
    {
        /// <summary>
        /// Information
        /// </summary>
        public void LogInformation()
        {
            SetLevel(LogLevel.Information).Log();
        }

        /// <summary>
        /// Warning
        /// </summary>
        public void LogWarning()
        {
            SetLevel(LogLevel.Warning).Log();
        }

        /// <summary>
        /// Error
        /// </summary>
        public void LogError()
        {
            SetLevel(LogLevel.Error).Log();
        }

        /// <summary>
        /// Debug
        /// </summary>
        public void LogDebug()
        {
            SetLevel(LogLevel.Debug).Log();
        }

        /// <summary>
        /// Trace
        /// </summary>
        public void LogTrace()
        {
            SetLevel(LogLevel.Trace).Log();
        }

        /// <summary>
        /// Critical
        /// </summary>
        public void LogCritical()
        {
            SetLevel(LogLevel.Critical).Log();
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <returns></returns>
        public void Log()
        {
            if (Message == null) return;

            var logger = !string.IsNullOrWhiteSpace(CategoryName)
               ? App.GetService<ILoggerFactory>(LoggerScoped ?? App.RootServices)?.CreateLogger(CategoryName)
               : App.GetService(typeof(ILogger<>).MakeGenericType(CategoryType), LoggerScoped ?? App.RootServices) as ILogger;

            // 如果没有异常且事件 Id 为空
            if (Exception == null && EventId == null)
            {
                logger.Log(Level, Message, Args);
            }
            // 如果存在异常且事件 Id 为空
            else if (Exception != null && EventId == null)
            {
                logger.Log(Level, Exception, Message, Args);
            }
            // 如果异常为空且事件 Id 不为空
            else if (Exception == null && EventId != null)
            {
                logger.Log(Level, EventId.Value, Message, Args);
            }
            // 如果存在异常且事件 Id 不为空
            else if (Exception != null && EventId != null)
            {
                logger.Log(Level, EventId.Value, Exception, Message, Args);
            }
            else { }
        }
    }
}