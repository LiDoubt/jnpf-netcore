using JNPF.Dependency;
using Microsoft.AspNetCore.Http;

namespace JNPF.Extend.Entitys.Dto.Document
{
    [SuppressSniffer]
    public class DocumentUploaderInput
    {
        /// <summary>
        /// 上级文件id
        /// </summary>
        public string parentId { get; set; }
        /// <summary>
        /// 上级文件id
        /// </summary>
        public IFormFile file { get; set; }
    }
}
