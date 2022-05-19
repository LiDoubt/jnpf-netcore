using System;

namespace JNPF.Common.FileManage
{
    /// <summary>
    /// 附件模型
    /// 版 本：V3.0.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// </summary>
    public class FileModel
    {
        public string FileId { get; set; }

        public string FileName { get; set; }

        public string FileSize { get; set; }

        public DateTime FileTime { get; set; }

        public string FileState { get; set; }

        public string FileType { get; set; }
    }
}
