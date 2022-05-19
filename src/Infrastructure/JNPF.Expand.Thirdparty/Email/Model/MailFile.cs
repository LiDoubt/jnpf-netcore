using JNPF.Dependency;
using System;

namespace JNPF.Expand.Thirdparty.Email.Model
{
    [SuppressSniffer]
    public class MailFile
    {
        /// <summary>
        /// 文件id
        /// </summary>
        public string fileId { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public string fileSize { get; set; }
        /// <summary>
        /// 文件时间
        /// </summary>
        public DateTime fileTime { get; set; }
        /// <summary>
        /// 文件状态
        /// </summary>
        public string fileState { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string name { get; set; }
    }
}
