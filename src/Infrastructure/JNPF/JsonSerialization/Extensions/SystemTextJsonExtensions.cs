﻿using JNPF.Dependency;
using JNPF.JsonSerialization;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace System.Text.Json
{
    /// <summary>
    /// System.Text.Json 拓展
    /// </summary>
    [SuppressSniffer]
    public static class SystemTextJsonExtensions
    {
        /// <summary>
        /// 添加时间格式化
        /// </summary>
        /// <param name="converters"></param>
        /// <param name="formatString"></param>
        /// <param name="outputToLocalDateTime">自动转换 DateTimeOffset 为当地时间</param>
        public static void AddDateFormatString(this IList<JsonConverter> converters, string formatString, bool outputToLocalDateTime = false)
        {
            converters.Add(new DateTimeJsonConverter(formatString));
            converters.Add(new DateTimeOffsetJsonConverter(formatString, outputToLocalDateTime));
        }
    }
}