using JNPF.Common.Core.Manager;
using JNPF.Common.Extension;
using JNPF.Common.Filter;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.System.Entitys.Dto.System.DataInterfaceLog;
using JNPF.System.Entitys.Permission;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UAParser;
using Yitter.IdGenerator;

namespace JNPF.System.Service.System
{
    /// <summary>
    /// 数据接口日志
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "DataInterfaceLog", Order = 204)]
    [Route("api/system/[controller]")]
    public class DataInterfaceLogService: IDataInterfaceLogService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<DataInterfaceLogEntity> _dataInterfaceLogRepository;
        private readonly IUserManager _userManager;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataInterfaceLogRepository"></param>
        /// <param name="userManager"></param>
        public DataInterfaceLogService(ISqlSugarRepository<DataInterfaceLogEntity> dataInterfaceLogRepository, IUserManager userManager)
        {
            _dataInterfaceLogRepository = dataInterfaceLogRepository;
            _userManager = userManager;
        }

        #region Get
        [HttpGet("{id}")]
        public async Task<dynamic> GetList(string id,[FromQuery] PageInputBase input)
        {
            var list = await _dataInterfaceLogRepository.Context.Queryable<DataInterfaceLogEntity, UserEntity>((a, b) => 
            new JoinQueryInfos(JoinType.Left, b.Id == a.UserId)).Select((a, b) => 
            new DataInterfaceLogListOutput { id = a.Id, invokDevice = a.InvokDevice, invokIp = a.InvokIp, 
                userId = SqlFunc.MergeString(b.RealName, "/", b.Account), invokTime = a.InvokTime, invokType = a.InvokType, 
                invokWasteTime = a.InvokWasteTime,invokeId=a.InvokId}).MergeTable().
                Where(x=>x.invokeId==id).WhereIF(input.keyword.IsNotEmptyOrNull()
                ,m=> m.userId.Contains(input.keyword) || m.invokIp.Contains(input.keyword)).OrderBy(t => t.invokTime)
               .ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<DataInterfaceLogListOutput>.SqlSugarPageResult(list);
        }
        #endregion

        #region PublicMethod
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="id">接口id</param>
        /// <param name="sw">请求时间</param>
        /// <returns></returns>
        [NonAction]
        public async Task CreateLog(string id, Stopwatch sw)
        {
            var httpContext = App.HttpContext;
            var headers = httpContext.Request.Headers;
            var clientInfo = Parser.GetDefault().Parse(headers["User-Agent"]);
            var log = new DataInterfaceLogEntity()
            {
                Id = YitIdHelper.NextId().ToString(),
                InvokId = id,
                InvokTime = DateTime.Now,
                UserId = _userManager.UserId,
                InvokIp = httpContext.GetLocalIpAddressToIPv4(),
                InvokDevice = clientInfo.String,
                InvokWasteTime = (int)sw.ElapsedMilliseconds,
                InvokType = httpContext.Request.Method
            };
            await _dataInterfaceLogRepository.InsertAsync(log);
        } 
        #endregion
    }
}
