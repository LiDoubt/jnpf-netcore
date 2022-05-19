using JNPF.Common.Enum;
using JNPF.Common.Filter;
using JNPF.DataEncryption;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.LinqBuilder;
using JNPF.Tenant.Entitys.Dto.Account;
using JNPF.Tenant.Entitys.Entity;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.Tenant
{
    /// <summary>
    /// 租户信息
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "Tenant", Name = "Account", Order = 600)]
    [Route("api/Tenant/[controller]")]
    public class AccountService : IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<AccountEntity> _accountRepository;

        public AccountService(ISqlSugarRepository<AccountEntity> accountRepository)
        {
            _accountRepository = accountRepository;
        }

        #region Get
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("getList")]
        public async Task<dynamic> GetList([FromQuery] PageInputBase input)
        {
            var list = await _accountRepository.Entities.Where(x => x.DeleteMark == null)
                .WhereIF(!input.keyword.IsNullOrEmpty(),m=> 
                m.RealName.Contains(input.keyword) || m.Account.Contains(input.keyword))
                .OrderBy(x => x.CreatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<AccountListOutput>()
            {
                list = list.list.Adapt<List<AccountListOutput>>(),
                pagination = list.pagination
            };
            return PageResult<AccountListOutput>.SqlSugarPageResult(pageList);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _accountRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null)).Adapt<AccountInfoOutput>();
            return output;
        }
        #endregion
        
        #region Post
        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create_Api([FromBody] AccountCrInput input)
        {
            if (await _accountRepository.AnyAsync(x => x.Account == input.account && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.Zh10002);
            var entity = input.Adapt<AccountEntity>();
            entity.Password = MD5Encryption.Encrypt(MD5Encryption.Encrypt("123456"));
            var isOk = await _accountRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete_Api(string id)
        {
            var entity = await _accountRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
            if (entity == null)
                throw JNPFException.Oh(ErrorCode.COM1005);
            var isOk = await _accountRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update_Api(string id, [FromBody] AccountUpInput input)
        {
            if (await _accountRepository.AnyAsync(x => x.Id != id && x.Account == input.account && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<AccountEntity>();
            var isOk = await _accountRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 修改单据规则状态
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/State")]
        public async Task ActionsState_Api(string id)
        {
            var entity = await _accountRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
            entity.EnabledMark = entity.EnabledMark == 0 ? 1 : 0;
            var isOk = await _accountRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1003);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPost("/ModifyPassword/{id}")]
        public async Task ModifyPassword(string id,[FromBody] AccountModifyPasswordInput input)
        {
            var entity = await _accountRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
            entity.Password = MD5Encryption.Encrypt(input.userPassword);
            var isOk = await _accountRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1003);
        }
        #endregion

    }
}
