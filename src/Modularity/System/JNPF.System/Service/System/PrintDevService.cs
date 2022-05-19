using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Filter;
using JNPF.Common.Util;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.JsonSerialization;
using JNPF.System.Entitys.Dto.System.PrintDev;
using JNPF.System.Entitys.Entity.System;
using JNPF.System.Entitys.Model.System.PrintDev;
using JNPF.System.Entitys.Permission;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.Common;
using JNPF.System.Interfaces.Permission;
using JNPF.System.Interfaces.System;
using JNPF.WorkFlow.Interfaces.FlowTask.Repository;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace JNPF.System.Service.System
{
    /// <summary>
    /// 打印模板配置
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "PrintDev", Order = 200)]
    [Route("api/system/[controller]")]
    public class PrintDevService : IPrintDevService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<PrintDevEntity> _printDevRepository;
        private readonly IDictionaryDataService _dictionaryDataService;
        private readonly IFileService _fileService;
        private readonly SqlSugarScope db;
        private readonly IDataBaseService _dataBaseService;
        private readonly IDbLinkService _dbLinkService;
        private readonly IUsersService _usersService;


        public PrintDevService(ISqlSugarRepository<PrintDevEntity> printDevRepository,
            IDictionaryDataService dictionaryDataService,
            IFileService fileService, IDataBaseService dataBaseService,
            IDbLinkService dbLinkService, IUsersService usersService)
        {
            _printDevRepository = printDevRepository;
            _dictionaryDataService = dictionaryDataService;
            _fileService = fileService;
            db = _printDevRepository.Context;
            _dataBaseService = dataBaseService;
            _dbLinkService = dbLinkService;
            _usersService = usersService;
        }

        #region Get
        /// <summary>
        /// 列表(分页)
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList_Api([FromQuery] KeywordInput input)
        {
            var list = await GetOutList();
            //数据库分类
            var dbTypeList = (await _dictionaryDataService.GetList("printdev")).FindAll(x => x.EnabledMark == 1);
            if (!string.IsNullOrEmpty(input.keyword))
            {
                list = list.FindAll(t => t.fullName.ToLower().Contains(input.keyword.ToLower()) || t.enCode.ToLower().Contains(input.keyword.ToLower()));
            }
            var result = new List<PrintDevListOutput>();
            foreach (var item in dbTypeList)
            {
                var index = list.FindAll(x => x.category.Equals(item.EnCode)).Count;
                if (index>0)
                {
                    result.Add(new PrintDevListOutput()
                    {
                        id = item.Id,
                        parentId = "0",
                        fullName = item.FullName,
                        num = index
                    });
                }
            }
            var treeList = result.Union(list).ToList();
            if (!string.IsNullOrEmpty(input.keyword))
            {
                treeList = treeList.TreeWhere(t => t.fullName.ToLower().Contains(input.keyword.ToLower()) || t.enCode.ToLower().Contains(input.keyword.ToLower()), t => t.id, t => t.parentId);
            }
            return new { list = treeList.ToTree() };
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> GetList_Api([FromQuery]string type)
        {
            var list = await GetOutList();
            if (type.IsNotEmptyOrNull())
                list = list.FindAll(x => x.type==type.ToInt()).ToList();
            //数据库分类
            var dbTypeList = (await _dictionaryDataService.GetList("printdev")).FindAll(x => x.EnabledMark == 1);
            var result = new List<PrintDevListOutput>();
            foreach (var item in dbTypeList)
            {
                var index = list.FindAll(x => x.category.Equals(item.EnCode)).Count;
                if (index>0)
                {
                    result.Add(new PrintDevListOutput()
                    {
                        id = item.Id,
                        parentId = "0",
                        fullName = item.FullName,
                        num = index
                    });
                }
            }
            var treeList = result.Union(list).ToList();
            return new { list = treeList.ToTree() };
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            return (await GetInfo(id)).Adapt<PrintDevInfoOutput>();
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Actions/Export")]
        public async Task<dynamic> ActionsExport(string id)
        {
            var importModel = await GetInfo(id);
            var jsonStr = importModel.Serialize();
            return _fileService.Export(jsonStr, importModel.FullName);
        }

        /// <summary>
        /// 表单字段
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Fields")]
        public async Task<dynamic> GetFields([FromBody] PrintDevFieldsQuery input)
        {
            var link = await _dbLinkService.GetInfo(input.dbLinkId);
            var parameter = new List<SugarParameter>()
                {
                    new SugarParameter("@formId",null)
                };
            var sqlList = input.sqlTemplate.ToList<PrintDevSqlModel>();
            var output = new Dictionary<string, object>();
            var index = 0;
            foreach (var item in sqlList)
            {
                var dataTable= _dataBaseService.GetPrintDevData(link, item.sql, parameter);
                var fieldModes = GetFieldModels(dataTable);
                if (index==0)
                    output.Add("headTable", fieldModes);
                else
                    output.Add("childrenDataTable"+(index-1), fieldModes);
                ++index;
            }
            return output;
        }

        /// <summary>
        /// 模板数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Data")]
        public async Task<dynamic> GetData([FromQuery] PrintDevSqlDataQuery input)
        {
            var output = new PrintDevDataOutput();
            var entity = await GetInfo(input.id);
            if (entity == null)
                throw JNPFException.Oh(ErrorCode.COM1005);
            var link = await _dbLinkService.GetInfo(entity.DbLinkId);
            var parameter = new List<SugarParameter>()
                {
                    new SugarParameter("@formId",input.formId)
                };
            var sqlList = entity.SqlTemplate.ToList<PrintDevSqlModel>();
            var dataTable = _dataBaseService.GetPrintDevData(link, sqlList.FirstOrDefault().sql, parameter);
            var dic = DateConver(DataTableToDicList(dataTable)).FirstOrDefault();
            for (int i = 1; i < sqlList.Count; i++)
            {
                var childDataTable = _dataBaseService.GetPrintDevData(link, sqlList[i].sql, parameter);
                dic.Add("childrenDataTable"+(i-1), DateConver(DataTableToDicList(childDataTable)));
            }
            output.printData = dic;
            output.printTemplate= entity.PrintTemplate;
            output.flowTaskOperatorRecordList =await GetPrintOperatorRecord(input.formId);
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
        public async Task Create_Api([FromBody] PrintDevCrInput input)
        {
            if (await _printDevRepository.AnyAsync(x => x.EnCode == input.enCode && x.DeleteMark == null) || await _printDevRepository.AnyAsync(x => x.FullName == input.fullName && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<PrintDevEntity>();
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
            var entity = await _printDevRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
            if (entity == null)
                throw JNPFException.Oh(ErrorCode.COM1005);
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
        public async Task Update_Api(string id, [FromBody] PrintDevUpInput input)
        {
            if (await _printDevRepository.AnyAsync(x => x.Id != id && x.EnCode == input.enCode && x.DeleteMark == null) || await _printDevRepository.AnyAsync(x => x.Id != id && x.FullName == input.fullName && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.COM1004);
            var entity = input.Adapt<PrintDevEntity>();
            var isOk = await Update(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 修改状态
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
        /// 复制
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPost("{id}/Actions/Copy")]
        public async Task ActionsCopy(string id)
        {
            var entity = await GetInfo(id);
            entity.FullName = entity.FullName + Ext.GetTimeStamp;
            entity.EnCode = entity.EnCode + Ext.GetTimeStamp;
            var isOk = await Create(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.WF0002);
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("Actions/ImportData")]
        public async Task ActionsImport(IFormFile file)
        {
            var josn = _fileService.Import(file);
            var model = josn.Deserialize<PrintDevEntity>();
            if (model == null)
                throw JNPFException.Oh(ErrorCode.D3006);
            await ImportData(model);
        }
        #endregion

        #region PublicMethod
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(PrintDevEntity entity)
        {
            return await _printDevRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(PrintDevEntity entity)
        {
            return await _printDevRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<PrintDevEntity> GetInfo(string id)
        {
            return await _printDevRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<PrintDevEntity>> GetList()
        {
            return await _printDevRepository.Entities.Where(x => x.DeleteMark == null).ToListAsync();
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<PrintDevListOutput>> GetOutList()
        {
            return await db.Queryable<PrintDevEntity, UserEntity, UserEntity, DictionaryDataEntity>((a, b, c, d) => new JoinQueryInfos(JoinType.Left, b.Id == a.CreatorUserId, JoinType.Left, c.Id == a.LastModifyUserId, JoinType.Left, a.Category == d.EnCode)).Select((a, b, c, d) => new
            PrintDevListOutput
            {
                category = a.Category,
                id = a.Id,
                fullName=a.FullName,
                description = a.Description,
                creatorTime = a.CreatorTime,
                creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account),
                enCode = a.EnCode,
                enabledMark = a.EnabledMark,
                lastModifyTime = a.LastModifyTime,
                lastModifyUser = SqlFunc.MergeString(c.RealName, "/", c.Account),
                sortCode = a.SortCode,
                type = a.Type,
                deleteMark = a.DeleteMark,
                parentId = d.Id,
                dictionaryTypeId= d.DictionaryTypeId
            }).MergeTable().Where(x => x.deleteMark == null && x.dictionaryTypeId == "202931027482510597").OrderBy(x => x.sortCode).OrderBy(x => x.creatorTime, OrderByType.Desc).ToListAsync();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(PrintDevEntity entity)
        {
            return await _printDevRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private async Task ImportData(PrintDevEntity entity)
        {
            try
            {
                db.BeginTran();
                var stor = db.Storageable(entity).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await stor.AsInsertable.ExecuteCommandAsync(); //执行插入
                await stor.AsUpdateable.ExecuteCommandAsync(); //执行更新　
                db.CommitTran();
            }
            catch (Exception ex)
            {
                db.RollbackTran();
                throw JNPFException.Oh(ErrorCode.D3006);
            }
        }

        private List<PrintDevFieldModel> GetFieldModels(DataTable dt)
        {
            var models = new List<PrintDevFieldModel>();
            foreach (var item in dt.Columns)
            {
                models.Add(new PrintDevFieldModel()
                {
                    field = item.ToString(),
                    fieldName= item.ToString(),
                }) ;
            }
            return models;
        }

        /// <summary>
        /// DataTable转DicList
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> DataTableToDicList(DataTable dt)
        {
            return dt.AsEnumerable().Select(
                    row => dt.Columns.Cast<DataColumn>().ToDictionary(
                    column => column.ColumnName,
                    column => row[column]
                    )).ToList();
        }

        /// <summary>
        /// 动态表单时间格式转换
        /// </summary>
        /// <param name="diclist"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> DateConver(List<Dictionary<string, object>> diclist)
        {
            foreach (var item in diclist)
            {
                foreach (var dic in item.Keys)
                {
                    if (item[dic] is DateTime)
                    {
                        item[dic] = item[dic].ToString() + " ";
                    }
                }
            }
            return diclist;
        }

        private async Task<List<PrintDevDataModel>> GetPrintOperatorRecord(string fromid)
        {
            var list =await App.GetService<IFlowTaskRepository>().GetTaskOperatorRecordList(fromid);
            var output = list.Adapt<List<PrintDevDataModel>>();
            foreach (var item in output)
            {
                item.userName = await _usersService.GetUserName(item.handleId);
                item.operatorId = await _usersService.GetUserName(item.operatorId);
            }
            return output;
        }
        #endregion
    }
}
