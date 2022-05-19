using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using JNPF.Dependency;
using JNPF.Expand.Thirdparty.DingDing;
using Mapster;
using System;

namespace JNPF.Expand.Thirdparty
{
    /// <summary>
    /// 钉钉
    /// </summary>
    [SuppressSniffer]
    public class Ding
    {
        public string token { get; private set; }

        public Ding(string appKey, string appSecret)
        {
            token = GetDingToken(appKey, appSecret);
        }

        /// <summary>
        /// 钉钉token
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="appsecret"></param>
        /// <returns></returns>
        public string GetDingToken(string appKey, string appSecret)
        {
            try
            {
                var tokenurl = "https://oapi.dingtalk.com/gettoken";
                DefaultDingTalkClient client = new DefaultDingTalkClient(tokenurl);
                OapiGettokenRequest req = new OapiGettokenRequest();
                req.SetHttpMethod("GET");
                req.Appkey = appKey;
                req.Appsecret = appSecret;
                OapiGettokenResponse response = client.Execute(req);
                if (response.Errcode == 0)
                {
                    //过期时间
                    var timeout = DateTime.Now.Subtract(DateTime.Now.AddSeconds(response.ExpiresIn));
                    return response.AccessToken;
                }
                else
                {
                    throw new Exception("获取钉钉Token失败,失败原因:" + response.Errmsg);
                }

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 钉钉登录
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public object DingLogin(string code)
        {
            var url = "https://oapi.dingtalk.com/user/getuserinfo";
            DefaultDingTalkClient client = new DefaultDingTalkClient(url);
            OapiUserGetuserinfoRequest req = new OapiUserGetuserinfoRequest();
            req.Code = code;
            req.SetHttpMethod("GET");
            OapiUserGetuserinfoResponse rsp = client.Execute(req, token);
            if (rsp.IsError)
            {
                throw new Exception(rsp.ErrMsg);
            }
            return rsp.Body;
        }

        #region 用户

        /// <summary>
        /// 添加钉钉用户
        /// </summary>
        /// <param name="dingModel"></param>
        /// <returns></returns>
        public string CreateUser(DingUserModel dingModel, ref string msg)
        {
            try
            {
                var url = "https://oapi.dingtalk.com/topapi/v2/user/create";
                DefaultDingTalkClient client = new DefaultDingTalkClient(url);
                OapiV2UserCreateRequest req = dingModel.Adapt<OapiV2UserCreateRequest>();
                OapiV2UserCreateResponse rsp = client.Execute(req, token);
                if (rsp.Errcode == 0)
                {
                    return rsp.Result.Userid;
                }
                else
                {
                    msg = rsp.Errmsg;
                    return "";
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 修改钉钉用户
        /// </summary>
        /// <param name="dingModel"></param>
        public bool UpdateUser(DingUserModel dingModel, ref string msg)
        {
            try
            {
                var url = "https://oapi.dingtalk.com/topapi/v2/user/update";
                DefaultDingTalkClient client = new DefaultDingTalkClient(url);
                OapiV2UserUpdateRequest req = dingModel.Adapt<OapiV2UserUpdateRequest>();
                OapiV2UserUpdateResponse rsp = client.Execute(req, token);
                if (rsp.Errcode != 0)
                {
                    msg = rsp.Errmsg;
                }
                return rsp.Errcode == 0;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 修改钉钉用户
        /// </summary>
        /// <param name="dingModel"></param>
        public void DeleteUser(DingUserModel dingModel, ref string msg)
        {
            try
            {
                var url = "https://oapi.dingtalk.com/topapi/v2/user/delete";
                DefaultDingTalkClient client = new DefaultDingTalkClient(url);
                OapiV2UserDeleteRequest req = dingModel.Adapt<OapiV2UserDeleteRequest>();
                OapiV2UserDeleteResponse rsp = client.Execute(req, token);
                if (rsp.Errcode != 0)
                {
                    msg = rsp.Errmsg;
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region 部门
        /// <summary>
        /// 添加钉钉部门
        /// </summary>
        /// <param name="dingModel"></param>
        /// <returns></returns>
        public string CreateDep(DingDepModel dingModel, ref string msg)
        {
            try
            {
                var url = "https://oapi.dingtalk.com/topapi/v2/department/create";
                DefaultDingTalkClient client = new DefaultDingTalkClient(url);
                OapiV2DepartmentCreateRequest req = dingModel.Adapt<OapiV2DepartmentCreateRequest>();
                OapiV2DepartmentCreateResponse rsp = client.Execute(req, token);
                if (rsp.Errcode==0)
                {
                    if (!rsp.IsError)
                    {
                        return rsp.Result.DeptId.ToString();
                    }
                    else
                    {
                        msg = rsp.Errmsg;
                        return null;
                    }
                }
                else
                {
                    msg = rsp.Errmsg;
                    return null;
                }
                
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 修改钉钉部门
        /// </summary>
        /// <param name="dingModel"></param>
        public bool UpdateDep(DingDepModel dingModel, ref string msg)
        {
            try
            {
                var url = "https://oapi.dingtalk.com/topapi/v2/department/update";
                DefaultDingTalkClient client = new DefaultDingTalkClient(url);
                OapiV2DepartmentUpdateRequest req = dingModel.Adapt<OapiV2DepartmentUpdateRequest>();
                OapiV2DepartmentUpdateResponse rsp = client.Execute(req, token);
                if (rsp.Errcode == 0)
                {
                    msg = rsp.IsError?rsp.Errmsg:"";
                    return !rsp.IsError;
                }
                else
                {
                    msg = rsp.Errmsg;
                    return false;
                }
                
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// 删除钉钉部门
        /// </summary>
        /// <param name="dingModel"></param>
        public bool DeleteDep(DingDepModel dingModel, ref string msg)
        {
            try
            {
                var url = "https://oapi.dingtalk.com/topapi/v2/department/delete";
                DefaultDingTalkClient client = new DefaultDingTalkClient(url);
                OapiV2DepartmentDeleteRequest req = dingModel.Adapt<OapiV2DepartmentDeleteRequest>();
                OapiV2DepartmentDeleteResponse rsp = client.Execute(req, token);
                if (rsp.Errcode == 0)
                {
                    msg = rsp.IsError ? rsp.Errmsg : "";
                    return !rsp.IsError;
                }
                else
                {
                    msg = rsp.Errmsg;
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// 发送工作消息
        /// </summary>
        /// <param name="dingModel"></param>
        public void SendWorkMsg(DingWorkMsgModel dingModel)
        {
            try
            {
                var url = "https://oapi.dingtalk.com/topapi/message/corpconversation/asyncsend_v2";
                DefaultDingTalkClient client = new DefaultDingTalkClient(url);
                OapiMessageCorpconversationAsyncsendV2Request request = new OapiMessageCorpconversationAsyncsendV2Request()
                {
                    AgentId = long.Parse(dingModel.agentId),
                    UseridList = dingModel.toUsers,
                    Msg = dingModel.msg
                };
                request.SetHttpMethod("POST");
                OapiMessageCorpconversationAsyncsendV2Response response = client.Execute(request, token);
                if (response.IsError)
                {
                    throw new Exception(response.ErrMsg);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
