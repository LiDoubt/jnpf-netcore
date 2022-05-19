using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Filter;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.LinqBuilder;
using JNPF.System.Entitys.Dto.System.SysLog;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.System.Service.System
{
    /// <summary>
    /// 系统日志
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "Log", Order = 211)]
    [Route("api/system/[controller]")]
    public class SysLogService : ISysLogService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<SysLogEntity> _sysLogRepository;

        /// <summary>
        /// 初始化一个<see cref="SysLogService"/>类型的新实例
        /// </summary>
        /// <param name="sysLogRepository"></param>
        public SysLogService(ISqlSugarRepository<SysLogEntity> sysLogRepository)
        {
            _sysLogRepository = sysLogRepository;
        }

        #region GET

        /// <summary>
        /// 获取系统日志列表-登录日志（带分页）
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <param name="Type">分类</param>
        /// <returns></returns>
        [HttpGet("{Type}")]
        public async Task<dynamic> GetList([FromQuery] LogListQuery input, int Type)
        {
            var whereLambda = LinqExpression.And<SysLogEntity>();
            whereLambda = whereLambda.And(x => x.Category == Type);
            var start = Ext.GetDateTime(input.startTime.ToString());
            var end = Ext.GetDateTime(input.endTime.ToString());
            if (input.endTime != null && input.startTime != null)
            {
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                whereLambda = whereLambda.And(x => SqlFunc.Between(x.CreatorTime, start, end));
            }
            //关键字（用户、IP地址、功能名称）
            if (!string.IsNullOrEmpty(input.keyword))
            {
                whereLambda = whereLambda.And(m => m.UserName.Contains(input.keyword) || m.IPAddress.Contains(input.keyword) || m.ModuleName.Contains(input.keyword));
            }
            var list = await _sysLogRepository.Entities.Where(whereLambda).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            object output = null;
            if (Type == 1)
            {
                var pageList = new SqlSugarPagedList<LogLoginOutput>()
                {
                    list = list.list.Adapt<List<LogLoginOutput>>(),
                    pagination = list.pagination
                };
                return PageResult<LogLoginOutput>.SqlSugarPageResult(pageList);
            }
            if (Type == 4)
            {
                var pageList = new SqlSugarPagedList<LogExceptionOutput>()
                {
                    list = list.list.Adapt<List<LogExceptionOutput>>(),
                    pagination = list.pagination
                };
                return PageResult<LogExceptionOutput>.SqlSugarPageResult(pageList);
            }
            if (Type == 5)
            {
                var pageList = new SqlSugarPagedList<LogRequestOutput>()
                {
                    list = list.list.Adapt<List<LogRequestOutput>>(),
                    pagination = list.pagination
                };
                return PageResult<LogRequestOutput>.SqlSugarPageResult(pageList);
            }
            return output;
        }

        #endregion

        #region POST

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task Delete([FromBody] LogDelInput input)
        {
            try
            {
                //开启事务
                _sysLogRepository.Ado.BeginTran();

                await _sysLogRepository.DeleteAsync(input.ids);

                _sysLogRepository.Ado.CommitTran();
            }
            catch (Exception)
            {
                _sysLogRepository.Ado.RollbackTran();
                throw JNPFException.Oh(ErrorCode.COM1001);
            }
        }

        #endregion
    }
}
