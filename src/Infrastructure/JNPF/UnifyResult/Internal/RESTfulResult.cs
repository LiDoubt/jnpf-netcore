using JNPF.Dependency;

namespace JNPF.UnifyResult
{
    /// <summary>
    /// RESTful 风格结果集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SuppressSniffer]
    public class RESTfulResult<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int? code { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        public object msg { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public T data { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public object extras { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long timestamp { get; set; }
    }
}