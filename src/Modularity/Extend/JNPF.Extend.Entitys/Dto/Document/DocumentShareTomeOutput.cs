using JNPF.Dependency;
using System;

namespace JNPF.Extend.Entitys.Dto.Document
{
    /// <summary>
    /// 获取知识管理列表（共享给我）
    /// </summary>
    [SuppressSniffer]
    public class DocumentShareTomeOutput
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
        /// 名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 共享人员Id
        /// </summary>
        public string creatorUserId { get; set; }
        /// <summary>
        /// 后缀名
        /// </summary>
        public string fileExtension { get; set; }
    }
}
