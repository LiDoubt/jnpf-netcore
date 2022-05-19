﻿using JNPF.Dependency;
using System;

namespace JNPF.TaskScheduler
{
    /// <summary>
    /// 解析 Cron 表达式出错异常类
    /// </summary>
    [SuppressSniffer, Serializable]
    public class CronFormatException : FormatException
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        public CronFormatException(string message) : base(message)
        {
        }

        /// <summary>
        /// 内部构造函数
        /// </summary>
        /// <param name="field"></param>
        /// <param name="message"></param>
        internal CronFormatException(CronField field, string message) : this($"{field}: {message}")
        {
        }
    }
}