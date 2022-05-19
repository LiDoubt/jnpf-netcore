using JNPF.Common.Extension;
using JNPF.Dependency;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JNPF.Common.Configuration
{
    /// <summary>
    /// Key常量
    /// </summary>
    [SuppressSniffer]
    public class KeyVariable
    {
        /// <summary>
        /// 多租户模式
        /// </summary>
        public static bool MultiTenancy
        {
            get
            {
                var flag = App.Configuration["JNPF_App:MultiTenancy"];
                return flag.ToBool();
            }
        }

        /// <summary>
        /// 系统文件路径
        /// </summary>
        public static string SystemPath
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["JNPF_App:SystemPath"]) ? Directory.GetCurrentDirectory() : App.Configuration["JNPF_App:SystemPath"];
            }
        }

        /// <summary>
        /// 命名空间
        /// </summary>
        public static List<string> AreasName
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["JNPF_APP:CodeAreasName"]) ? new List<string>() : App.Configuration["JNPF_APP:CodeAreasName"].Split(',').ToList();
            }
        }

        /// <summary>
        /// 允许上传图片类型
        /// </summary>
        public static List<string> AllowImageType
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["JNPF_APP:AllowUploadImageType"]) ? new List<string>() : App.Configuration["JNPF_APP:AllowUploadImageType"].Split(',').ToList();
            }
        }

        /// <summary>
        /// 允许上传文件类型
        /// </summary>
        public static List<string> AllowUploadFileType
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["JNPF_APP:AllowUploadFileType"]) ? new List<string>() : App.Configuration["JNPF_APP:AllowUploadFileType"].Split(',').ToList();
            }
        }

        /// <summary>
        /// 微信允许上传文件类型
        /// </summary>
        public static List<string> WeChatUploadFileType
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["JNPF_APP:WeChatUploadFileType"]) ? new List<string>() : App.Configuration["JNPF_APP:WeChatUploadFileType"].Split(',').ToList();
            }
        }

        /// <summary>
        /// MinIO桶
        /// </summary>
        public static string BucketName
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["JNPF_APP:BucketName"]) ? "" : App.Configuration["JNPF_APP:BucketName"];
            }
        }

        /// <summary>
        /// 文件储存类型
        /// </summary>
        public static string FileStoreType
        {
            get
            {
                return string.IsNullOrEmpty(App.Configuration["JNPF_APP:FileStoreType"]) ? "local" : App.Configuration["JNPF_APP:FileStoreType"];
            }
        }
    }
}
