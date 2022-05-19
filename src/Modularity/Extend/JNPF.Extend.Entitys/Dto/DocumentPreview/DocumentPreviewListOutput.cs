using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.DocumentPreview
{
    /// <summary>
    /// 获取文档列表
    /// </summary>
    [SuppressSniffer]
    public class DocumentPreviewListOutput
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public string fileSize { get; set; }
        /// <summary>
        /// 主键id	
        /// </summary>
        public string fileId { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public string fileTime { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public string fileType { get; set; }
    }
}
