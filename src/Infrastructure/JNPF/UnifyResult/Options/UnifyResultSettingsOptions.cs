﻿using JNPF.ConfigurableOptions;
using Microsoft.Extensions.Configuration;

namespace JNPF.UnifyResult
{
    /// <summary>
    /// 规范化配置选项
    /// </summary>
    public sealed class UnifyResultSettingsOptions : IConfigurableOptions<UnifyResultSettingsOptions>
    {
        /// <summary>
        /// 设置返回 200 状态码列表
        /// <para>默认：401，403，如果设置为 null，则标识所有状态码都返回 200 </para>
        /// </summary>
        public int[] Return200StatusCodes { get; set; }

        /// <summary>
        /// 适配（篡改）Http 状态码（只支持短路状态码，比如 401，403，500 等）
        /// </summary>
        public int[][] AdaptStatusCodes { get; set; }

        /// <summary>
        /// 是否支持 MVC 控制台规范化处理
        /// </summary>
        public bool? SupportMvcController { get; set; }

        /// <summary>
        /// 选项后期配置
        /// </summary>
        /// <param name="options"></param>
        /// <param name="configuration"></param>
        public void PostConfigure(UnifyResultSettingsOptions options, IConfiguration configuration)
        {
            options.Return200StatusCodes ??= new[] { 401, 403 };
            options.SupportMvcController ??= false;
        }
    }
}
