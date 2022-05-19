using JNPF.Dependency;
using System;

namespace JNPF.System.Entitys.Dto.System.SysLog
{
    /// <summary>
    /// 请求日记输出
    /// </summary>
    [SuppressSniffer]
    public class LogRequestOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 请求时间
        /// </summary>
        public DateTime? creatorTime { get; set; }

        /// <summary>
        /// 请求用户名
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 请求IP
        /// </summary>
        public string ipaddress { get; set; }

        /// <summary>
        /// 请求设备
        /// </summary>
        public string platForm { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string requestURL { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        public string requestMethod { get; set; }

        /// <summary>
        /// 请求耗时
        /// </summary>
        public int requestDuration { get; set; }
    }
}
