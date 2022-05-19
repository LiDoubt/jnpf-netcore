﻿using JNPF.Dependency;
using Microsoft.OpenApi.Models;

namespace JNPF.SpecificationDocument
{
    /// <summary>
    /// 安全定义需求子项
    /// </summary>
    [SuppressSniffer]
    public sealed class SpecificationOpenApiSecurityRequirementItem
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SpecificationOpenApiSecurityRequirementItem()
        {
            Accesses = System.Array.Empty<string>();
        }

        /// <summary>
        /// 安全Schema
        /// </summary>
        public OpenApiSecurityScheme Scheme { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public string[] Accesses { get; set; }
    }
}