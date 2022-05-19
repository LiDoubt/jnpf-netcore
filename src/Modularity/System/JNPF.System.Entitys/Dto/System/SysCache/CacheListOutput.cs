using JNPF.Dependency;
using System;

namespace JNPF.System.Entitys.Dto.System.SysCache
{
    /// <summary>
    /// 缓存列表输出
    /// </summary>
    [SuppressSniffer]
    public class CacheListOutput
    {
        /// <summary>
        /// 缓存名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime overdueTime { get; set; }
    }
}
