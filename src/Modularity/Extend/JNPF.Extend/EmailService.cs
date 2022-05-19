using JNPF.Common.Configuration;
using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.FileManage;
using JNPF.Common.Filter;
using JNPF.Common.Helper;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.Expand.Thirdparty.Email;
using JNPF.Expand.Thirdparty.Email.Model;
using JNPF.Extend.Entitys;
using JNPF.Extend.Entitys.Dto.Email;
using JNPF.Extend.Interfaces;
using JNPF.FriendlyException;
using JNPF.LinqBuilder;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace JNPF.Extend
{
    /// <summary>
    /// 邮件收发
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "Extend", Name = "Email", Order = 600)]
    [Route("api/extend/[controller]")]
    public class EmailService : IEmailService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<EmailReceiveEntity> _emailReceiveRepository;
        private readonly ISqlSugarRepository<EmailSendEntity> _emailSendRepository;
        private readonly ISqlSugarRepository<EmailConfigEntity> _emailConfigRepository;
        private readonly IUserManager _userManager;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailReceiveRepository"></param>
        /// <param name="emailSendRepository"></param>
        /// <param name="emailConfigRepository"></param>
        /// <param name="userManager"></param>
        public EmailService(ISqlSugarRepository<EmailReceiveEntity> emailReceiveRepository, ISqlSugarRepository<EmailSendEntity> emailSendRepository, ISqlSugarRepository<EmailConfigEntity> emailConfigRepository, IUserManager userManager)
        {
            _emailReceiveRepository = emailReceiveRepository;
            _emailSendRepository = emailSendRepository;
            _emailConfigRepository = emailConfigRepository;
            _userManager = userManager;
        }

        #region Get
        /// <summary>
        /// (带分页)获取邮件列表(收件箱、标星件、草稿箱、已发送)
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] EmailListQuery input)
        {
            switch (input.type)
            {
                case "inBox"://收件箱
                    return await GetReceiveList(input);
                case "star"://标星件
                    return await GetStarredList(input);
                case "draft"://草稿箱
                    return await GetDraftList(input);
                case "sent"://已发送
                    return await GetSentList(input);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 信息（收件/发件）
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            var output = new EmailInfoOutput();
            var data = await GetInfo(id);
            var jobj = JsonHelper.ToObject(data.ToJson());
            if (jobj.ContainsKey("Read"))
            {
                var entity = data.Adapt<EmailReceiveEntity>();
                output = entity.Adapt<EmailInfoOutput>();
                output.bodyText = HttpUtility.HtmlDecode(entity.BodyText);
            }
            else
            {
                var entity = data.Adapt<EmailSendEntity>();
                output = entity.Adapt<EmailInfoOutput>();
                output.bodyText = HttpUtility.HtmlDecode(entity.BodyText);
            }
            return output;
        }

        /// <summary>
        /// 信息（配置）
        /// </summary>
        /// <returns></returns>
        [HttpGet("Config")]
        public async Task<dynamic> GetConfigInfo_Api()
        {
            var data = (await GetConfigInfo()).Adapt<EmailConfigInfoOutput>();
            return data;
        }
        #endregion

        #region Post
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            try
            {
                DbScoped.SugarScope.BeginTran();
                var entity = await this.GetInfo(id);
                if (entity is EmailReceiveEntity)
                {
                    //删除邮件
                    var mailConfig = await this.GetConfigInfo();
                    var mailReceiveEntity = entity.Adapt<EmailReceiveEntity>();
                    Mail.Delete(new MailAccount { Account = mailConfig.Account, Password = mailConfig.Password, POP3Host = mailConfig.POP3Host, POP3Port = mailConfig.POP3Port.ToInt() }, mailReceiveEntity.MID);
                }
                //删除数据
                var isOk = 0;
                if (entity is EmailReceiveEntity)
                    isOk = await _emailReceiveRepository.Context.Updateable((EmailReceiveEntity)entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
                else
                    isOk = await _emailSendRepository.Context.Updateable((EmailSendEntity)entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
                if (isOk < 1)
                    throw JNPFException.Oh(ErrorCode.COM1002);
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.Sugar.RollbackTran();
                throw JNPFException.Oh(ex.Message);
            }
        }

        /// <summary>
        /// 设置已读邮件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/Read")]
        public async Task ReceiveRead(string id)
        {
            var entity = (await this.GetInfo(id)).Adapt<EmailReceiveEntity>();
            entity.Read = 1;
            var isOk = await _emailReceiveRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1008);
        }

        /// <summary>
        /// 设置未读邮件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/Unread")]
        public async Task ReceiveUnread(string id)
        {
            var entity = (await this.GetInfo(id)).Adapt<EmailReceiveEntity>();
            entity.Read = 0;
            var isOk = await _emailReceiveRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1008);
        }

        /// <summary>
        /// 设置星标邮件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/Star")]
        public async Task ReceiveYesStarred(string id)
        {
            var entity = (await this.GetInfo(id)).Adapt<EmailReceiveEntity>();
            entity.Starred = 1;
            var isOk = await _emailReceiveRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1008);
        }

        /// <summary>
        /// 设置取消星标
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/Unstar ")]
        public async Task ReceiveNoStarred(string id)
        {
            var entity = (await this.GetInfo(id)).Adapt<EmailReceiveEntity>();
            entity.Starred = 0;
            var isOk = await _emailReceiveRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1008);
        }

        /// <summary>
        /// 收邮件
        /// </summary>
        /// <returns></returns>
        [HttpPost("Receive")]
        public async Task<dynamic> Receive()
        {
            var mailConfig = await GetConfigInfo();
            if (mailConfig != null)
            {
                var mailAccount = mailConfig.Adapt<MailAccount>();
                if (Mail.CheckConnected(mailAccount))
                {
                    List<EmailReceiveEntity> entitys = new List<EmailReceiveEntity>();
                    var startTime = (DateTime.Now.ToString("yyyy-MM-dd") + " 00:00").ToDate();
                    var endTime = (DateTime.Now.ToString("yyyy-MM-dd") + " 23:59").ToDate();
                    var receiveCount = await _emailReceiveRepository.CountAsync(x => x.MAccount == mailConfig.Account && SqlFunc.Between(x.CreatorTime, startTime, endTime));
                    var mailList = Mail.Get(mailAccount, receiveCount);
                    foreach (var item in mailList)
                    {
                        entitys.Add(new EmailReceiveEntity
                        {
                            MAccount = mailConfig.Account,
                            MID = item.UID,
                            Sender = item.To,
                            SenderName = item.ToName,
                            Subject = item.Subject,
                            BodyText = HttpUtility.HtmlEncode(item.BodyText),
                            Attachment = item.Attachment.ToJson(),
                            Date = item.Date,
                            Read = 0
                        });
                    }
                    if (entitys.Count>0)
                    {
                        await _emailReceiveRepository.Context.Insertable(entitys).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
                    }
                    return entitys.Count;
                }
                else
                {
                    throw JNPFException.Oh(ErrorCode.Ex0003);
                }
            }
            else
            {
                throw JNPFException.Oh(ErrorCode.Ex0004);
            }
        }

        /// <summary>
        /// 存草稿
        /// </summary>
        /// <param name="input">对象实体</param>
        /// <returns></returns>
        [HttpPost("Actions/SaveDraft")]
        //[IgnoreCheckScript]
        public async Task SaveDraft([FromBody] EmailActionsSaveDraftInput input)
        {
            var entity = input.Adapt<EmailSendEntity>();
            entity.BodyText = HttpUtility.HtmlEncode(entity.BodyText);
            entity.To = input.recipient;
            entity.Sender = App.Configuration["JNPF_APP:ErrorReportTo"];
            var isOk = 0;
            entity.State = -1;
            if (entity.Id.IsEmpty())
            {
                isOk = await _emailSendRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
            }
            else
            {
                isOk = await _emailSendRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            }
            if(isOk<1)
                throw JNPFException.Oh(ErrorCode.COM1008);
        }

        /// <summary>
        /// 发邮件
        /// </summary>
        /// <param name="input">对象实体</param>
        /// <returns></returns>
        [HttpPost("")]
        //[IgnoreCheckScript]
        public async Task SaveSent([FromBody] EmailSendInput input)
        {
            var entity = input.Adapt<EmailSendEntity>();
            var mailConfig = await GetConfigInfo();
            if (!RegularHelper.Check(input.recipient, RegularHelper.Type.邮件))
                throw JNPFException.Oh(ErrorCode.Ex0003);
            if (mailConfig != null)
            {
                entity.BodyText = HttpUtility.HtmlEncode(entity.BodyText);
                entity.To = input.recipient;
                entity.Sender = App.Configuration["JNPF_APP:ErrorReportTo"];
                entity.State = 1;
                var isOk = 0;
                if (entity.Id.IsEmpty())
                {
                    entity.Sender = mailConfig.Account;
                    isOk = await _emailSendRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
                }
                else
                {
                    isOk = await _emailSendRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
                }
                //拷贝文件,注意：从临时文件夹拷贝到邮件文件夹
                var attachmentList = JsonHelper.ToList<MailFile>(entity.Attachment);
                var temporaryFile = FileVariable.TemporaryFilePath;
                var mailFilePath = FileVariable.EmailFilePath;
                foreach (MailFile mailFile in attachmentList)
                {
                    FileHelper.MoveFile(temporaryFile + mailFile.fileId, mailFilePath + mailFile.fileId);
                    mailFile.fileName = mailFile.name;
                }
                //发送邮件
                var mailModel = new MailModel();
                mailModel.To = entity.To;
                mailModel.CC = entity.CC;
                mailModel.Bcc = entity.BCC;
                mailModel.Subject = entity.Subject;
                mailModel.BodyText = HttpUtility.HtmlDecode(entity.BodyText);
                mailModel.Attachment = attachmentList;
                Mail.Send(new MailAccount { AccountName = mailConfig.SenderName, Account = mailConfig.Account, Password = mailConfig.Password, SMTPHost = mailConfig.SMTPHost, SMTPPort = mailConfig.SMTPPort.ToInt() }, mailModel);
                if (isOk < 1)
                    throw JNPFException.Oh(ErrorCode.COM1008);
            }
            else
            {
                throw JNPFException.Oh(ErrorCode.Ex0004);
            }
        }

        /// <summary>
        /// 保存邮箱配置
        /// </summary>
        /// <param name="input">对象实体</param>
        /// <returns></returns>
        [HttpPut("Config")]
        public async Task SaveConfig([FromBody] EmailConfigUpInput input)
        {
            var entity = input.Adapt<EmailConfigEntity>();
            var data = await GetConfigInfo();
            var isOk = 0;
            if (data == null)
            {
                isOk= await _emailConfigRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
            }
            else
            {
                entity.Id = data.Id;
                isOk= await _emailConfigRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            }
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1008);
        }

        /// <summary>
        /// 邮箱账户密码验证
        /// </summary>
        /// <param name="input">对象实体</param>
        /// <returns></returns>
        [HttpPost("Config/Actions/CheckMail")]
        public void CheckLogin([FromBody] EmailConfigActionsCheckMailInput input)
        {
            var entity = input.Adapt<EmailConfigEntity>();
            if (!Mail.CheckConnected(entity.Adapt<MailAccount>()))
                throw JNPFException.Oh(ErrorCode.Ex0003);
        }

        /// <summary>
        /// 下载附件
        /// </summary>
        /// <param name="fileModel">文件对象</param>
        [HttpPost("Download")]
        public void Download(FileModel fileModel)
        {
            var filePath = FileVariable.EmailFilePath + fileModel.FileId;
            if (FileHelper.Exists(filePath))
            {
                FileHelper.DownloadFile(filePath, fileModel.FileName);
            }
        }
        #endregion

        #region PublicMethod
        /// <summary>
        /// 门户未读邮件
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetUnreadList()
        {
            return (await _emailReceiveRepository.Entities.Where(x => x.Read == 0 && x.CreatorUserId == _userManager.UserId&&x.DeleteMark==null).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToListAsync()).Adapt<List<EmailHomeOutput>>();
        }

        /// <summary>
        /// 信息（配置）
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<EmailConfigEntity> GetConfigInfo()
        {
            return await _emailConfigRepository.FirstOrDefaultAsync(x => x.CreatorUserId == _userManager.UserId);
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 列表（收件箱）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<dynamic> GetReceiveList(EmailListQuery input)
        {
            var whereLambda = LinqExpression.And<EmailReceiveEntity>();
            whereLambda = whereLambda.And(x => x.CreatorUserId == _userManager.UserId && x.DeleteMark == null);
            var start = Ext.GetDateTime(input.startTime.ToString());
            var end = Ext.GetDateTime(input.endTime.ToString());
            if (input.endTime != null && input.startTime != null)
            {
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                whereLambda = whereLambda.And(x => SqlFunc.Between(x.Date, start, end));
            }
            if (!string.IsNullOrEmpty(input.keyword))
            {
                whereLambda = whereLambda.And(m => m.Sender.Contains(input.keyword) || m.Subject.Contains(input.keyword));
            }
            var list = await _emailReceiveRepository.Entities.Where(whereLambda).OrderBy(x => x.Date, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<EmailListOutput>()
            {
                list = list.list.Adapt<List<EmailListOutput>>(),
                pagination = list.pagination
            };
            return PageResult<EmailListOutput>.SqlSugarPageResult(pageList);
        }

        /// <summary>
        /// 列表（未读邮件）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<dynamic> GetUnreadList(EmailListQuery input)
        {
            var whereLambda = LinqExpression.And<EmailReceiveEntity>();
            whereLambda = whereLambda.And(x => x.CreatorUserId == _userManager.UserId && x.Read == 0 && x.DeleteMark == null);
            var start = Ext.GetDateTime(input.startTime.ToString());
            var end = Ext.GetDateTime(input.endTime.ToString());
            if (input.endTime != null && input.startTime != null)
            {
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                whereLambda = whereLambda.And(x => SqlFunc.Between(x.CreatorTime, start, end));
            }
            if (!string.IsNullOrEmpty(input.keyword))
            {
                whereLambda = whereLambda.And(m => m.Sender.Contains(input.keyword) || m.Subject.Contains(input.keyword));
            }
            var list = await _emailReceiveRepository.Entities.Where(whereLambda).OrderBy(x => x.Date, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<EmailListOutput>()
            {
                list = list.list.Adapt<List<EmailListOutput>>(),
                pagination = list.pagination
            };
            return PageResult<EmailListOutput>.SqlSugarPageResult(pageList);
        }

        /// <summary>
        /// 列表（星标件）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<dynamic> GetStarredList(EmailListQuery input)
        {
            var whereLambda = LinqExpression.And<EmailReceiveEntity>();
            whereLambda = whereLambda.And(x => x.CreatorUserId == _userManager.UserId && x.Starred == 1 && x.DeleteMark == null);
            var start = Ext.GetDateTime(input.startTime.ToString());
            var end = Ext.GetDateTime(input.endTime.ToString());
            if (input.endTime != null && input.startTime != null)
            {
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                whereLambda = whereLambda.And(x => SqlFunc.Between(x.Date, start, end));
            }
            //关键字（用户、IP地址、功能名称）
            if (!string.IsNullOrEmpty(input.keyword))
            {
                whereLambda = whereLambda.And(m => m.Sender.Contains(input.keyword) || m.Subject.Contains(input.keyword));
            }
            var list = await _emailReceiveRepository.Entities.Where(whereLambda).OrderBy(x => x.Date, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<EmailListOutput>()
            {
                list = list.list.Adapt<List<EmailListOutput>>(),
                pagination = list.pagination
            };
            return PageResult<EmailListOutput>.SqlSugarPageResult(pageList);
        }

        /// <summary>
        /// 列表（草稿箱）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<dynamic> GetDraftList(EmailListQuery input)
        {
            var whereLambda = LinqExpression.And<EmailSendEntity>();
            whereLambda = whereLambda.And(x => x.CreatorUserId == _userManager.UserId && x.State == -1 && x.DeleteMark == null);
            var start = Ext.GetDateTime(input.startTime.ToString());
            var end = Ext.GetDateTime(input.endTime.ToString());
            if (input.endTime != null && input.startTime != null)
            {
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                whereLambda = whereLambda.And(x => SqlFunc.Between(x.CreatorTime, start, end));
            }
            if (!string.IsNullOrEmpty(input.keyword))
            {
                whereLambda = whereLambda.And(m => m.Sender.Contains(input.keyword) || m.Subject.Contains(input.keyword));
            }
            var list = await _emailSendRepository.Entities.Where(whereLambda).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<EmailListOutput>()
            {
                list = list.list.Adapt<List<EmailListOutput>>(),
                pagination = list.pagination
            };
            return PageResult<EmailListOutput>.SqlSugarPageResult(pageList);
        }

        /// <summary>
        /// 列表（已发送）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<dynamic> GetSentList(EmailListQuery input)
        {
            var whereLambda = LinqExpression.And<EmailSendEntity>();
            whereLambda = whereLambda.And(x => x.CreatorUserId == _userManager.UserId && x.State != -1 && x.DeleteMark == null);
            var start = Ext.GetDateTime(input.startTime.ToString());
            var end = Ext.GetDateTime(input.endTime.ToString());
            if (input.endTime != null && input.startTime != null)
            {
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                whereLambda = whereLambda.And(x => SqlFunc.Between(x.CreatorTime, start, end));
            }
            if (!string.IsNullOrEmpty(input.keyword))
            {
                whereLambda = whereLambda.And(m => m.Sender.Contains(input.keyword) || m.Subject.Contains(input.keyword));
            }
            var list = await _emailSendRepository.Entities.Where(whereLambda).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<EmailListOutput>()
            {
                list = list.list.Adapt<List<EmailListOutput>>(),
                pagination = list.pagination
            };
            return PageResult<EmailListOutput>.SqlSugarPageResult(pageList);
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<object> GetInfo(string id)
        {
            var entity = new object();
            if (_emailReceiveRepository.Any(x=>x.Id==id&&x.DeleteMark==null))
            {
                var receiveInfo = await _emailReceiveRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
                receiveInfo.Read = 1;
                await _emailReceiveRepository.Context.Updateable(receiveInfo).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
                entity = receiveInfo;
            }
            else
            {
                entity = await _emailSendRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
            }
            return entity;
        }
        #endregion
    }
}
