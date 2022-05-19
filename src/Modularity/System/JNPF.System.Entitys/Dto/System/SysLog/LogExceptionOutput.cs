using JNPF.Dependency;
using System;

namespace JNPF.System.Entitys.Dto.System.SysLog
{
    /// <summary>
    /// 异常日记输出
    /// </summary>
    [SuppressSniffer]
    public class LogExceptionOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 异常IP
        /// </summary>
        public string ipaddress { get; set; }

        /// <summary>
        /// 异常功能
        /// </summary>
        public string moduleName { get; set; }

        /// <summary>
        /// 日志分类
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// 异常描述
        /// </summary>
        public string json { get; set; }
    }
}
