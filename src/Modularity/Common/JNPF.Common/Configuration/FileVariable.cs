using JNPF.Dependency;
using System.IO;

namespace JNPF.Common.Configuration
{
    /// <summary>
    /// 配置文件
    /// </summary>
    [SuppressSniffer]
    public class FileVariable
    {
        public static string SystemPath = KeyVariable.SystemPath;

        /// <summary>
        /// 用户头像存储路径
        /// </summary>
        public static string UserAvatarFilePath = SystemPath + "UserAvatar/";

        /// <summary>
        /// 临时文件存储路径
        /// </summary>
        public static string TemporaryFilePath = SystemPath + "TemporaryFile/";

        /// <summary>
        /// 备份数据存储路径
        /// </summary>
        public static string DataBackupFilePath = SystemPath + "DataBackupFile/";

        /// <summary>
        /// IM内容文件存储路径
        /// </summary>
        public static string IMContentFilePath = SystemPath + "IMContentFile/";

        /// <summary>
        /// 系统文件存储路径
        /// </summary>
        public static string SystemFilePath = SystemPath + "SystemFile/";

        /// <summary>
        /// 微信公众号资源存储路径
        /// </summary>
        public static string MPMaterialFilePath = SystemPath + "MPMaterial/";

        /// <summary>
        /// 文档管理存储路径
        /// </summary>
        public static string DocumentFilePath = SystemPath + "DocumentFile/";

        /// <summary>
        /// 生成代码路径
        /// </summary>
        public static string GenerateCodePath = SystemPath + "CodeGenerate";

        /// 文件在线预览存储PDF
        /// </summary>
        public static string DocumentPreviewFilePath = SystemPath + "DocumentPreview/";

        /// 邮件文件存储路径
        /// </summary>
        public static string EmailFilePath = SystemPath + "EmailFile/";

        /// <summary>
        /// 大屏图片路径
        /// </summary>
        public static string BiVisualPath = Path.Combine(SystemPath, "BiVisualPath");

        /// <summary>
        /// 模板路径
        /// </summary>
        public static string TemplateFilePath = SystemPath + "TemplateFile/";
    }
}
