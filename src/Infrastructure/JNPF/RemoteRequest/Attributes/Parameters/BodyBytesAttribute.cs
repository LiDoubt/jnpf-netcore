using JNPF.Dependency;
using System;

namespace JNPF.RemoteRequest
{
    /// <summary>
    /// 配置 Body Bytes 参数
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Parameter)]
    public class BodyBytesAttribute : ParameterBaseAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="alias"></param>
        public BodyBytesAttribute(string alias)
        {
            Alias = alias;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="fileName"></param>
        public BodyBytesAttribute(string alias, string fileName)
        {
            Alias = alias;
            FileName = fileName;
        }

        /// <summary>
        /// 参数别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
    }
}