using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.DocumentPreview
{
    /// <summary>
    /// 预览文档
    /// </summary>
    [SuppressSniffer]
    public class DocumentPreviewPreviewInput
    {
        /// <summary>
        /// 文件id
        /// </summary>
        public string fileId { get; set; }
        /// <summary>
        /// 是否强制重新转换（忽略缓存）,true为强制重新转换，false为不强制重新转换。
        /// </summary>
        public bool noCache { get; set; }
        /// <summary>
        ///  针对单文档设置水印内容
        /// </summary>
        public string watermark { get; set; }
        /// <summary>
        /// 0否1是，默认为0。针对单文档设置是否防复制
        /// </summary>
        public int isCopy{ get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string pageStart { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string pageEnd { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 预览方式（localPreview：本地，yozoOnlinePreview：在线）
        /// </summary>
        public string previewType { get; set; }

    }
}
