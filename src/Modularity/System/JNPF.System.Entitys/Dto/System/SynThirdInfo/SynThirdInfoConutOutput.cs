using JNPF.Dependency;
using System;

namespace JNPF.System.Entitys.Dto.System.SynThirdInfo
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressSniffer]
    public class SynThirdInfoConutOutput
    {
        /// <summary>
        /// 总数
        /// </summary>
        public int recordTotal { get; set; }

        /// <summary>
        /// 同步时间
        /// </summary>
        public DateTime? synDate { get; set; }

        /// <summary>
        /// 失败条数
        /// </summary>
        public int synFailCount { get; set; }

        /// <summary>
        /// 成功条数
        /// </summary>
        public int synSuccessCount { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string synType { get; set; }

        /// <summary>
        /// 未同步条数
        /// </summary>
        public int unSynCount { get; set; }
    }
}
