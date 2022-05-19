using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.DocumentPreview
{
    /// <summary>
    /// 预览文档
    /// </summary>
    [SuppressSniffer]
    public class DocumentPreviewPreviewOutput
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string filePath { get; set; }

    }
}
