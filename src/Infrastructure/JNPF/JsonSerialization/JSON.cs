using JNPF.Dependency;
using System;
using System.Collections.Generic;

namespace JNPF.JsonSerialization
{
    /// <summary>
    /// JSON 静态帮助类
    /// </summary>
    [SuppressSniffer]
    public static class JSON
    {
        /// <summary>
        /// 获取 JSON 序列化提供器
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static IJsonSerializerProvider GetJsonSerializer(IServiceProvider serviceProvider = default)
        {
            return App.GetService<IJsonSerializerProvider>(serviceProvider ?? App.RootServices);
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="jsonSerializerOptions"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static string Serialize(this object value, object jsonSerializerOptions = default, IServiceProvider serviceProvider = default)
        {
            return GetJsonSerializer(serviceProvider).Serialize(value, jsonSerializerOptions);
        }

        /// <summary>
        /// 反序列化字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="jsonSerializerOptions"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string json, object jsonSerializerOptions = default, IServiceProvider serviceProvider = default)
        {
            return GetJsonSerializer(serviceProvider).Deserialize<T>(json, jsonSerializerOptions);
        }

        /// <summary>
        /// 获取 JSON 配置选项
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static TOptions GetSerializerOptions<TOptions>(IServiceProvider serviceProvider = default)
            where TOptions : class
        {
            return GetJsonSerializer(serviceProvider).GetSerializerOptions() as TOptions;
        }

        /// <summary>
        /// 把对象类型转化为指定类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="jsonSerializerOptions"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static T ToObeject<T>(this Object json, object jsonSerializerOptions = default, IServiceProvider serviceProvider = default)
        {
            return Serialize(json).Deserialize<T>();
        }

        /// <summary>
        /// 把数组字符串转化为指定类型数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="jsonSerializerOptions"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this string json, object jsonSerializerOptions = default, IServiceProvider serviceProvider = default)
        {
            return json == null ? null : json.Deserialize<List<T>>(); ;
        }
    }
}