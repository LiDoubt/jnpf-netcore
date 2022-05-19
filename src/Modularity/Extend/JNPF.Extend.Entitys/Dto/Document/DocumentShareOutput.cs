using JNPF.Dependency;
using System;

namespace JNPF.Extend.Entitys.Dto.Document
{
    /// <summary>
    /// 知识管理（我的共享列表）
    /// </summary>
    [SuppressSniffer]
    public class DocumentShareOutput
    {
        /// <summary>
        /// 共享日期
        /// </summary>
        public DateTime? shareTime { get; set; }
        /// <summary>
        /// 大小
        /// </summary>
        public string fileSize { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public int? type { get; set; }
        /// <summary>
        /// 后缀名
        /// </summary>
        public string fileExtension { get; set; }
    }
}
