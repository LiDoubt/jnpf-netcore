using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.Document
{
    /// <summary>
    /// 添加文件夹
    /// </summary>
    [SuppressSniffer]
    public class DocumentCrInput
    {
        /// <summary>
        /// 文件夹名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 文档父级
        /// </summary>
        public string parentId { get; set; }
        /// <summary>
        /// 文档分类
        /// </summary>
        public int? type { get; set; }
    }
}
