using JNPF.Common.Configuration;
using JNPF.Common.Core.Captcha.General;
using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Helper;
using JNPF.DataEncryption;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.JsonSerialization;
using JNPF.RemoteRequest.Extensions;
using JNPF.System.Interfaces.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OnceMi.AspNetCore.OSS;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Yitter.IdGenerator;

namespace JNPF.System.Service.Common
{
    /// <summary>
    /// 业务实现：通用控制器
    /// </summary>
    [ApiDescriptionSettings(Tag = "Common", Name = "File", Order = 161)]
    [Route("api/[controller]")]
    public class FileService : IFileService, IDynamicApiController, ITransient
    {
        private readonly IGeneralCaptcha _captchaHandle;// 验证码服务
        private readonly IConfiguration _configuration;
        private readonly IUserManager _userManager;
        private readonly IOSSServiceFactory _oSSServiceFactory;

        /// <summary>
        /// 初始化一个<see cref="FileService"/>类型的新实例
        /// </summary>
        public FileService(IGeneralCaptcha captchaHandle, IConfiguration configuration, IUserManager userManager, IOSSServiceFactory oSSServiceFactory)
        {
            _captchaHandle = captchaHandle;
            _configuration = configuration;
            _userManager = userManager;
            _oSSServiceFactory = oSSServiceFactory;
        }

        /// <summary>
        /// 上传文件/图片
        /// </summary>
        /// <returns></returns>
        [HttpPost("Uploader/{type}")]
        [AllowAnonymous]
        public async Task<dynamic> Uploader(string type, IFormFile file)
        {
            var fileType = Path.GetExtension(file.FileName).Replace(".", "");
            if (!this.AllowFileType(fileType, type))
                throw JNPFException.Oh(ErrorCode.D1800);
            var _filePath = GetPathByType(type);
            var _fileName = DateTime.Now.ToString("yyyyMMdd") + "_" + YitIdHelper.NextId().ToString() + Path.GetExtension(file.FileName);
            await UploadFileByType(file, _filePath, _fileName);
            return new { name = _fileName, url = string.Format("/api/File/Image/{0}/{1}", type, _fileName) };
        }

        /// <summary>
        /// 生成图片链接
        /// </summary>
        /// <param name="type">图片类型 </param>
        /// <param name="fileName">注意 后缀名前端故意把 .替换@ </param>
        /// <returns></returns>
        [HttpGet("Image/{type}/{fileName}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetImg(string type, string fileName)
        {
            var filePath = Path.Combine(GetPathByType(type), fileName.Replace("@", "."));
            return await DownloadFileByType(filePath, fileName);
            //return new FileStreamResult(new FileStream(filePath, FileMode.Open), "application/octet-stream") { FileDownloadName = fileName };
        }

        /// <summary>
        /// 生成大屏图片链接
        /// </summary>
        /// <param name="type">图片类型 </param>
        /// <param name="fileName">注意 后缀名前端故意把 .替换@ </param>
        /// <returns></returns>
        [HttpGet("VisusalImg/{type}/{fileName}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetScreenImg(string type, string fileName)
        {
            var filePath = Path.Combine(GetPathByType(type), type, fileName.Replace("@", "."));
            return await DownloadFileByType(filePath, fileName);
            //return new FileStreamResult(new FileStream(filePath, FileMode.Open), "application/octet-stream") { FileDownloadName = fileName };
        }

        /// <summary>
        /// 获取图形验证码
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <returns></returns>
        [HttpGet("ImageCode/{timestamp}")]
        [AllowAnonymous]
        [NonUnify]
        public IActionResult GetCode(string timestamp)
        {
            return new FileContentResult(_captchaHandle.CreateCaptchaImage(timestamp, 114, 32), "image/jpeg");
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        [HttpPost("Uploader/userAvatar")]
        [AllowAnonymous]
        public async Task<dynamic> UploadImage(IFormFile file)
        {
            var ImgType = Path.GetExtension(file.FileName).Replace(".", "");
            if (!this.AllowImageType(ImgType))
                throw JNPFException.Oh(ErrorCode.D5013);
            var filePath = FileVariable.UserAvatarFilePath;
            var fileName = DateTime.Now.ToString("yyyyMMdd") + "_" + YitIdHelper.NextId().ToString() + Path.GetExtension(file.FileName);
            await UploadFileByType(file, filePath, fileName);
            return new { name = fileName, url = "/api/file/Image/userAvatar/" + fileName };
        }

        #region 下载附件

        /// <summary>
        /// 获取下载文件链接
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet("Download/{type}/{fileName}")]
        [AllowAnonymous]
        public dynamic DownloadUrl(string type, string fileName)
        {
            var url = _userManager.UserId + "|" + fileName + "|" + type;
            var encryptStr = DESCEncryption.Encrypt(url, "JNPF");
            return new { name = fileName, url = "/api/file/Download?encryption=" + encryptStr };
        }

        /// <summary>
        /// 下载文件链接
        /// </summary>
        [HttpGet("Download")]
        [AllowAnonymous]
        public async Task<dynamic> DownloadFile([FromQuery] string encryption)
        {
            var decryptStr = DESCEncryption.Decrypt(encryption, "JNPF");
            var paramsList = decryptStr.Split("|").ToList();
            if (paramsList.Count > 0)
            {
                var fileName = paramsList.Count > 1 ? paramsList[1] : "";
                string type = paramsList.Count > 2 ? paramsList[2] : "";
                var filePath = Path.Combine(GetPathByType(type), fileName.Replace("@", "."));
                var fileDownloadName = fileName.Replace(GetPathByType(type), "");
                return await DownloadFileByType(filePath, fileDownloadName);
            }
            else
            {
                throw JNPFException.Oh(ErrorCode.D8000);
            }
        }

        #endregion

        #region 多种存储文件
        /// <summary>
        /// 根据存储类型上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [NonAction]
        public async Task UploadFileByType(IFormFile file, string filePath, string fileName)
        {
            try
            {
                var bucketName = KeyVariable.BucketName;
                var fileStoreType = KeyVariable.FileStoreType;
                var uploadPath = Path.Combine(filePath, fileName);
                var stream = file.OpenReadStream();
                switch (fileStoreType)
                {
                    case "minio":
                        await _oSSServiceFactory.Create().PutObjectAsync(bucketName, uploadPath, stream);
                        break;
                    case "aliyun-oss":
                        await _oSSServiceFactory.Create("aliyun").PutObjectAsync(bucketName, uploadPath, stream);
                        break;
                    case "tencent-cos":
                        await _oSSServiceFactory.Create("qcloud").PutObjectAsync(bucketName, uploadPath, stream);
                        break;
                    default:
                        if (!Directory.Exists(filePath))
                            Directory.CreateDirectory(filePath);
                        using (var stream4 = File.Create(uploadPath))
                        {
                            await file.CopyToAsync(stream4);
                        }
                        break;
                }
            }
            catch (Exception)
            {
                throw JNPFException.Oh(ErrorCode.D8003);
            }
        }

        /// <summary>
        /// 根据存储类型下载文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileDownLoadName"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<FileStreamResult> DownloadFileByType(string filePath, string fileDownLoadName)
        {
            try
            {
                var bucketName = KeyVariable.BucketName;
                var fileStoreType = KeyVariable.FileStoreType;
                switch (fileStoreType)
                {
                    case "minio":
                        var url1 = await _oSSServiceFactory.Create().PresignedGetObjectAsync(bucketName, filePath, 86400);
                        var stream1 = await url1.GetAsStreamAsync();
                        return new FileStreamResult(stream1, "application/octet-stream") { FileDownloadName = fileDownLoadName };
                    case "aliyun-oss":
                        var url2 = await _oSSServiceFactory.Create("Aliyun").PresignedGetObjectAsync(bucketName, filePath, 86400);
                        var stream2 = await url2.GetAsStreamAsync();
                        return new FileStreamResult(stream2, "application/octet-stream") { FileDownloadName = fileDownLoadName };
                    case "tencent-cos":
                        var url3 = await _oSSServiceFactory.Create("QCloud").PresignedGetObjectAsync(bucketName, filePath, 86400);
                        var stream3 = await url3.GetAsStreamAsync();
                        return new FileStreamResult(stream3, "application/octet-stream") { FileDownloadName = fileDownLoadName };
                    default:
                        return new FileStreamResult(new FileStream(filePath, FileMode.Open), "application/octet-stream") { FileDownloadName = fileDownLoadName };
                }
            }
            catch (Exception e)
            {
                throw JNPFException.Oh(ErrorCode.D8003);
            }
        }
        #endregion

        /// <summary>
        /// 根据类型获取文件存储路径
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [NonAction]
        public string GetPathByType(string type)
        {
            switch (type)
            {
                case "userAvatar":
                    return FileVariable.UserAvatarFilePath;
                case "mail":
                    return FileVariable.EmailFilePath;
                case "IM":
                    return FileVariable.IMContentFilePath;
                case "weixin":
                    return FileVariable.MPMaterialFilePath;
                case "workFlow":
                    return FileVariable.SystemFilePath;
                case "annex":
                    return FileVariable.SystemFilePath;
                case "annexpic":
                    return FileVariable.SystemFilePath;
                case "document":
                    return FileVariable.DocumentFilePath;
                //case "dataBackup":
                //    return ConfigurationFileConst.DataBackupFilePath;
                case "preview":
                    return FileVariable.DocumentPreviewFilePath;
                case "screenShot":
                case "banner":
                case "bg":
                case "border":
                case "source":
                    return FileVariable.BiVisualPath;
                case "template":
                    return FileVariable.TemplateFilePath;
                case "codeGenerator":
                    return FileVariable.GenerateCodePath;
                default:
                    return FileVariable.TemporaryFilePath;
            }
        }

        /// <summary>
        /// 允许文件类型
        /// </summary>
        /// <param name="fileExtension">文件后缀名</param>
        /// <param name="type">文件类型</param>
        /// <returns></returns>
        private bool AllowFileType(string fileExtension, string type)
        {
            var allowExtension = KeyVariable.AllowUploadFileType;
            if (type.Equals("weixin"))
            {
                allowExtension = KeyVariable.WeChatUploadFileType;
            }
            var isExist = allowExtension.Find(a => a == fileExtension.ToLower());
            if (!string.IsNullOrEmpty(isExist))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 允许文件类型
        /// </summary>
        /// <param name="fileExtension">文件后缀名</param>
        /// <returns></returns>
        private bool AllowImageType(string fileExtension)
        {
            var allowExtension = KeyVariable.AllowImageType;
            var isExist = allowExtension.Find(a => a == fileExtension.ToLower());
            if (!string.IsNullOrEmpty(isExist))
                return true;
            else
                return false;
        }

        #region 导入导出

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [NonAction]
        public dynamic Export(string jsonStr, string name)
        {
            var _filePath = GetPathByType("");
            var _fileName = name + Ext.GetTimeStamp + ".Json";
            if (!Directory.Exists(_filePath))
                Directory.CreateDirectory(_filePath);
            var byteList = new UTF8Encoding(true).GetBytes(jsonStr.ToCharArray());
            FileHelper.CreateFile(_filePath + _fileName, byteList);
            var fileName = _userManager.UserId + "|" + _filePath + _fileName + "|json";
            var output = new
            {
                name = _fileName,
                url = "/api/file/Download?encryption=" + DESCEncryption.Encrypt(fileName, "JNPF")
            };
            return output;
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [NonAction]
        public string Import(IFormFile file)
        {
            var stream = file.OpenReadStream();
            var byteList = new byte[file.Length];
            stream.Read(byteList, 0, (int)file.Length);
            stream.Position = 0;
            var sr = new StreamReader(stream, Encoding.Default);
            var json = sr.ReadToEnd();
            sr.Close();
            stream.Close();
            return json;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="file"></param>
        [NonAction]
        public void UploadFile(string type, IFormFile file)
        {
            var fileType = Path.GetExtension(file.FileName).Replace(".", "");
            if (!this.AllowFileType(fileType, type))
                throw JNPFException.Oh(ErrorCode.D1800);
            var _filePath = GetPathByType(type);
            var _fileName = file.FileName;
            if (!Directory.Exists(_filePath))
                Directory.CreateDirectory(_filePath);
            using (var stream = File.Create(Path.Combine(_filePath, _fileName)))
            {
                file.CopyTo(stream);
            }
        }
        #endregion
    }
}
