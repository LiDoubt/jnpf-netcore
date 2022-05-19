using JNPF.Dependency;
using System;

namespace JNPF.Extend.Entitys.Dto.Document
{
    /// <summary>
    /// 获取知识管理列表（共享人员）
    /// </summary>
    [SuppressSniffer]
    public class DocumentShareUserOutput
    {
        /// <summary>
        /// 共享时间
        /// </summary>
        public DateTime? shareTime { get; set; }
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 共享人员id
        /// </summary>
        public string shareUserId { get; set; }
        /// <summary>
        /// 文档id
        /// </summary>
        public string documentId { get; set; }
    }
}
