﻿using JNPF.Dependency;
using System;

namespace JNPF.RemoteRequest
{
    /// <summary>
    /// 配置Body参数
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Parameter)]
    public class BodyAttribute : ParameterBaseAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BodyAttribute()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="contentType"></param>
        public BodyAttribute(string contentType)
        {
            ContentType = contentType;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="encoding"></param>
        public BodyAttribute(string contentType, string encoding)
        {
            ContentType = contentType;
            Encoding = encoding;
        }

        /// <summary>
        /// 内容类型
        /// </summary>
        public string ContentType { get; set; } = "application/json";

        /// <summary>
        /// 内容编码
        /// </summary>
        public string Encoding { get; set; } = "UTF-8";
    }
}