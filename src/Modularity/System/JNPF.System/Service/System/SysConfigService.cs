using JNPF.Common.Enum;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.Expand.Thirdparty;
using JNPF.Expand.Thirdparty.Email;
using JNPF.Expand.Thirdparty.Email.Model;
using JNPF.FriendlyException;
using JNPF.JsonSerialization;
using JNPF.System.Entitys.Dto.System.SysConfig;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace JNPF.System.Service.System
{
    /// <summary>
    /// 系统配置
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "SysConfig", Order = 211)]
    [Route("api/system/[controller]")]
    public class SysConfigService : ISysConfigService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<SysConfigEntity> _sysConfigRepository;

        /// <summary>
        /// 初始化一个<see cref="SysConfigService"/>类型的新实例
        /// </summary>
        /// <param name="sysConfigRepository"></param>
        public SysConfigService(ISqlSugarRepository<SysConfigEntity> sysConfigRepository)
        {
            _sysConfigRepository = sysConfigRepository;
        }

        #region GET

        /// <summary>
        /// 获取系统配置
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<SysConfigOutput> GetInfo()
        {
            var array = new Dictionary<string, string>();
            var data = await _sysConfigRepository.Entities.Where(x => x.Category.Equals("SysConfig")).ToListAsync();
            foreach (var item in data)
            {
                array.Add(item.Key, item.Value);
            }
            var output = array.Serialize().Deserialize<SysConfigOutput>();
            return output;
        }

        #endregion

        #region Post

        /// <summary>
        /// 邮箱链接测试
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Email/Test")]
        public void EmailTest([FromBody] SysConfigEmailTestInput input)
        {
            var mailAccount = input.Adapt<MailAccount>();
            if ("1".Equals(input.ssl))
            {
                mailAccount.Ssl = true;
            }
            else
            {
                mailAccount.Ssl = false;
            }
            var result = Mail.CheckConnected(mailAccount);
            if (!result)
                throw JNPFException.Oh(ErrorCode.D9003);
        }

        /// <summary>
        /// 钉钉链接测试
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("testDingTalkConnect")]
        public void testDingTalkConnect([FromBody] SysConfigDingTestInput input)
        {
            var dingUtil = new Ding(input.dingSynAppKey, input.dingSynAppSecret);
            if (string.IsNullOrEmpty(dingUtil.token))
                throw JNPFException.Oh(ErrorCode.D9003);
        }

        /// <summary>
        /// 企业微信链接测试
        /// </summary>
        /// <param name="type"></param>
        /// <param name="input"></param>
        [HttpPost("{type}/testQyWebChatConnect")]
        public void testQyWebChatConnect(int type, [FromBody] SysConfigWeChatTestInput input)
        {
            var weChatUtil = new WeChat(input.qyhCorpId, input.qyhCorpSecret);
            if (string.IsNullOrEmpty(weChatUtil.accessToken))
                throw JNPFException.Oh(ErrorCode.D9003);
        }

        /// <summary>
        /// 更新系统配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task Update([FromBody] SysConfigUpInput input)
        {
            var configDic = input.Adapt<Dictionary<string, object>>();
            var entitys = new List<SysConfigEntity>();
            foreach (var item in configDic.Keys)
            {
                if (configDic[item] != null)
                {
                    SysConfigEntity sysConfigEntity = new SysConfigEntity();
                    sysConfigEntity.Id = YitIdHelper.NextId().ToString();
                    sysConfigEntity.Key = item;
                    sysConfigEntity.Value = configDic[item].ToString();
                    sysConfigEntity.Category = "SysConfig";
                    entitys.Add(sysConfigEntity);
                }
            }
            await Save(entitys, "SysConfig");
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 系统配置信息
        /// </summary>
        /// <param name="category">分类</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        [NonAction]
        public async Task<SysConfigEntity> GetInfo(string category, string key)
        {
            return await _sysConfigRepository.FirstOrDefaultAsync(s => s.Category == category && s.Key == key);
        }

        #endregion

        #region PrivateMethod

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        private async Task Save(List<SysConfigEntity> entitys, string category)
        {
            try
            {
                _sysConfigRepository.Context.Ado.BeginTran();
                await _sysConfigRepository.DeleteAsync(x => x.Category.Equals(category));
                await _sysConfigRepository.InsertAsync(entitys);
                _sysConfigRepository.Context.Ado.CommitTran();
            }
            catch (Exception)
            {
                _sysConfigRepository.Context.Ado.RollbackTran();
            }
        }

        #endregion
    }
}
