using JNPF.Common.Configuration;
using JNPF.Common.Extension;
using JNPF.Common.Helper;
using JNPF.Dependency;
using JNPF.Expand.Thirdparty.Email.Model;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Yitter.IdGenerator;

namespace JNPF.Expand.Thirdparty.Email
{
    /// <summary>
    /// 基于MailKit的邮件帮助类
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// </summary>
    [SuppressSniffer]
    public class Mail
    {
        private static string mailFilePath = FileVariable.EmailFilePath;

        /// <summary>
        /// 发送：协议Smtp
        /// </summary>
        /// <param name="mailConfig">配置</param>
        /// <param name="mailModel">信息</param>
        public static void Send(MailAccount mailConfig, MailModel mailModel)
        {
            MimeMessage message = new MimeMessage();
            //发送方信息
            message.From.AddRange(new MailboxAddress[] { new MailboxAddress(mailConfig.AccountName, mailConfig.Account) });
            //发件人
            if (!string.IsNullOrEmpty(mailModel.To))
            {
                List<MailboxAddress> toAddress = new List<MailboxAddress>();
                var toArray = mailModel.To.Split(',');
                foreach (var item in toArray)
                {
                    toAddress.Add(new MailboxAddress(item));
                }
                message.To.AddRange(toAddress.ToArray());
            }
            //抄送人
            if (!string.IsNullOrEmpty(mailModel.CC))
            {
                List<MailboxAddress> ccAddress = new List<MailboxAddress>();
                var ccArray = mailModel.CC.Split(',');
                foreach (var item in ccArray)
                {
                    ccAddress.Add(new MailboxAddress(item));
                }
                message.Cc.AddRange(ccAddress.ToArray());
            }
            //密送人
            if (!string.IsNullOrEmpty(mailModel.Bcc))
            {
                List<MailboxAddress> bccAddress = new List<MailboxAddress>();
                var bccArray = mailModel.Bcc.Split(',');
                foreach (var item in bccArray)
                {
                    bccAddress.Add(new MailboxAddress(item));
                }
                message.Bcc.AddRange(bccAddress.ToArray());
            }
            message.Subject = mailModel.Subject;
            TextPart body = new TextPart(TextFormat.Html) { Text = mailModel.BodyText };
            MimeEntity entity = body;
            //附件
            if (mailModel.Attachment != null)
            {
                var mult = new Multipart("mixed") { body };
                foreach (var attachment in mailModel.Attachment)
                {
                    var file = new FileInfo(mailFilePath + attachment.fileId);
                    if (file.Exists)
                    {
                        var mimePart = new MimePart();
                        mimePart.Content = new MimeContent(file.OpenRead());
                        mimePart.ContentDisposition = new ContentDisposition(ContentDisposition.Attachment);
                        mimePart.ContentTransferEncoding = ContentEncoding.Base64;
                        mimePart.FileName = attachment.fileName;
                        mult.Add(mimePart);
                    }
                }
                entity = mult;
            }
            message.Body = entity;
            message.Date = DateTime.Now;
            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(mailConfig.SMTPHost, mailConfig.SMTPPort, mailConfig.Ssl);
                client.Authenticate(mailConfig.Account, mailConfig.Password);
                client.Send(message);
                client.Disconnect(true);
            }
        }

        /// <summary>
        /// 获取：协议Pop3
        /// </summary>
        /// <param name="mailConfig">配置</param>
        /// <param name="receiveCount">已收邮件数、注意：如果已收邮件数和邮件数量一致则不获取</param>
        /// <returns></returns>
        public static List<MailModel> Get(MailAccount mailConfig, int receiveCount)
        {
            var resultList = new List<MailModel>();
            using (var client = new Pop3Client())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(mailConfig.POP3Host, mailConfig.POP3Port, mailConfig.Ssl);
                client.Authenticate(mailConfig.Account, mailConfig.Password);
                if (receiveCount == client.Count)
                    return resultList;
                for (int i = client.Count - 1; receiveCount <= i; i--)
                {
                    var message = client.GetMessage(i);
                    var from = (MailboxAddress)message.From[0];
                    var attachment = message.Attachments;
                    resultList.Add(new MailModel()
                    {
                        UID = message.MessageId,
                        To = from.Address,
                        ToName = from.Name,
                        Subject = message.Subject,
                        BodyText = message.HtmlBody,
                        Attachment = GetEmailAttachments(attachment, message.MessageId),
                        Date = message.Date.ToDate(),
                    });
                }
                client.Disconnect(true);
            }
            return resultList;
        }

        /// <summary>
        /// 删除：协议Pop3
        /// </summary>
        /// <param name="mailConfig">配置</param>
        /// <param name="messageId">messageId</param>
        public static void Delete(MailAccount mailConfig, string messageId)
        {
            using (var client = new Pop3Client())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(mailConfig.POP3Host, mailConfig.POP3Port, mailConfig.Ssl);
                client.Authenticate(mailConfig.Account, mailConfig.Password);
                for (int i = 0; i < client.Count; i++)
                {
                    if (client.GetMessage(i).MessageId == messageId)
                    {
                        client.DeleteMessage(i);
                    }
                }
            }
        }

        /// <summary>
        /// 验证连接：协议Smtp、Pop3
        /// </summary>
        /// <param name="mailConfig">配置</param>
        /// <returns></returns>
        public static bool CheckConnected(MailAccount mailConfig)
        {
            try
            {
                if (!string.IsNullOrEmpty(mailConfig.SMTPHost))
                {
                    using (var client = new SmtpClient())
                    {
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.Connect(mailConfig.SMTPHost, mailConfig.SMTPPort, mailConfig.Ssl);
                        client.Authenticate(mailConfig.Account, mailConfig.Password);
                        client.Disconnect(true);
                        return true;
                    }
                }
                if (!string.IsNullOrEmpty(mailConfig.POP3Host))
                {
                    using (var client = new Pop3Client())
                    {
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.Connect(mailConfig.POP3Host, mailConfig.POP3Port, mailConfig.Ssl);
                        client.Authenticate(mailConfig.Account, mailConfig.Password);
                        client.Disconnect(true);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.Write("邮箱验证失败原因:" + ex + ",失败详情:" + ex.StackTrace);
                return false;
            }
        }

        #region Method
        private static List<MailFile> GetEmailAttachments(IEnumerable<MimeEntity> attachments, string messageId)
        {
            var resultList = new List<MailFile>();
            foreach (var attachment in attachments)
            {
                if (attachment.IsAttachment)
                {
                    var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;
                    var fileId = YitIdHelper.NextId().ToString() + "_" + fileName;
                    var filePath = mailFilePath + fileId;
                    using (var stream = File.Create(filePath))
                    {
                        if (attachment is MessagePart rfc822)
                        {
                            rfc822.Message.WriteTo(stream);
                        }
                        else
                        {
                            var part = (MimePart)attachment;
                            part.Content.DecodeTo(stream);
                        }
                    }
                    var mailFileInfo = new FileInfo(filePath);
                    resultList.Add(new MailFile { fileId = fileId, fileName = fileName, fileSize = FileHelper.ToFileSize(mailFileInfo.Length) });
                }
            }
            return resultList;
        }
        private static string ConvertToBase64(string inputStr, Encoding encoding)
        {
            return Convert.ToBase64String(encoding.GetBytes(inputStr));
        }
        private static string ConvertHeaderToBase64(string inputStr, Encoding encoding)
        {
            var encode = !string.IsNullOrEmpty(inputStr) && inputStr.Any(c => c > 127);
            if (encode)
            {
                return "=?" + encoding.WebName + "?B?" + ConvertToBase64(inputStr, encoding) + "?=";
            }
            return inputStr;
        }
        #endregion
    }
}
