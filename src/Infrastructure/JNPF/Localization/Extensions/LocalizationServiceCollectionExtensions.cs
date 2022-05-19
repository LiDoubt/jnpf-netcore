﻿using JNPF;
using JNPF.Dependency;
using JNPF.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 多语言服务拓展类
    /// </summary>
    [SuppressSniffer]
    public static class LocalizationServiceCollectionExtensions
    {
        /// <summary>
        /// 配置多语言服务
        /// </summary>
        /// <param name="mvcBuilde"></param>
        /// <returns></returns>
        public static IMvcBuilder AddAppLocalization(this IMvcBuilder mvcBuilde)
        {
            var services = mvcBuilde.Services;

            // 添加多语言配置选项
            services.AddConfigurableOptions<LocalizationSettingsOptions>();

            // 获取多语言配置选项
            var localizationSettings = App.GetConfig<LocalizationSettingsOptions>("LocalizationSettings", true);

            // 如果没有配置多语言选项，则不注册服务
            if (localizationSettings.SupportedCultures == null || localizationSettings.SupportedCultures.Length == 0) return mvcBuilde;

            // 注册多语言服务
            services.AddLocalization(options =>
            {
                if (!string.IsNullOrWhiteSpace(localizationSettings.ResourcesPath))
                    options.ResourcesPath = localizationSettings.ResourcesPath;
            });

            // 配置视图多语言和验证多语言
            mvcBuilde.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                     .AddDataAnnotationsLocalization(options =>
                     {
                         options.DataAnnotationLocalizerProvider = (type, factory) =>
                             factory.Create(localizationSettings.LanguageFilePrefix, localizationSettings.AssemblyName);
                     });

            // 注册请求多语言配置选项
            services.Configure((Action<RequestLocalizationOptions>)(options =>
            {
                Penetrates.SetRequestLocalization(options, localizationSettings);
            }));

            // 处理多语言在 Razor 视图中文乱码问题
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            return mvcBuilde;
        }
    }
}