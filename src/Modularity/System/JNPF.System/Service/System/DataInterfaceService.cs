using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Filter;
using JNPF.DataEncryption;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.JsonSerialization;
using JNPF.LinqBuilder;
using JNPF.RemoteRequest.Extensions;
using JNPF.System.Entitys.Dto.System.DataInterFace;
using JNPF.System.Entitys.Model.System.DataInterFace;
using JNPF.System.Entitys.Permission;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.Common;
using JNPF.System.Interfaces.System;
using JNPF.System.Service.System;
using JNPF.UnifyResult;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace JNPF.System.Core.Service.DataInterFace
{
    /// <summary>
    /// 数据接口
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "DataInterface", Order = 204)]
    [Route("api/system/[controller]")]
    public class DataInterfaceService : IDataInterfaceService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<DataInterfaceEntity> _dataInterfaceRepository;
        private readonly DataInterfaceLogService _dataInterfaceLogService;
        private readonly IDictionaryDataService _dictionaryDataService;
        private readonly IDbLinkService _dbLinkService;
        private readonly IDataBaseService _dataBaseService;
        private readonly IUserManager _userManager;
        private readonly IFileService _fileService;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataInterfaceRepository"></param>
        /// <param name="dataInterfaceLogService"></param>
        /// <param name="dictionaryDataService"></param>
        /// <param name="dbLinkService"></param>
        /// <param name="dataBaseService"></param>
        /// <param name="userManager"></param>
        /// <param name="fileService"></param>
        public DataInterfaceService(ISqlSugarRepository<DataInterfaceEntity> dataInterfaceRepository, DataInterfaceLogService dataInterfaceLogService, IDictionaryDataService dictionaryDataService, IDbLinkService dbLinkService, IDataBaseService dataBaseService, IUserManager userManager, IFileService fileService)
        {
            _dataInterfaceRepository = dataInterfaceRepository;
            _dataInterfaceLogService = dataInterfaceLogService;
            _dictionaryDataService = dictionaryDataService;
            _dbLinkService = dbLinkService;
            _dataBaseService = dataBaseService;
            _userManager = userManager;
            _fileService = fileService;
        }

        #region Get
        /// <summary>
        /// 获取接口列表(分页)
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList_Api([FromQuery] DataInterfaceListQuery input)
        {
            var pageInput = input.Adapt<PageInputBase>();
            var queryWhere = LinqExpression.And<DataInterfaceListOutput>();
            if (!string.IsNullOrEmpty(input.categoryId))
                queryWhere = queryWhere.And(m => m.categoryId == input.categoryId);
            //关键字（名称、编码）
            if (!string.IsNullOrEmpty(input.keyword))
                queryWhere = queryWhere.And(m => m.fullName.Contains(input.keyword) || m.enCode.Contains(input.keyword));
            var list = await _dataInterfaceRepository.Context.Queryable<DataInterfaceEntity, UserEntity>((a, b) =>
            new JoinQueryInfos(JoinType.Left, b.Id == a.CreatorUserId)).Where(a => a.DeleteMark == null).Select((a, b) =>
                new DataInterfaceListOutput
                {
                    id = a.Id,
                    categoryId = a.CategoryId,
                    creatorTime = a.CreatorTime,
                    creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account),
                    dataType = a.DataType,
                    dbLinkId = a.DBLinkId,
                    description = a.Description,
                    enCode = a.EnCode,
                    fullName = a.FullName,
                    enabledMark = a.EnabledMark,
                    path = a.Path,
                    query = a.Query,
                    requestMethod = a.RequestMethod,
                    requestParameters = a.RequestParameters,
                    responseType = a.ResponseType,
                    sortCode = a.SortCode,
                    checkType = a.CheckType
                }).MergeTable().Where(queryWhere).OrderBy(x => x.sortCode)
            .OrderBy(t => t.creatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            return PageResult<DataInterfaceListOutput>.SqlSugarPageResult(list);
        }

        /// <summary>
        /// 获取接口列表下拉框
        /// </summary>
        /// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> GetSelector()
        {
            List<DataInterfaceSelectorOutput> tree = new List<DataInterfaceSelectorOutput>();
            var data = (await GetList()).FindAll(x => x.EnabledMark == 1);
            foreach (var entity in data)
            {
                var dictionaryDataEntity = await _dictionaryDataService.GetInfo(entity.CategoryId);
                if (dictionaryDataEntity != null && tree.Where(t => t.id == entity.CategoryId).Count() == 0)
                {
                    DataInterfaceSelectorOutput firstModel = dictionaryDataEntity.Adapt<DataInterfaceSelectorOutput>();
                    firstModel.categoryId = "0";
                    DataInterfaceSelectorOutput treeModel = entity.Adapt<DataInterfaceSelectorOutput>();
                    treeModel.categoryId = "1";
                    treeModel.parentId = dictionaryDataEntity.Id;
                    firstModel.children.Add(treeModel);
                    tree.Add(firstModel);
                }
                else
                {
                    DataInterfaceSelectorOutput treeModel = entity.Adapt<DataInterfaceSelectorOutput>();
                    treeModel.categoryId = "1";
                    treeModel.parentId = entity.CategoryId;
                    var parent = tree.Where(t => t.id == entity.CategoryId).FirstOrDefault();
                    if (parent != null)
                    {
                        parent.children.Add(treeModel);
                    }
                }
            }
            return tree;
        }

        /// <summary>
        /// 获取接口数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            var data = (await GetInfo(id)).Adapt<DataInterfaceInfoOutput>();
            return data;
        }

        /// <summary>
        /// 访问接口
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}/Actions/Response")]
        public async Task<dynamic> ActionsResponse(string id)
        {
            var sw = new Stopwatch();
            sw.Start();

            var data = await GetInfo(id);
            object output = null;

            if (data.CheckType == 1)
            {
                var tokenStr= App.HttpContext.Request.Headers["Authorization"].ToString();
                if (tokenStr.IsNullOrEmpty())
                    throw JNPFException.Oh(ErrorCode.D9007);
                var token = new JsonWebToken(tokenStr.Replace("Bearer ", ""));
                var flag = JWTEncryption.ValidateJwtBearerToken((DefaultHttpContext)App.HttpContext, out token);
                if (!flag)
                    throw JNPFException.Oh(ErrorCode.D9007);
            }
            else if (data.CheckType == 2)
            {
                var ipList = data.RequestHeaders.Split(",").ToList();
                if (!ipList.Contains(App.HttpContext.GetLocalIpAddressToIPv4()))
                    throw JNPFException.Oh(ErrorCode.D9002);
            }

            if (1.Equals(data.DataType))
            {
                output = await GetData(data);
            }
            else if (2.Equals(data.DataType))
            {
                output = JSON.Deserialize<object>(data.Query);
            }
            else
            {
                output = await GetApiDataByType(data);
            }
            sw.Stop();
            await _dataInterfaceLogService.CreateLog(id, sw);
            return output;
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
        /// 添加接口
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create_Api([FromBody] DataInterfaceCrInput input)
        {
            var entity = input.Adapt<DataInterfaceEntity>();
            var isOk = await Create(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 修改接口
        /// </summary>
        /// <param name="id">主键id</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update_Api(string id, [FromBody] DataInterfaceUpInput input)
        {
            var entity = input.Adapt<DataInterfaceEntity>();
            var isOk = await Update(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除接口
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete_Api(string id)
        {
            var entity = await GetInfo(id);
            var isOk = await Delete(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 更新接口状态
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/State")]
        public async Task UpdateState_Api(string id)
        {
            var entity = await GetInfo(id);
            entity.EnabledMark = entity.EnabledMark == 1 ? 0 : 1;
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
            var data = josn.Deserialize<DataInterfaceEntity>();
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
        public async Task<List<DataInterfaceEntity>> GetList()
        {
            return await _dataInterfaceRepository.Entities.Where(x => x.DeleteMark == null).OrderBy(x => x.SortCode).ToListAsync();
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [NonAction]
        public async Task<DataInterfaceEntity> GetInfo(string id)
        {
            return await _dataInterfaceRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(DataInterfaceEntity entity)
        {
            return await _dataInterfaceRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(DataInterfaceEntity entity)
        {
            return await _dataInterfaceRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(DataInterfaceEntity entity)
        {
            return await _dataInterfaceRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<DataTable> GetData(DataInterfaceEntity entity)
        {
            var result = await connection(entity.DBLinkId, entity.Query, entity.CheckType);
            return result;
        }

        /// <summary>
        /// 查询(工作流)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<DataTable> GetData(string id)
        {
            var data = await _dataInterfaceRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
            var result = await connection(data.DBLinkId, data.Query,data.CheckType);
            return result;
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 通过连接执行sql
        /// </summary>
        /// <param name="dbLinkId"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        private async Task<DataTable> connection(string dbLinkId, string sql,int? checkType)
        {
            var linkEntity = await _dbLinkService.GetInfo(dbLinkId);
            var parameter = new List<SugarParameter>();
            if (checkType!=0&&_userManager.ToKen!=null)
            {
                parameter.Add(new SugarParameter("@user", _userManager.UserId));
                parameter.Add(new SugarParameter("@organize", _userManager.User.OrganizeId));
                parameter.Add(new SugarParameter("@department", _userManager.User.OrganizeId));
                parameter.Add(new SugarParameter("@postion", _userManager.User.PositionId));
            }
            var dt = _dataBaseService.GetInterFaceData(linkEntity, sql, parameter.ToArray());
            return dt;
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task ImportData(DataInterfaceEntity data)
        {
            try
            {
                _dataInterfaceRepository.Context.BeginTran();
                var stor = _dataInterfaceRepository.Context.Storageable(data).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await stor.AsInsertable.ExecuteCommandAsync(); //执行插入
                await stor.AsUpdateable.ExecuteCommandAsync(); //执行更新　
                _dataInterfaceRepository.Context.CommitTran();
            }
            catch (Exception ex)
            {
                _dataInterfaceRepository.Context.RollbackTran();
                throw JNPFException.Oh(ErrorCode.D3006);
            }
        }

        /// <summary>
        /// 根据不同规则请求接口
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private async Task<object> GetApiDataByType(DataInterfaceEntity entity)
        {
            var parameters = JSON.Deserialize<List<DataInterfaceReqParameter>>(entity.RequestParameters);
            var dic = new Dictionary<string, object>();
            foreach (var item in parameters)
            {
                dic.Add(item.field, item.value);
            }

            var result = await entity.Path.SetHeaders(new { Authorization = _userManager.ToKen }).SetQueries(dic).GetAsStringAsync();
            return JSON.Deserialize<RESTfulResult<object>>(result).data;

            //switch (entity.CheckType)
            //{
            //    case 1:
            //        var result2 = await entity.Path.SetHeaders(new { Authorization = _userManager.ToKen }).SetQueries(dic).GetAsStringAsync();
            //        return JSON.Deserialize<RESTfulResult<object>>(result2).data;
            //    case 2:
            //        var ipList = entity.RequestHeaders.Split(",").ToList();
            //        if (ipList.Contains(App.HttpContext.GetLocalIpAddressToIPv4()))
            //            throw JNPFException.Oh(ErrorCode.D9002);
            //        var result3 = await entity.Path.SetQueries(dic).GetAsStringAsync();
            //        return JSON.Deserialize<RESTfulResult<object>>(result3).data;
            //    default:
            //        var result1 = await entity.Path.SetQueries(dic).GetAsStringAsync();
            //        return JSON.Deserialize<RESTfulResult<object>>(result1).data;
            //}
        }
        #endregion
    }
}
