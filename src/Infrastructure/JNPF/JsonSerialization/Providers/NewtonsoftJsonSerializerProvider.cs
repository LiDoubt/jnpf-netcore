using JNPF.Dependency;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace JNPF.JsonSerialization.Providers
{
    /// <summary>
    /// Newtonsoft.Json 序列化提供器
    /// </summary>
    [Injection(Order = -999)]
    public class NewtonsoftJsonSerializerProvider : IJsonSerializerProvider, ISingleton
    {
        /// <summary>
        /// 获取 JSON 配置选项
        /// </summary>
        private readonly MvcNewtonsoftJsonOptions _jsonOptions;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options"></param>
        public NewtonsoftJsonSerializerProvider(IOptions<MvcNewtonsoftJsonOptions> options)
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
            return JsonConvert.SerializeObject(value, (jsonSerializerOptions ?? GetSerializerOptions()) as JsonSerializerSettings);
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
            return JsonConvert.DeserializeObject<T>(json, (jsonSerializerOptions ?? GetSerializerOptions()) as JsonSerializerSettings);
        }

        /// <summary>
        /// 返回读取全局配置的 JSON 选项
        /// </summary>
        /// <returns></returns>
        public object GetSerializerOptions()
        {
            return _jsonOptions?.SerializerSettings;
        }
    }
}
