using JNPF.Common.Configuration;
using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.FileManage;
using JNPF.Common.Filter;
using JNPF.Common.Helper;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.Extend.Entitys.Dto.DocumentPreview;
using JNPF.FriendlyException;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JNPF.Extend
{
    /// <summary>
    /// 文件预览
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "Extend", Name = "DocumentPreview", Order = 600)]
    [Route("api/extend/[controller]")]
    public class DocumentPreview : IDynamicApiController, ITransient
    {
        #region Get
        /// <summary>
        /// 获取文档列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("")]
        public dynamic GetList_Api([FromQuery] KeywordInput input)
        {
            var output = GetList(input);
            return output;
        }

        /// <summary>
        /// 文件在线预览
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("{fileId}/Preview")]
        public dynamic DocumentPreview_Api(string fileId, [FromQuery] DocumentPreviewPreviewInput input)
        {
            var output = new DocumentPreviewPreviewOutput();
            var filePath = FileVariable.DocumentPreviewFilePath;
            var files = FileHelper.GetAllFiles(filePath);
            if (fileId.ToInt() > files.Count)
                throw JNPFException.Oh(ErrorCode.D8000);
            var file = files[fileId.ToInt()];
            if (file != null)
            {
                var domain = App.Configuration["JNPF_APP:Domain"];
                var yozoUrl= App.Configuration["JNPF_APP:YOZO:domain"];
                var yozoKey = App.Configuration["JNPF_APP:YOZO:domainKey"];
                var httpContext = App.HttpContext;
                output.fileName = file.Name;
                output.filePath = domain + "/api/Extend/DocumentPreview/down/" + file.Name;
                var url = string.Format("{0}?k={1}&url={2}", yozoUrl,
                    yozoKey, output.filePath, input.noCache, input.watermark, input.isCopy, input.pageStart, input.pageEnd, input.type);
                //if (input.previewType.Equals("localPreview"))
                //{
                //    url = "http://" + httpContext.GetLocalIpAddressToIPv4() + ":" + httpContext.Connection.LocalPort + "/api/Extend/DocumentPreview/down/" + file.Name;
                //}
                return url;
            }
            else
                throw JNPFException.Oh(ErrorCode.D8000);

        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="fileName"></param>
        [HttpGet("down/{fileName}")]
        [AllowAnonymous]
        public void FileDown(string fileName)
        {
            var filePath = FileVariable.DocumentPreviewFilePath + fileName;
            var systemFilePath = FileVariable.SystemFilePath + fileName;
            if (FileHelper.Exists(filePath))
                FileHelper.DownloadFile(filePath, fileName);
            else
                FileHelper.DownloadFile(systemFilePath, fileName);
        }

        /// <summary>
        /// 本地预览
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpGet("PreviewLocal/{fileId}")]
        [AllowAnonymous]
        public dynamic PreviewLocalFile(string fileId)
        {
            var filePath = FileVariable.DocumentPreviewFilePath;
            var fileInfo = FileHelper.GetAllFiles(filePath)[fileId.ToInt()];
            var fileName = filePath + fileInfo.Name;
            if (fileInfo.Extension == ".xlsx" || fileInfo.Extension == ".xls")
            {
                PDFHelper.AsposeExcelToPDF(filePath + fileInfo.Name);
            }
            else if (fileInfo.Extension == ".docx" || fileInfo.Extension == ".doc")
            {
                PDFHelper.AsposeWordToPDF(filePath + fileInfo.Name);
            }
            else if (fileInfo.Extension == ".pptx" || fileInfo.Extension == ".ppt")
            {
                PDFHelper.PPTToPDF(filePath + fileInfo.Name);
            }
            //输出pdf文件
            return new FileStreamResult(new FileStream(fileName, FileMode.Open), "application/pdf") { FileDownloadName = fileInfo.Name };
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 文档列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private List<DocumentPreviewListOutput> GetList(KeywordInput input)
        {
            var filePath = FileVariable.DocumentPreviewFilePath;
            var files = FileHelper.GetAllFiles(filePath);
            List<FileModel> data = new List<FileModel>();
            if (files != null)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    var item = files[i];
                    FileModel fileModel = new FileModel();
                    fileModel.FileId = i.ToString();
                    fileModel.FileName = item.Name;
                    fileModel.FileType = FileHelper.GetFileType(item);
                    fileModel.FileSize = FileHelper.GetFileSize(item.FullName).ToString();
                    fileModel.FileTime = item.LastWriteTime;
                    data.Add(fileModel);
                }
                data = data.FindAll(x => "xlsx".Equals(x.FileType) || "xls".Equals(x.FileType) || "docx".Equals(x.FileType) || "doc".Equals(x.FileType) || "pptx".Equals(x.FileType) || "ppt".Equals(x.FileType));
            }
            if (!input.keyword.IsNullOrEmpty())
            {
                data = data.FindAll(x => x.FileName.Contains(input.keyword));
            }
            var output = data.OrderByDescending(x=>x.FileTime).Adapt<List<DocumentPreviewListOutput>>();
            return output;
        }
        #endregion
    }
}
