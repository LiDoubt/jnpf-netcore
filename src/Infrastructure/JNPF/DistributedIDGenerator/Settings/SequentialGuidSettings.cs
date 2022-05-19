using JNPF.Dependency;
using System;

namespace JNPF.DistributedIDGenerator
{
    /// <summary>
    /// 连续 GUID 配置
    /// </summary>
    [SuppressSniffer]
    public sealed class SequentialGuidSettings
    {
        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTimeOffset? TimeNow { get; set; }

        /// <summary>
        /// LittleEndianBinary 16 格式化
        /// </summary>
        public bool LittleEndianBinary16Format { get; set; }
    }
}
