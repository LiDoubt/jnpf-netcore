﻿using JNPF.Dependency;
using System;

namespace JNPF.DataValidation
{
    /// <summary>
    /// 验证消息特性
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Field)]
    public sealed class ValidationMessageAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="errorMessage"></param>
        public ValidationMessageAttribute(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}