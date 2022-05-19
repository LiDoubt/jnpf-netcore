﻿using JNPF.Dependency;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JNPF.Localization
{
    /// <summary>
    /// 全局多语言静态类
    /// </summary>
    [SuppressSniffer]
    public static class L
    {
        /// <summary>
        /// String 多语言
        /// </summary>
        public static IStringLocalizer @Text => App.GetService<IStringLocalizerFactory>(App.RootServices)?.Create();

        /// <summary>
        /// Html 多语言
        /// </summary>
        public static IHtmlLocalizer @Html => App.GetService<IHtmlLocalizerFactory>(App.RootServices)?.Create();

        /// <summary>
        /// 设置多语言区域
        /// </summary>
        /// <param name="culture"></param>
        public static void SetCulture(string culture)
        {
            var httpContext = App.HttpContext;
            if (httpContext == null) return;

            httpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
        }

        /// <summary>
        /// 获取当前选择的语言
        /// </summary>
        /// <returns></returns>
        public static RequestCulture GetSelectCulture()
        {
            var httpContext = App.HttpContext;
            if (httpContext == null) return default;

            // 获取请求特性
            var requestCulture = httpContext.Features.Get<IRequestCultureFeature>();
            return requestCulture.RequestCulture;
        }

        /// <summary>
        /// 获取系统提供的语言列表
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetCultures()
        {
            var httpContext = App.HttpContext;
            if (httpContext == null) return new Dictionary<string, string>();

            // 获取请求本地特性选项
            var locOptions = httpContext.RequestServices.GetService<IOptions<RequestLocalizationOptions>>().Value;

            // 获取语言符号和名称
            var cultureItems = locOptions.SupportedUICultures
                .ToDictionary(u => u.Name, u => u.DisplayName);

            return cultureItems;
        }
    }
}