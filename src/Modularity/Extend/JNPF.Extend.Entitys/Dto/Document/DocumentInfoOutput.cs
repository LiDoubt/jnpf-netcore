using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.Document
{
    /// <summary>
    /// 获取文件/文件夹信息
    /// </summary>
    [SuppressSniffer]
    public class DocumentInfoOutput
    {
        /// <summary>
        /// 父级id
        /// </summary>
        public string parentId { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public int? type { get; set; }
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 文件名/文件夹名
        /// </summary>
        public string fullName { get; set; }
    }
}
