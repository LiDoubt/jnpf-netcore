using JNPF.Dependency;
using System;

namespace JNPF.System.Entitys.Dto.System.DbBackup
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressSniffer]
    public class DbBackupListOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public string fileSize { get; set; }

        /// <summary>
        /// 备份时间
        /// </summary>
        public DateTime? creatorTime { get; set; }

        /// <summary>
        /// 文件下载地址
        /// </summary>
        public string fileUrl { get; set; }
    }
}
