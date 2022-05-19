using JNPF.Dependency;

namespace JNPF.Extend.Entitys.Dto.Document
{
    /// <summary>
    /// 修改文件名/文件夹名
    /// </summary>
    [SuppressSniffer]
    public class DocumentUpInput : DocumentCrInput
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
    }
}
