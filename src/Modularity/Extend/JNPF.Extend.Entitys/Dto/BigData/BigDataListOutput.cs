using JNPF.Dependency;
using System;

namespace JNPF.Extend.Entitys.Dto.BigData
{
    /// <summary>
    /// 大数据列表
    /// </summary>
    [SuppressSniffer]
    public class BigDataListOutput
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }
    }
}
