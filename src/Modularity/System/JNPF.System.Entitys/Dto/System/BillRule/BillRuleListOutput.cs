using JNPF.Dependency;
using Newtonsoft.Json;
using System;

namespace JNPF.System.Entitys.Dto.System.BillRule
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressSniffer]
    public class BillRuleListOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 业务名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 业务编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 当前流水号
        /// </summary>
        public string outputNumber { get; set; }

        /// <summary>
        /// 流水位数
        /// </summary>
        public double digit { get; set; }

        /// <summary>
        /// 流水起始
        /// </summary>
        public string startNumber { get; set; }

        /// <summary>
        /// 流水状态
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public long? sortCode { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        //[JsonIgnore]
        public DateTime? creatorTime { get; set; }
    }
}
