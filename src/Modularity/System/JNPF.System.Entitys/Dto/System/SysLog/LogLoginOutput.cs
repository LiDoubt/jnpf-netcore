using JNPF.Dependency;
using System;

namespace JNPF.System.Entitys.Dto.System.SysLog
{
    /// <summary>
    /// 登录日记输出
    /// </summary>
    [SuppressSniffer]
    public class LogLoginOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime? creatorTime { get; set; }

        /// <summary>
        /// 登录用户
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 登录IP
        /// </summary>
        public string ipaddress { get; set; }

        /// <summary>
        /// 登录摘要
        /// </summary>
        public string platForm { get; set; }
    }
}
