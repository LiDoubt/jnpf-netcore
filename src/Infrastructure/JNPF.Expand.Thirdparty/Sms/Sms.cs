using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Profile;
using JNPF.Dependency;
using JNPF.Expand.Thirdparty.Sms.Model;
using Newtonsoft.Json;
using System;
using System.Text;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Sms.V20210111;
using TencentCloud.Sms.V20210111.Models;

namespace JNPF.Expand.Thirdparty.Sms
{
    /// <summary>
    /// 短信帮助
    /// 版 本：V3.0.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// </summary>
    [SuppressSniffer]
    public class Sms
    {
        #region 阿里云
        /// <summary>
        /// 发送（阿里云短信）
        /// </summary>
        /// <param name="smsModel"></param>
        /// <returns></returns>
        public static string SendSmsByAli(SmsModel smsModel)
        {
            try
            {
                IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", smsModel.keyId, smsModel.keySecret);
                DefaultAcsClient client = new DefaultAcsClient(profile);
                CommonRequest request = new CommonRequest();
                request.Method = MethodType.POST;
                request.Domain = "dysmsapi.aliyuncs.com";
                request.Version = "2017-05-25";
                request.Action = "SendSms";
                request.AddQueryParameters("PhoneNumbers", smsModel.mobileAli);
                request.AddQueryParameters("SignName", smsModel.signName);
                request.AddQueryParameters("TemplateCode", smsModel.templateId);
                request.AddQueryParameters("TemplateParam", smsModel.templateParamAli);

                CommonResponse response = client.GetCommonResponse(request);
                return MessageHandle(Encoding.Default.GetString(response.HttpResponse.Content));
            }
            catch (Exception ex)
            {
                return "短信发送失败";
            }
        }
        /// <summary>
        /// 消息处理机制
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string MessageHandle(string str)
        {
            SmsMessageModel message = JsonConvert.DeserializeObject<SmsMessageModel>(str);
            string result = "";
            switch (message.Code)
            {
                case "OK":
                    //短信发送成功
                    result = "OK";
                    break;
                case "isp.RAM_PERMISSION_DENY":
                    result = "RAM权限DENY";
                    break;
                case "isv.OUT_OF_SERVICE":
                    result = "业务停机";
                    break;
                case "isv.PRODUCT_UN_SUBSCRIPT":
                    result = "未开通云通信产品的阿里云客户";
                    break;
                case "isv.PRODUCT_UNSUBSCRIBE":
                    result = "产品未开通";
                    break;
                case "isv.ACCOUNT_NOT_EXISTS":
                    result = "账户不存在";
                    break;
                case "isv.ACCOUNT_ABNORMAL":
                    result = "账户异常    ";
                    break;
                case "isv.SMS_TEMPLATE_ILLEGAL":
                    result = "短信模板不合法";
                    break;
                case "isv.SMS_SIGNATURE_ILLEGAL":
                    result = "短信签名不合法";
                    break;
                case "isv.INVALID_PARAMETERS":
                    result = "参数异常";
                    break;
                case "isv.MOBILE_NUMBER_ILLEGAL":
                    result = "非法手机号";
                    break;
                case "isv.MOBILE_COUNT_OVER_LIMIT":
                    result = "手机号码数量超过限制";
                    break;
                case "isv.TEMPLATE_MISSING_PARAMETERS":
                    result = "模板缺少变量";
                    break;
                case "isv.BUSINESS_LIMIT_CONTROL":
                    result = "业务限流";
                    break;
                case "isv.INVALID_JSON_PARAM":
                    result = "JSON参数不合法，只接受字符串值";
                    break;
                case "isv.PARAM_LENGTH_LIMIT":
                    result = "参数超出长度限制";
                    break;
                case "isv.PARAM_NOT_SUPPORT_URL":
                    result = "不支持URL";
                    break;
                case "isv.AMOUNT_NOT_ENOUGH":
                    result = "账户余额不足";
                    break;
                case "isv.TEMPLATE_PARAMS_ILLEGAL":
                    result = "模板变量里包含非法关键字";
                    break;
            }
            return result;
        }
        #endregion

        #region 腾讯云
        /// <summary>
        /// 腾讯云短信
        /// </summary>
        /// <param name="secretId">secretId</param>
        /// <param name="secretKey">secretKey</param>
        /// <param name="region">地域(华北地区:ap-beijing,华南地区:ap-guangzhou,华东地区:ap-nanjing)</param>
        /// <param name="signName">签名</param>
        /// <param name="mobile">手机号(国内+86)</param>
        /// <param name="smsSdkAppId">smsSdkAppId</param>
        /// <param name="templateId">模板id</param>
        /// <param name="templateParam">模板参数</param>
        /// <returns></returns>
        public static string SendSmsByTencent(SmsModel smsModel)
        {
            try
            {
                Credential cred = new Credential
                {
                    SecretId = smsModel.keyId,
                    SecretKey = smsModel.keySecret
                };

                ClientProfile clientProfile = new ClientProfile();
                HttpProfile httpProfile = new HttpProfile();
                httpProfile.Endpoint = ("sms.tencentcloudapi.com");
                clientProfile.HttpProfile = httpProfile;

                SmsClient client = new SmsClient(cred, smsModel.region, clientProfile);
                SendSmsRequest req = new SendSmsRequest();
                req.PhoneNumberSet = smsModel.mobileTx;
                req.SmsSdkAppId = smsModel.appId;
                req.SignName = smsModel.signName;
                req.TemplateId = smsModel.templateId;
                req.TemplateParamSet = smsModel.templateParamTx;
                SendSmsResponse resp = client.SendSmsSync(req);
                return resp.RequestId;
            }
            catch (Exception ex)
            {
                return "短信发送失败";
            }
        }
        #endregion
    }
}
