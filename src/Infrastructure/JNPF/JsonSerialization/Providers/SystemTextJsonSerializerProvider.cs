﻿using JNPF.Dependency;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace JNPF.JsonSerialization
{
    /// <summary>
    /// System.Text.Json 序列化提供器（默认实现）
    /// </summary>
    [Injection(Order = -999)]
    public class SystemTextJsonSerializerProvider : IJsonSerializerProvider, ISingleton
    {
        /// <summary>
        /// 获取 JSON 配置选项
        /// </summary>
        private readonly JsonOptions _jsonOptions;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options"></param>
        public SystemTextJsonSerializerProvider(IOptions<JsonOptions> options)
        {
            _jsonOptions = options.Value;
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="jsonSerializerOptions"></param>
        /// <returns></returns>
        public string Serialize(object value, object jsonSerializerOptions = null)
        {
            return JsonSerializer.Serialize(value, (jsonSerializerOptions ?? GetSerializerOptions()) as JsonSerializerOptions);
        }

        /// <summary>
        /// 反序列化字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="jsonSerializerOptions"></param>
        /// <returns></returns>
        public T Deserialize<T>(string json, object jsonSerializerOptions = null)
        {
            return JsonSerializer.Deserialize<T>(json, (jsonSerializerOptions ?? GetSerializerOptions()) as JsonSerializerOptions);
        }

        /// <summary>
        /// 返回读取全局配置的 JSON 选项
        /// </summary>
        /// <returns></returns>
        public object GetSerializerOptions()
        {
            return _jsonOptions?.JsonSerializerOptions;
        }
    }
}