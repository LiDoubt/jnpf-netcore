using JNPF.Common.Model.Machine;
using JNPF.Dependency;
using System;

namespace JNPF.System.Entitys.Dto.System.Monitor.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressSniffer]
    public class MonitorOutput
    {
        /// <summary>
        /// 系统信息
        /// </summary>
        public SystemInfoModel system { get; set; }

        /// <summary>
        /// CPU信息
        /// </summary>
        public CpuInfoModel cpu { get; set; }

        /// <summary>
        /// 内存信息
        /// </summary>
        public MemoryInfoModel memory { get; set; }

        /// <summary>
        /// 硬盘信息
        /// </summary>
        public DiskInfoModel disk { get; set; }

        /// <summary>
        /// 服务器当时时间戳
        /// </summary>
        public DateTime? time { get; set; }
    }
}
