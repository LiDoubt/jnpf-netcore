using JNPF.Common.Const;
using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Filter;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.JsonSerialization;
using JNPF.LinqBuilder;
using JNPF.System.Entitys.Dto.System.BillRule;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.Common;
using JNPF.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.System.Service.System
{
    /// <summary>
    /// 单据规则
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "BillRule", Order = 200)]
    [Route("api/system/[controller]")]
    public class BillRuleService : IBillRullService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<BillRuleEntity> _billRuleRepository;
        private readonly ISysCacheService _sysCacheService;
        private readonly IUserManager _userManager; // 用户管理
        private readonly IFileService _fileService;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="billRuleRepository"></param>
        /// <param name="sysCacheService"></param>
        /// <param name="userManager"></param>
        /// <param name="fileService"></param>
        public BillRuleService(ISqlSugarRepository<BillRuleEntity> billRuleRepository, 
            ISysCacheService sysCacheService, 
            IUserManager userManager,
            IFileService fileService)
        {
            _billRuleRepository = billRuleRepository;
            _sysCacheService = sysCacheService;
            _userManager = userManager;
            _fileService = fileService;
        }

        #region Get
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> GetList_Api()
        {
            var data = (await GetList()).Adapt<List<BillRuleListOutput>>();
            return new { list = data };
        }

        /// <summary>
        /// 获取单据规则列表(带分页)
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList_Api([FromQuery] PageInputBase input)
        {
            var list = await _billRuleRepository.Entities
                .WhereIF(!string.IsNullOrEmpty(input.keyword), it => it.FullName.Contains(input.keyword) || it.EnCode.Contains(input.keyword))
                .Where(it => it.DeleteMark == null).OrderBy(x=>x.SortCode)
                .OrderBy(x => x.CreatorTime, OrderByType.Desc)
                .ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<BillRuleListOutput>()
            {
                list = list.list.Adapt<List<BillRuleListOutput>>(),
                pagination = list.pagination
            };
            return PageResult<BillRuleListOutput>.SqlSugarPageResult(pageList);

        }

        /// <summary>
        /// 获取单据流水号（工作流程调用）
        /// </summary>
        /// <param name="enCode">编码</param>
        /// <returns></returns>
        [HttpGet("BillNumber/{enCode}")]
        public async Task<dynamic> GetBillNumber_Api(string enCode)
        {
            var data = await GetBillNumber(enCode, true);
            return data;
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            return (await GetInfo(id)).Adapt<BillRuleInfoOutput>();
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Action/Export")]
        public async Task<dynamic> ActionsExport(string id)
        {
            var data = await GetInfo(id);
            var jsonStr = data.Serialize();
            return _fileService.Export(jsonStr, data.FullName);
        }
        #endregion

        #region Post
        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create_Api([FromBody] BillRuleCrInput input)
        {
            if (await _billRuleRepository.AnyAsync(x => x.EnCode == input.enCode && x.DeleteMark == null) || await _billRuleRepository.AnyAsync(x => x.FullName == input.fullName && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<BillRuleEntity>();
            var isOk = await Create(entity);
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
            var entity = await _billRuleRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
            if (entity == null)
                throw JNPFException.Oh(ErrorCode.COM1005);
            if (!string.IsNullOrEmpty(entity.OutputNumber))
                throw JNPFException.Oh(ErrorCode.BR0001);
            var isOk = await Delete(entity);
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
        public async Task Update_Api(string id, [FromBody] BillRuleUpInput input)
        {
            if (await _billRuleRepository.AnyAsync(x => x.Id != id && x.EnCode == input.enCode && x.DeleteMark == null) || await _billRuleRepository.AnyAsync(x => x.Id != id && x.FullName == input.fullName && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<BillRuleEntity>();
            var isOk = await Update(entity);
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
            var entity = await GetInfo(id);
            entity.EnabledMark = entity.EnabledMark == 0 ? 1 : 0;
            var isOk = await Update(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1003);

        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("Action/Import")]
        public async Task ActionsImport(IFormFile file)
        {
            var josn = _fileService.Import(file);
            var data = josn.Deserialize<BillRuleEntity>();
            if (data == null)
                throw JNPFException.Oh(ErrorCode.D3006);
            await ImportData(data);
        }
        #endregion

        #region PublicMethod
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<BillRuleEntity>> GetList()
        {
            return await _billRuleRepository.Entities.Where(x => x.DeleteMark == null && x.EnabledMark == 1).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToListAsync();
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<BillRuleEntity> GetInfo(string id)
        {
            return await _billRuleRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(BillRuleEntity entity)
        {
            return await _billRuleRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(BillRuleEntity entity)
        {
            return await _billRuleRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(BillRuleEntity entity)
        {
            return await _billRuleRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取单据流水号
        /// </summary>
        /// <param name="enCode"></param>
        /// <param name="isCache"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<string> GetBillNumber(string enCode, bool isCache = false)
        {
            try
            {
                var strNumber = "";
                if (isCache == true)
                {
                    var cacheKey = CommonConst.CACHE_KEY_BILLRULE + _userManager.TenantId + "_" + _userManager.UserId + enCode;
                    if (!_sysCacheService.Exists(cacheKey))
                    {
                        strNumber = await GetNumber(enCode);
                        _sysCacheService.Set(cacheKey, strNumber,new TimeSpan(0,3,0));
                    }
                    else
                    {
                        strNumber = _sysCacheService.Get(cacheKey);
                    }
                }
                else
                {
                    strNumber = await GetNumber(enCode);
                }
                return strNumber;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 使用单据流水号（注意：必须是缓存的单据才可以调用这个方法，否则无效）
        /// </summary>
        /// <param name="enCode">流水编码</param>
        [NonAction]
        public void UseBillNumber(string enCode)
        {
            var cacheKey = CommonConst.CACHE_KEY_BILLRULE + _userManager.TenantId + "_" + _userManager.UserId + enCode;
            _sysCacheService.Del(cacheKey);
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 获取流水号
        /// </summary>
        /// <param name="enCode"></param>
        /// <returns></returns>
        private async Task<string> GetNumber(string enCode)
        {
            StringBuilder strNumber = new StringBuilder();
            var entity = await _billRuleRepository.FirstOrDefaultAsync(m => m.EnCode == enCode && m.DeleteMark == null);
            if (entity != null)
            {
                //处理隔天流水号归0
                if (entity.OutputNumber != null)
                {
                    var serialDate = entity.OutputNumber.Remove(entity.OutputNumber.Length - (int)entity.Digit).Replace(entity.Prefix, "");
                    var thisDate = entity.DateFormat == "no" ? "" : DateTime.Now.ToString(entity.DateFormat);
                    if (serialDate != thisDate)
                    {
                        entity.ThisNumber = 0;
                    }
                    entity.ThisNumber = entity.ThisNumber + 1;
                }
                else
                {
                    entity.ThisNumber = 1;
                }
                //拼接单据编码
                strNumber.Append(entity.Prefix);                                                                  //前缀
                if (entity.DateFormat != "no")
                {
                    strNumber.Append(DateTime.Now.ToString(entity.DateFormat));                                  //日期格式
                }
                var number = int.Parse(entity.StartNumber) + entity.ThisNumber;
                strNumber.Append(number.ToString().PadLeft((int)entity.Digit, '0'));              //流水号

                //更新流水号
                entity.OutputNumber = strNumber.ToString();
                await Update(entity);
            }
            else
            {
                strNumber.Append("单据规则不存在");
            }
            return strNumber.ToString();
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task ImportData(BillRuleEntity data)
        {
            try
            {
                _billRuleRepository.Context.BeginTran();
                var stor = _billRuleRepository.Context.Storageable(data).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await stor.AsInsertable.ExecuteCommandAsync(); //执行插入
                await stor.AsUpdateable.ExecuteCommandAsync(); //执行更新　
                _billRuleRepository.Context.CommitTran();
            }
            catch (Exception ex)
            {
                _billRuleRepository.Context.RollbackTran();
                throw JNPFException.Oh(ErrorCode.D3006);
            }
        }
        #endregion


    }
}
