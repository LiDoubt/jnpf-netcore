using JNPF.Common.Configuration;
using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Filter;
using JNPF.Common.Helper;
using JNPF.Common.Util;
using JNPF.DataEncryption;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.Extend.Entitys;
using JNPF.Extend.Entitys.Dto.Document;
using JNPF.FriendlyException;
using JNPF.System.Entitys.Permission;
using JNPF.System.Interfaces.Common;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace JNPF.Extend
{
    /// <summary>
    /// 知识管理
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "Extend", Name = "Document", Order = 601)]
    [Route("api/extend/[controller]")]
    public class DocumentService : IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<DocumentEntity> _documentRepository;
        private readonly IFileService _fileService;
        private readonly SqlSugarScope db;// 核心对象：拥有完整的SqlSugar全部功能
        private readonly IUserManager _userManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentRepository"></param>
        /// <param name="fileService"></param>
        /// <param name="userManager"></param>
        public DocumentService(ISqlSugarRepository<DocumentEntity> documentRepository, IFileService fileService, IUserManager userManager)
        {
            _documentRepository = documentRepository;
            _fileService = fileService;
            _userManager = userManager;
            db = documentRepository.Context;
        }

        #region Get
        /// <summary>
        /// 列表（文件夹树）
        /// </summary>
        /// <returns></returns>
        [HttpGet("FolderTree/{id}")]
        public async Task<dynamic> GetFolderTree(string id)
        {
            var data = (await _documentRepository.Entities.Where(x => x.CreatorUserId == _userManager.UserId && x.Type == 0 && x.DeleteMark == 0).ToListAsync()).Adapt<List<DocumentFolderTreeOutput>>();
            data.Add(new DocumentFolderTreeOutput
            {
                id = "0",
                fullName = "全部文档",
                parentId = "-1",
                icon = "fa fa-folder",
            });
            if (!id.Equals("0"))
            {
                data.RemoveAll(x => x.id == id);
            }
            var treeList = data.ToTree("-1");
            return new { list = treeList };
        }

        /// <summary>
        /// 列表（全部文档）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <param name="parentId">文档层级</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetAllList([FromQuery] KeywordInput input, string parentId)
        {
            var data = (await _documentRepository.Entities.Where(m => m.CreatorUserId == _userManager.UserId && m.ParentId == parentId && m.DeleteMark == 0).WhereIF(input.keyword.IsNotEmptyOrNull(), t => t.FullName.Contains(input.keyword)).OrderBy(x=>x.CreatorTime,OrderByType.Desc).ToListAsync()).Adapt<List<DocumentListOutput>>();
            return new { list = data };
        }

        /// <summary>
        /// 列表（我的分享）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("Share")]
        public async Task<dynamic> GetShareOutList([FromQuery] KeywordInput input)
        {
            var data = (await _documentRepository.Entities.Where(m => m.CreatorUserId == _userManager.UserId && m.IsShare > 0 && m.DeleteMark == 0).WhereIF(input.keyword.IsNotEmptyOrNull(), t => t.FullName.Contains(input.keyword)).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToListAsync()).Adapt<List<DocumentShareOutput>>();
            return new { list = data };
        }

        /// <summary>
        /// 列表（共享给我）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("ShareTome")]
        public async Task<dynamic> GetShareTomeList([FromQuery] KeywordInput input)
        {
            var output = await db.Queryable<DocumentEntity, DocumentShareEntity, UserEntity>((a, b, c) => new JoinQueryInfos(JoinType.Left, a.Id == b.DocumentId, JoinType.Left, a.CreatorUserId == c.Id)).Where((a, b, c) => a.DeleteMark == 0 && b.ShareUserId == _userManager.UserId).WhereIF(input.keyword.IsNotEmptyOrNull(), a => a.FullName.Contains(input.keyword)).Select((a, b, c) => new DocumentShareTomeOutput()
            {
                shareTime = a.ShareTime,
                fileSize = a.FileSize,
                fullName = a.FullName,
                id = a.Id,
                creatorUserId =SqlFunc.MergeString(c.RealName,"/", c.Account),
                fileExtension = a.FileExtension
            }).MergeTable().OrderBy(a => a.shareTime, OrderByType.Desc).ToListAsync();
            return new { list = output };
        }

        /// <summary>
        /// 列表（回收站）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("Trash")]
        public async Task<dynamic> GetTrashList([FromQuery] KeywordInput input)
        {
            var data = (await _documentRepository.Entities.Where(m => m.CreatorUserId == _userManager.UserId && m.DeleteMark == 1).WhereIF(input.keyword.IsNotEmptyOrNull(), t => t.FullName.Contains(input.keyword)).OrderBy(x=>x.CreatorTime,OrderByType.Desc).ToListAsync()).Adapt<List<DocumentTrashOutput>>();
            return new { list = data };
        }

        /// <summary>
        /// 列表（共享人员）
        /// </summary>
        /// <param name="documentId">文档主键</param>
        /// <returns></returns>
        [HttpGet("ShareUser/{documentId}")]
        public async Task<dynamic> GetShareUserList(string documentId)
        {
            var data = (await db.Queryable<DocumentShareEntity>().Where(x => x.DocumentId == documentId).OrderBy(x => x.ShareTime, OrderByType.Desc).ToListAsync()).Adapt<List<DocumentShareUserOutput>>();
            return new { list = data };
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var data = (await _documentRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == 0)).Adapt<DocumentInfoOutput>();
            data.fullName = data.fullName.Remove(data.fullName.LastIndexOf('.')); 
            return data;
        }
        #endregion

        #region Post
        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] DocumentCrInput input)
        {
            if (await _documentRepository.AnyAsync(x => x.FullName == input.fullName && x.Type == 0 && x.DeleteMark != 1))
                throw JNPFException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<DocumentEntity>();
            entity.DeleteMark = 0;
            var isOk = await _documentRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] DocumentUpInput input)
        {
            if (await _documentRepository.AnyAsync(x => x.Id != id && x.Type==input.type&& x.FullName == input.fullName && x.DeleteMark != 1))
                throw JNPFException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<DocumentEntity>();
            var isOk = await _documentRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _documentRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark !=1);
            if (entity == null)
                throw JNPFException.Oh(ErrorCode.COM1005);
            var isOk = await _documentRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        [HttpPost("Uploader")]
        public async Task Uploader([FromForm] DocumentUploaderInput input)
        {
            #region 上传图片
            if (await _documentRepository.AnyAsync(x => x.FullName == input.file.FileName && x.Type == 1 && x.DeleteMark !=1))
                throw JNPFException.Oh(ErrorCode.D8002);
            Thread.Sleep(1000);
            _fileService.UploadFile("document", input.file);
            #endregion

            #region 保存数据
            var entity = new DocumentEntity();
            entity.Type = 1;
            entity.FullName = input.file.FileName;
            entity.ParentId = input.parentId;
            entity.FileExtension = Path.GetExtension(input.file.FileName).Replace(".", "");
            entity.FilePath = _fileService.GetPathByType("document") + input.file.FileName;
            entity.FileSize = input.file.Length.ToString();
            entity.DeleteMark = 0;
            var isOk = await _documentRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.D8001);
            #endregion
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="id">主键值</param>
        [HttpPost("Download/{id}")]
        public async Task<dynamic> Download(string id)
        {
            var entity = await _documentRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == 0);
            if (entity == null)
                throw JNPFException.Oh(ErrorCode.D8000);
            var fileName = _userManager.UserId + "|" + entity.FilePath + "|document";
            var output = new
            {
                name = entity.FullName,
                url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "JNPF")
            };
            return output;
        }

        /// <summary>
        /// 回收站（彻底删除）
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpDelete("Trash/{id}")]
        public async Task TrashDelete(string id)
        {
            var list = await _documentRepository.Entities.Where(m =>m.ParentId==id&& m.CreatorUserId == _userManager.UserId && m.DeleteMark == 0).ToListAsync();
            foreach (var item in list)
            {
                if (item.Type == 1)
                {
                    FileHelper.DeleteFile(FileVariable.DocumentFilePath + item.FilePath);
                }
                var isOk = await _documentRepository.Context.Updateable(item).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
                if (isOk < 1)
                    throw JNPFException.Oh(ErrorCode.COM1002);
            }
            
        }

        /// <summary>
        /// 回收站（还原文件）
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPost("Trash/{id}/Actions/Recovery")]
        public async Task TrashRecovery(string id)
        {
            var entity = await _documentRepository.FirstOrDefaultAsync(x => x.Id == id);
            entity.DeleteMark = 0;
            entity.DeleteTime = null;
            entity.DeleteUserId = null;
            var isOk = await _documentRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 共享文件（创建）
        /// </summary>
        /// <param name="id">共享文件id</param>
        /// <param name="input">共享人</param>
        /// <returns></returns>
        [HttpPost("{id}/Actions/Share")]
        public async Task ShareCreate(string id, [FromBody] DocumentActionsShareInput input)
        {
            try
            {
                var userIds = input.userId.Split(",");
                List<DocumentShareEntity> documentShareEntityList = new List<DocumentShareEntity>();
                foreach (var item in userIds)
                {
                    documentShareEntityList.Add(new DocumentShareEntity
                    {
                        Id = YitIdHelper.NextId().ToString(),
                        DocumentId = id,
                        ShareUserId = item,
                        ShareTime = DateTime.Now,
                    });
                }
                var entity =await _documentRepository.FirstOrDefaultAsync(x => x.Id == id&&x.DeleteMark==0);
                entity.IsShare = documentShareEntityList.Count;
                entity.ShareTime = DateTime.Now;
                db.BeginTran();
                db.Deleteable<DocumentShareEntity>().Where(x => x.DocumentId == id).ExecuteCommand();
                db.Insertable(documentShareEntityList).ExecuteCommand();
                var isOk = await _documentRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
                if (isOk < 1)
                    throw JNPFException.Oh(ErrorCode.COM1001);
                db.CommitTran();
            }
            catch (Exception)
            {

                db.RollbackTran();
            }
        }

        /// <summary>
        /// 共享文件（取消）
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpDelete("{id}/Actions/Share")]
        public async Task ShareCancel(string id)
        {
            try
            {
                var entity = await _documentRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == 0);
                entity.IsShare = 0;
                entity.ShareTime = DateTime.Now;
                db.BeginTran();
                db.Deleteable<DocumentShareEntity>().Where(x => x.DocumentId == id).ExecuteCommand();
                var isOk = await _documentRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
                if (isOk < 1)
                    throw JNPFException.Oh(ErrorCode.COM1001);
                db.CommitTran();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 文件/夹移动到
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="toId">将要移动到Id</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/MoveTo/{toId}")]
        public async Task MoveTo(string id, string toId)
        {
            var entity = await _documentRepository.FirstOrDefaultAsync(x => x.Id == id);
            var entityTo = await _documentRepository.FirstOrDefaultAsync(x => x.Id == toId);
            if (id == toId && entity.Type == 0 && entityTo.Type == 0)
                throw JNPFException.Oh(ErrorCode.Ex0002);
            if (entityTo.IsNotEmptyOrNull()&&id == entityTo.ParentId&&entity.Type==0 && entityTo.Type == 0)
                throw JNPFException.Oh(ErrorCode.Ex0005);
            entity.ParentId = toId;
            var isOk = await _documentRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1001);
        }
        #endregion
    }
}
