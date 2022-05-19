using JNPF.Dependency;
using System;

namespace JNPF.Extend.Entitys.Dto.Document
{
    /// <summary>
    /// 回收站（彻底删除）
    /// </summary>
    [SuppressSniffer]
    public class DocumentTrashOutput
    {
        /// <summary>
        /// 删除日期
        /// </summary>
        public DateTime? deleteTime { get; set; }
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
        /// 后缀名
        /// </summary>
        public string fileExtension { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int? type { get; set; }
    }
}
