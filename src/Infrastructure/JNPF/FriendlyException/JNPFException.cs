﻿using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.Extensions;
using JNPF.Localization;
using JNPF.Templates.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;


namespace JNPF.FriendlyException
{
    /// <summary>
    /// 抛异常静态类
    /// </summary>
    [SuppressSniffer]
    public static class JNPFException
    {
        /// <summary>
        /// 方法错误异常特性
        /// </summary>
        private static readonly ConcurrentDictionary<MethodBase, MethodIfException> ErrorMethods;

        /// <summary>
        /// 错误代码类型
        /// </summary>
        private static readonly IEnumerable<Type> ErrorCodeTypes;

        /// <summary>
        /// 错误消息字典
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> ErrorCodeMessages;

        /// <summary>
        /// 友好异常设置
        /// </summary>
        private static readonly FriendlyExceptionSettingsOptions _friendlyExceptionSettings;

        /// <summary>
        /// 构造函数
        /// </summary>
        static JNPFException()
        {
            ErrorMethods = new ConcurrentDictionary<MethodBase, MethodIfException>();
            _friendlyExceptionSettings = App.GetOptions<FriendlyExceptionSettingsOptions>();
            ErrorCodeTypes = GetErrorCodeTypes();
            ErrorCodeMessages = GetErrorCodeMessages();
        }

        /// <summary>
        /// 抛出业务异常日志
        /// </summary>
        /// <param name="errorMessage">异常消息</param>
        /// <param name="args">String.Format 参数</param>
        /// <returns>异常实例</returns>
        public static AppFriendlyException Bah(string errorMessage, params object[] args)
        {
            var friendlyException = Oh(errorMessage, typeof(ValidationException), args).StatusCode(StatusCodes.Status400BadRequest);
            friendlyException.ValidationException = true;
            return friendlyException;
        }

        /// <summary>
        /// 抛出业务异常日志
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="args">String.Format 参数</param>
        /// <returns>异常实例</returns>
        public static AppFriendlyException Bah(object errorCode, params object[] args)
        {
            var friendlyException = Oh(errorCode, typeof(ValidationException), args).StatusCode(StatusCodes.Status400BadRequest);
            friendlyException.ValidationException = true;
            return friendlyException;
        }

        /// <summary>
        /// 抛出字符串异常
        /// </summary>
        /// <param name="errorMessage">异常消息</param>
        /// <param name="args">String.Format 参数</param>
        /// <returns>异常实例</returns>
        public static AppFriendlyException Oh(string errorMessage, params object[] args)
        {
            return new AppFriendlyException(MontageErrorMessage(errorMessage, default, args), default);
        }

        /// <summary>
        /// 抛出字符串异常
        /// </summary>
        /// <param name="errorMessage">异常消息</param>
        /// <param name="exceptionType">具体异常类型</param>
        /// <param name="args">String.Format 参数</param>
        /// <returns>异常实例</returns>
        public static AppFriendlyException Oh(string errorMessage, Type exceptionType, params object[] args)
        {
            var exceptionMessage = MontageErrorMessage(errorMessage, default, args);
            return new AppFriendlyException(exceptionMessage, default,
                Activator.CreateInstance(exceptionType, new object[] { exceptionMessage }) as Exception);
        }

        /// <summary>
        /// 抛出错误码异常
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="args">String.Format 参数</param>
        /// <returns>异常实例</returns>
        public static AppFriendlyException Oh(object errorCode, params object[] args)
        {
            return new AppFriendlyException(GetErrorCodeMessage(errorCode, args), errorCode);
        }

        /// <summary>
        /// 抛出错误码异常
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="exceptionType">具体异常类型</param>
        /// <param name="args">String.Format 参数</param>
        /// <returns>异常实例</returns>
        public static AppFriendlyException Oh(object errorCode, Type exceptionType, params object[] args)
        {
            var exceptionMessage = GetErrorCodeMessage(errorCode, args);
            return new AppFriendlyException(exceptionMessage, errorCode,
                Activator.CreateInstance(exceptionType, new object[] { exceptionMessage }) as Exception);
        }

        /// <summary>
        /// 重试有异常的方法，还可以指定特定异常
        /// </summary>
        /// <param name="action"></param>
        /// <param name="numRetries">重试次数</param>
        /// <param name="retryTimeout">重试间隔时间</param>
        /// <param name="exceptionTypes">异常类型,可多个</param>
        public static void Retry(Action action, int numRetries, int retryTimeout, params Type[] exceptionTypes)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            _ = Retry(() =>
            {
                action();
                return 0;
            }, numRetries, retryTimeout, exceptionTypes);
        }

        /// <summary>
        /// 重试有异常的方法，还可以指定特定异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="numRetries">重试次数</param>
        /// <param name="retryTimeout">重试间隔时间</param>
        /// <param name="exceptionTypes">异常类型,可多个</param>
        public static T Retry<T>(Func<T> action, int numRetries, int retryTimeout, params Type[] exceptionTypes)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            // 不断重试
            while (true)
            {
                try
                {
                    return action();
                }
                catch (Exception ex)
                {
                    // 如果可重试次数小于或等于0，则终止重试
                    if (--numRetries <= 0) throw;

                    // 如果填写了 exceptionTypes 且异常类型不在 exceptionTypes 之内，则终止重试
                    if (exceptionTypes != null && exceptionTypes.Length > 0 && !exceptionTypes.Any(u => u.IsAssignableFrom(ex.GetType()))) throw;

                    // 如果可重试异常数大于 0，则间隔指定时间后继续执行
                    if (retryTimeout > 0) Thread.Sleep(retryTimeout);
                }
            }
        }

        /// <summary>
        /// 获取错误码消息
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string GetErrorCodeMessage(object errorCode, params object[] args)
        {
            errorCode = HandleEnumErrorCode(errorCode);

            // 获取出错的方法
            var methodIfException = GetEndPointExceptionMethod();

            // 获取异常特性
            var ifExceptionAttribute = methodIfException.IfExceptionAttributes.FirstOrDefault(u => HandleEnumErrorCode(u.ErrorCode).ToString().Equals(errorCode.ToString()));

            // 获取错误码消息
            var errorCodeMessage = ifExceptionAttribute == null || string.IsNullOrWhiteSpace(ifExceptionAttribute.ErrorMessage)
                ? (ErrorCodeMessages.GetValueOrDefault(errorCode.ToString()) ?? _friendlyExceptionSettings.DefaultErrorMessage)
                : ifExceptionAttribute.ErrorMessage;

            // 字符串格式化
            return MontageErrorMessage(errorCodeMessage, errorCode.ToString()
                , args != null && args.Length > 0 ? args : ifExceptionAttribute?.Args);
        }

        /// <summary>
        /// 处理枚举类型错误码
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <returns></returns>
        private static object HandleEnumErrorCode(object errorCode)
        {
            // 获取类型
            var errorType = errorCode.GetType();

            // 判断是否是内置枚举类型，如果是解析特性
            if (ErrorCodeTypes.Any(u => u == errorType))
            {
                var fieldinfo = errorType.GetField(Enum.GetName(errorType, errorCode));
                if (fieldinfo.IsDefined(typeof(ErrorCodeItemMetadataAttribute), true))
                {
                    errorCode = GetErrorCodeItemMessage(fieldinfo).Key;
                }
            }

            return errorCode;
        }

        /// <summary>
        /// 获取错误代码类型
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Type> GetErrorCodeTypes()
        {
            // 查找所有公开的枚举贴有 [ErrorCodeType] 特性的类型
            var errorCodeTypes = App.EffectiveTypes
                .Where(u => u.IsDefined(typeof(ErrorCodeTypeAttribute), true) && u.IsEnum);

            // 获取错误代码提供器中定义的类型
            var errorCodeTypeProvider = App.GetService<IErrorCodeTypeProvider>(App.RootServices);
            if (errorCodeTypeProvider is { Definitions: not null }) errorCodeTypes = errorCodeTypes.Concat(errorCodeTypeProvider.Definitions);

            return errorCodeTypes.Distinct();
        }

        /// <summary>
        /// 获取所有错误消息
        /// </summary>
        /// <returns></returns>
        private static ConcurrentDictionary<string, string> GetErrorCodeMessages()
        {
            var defaultErrorCodeMessages = new ConcurrentDictionary<string, string>();

            // 查找所有 [ErrorCodeType] 类型中的 [ErrorCodeMetadata] 元数据定义
            var errorCodeMessages = ErrorCodeTypes.SelectMany(u => u.GetFields().Where(u => u.IsDefined(typeof(ErrorCodeItemMetadataAttribute))))
                .Select(u => GetErrorCodeItemMessage(u))
               .ToDictionary(u => u.Key.ToString(), u => u.Value);

            defaultErrorCodeMessages.AddOrUpdate(errorCodeMessages);

            // 加载配置文件状态码
            var errorCodeMessageSettings = App.GetConfig<ErrorCodeMessageSettingsOptions>("ErrorCodeMessageSettings", true);
            if (errorCodeMessageSettings is { Definitions: not null })
            {
                // 获取所有参数大于1的配置
                var fitErrorCodes = errorCodeMessageSettings.Definitions
                    .Where(u => u.Length > 1)
                    .ToDictionary(u => u[0].ToString(), u => FixErrorCodeSettingMessage(u));

                defaultErrorCodeMessages.AddOrUpdate(fitErrorCodes);
            }

            return defaultErrorCodeMessages;
        }

        /// <summary>
        /// 处理异常配置数据
        /// </summary>
        /// <param name="errorCodes">错误消息配置对象</param>
        /// <remarks>
        /// 方式：数组第一个元素为错误码，第二个参数为错误消息，剩下的参数为错误码格式化字符串
        /// </remarks>
        /// <returns></returns>
        private static string FixErrorCodeSettingMessage(object[] errorCodes)
        {
            var args = errorCodes.Skip(2).ToArray();
            var errorMessage = errorCodes[1].ToString();
            return errorMessage.Format(args);
        }

        /// <summary>
        /// 获取堆栈中顶部抛异常方法
        /// </summary>
        /// <returns></returns>
        private static MethodIfException GetEndPointExceptionMethod()
        {
            // 获取调用堆栈信息
            var stackTrace = EnhancedStackTrace.Current();

            // 获取出错的堆栈信息
            var stackFrame = stackTrace.FirstOrDefault(u => typeof(ControllerBase).IsAssignableFrom(u.MethodInfo.DeclaringType) || typeof(IDynamicApiController).IsAssignableFrom(u.MethodInfo.DeclaringType) || u.StackFrame.GetMethod().IsFinal);

            // 获取出错的方法
            var errorMethod = stackFrame.MethodInfo.MethodBase;

            // 判断是否已经缓存过该方法，避免重复解析
            var isCached = ErrorMethods.TryGetValue(errorMethod, out var methodIfException);
            if (isCached) return methodIfException;

            // 获取堆栈中所有的 [IfException] 特性
            var ifExceptionAttributes = stackTrace
                .Where(u => u.MethodInfo.MethodBase != null && u.MethodInfo.MethodBase.IsDefined(typeof(IfExceptionAttribute), true))
                .SelectMany(u => u.MethodInfo.MethodBase.GetCustomAttributes<IfExceptionAttribute>(true))
                .Where(u => u.ErrorCode != null);

            // 组装方法异常对象
            methodIfException = new MethodIfException
            {
                ErrorMethod = errorMethod,
                IfExceptionAttributes = ifExceptionAttributes
            };

            // 存入缓存
            ErrorMethods.TryAdd(errorMethod, methodIfException);

            return methodIfException;
        }

        /// <summary>
        /// 获取错误代码消息实体
        /// </summary>
        /// <param name="fieldInfo">字段对象</param>
        /// <returns>(object key, object value)</returns>
        private static (object Key, string Value) GetErrorCodeItemMessage(FieldInfo fieldInfo)
        {
            var errorCodeItemMetadata = fieldInfo.GetCustomAttribute<ErrorCodeItemMetadataAttribute>();
            return (errorCodeItemMetadata.ErrorCode ?? fieldInfo.Name, errorCodeItemMetadata.ErrorMessage.Format(errorCodeItemMetadata.Args));
        }

        /// <summary>
        /// 获取错误码字符串
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="errorCode"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string MontageErrorMessage(string errorMessage, string errorCode, params object[] args)
        {
            // 支持读取配置渲染
            var realErrorMessage = errorMessage.Render();

            // 多语言处理
            realErrorMessage = L.Text == null ? realErrorMessage : L.Text[realErrorMessage];

            // 判断是否隐藏错误码
            var msg = (_friendlyExceptionSettings.HideErrorCode == true || string.IsNullOrWhiteSpace(errorCode)
                ? string.Empty
                : $"[{errorCode}] ") + realErrorMessage;

            return msg.Format(args);
        }
    }
}