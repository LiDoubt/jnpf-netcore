using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Filter;
using JNPF.Common.Helper;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.JsonSerialization;
using JNPF.System.Entitys.Dto.System.Database;
using JNPF.System.Entitys.Model.System.DataBase;
using JNPF.System.Entitys.Permission;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.Common;
using JNPF.System.Interfaces.Permission;
using JNPF.System.Interfaces.System;
using JNPF.VisualDev.Entitys.Dto.VisualDevModelData;
using JNPF.VisualDev.Entitys.Model.VisualDevModelData;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.System.Core.Service.DataBase
{
    /// <summary>
    /// 数据管理
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "DataModel", Order = 208)]
    [Route("api/system/[controller]")]
    public class DataBaseService : IDataBaseService, IDynamicApiController, ITransient
    {
        private readonly SqlSugarScope db;
        private readonly IUserManager _userManager;
        private readonly IDbLinkService _dbLinkService;
        private readonly IAuthorizeService _authorizeService;
        private readonly IFileService _fileService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlSugarRepository"></param>
        /// <param name="dbLinkService"></param>
        /// <param name="userManager"></param>
        public DataBaseService(ISqlSugarRepository<UserEntity> sqlSugarRepository, IDbLinkService dbLinkService, IUserManager userManager, IAuthorizeService authorizeService, IFileService fileService)
        {
            db = sqlSugarRepository.Context;
            _dbLinkService = dbLinkService;
            _userManager = userManager;
            _authorizeService = authorizeService;
            _fileService = fileService;
        }

        #region GET
        /// <summary>
        /// 表名列表
        /// </summary>
        /// <param name="id">连接Id</param>
        /// <param name="input">过滤条件</param>
        /// <returns></returns>
        [HttpGet("{id}/Tables")]
        public async Task<dynamic> GetList_Api(string id, [FromQuery] KeywordInput input)
        {
            try
            {
                var link = (await _dbLinkService.GetInfo(id));
                ChangeDatabase(link);
                var tables = db.DbMaintenance.GetTableInfoList(false);
                var output = tables.Adapt<List<DatabaseTableListOutput>>();
                if (!string.IsNullOrEmpty(input.keyword))
                    output = output.FindAll(d => d.table.ToLower().Contains(input.keyword.ToLower()) || (d.tableName.IsNotEmptyOrNull() && d.tableName.ToLower().Contains(input.keyword.ToLower())));
                GetTableCount(output);
                return new { list = output.OrderBy(x => x.table).ToList() };
            }
            catch (Exception ex)
            {
                var data = new List<DatabaseTableListOutput>();
                return new { list = data };
            }
        }

        /// <summary>
        /// 预览数据
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <param name="DBId">连接Id</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        [HttpGet("{DBId}/Table/{tableName}/Preview")]
        public async Task<dynamic> GetData_Api([FromQuery] DatabaseTablePreviewQuery input, string DBId, string tableName)
        {
            var link = await _dbLinkService.GetInfo(DBId);
            if (string.IsNullOrEmpty(tableName))
                return new PageResult();
            ChangeDatabase(link);
            StringBuilder dbSql = new StringBuilder();
            dbSql.AppendFormat("SELECT * FROM {0} WHERE 1=1", tableName);
            if (!string.IsNullOrEmpty(input.field) && !string.IsNullOrEmpty(input.keyword))
                dbSql.AppendFormat(" AND {0} like '%{1}%'", input.field, input.keyword);
            RefAsync<int> total = 0;
            var list = await db.SqlQueryable<object>(dbSql.ToString()).ToDataTablePageAsync(input.currentPage, input.pageSize, total);
            var pageList = new SqlSugarPagedList<Dictionary<string, object>>()
            {
                list = DataTableToDicList(list),
                pagination = new PagedModel()
                {
                    PageIndex = input.currentPage,
                    PageSize = input.pageSize,
                    Total = total
                }
            };
            return PageResult<Dictionary<string, object>>.SqlSugarPageResult(pageList);
        }

        /// <summary>
        /// 字段列表
        /// </summary>
        /// <param name="DBId">连接Id</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        [HttpGet("{DBId}/Tables/{tableName}/Fields")]
        public async Task<dynamic> GetFieldList_Api(string DBId, string tableName,[FromQuery]string type)
        {
            var link = await _dbLinkService.GetInfo(DBId);
            var data = GetFieldList(link, tableName).Adapt<List<DatabaseTableFieldsListOutput>>();
            if (type.Equals("1"))
            {
                foreach (var item in data)
                {
                    var field = item.field.Replace("F_", "").Replace("f_", "").ToPascalCase();
                    item.field = field.Substring(0, 1).ToLower() + field[1..];
                }
            }
            return new { list = data };
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="DBId">连接Id</param>
        /// <param name="tableName">主键值</param>
        /// <returns></returns>
        [HttpGet("{DBId}/Table/{tableName}")]
        public async Task<dynamic> GetInfo_Api(string DBId, string tableName)
        {
            var link = await _dbLinkService.GetInfo(DBId);
            ChangeDatabase(link);
            var data = new DatabaseTableInfoOutput();
            data.tableInfo = db.DbMaintenance.GetTableInfoList(false).Find(m => m.Name == tableName).Adapt<TableInfo>();
            data.tableFieldList = db.DbMaintenance.GetColumnInfosByTableName(tableName, false).Adapt<List<TableFieldList>>();
            ViewDataTypeConversion(data.tableFieldList, db.CurrentConnectionConfig.DbType);
            return data;
        }

        /// <summary>
        /// 获取数据库表字段下拉框列表
        /// </summary>
        /// <param name="DBId">连接Id</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        [HttpGet("{DBId}/Tables/{tableName}/Fields/Selector")]
        public async Task<dynamic> SelectorData_Api(string DBId, string tableName)
        {
            var link = await _dbLinkService.GetInfo(DBId);
            var data = GetList(link).FindAll(m => m.table == tableName).Adapt<List<DatabaseTableFieldsSelectorOutput>>();
            return new { list = data };
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="linkId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{linkId}/Table/{id}/Action/Export")]
        public async Task<dynamic> ActionsExport(string linkId, string id)
        {
            var link = await _dbLinkService.GetInfo(linkId);
            ChangeDatabase(link);
            var data = new DatabaseTableInfoOutput();
            data.tableInfo = db.DbMaintenance.GetTableInfoList(false).Find(m => m.Name == id).Adapt<TableInfo>();
            data.tableFieldList = db.DbMaintenance.GetColumnInfosByTableName(id, false).Adapt<List<TableFieldList>>();
            ViewDataTypeConversion(data.tableFieldList, db.CurrentConnectionConfig.DbType);
            var jsonStr = data.Serialize();
            return _fileService.Export(jsonStr, data.tableInfo.tableName);
        }
        #endregion

        #region POST
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="DBId">连接Id</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        [HttpDelete("{DBId}/Table/{tableName}")]
        public async Task Delete_Api(string DBId, string tableName)
        {
            var link = await _dbLinkService.GetInfo(DBId);
            var data = GetData(link, tableName);
            if (data.Rows.Count > 0)
                throw JNPFException.Oh(ErrorCode.D1508);
            if (IsSysTable(tableName))
                throw JNPFException.Oh(ErrorCode.D1504);
            var isOk = Delete(link, tableName);
            if (!isOk)
                throw JNPFException.Oh(ErrorCode.D1500);
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="DBId">连接Id</param>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPost("{DBId}/Table")]
        public async Task Create_Api(string DBId, [FromBody] DatabaseTableCrInput input)
        {
            var link = await _dbLinkService.GetInfo(DBId);
            if (db.DbMaintenance.IsAnyTable(input.tableInfo.newTable))
                throw JNPFException.Oh(ErrorCode.D1503);
            var tableInfo = input.tableInfo.Adapt<DbTableModel>();
            tableInfo.table = input.tableInfo.newTable;
            var tableFieldList = input.tableFieldList.Adapt<List<DbTableFieldModel>>();
            var isOk = await Create(link, tableInfo, tableFieldList);
            if (!isOk)
                throw JNPFException.Oh(ErrorCode.D1501);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="DBId">连接Id</param>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpPut("{DBId}/Table")]
        public async Task Update_Api(string DBId, [FromBody] DatabaseTableUpInput input)
        {
            var link = await _dbLinkService.GetInfo(DBId);
            var oldFieldList = GetFieldList(link, input.tableInfo.table).Adapt<List<TableFieldList>>();
            ViewDataTypeConversion(oldFieldList, db.CurrentConnectionConfig.DbType);
            var oldTableInfo = db.DbMaintenance.GetTableInfoList(false).Find(m => m.Name == input.tableInfo.table).Adapt<DbTableModel>();
            try
            {
                var data = GetData(link, input.tableInfo.table);
                if (data.Rows.Count > 0)
                    throw JNPFException.Oh(ErrorCode.D1508);
                var tableInfo = input.tableInfo.Adapt<DbTableModel>();
                tableInfo.table = input.tableInfo.newTable;
                var tableFieldList = input.tableFieldList.Adapt<List<DbTableFieldModel>>();
                if (IsSysTable(tableInfo.table))
                    throw JNPFException.Oh(ErrorCode.D1504);
                if (!input.tableInfo.table.Equals(input.tableInfo.newTable) && db.DbMaintenance.IsAnyTable(input.tableInfo.newTable))
                    throw JNPFException.Oh(ErrorCode.D1503);
                var isOk = await Update(link, input.tableInfo.table, tableInfo, tableFieldList);
                if (!isOk)
                    throw JNPFException.Oh(ErrorCode.D1502);
            }
            catch (Exception ex)
            {
                await Create(link, oldTableInfo, oldFieldList.Adapt<List<DbTableFieldModel>>());
                throw JNPFException.Oh(ErrorCode.D1502);
            }
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="linkid"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("{linkid}/Action/Import")]
        public async Task ActionsImport(string linkid,IFormFile file)
        {
            var josn = _fileService.Import(file);
            var data = josn.Deserialize<DatabaseTableCrInput>();
            if (data == null||data.tableFieldList==null||data.tableInfo==null)
                throw JNPFException.Oh(ErrorCode.D3006);
            data.tableInfo.newTable = data.tableInfo.table;
            await Create_Api(linkid, data);
        }
        #endregion

        #region PulicMethod
        /// <summary>
        /// 表列表
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <returns></returns>
        [NonAction]
        public List<DbTableModel> GetList(DbLinkEntity link)
        {
            try
            {
                ChangeDatabase(link);
                var dbType = link == null ? "SQLServer" : link.DbType;
                var sql = DBTableSql(dbType);
                var modelList = db.Ado.SqlQuery<DynamicDbTableModel>(sql);
                var list = modelList.Adapt<List<DbTableModel>>();
                return list;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 表字段
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="table">表名</param>
        /// <returns></returns>
        [NonAction]
        public List<DbTableFieldModel> GetFieldList(DbLinkEntity link, string table)
        {
            try
            {
                if (string.IsNullOrEmpty(table))
                {
                    return new List<DbTableFieldModel>();
                }
                ChangeDatabase(link);
                var list = db.DbMaintenance.GetColumnInfosByTableName(table, false);
                return list.Adapt<List<DbTableFieldModel>>();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// 表字段(非异步)
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="table">表名</param>
        /// <returns></returns>
        [NonAction]
        public List<DbTableFieldModel> GetFieldListByNoAsync(DbLinkEntity link, string table)
        {
            try
            {
                if (string.IsNullOrEmpty(table))
                {
                    return new List<DbTableFieldModel>();
                }
                ChangeDatabase(link);
                var list = db.DbMaintenance.GetColumnInfosByTableName(table);
                ChangeDatabase(null);
                return list.Adapt<List<DbTableFieldModel>>();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// 表数据（分页）
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="table">表名</param>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetData(DbLinkEntity link, string table, PageInputBase input)
        {
            if (string.IsNullOrEmpty(table))
            {
                return new PageResult();
            }
            ChangeDatabase(link);
            var queryParam = JSON.Deserialize<Dictionary<string, object>>(input.queryJson);
            StringBuilder dbSql = new StringBuilder();
            dbSql.AppendFormat("SELECT * FROM {0} WHERE 1=1", table);
            if (!string.IsNullOrEmpty(queryParam["field"].ToString()) && !string.IsNullOrEmpty(input.keyword))
            {
                dbSql.AppendFormat(" AND {0} like '%{1}%'", queryParam["field"].ToString(), input.keyword);
            }
            RefAsync<int> total = 0;
            var list = await db.SqlQueryable<object>(dbSql.ToString()).ToDataTablePageAsync(input.currentPage, input.pageSize, total);
            var pageList = new SqlSugarPagedList<Dictionary<string, object>>()
            {
                list = DataTableToDicList(list),
                pagination = new PagedModel()
                {
                    PageIndex = input.currentPage,
                    PageSize = input.pageSize,
                    Total = total
                }
            };
            return pageList;
        }

        /// <summary>
        /// 获取表数据
        /// </summary>
        /// <param name="link"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        [NonAction]
        public DataTable GetData(DbLinkEntity link, string table)
        {
            try
            {
                if (string.IsNullOrEmpty(table))
                    return new DataTable();
                ChangeDatabase(link);
                var data = db.Queryable<dynamic>().AS(table).ToDataTable();
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="strSql">sql语句</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> ExecuteSql(DbLinkEntity link, string strSql)
        {
            try
            {
                ChangeDatabase(link);
                db.BeginTran();
                var flag = 0;
                if (db.CurrentConnectionConfig.DbType==SqlSugar.DbType.Oracle)
                {
                    var sqlList = strSql.Split(";").ToList();
                    foreach (var item in sqlList)
                    {
                        if (item.IsNotEmptyOrNull())
                        {
                            await db.Ado.ExecuteCommandAsync(item);
                        }
                    }
                }
                else
                {
                    flag = await db.Ado.ExecuteCommandAsync(strSql);
                }
                db.CommitTran();
                ChangeDatabase(null);
                return flag;
            }
            catch (Exception ex)
            {
                db.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 远端操作数据
        /// </summary>
        /// <param name="link">连接对象</param>
        /// <param name="table">表名</param>
        /// <param name="dicList">参数</param>
        /// <param name="primaryField">主键，主键为空新增则修改</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> ExecuteCom(DbLinkEntity link, string table, List<Dictionary<string, object>> dicList, string primaryField = "")
        {
            try
            {
                ChangeDatabase(link);
                db.BeginTran();
                var flag = 0;
                if (primaryField == "")
                {
                    flag = await db.Insertable(dicList).AS(table).ExecuteCommandAsync();
                }
                else
                {
                    flag = await db.Updateable(dicList).AS(table).WhereColumns(primaryField).ExecuteCommandAsync();
                }
                db.CommitTran();
                ChangeDatabase(null);
                return flag;
            }
            catch (Exception ex)
            {
                db.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="table">表名</param>
        [NonAction]
        public bool Delete(DbLinkEntity link, string table)
        {
            try
            {
                ChangeDatabase(link);
                db.BeginTran();
                var isOk = db.DbMaintenance.DropTable(table);
                db.CommitTran();
                return isOk;
            }
            catch (Exception ex)
            {
                db.RollbackTran();
                return false;
            }
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="tableModel">表对象</param>
        /// <param name="tableFieldList">字段对象</param>
        [NonAction]
        public async Task<bool> Create(DbLinkEntity link, DbTableModel tableModel, List<DbTableFieldModel> tableFieldList)
        {
            try
            {
                ChangeDatabase(link);
                db.BeginTran();
                if (db.CurrentConnectionConfig.DbType == SqlSugar.DbType.MySql)
                {
                    await CreateTableMySql(tableModel, tableFieldList);
                }
                else
                {
                    CreateTable(tableModel, tableFieldList);
                }
                db.CommitTran();
                return true;
            }
            catch (Exception ex)
            {
                db.RollbackTran();
                return false;
            }
        }

        /// <summary>
        /// 修改表
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="oldTable">主键值</param>
        /// <param name="tableModel">表对象</param>
        /// <param name="tableFieldList">字段对象</param>
        [NonAction]
        public async Task<bool> Update(DbLinkEntity link, string oldTable, DbTableModel tableModel, List<DbTableFieldModel> tableFieldList)
        {
            try
            {
                ChangeDatabase(link);
                db.BeginTran();
                db.DbMaintenance.DropTable(oldTable);
                if (db.CurrentConnectionConfig.DbType == SqlSugar.DbType.MySql)
                {
                    await CreateTableMySql(tableModel, tableFieldList);
                }
                else
                {
                    CreateTable(tableModel, tableFieldList);
                }
                db.CommitTran();
                return true;
            }
            catch (Exception ex)
            {

                db.RollbackTran();
                return false;
            }
        }

        /// <summary>
        /// 根据链接获取数据
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public DataTable GetInterFaceData(DbLinkEntity link, string strSql,params SugarParameter[] parameters)
        {
            try
            {
                ChangeDatabase(link);
                if (db.CurrentConnectionConfig.DbType==SqlSugar.DbType.Oracle)
                {
                    strSql = strSql.Replace(";", "");
                }
                var dt = db.Ado.GetDataTable(strSql, parameters);
                ChangeDatabase(null);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 根据链接获取数据(打印模板)
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public DataTable GetPrintDevData(DbLinkEntity link, string strSql, List<SugarParameter> sugarParameters=null)
        {
            try
            {
                ChangeDatabase(link);
                if (db.CurrentConnectionConfig.DbType == SqlSugar.DbType.Oracle)
                {
                    strSql = strSql.Replace(";", "");
                }
                var dt = db.Ado.GetDataTable(strSql, sugarParameters);
                ChangeDatabase(null);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 测试数据库连接
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        [NonAction]
        public bool IsConnection(DbLinkEntity link)
        {
            try
            {
                ChangeDatabase(link);
                db.Open();
                db.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 同步表操作
        /// </summary>
        /// <param name="linkFrom"></param>
        /// <param name="linkTo"></param>
        /// <param name="table"></param>
        /// <param name="type"></param>
        [NonAction]
        public void SyncTable(DbLinkEntity linkFrom, DbLinkEntity linkTo, string table, int type)
        {
            try
            {
                if (type == 2)
                {
                    ChangeDatabase(linkFrom);
                    var columns = db.DbMaintenance.GetColumnInfosByTableName(table, false);
                    ChangeDatabase(linkTo);
                    DelDataLength(columns);
                    db.DbMaintenance.CreateTable(table, columns);
                }
                else if (type == 3)
                {
                    ChangeDatabase(linkTo);
                    db.DbMaintenance.TruncateTable(table);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="link"></param>
        /// <param name="dt"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        [NonAction]
        public bool SyncData(DbLinkEntity link, DataTable dt, string table)
        {
            try
            {
                ChangeDatabase(link);
                var str = dt.Serialize();
                List<Dictionary<string, object>> dc = db.Utilities.DataTableToDictionaryList(dt);//5.0.23版本支持
                var isOk = db.Insertable(dc).AS(table).ExecuteCommand();
                return isOk > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 根据链接获取分页数据
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public PageResult<Dictionary<string, object>> GetInterFaceData(DbLinkEntity link, string strSql, VisualDevModelListQueryInput pageInput, ColumnDesignModel columnDesign,string menuId)
        {
            var authorizeWhere = new List<IConditionalModel>();
            if (columnDesign.useDataPermission)
            {
                if (_userManager.User.IsAdministrator == 0)
                {
                    authorizeWhere = _authorizeService.GetCondition<Dictionary<string, object>>(menuId);
                }
            }
            try
            {
                ChangeDatabase(link);
                int total = 0;
                //将查询的关键字json转成Dictionary
                Dictionary<string, object> keywordJsonDic = string.IsNullOrEmpty(pageInput.json) ? null : pageInput.json.Deserialize<Dictionary<string, object>>();
                var conModels = new List<IConditionalModel>();
                if (keywordJsonDic != null)
                {
                    foreach (KeyValuePair<string, object> item in keywordJsonDic)
                    {
                        var model = columnDesign.searchList.Find(it => it.__vModel__.Equals(item.Key));
                        var type = model.__config__.jnpfKey;
                        switch (type)
                        {
                            case "date":
                                {
                                    var timeRange = item.Value.ToObeject<List<string>>();
                                    var startTime = Ext.GetDateTime(timeRange.First());
                                    var endTime = Ext.GetDateTime(timeRange.Last());
                                    conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = new DateTime(startTime.ToDate().Year, startTime.ToDate().Month, startTime.ToDate().Day, 0, 0, 0, 0).ToString() });
                                    conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = new DateTime(endTime.ToDate().Year, endTime.ToDate().Month, endTime.ToDate().Day, 23, 59, 59, 999).ToString() });
                                }
                                break;
                            case "time":
                                {
                                    var timeRange = item.Value.ToObeject<List<string>>();
                                    var startTime = timeRange.First();
                                    var endTime = timeRange.Last();
                                    conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startTime });
                                    conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endTime });
                                }
                                break;
                            case "createTime":
                                {
                                    var timeRange = item.Value.ToObeject<List<string>>();
                                    var startTime = Ext.GetDateTime(timeRange.First());
                                    var endTime = Ext.GetDateTime(timeRange.Last());
                                    conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = new DateTime(startTime.ToDate().Year, startTime.ToDate().Month, startTime.ToDate().Day, 0, 0, 0, 0).ToString() });
                                    conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = new DateTime(endTime.ToDate().Year, endTime.ToDate().Month, endTime.ToDate().Day, 23, 59, 59, 999).ToString() });
                                }
                                break;
                            case "modifyTime":
                                {
                                    var timeRange = item.Value.ToObeject<List<string>>();
                                    var startTime = Ext.GetDateTime(timeRange.First());
                                    var endTime = Ext.GetDateTime(timeRange.Last());
                                    conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = new DateTime(startTime.ToDate().Year, startTime.ToDate().Month, startTime.ToDate().Day, 0, 0, 0, 0).ToString() });
                                    conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = new DateTime(endTime.ToDate().Year, endTime.ToDate().Month, endTime.ToDate().Day, 23, 59, 59, 999).ToString() });
                                }
                                break;
                            case "numInput":
                                {
                                    List<string> numArray = item.Value.ToObeject<List<string>>();
                                    var startNum = numArray.First().ToInt();
                                    var endNum = numArray.Last() == null ? Int64.MaxValue : numArray.Last().ToInt();
                                    conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startNum.ToString() });
                                    conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endNum.ToString() });
                                }
                                break;
                            case "calculate":
                                {
                                    List<string> numArray = item.Value.ToObeject<List<string>>();
                                    var startNum = numArray.First().ToInt();
                                    var endNum = numArray.Last() == null ? Int64.MaxValue : numArray.Last().ToInt();
                                    conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = startNum.ToString() });
                                    conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = endNum.ToString() });
                                }
                                break;
                            default:
                                {
                                    if (model.searchType == 2)
                                    {
                                        conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.Like, FieldValue = item.Value.ToString() });
                                    }
                                    else if (model.searchType == 1)
                                    {
                                        //多选时为模糊查询
                                        if (model.multiple || type == "checkbox")
                                        {
                                            conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.Like, FieldValue = item.Value.ToString() });
                                        }
                                        else
                                        {
                                            conModels.Add(new ConditionalModel { FieldName = item.Key, ConditionalType = ConditionalType.Equal, FieldValue = item.Value.ToString().Replace("\r\n ", "").Replace("\r\n", "").Replace(" ", "") });
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }

                if (db.CurrentConnectionConfig.DbType == SqlSugar.DbType.Oracle)
                {
                    strSql = strSql.Replace(";", "");
                }

                var dt = db.SqlQueryable<object>(strSql).Where(conModels).Where(authorizeWhere).ToDataTablePage(pageInput.currentPage, pageInput.pageSize, ref total);
                ChangeDatabase(null);
                return new PageResult<Dictionary<string, object>>()
                {
                    pagination = new PageResult()
                    {
                        pageIndex = pageInput.currentPage,
                        pageSize = pageInput.pageSize,
                        total = total
                    },
                    list = dt.ToObeject<List<Dictionary<string, object>>>()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="link"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        [NonAction]
        public bool IsAnyTable(DbLinkEntity link, string table)
        {
            try
            {
                ChangeDatabase(link);
                return db.DbMaintenance.IsAnyTable(table, false);
            }
            catch (Exception)
            {

                return false;
            }
        }

        /// <summary>
        /// 添加链接
        /// </summary>
        /// <param name="link"></param>
        [NonAction]
        public void AddConnection(DbLinkEntity link)
        {
            db.AddConnection(new ConnectionConfig()
            {
                ConfigId = link.Id,
                DbType = ToDbType(link.DbType),
                ConnectionString = ToConnectionString(link),
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
        }

        #endregion

        #region PrivateMethod
        /// <summary>
        /// 是否系统表
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private bool IsSysTable(string table)
        {
            string[] byoTable = "base_authorize,base_billrule,base_dbbackup,base_dblink,base_dictionarydata,base_dictionarytype,base_imcontent,base_languagemap,base_languagetype,base_menu,base_message,base_messagereceive,base_module,base_modulebutton,base_modulecolumn,base_moduledataauthorize,base_moduledataauthorizescheme,base_organize,base_position,base_province,base_role,base_sysconfig,base_syslog,base_timetask,base_timetasklog,base_user,base_userrelation,crm_busines,crm_businesproduct,crm_clue,crm_contract,crm_contractinvoice,crm_contractmoney,crm_contractproduct,crm_customer,crm_customercontacts,crm_followlog,crm_invoice,crm_product,crm_receivable,ext_bigdata,ext_document,ext_documentshare,ext_emailconfig,ext_emailreceive,ext_emailsend,ext_employee,ext_order,ext_orderentry,ext_orderreceivable,ext_projectgantt,ext_schedule,ext_tableexample,ext_worklog,ext_worklogshare,flow_delegate,flow_engine,flow_engineform,flow_enginevisible,flow_task,flow_taskcirculate,flow_tasknode,flow_taskoperator,flow_taskoperatorrecord,wechat_mpeventcontent,wechat_mpmaterial,wechat_mpmessage,wechat_qydepartment,wechat_qymessage,wechat_qyuser,wform_applybanquet,wform_applydelivergoods,wform_applydelivergoodsentry,wform_applymeeting,wform_archivalborrow,wform_articleswarehous,wform_batchpack,wform_batchtable,wform_conbilling,wform_contractapproval,wform_contractapprovalsheet,wform_debitbill,wform_documentapproval,wform_documentsigning,wform_expenseexpenditure,wform_finishedproduct,wform_finishedproductentry,wform_incomerecognition,wform_leaveapply,wform_letterservice,wform_materialrequisition,wform_materialrequisitionentry,wform_monthlyreport,wform_officesupplies,wform_outboundorder,wform_outboundorderentry,wform_outgoingapply,wform_paydistribution,wform_paymentapply,wform_postbatchtab,wform_procurementmaterial,wform_procurementmaterialentry,wform_purchaselist,wform_purchaselistentry,wform_quotationapproval,wform_receiptprocessing,wform_receiptsign,wform_rewardpunishment,wform_salesorder,wform_salesorderentry,wform_salessupport,wform_staffovertime,wform_supplementcard,wform_travelapply,wform_travelreimbursement,wform_vehicleapply,wform_violationhandling,wform_warehousereceipt,wform_warehousereceiptentry,wform_workcontactsheet".Split(',');
            bool exists = ((IList)byoTable).Contains(table.ToLower());
            return exists;
        }

        /// <summary>
        /// 根据链接对象链接数据库
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        private void ChangeDatabase(DbLinkEntity link)
        {
            try
            {
                if (link != null)
                {
                    db.AddConnection(new ConnectionConfig()
                    {
                        ConfigId = link.Id,
                        DbType = ToDbType(link.DbType),
                        ConnectionString = ToConnectionString(link),
                        InitKeyType = InitKeyType.Attribute,
                        IsAutoCloseConnection = true
                    });
                    db.ChangeDatabase(link.Id);
                }
                else
                {
                    var defaultId = App.Configuration["ConnectionStrings:ConfigId"];
                    db.ChangeDatabase(defaultId);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// 数据库表SQL
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        [NonAction]
        public string DBTableSql(string dbType)
        {
            StringBuilder sb = new StringBuilder();
            switch (dbType.ToLower())
            {
                case "sqlserver":
                    sb.Append(@"DECLARE @TABLEINFO TABLE ( NAME VARCHAR(50) , SUMROWS VARCHAR(11) , RESERVED VARCHAR(50) , DATA VARCHAR(50) , INDEX_SIZE VARCHAR(50) , UNUSED VARCHAR(50) , PK VARCHAR(50) ) DECLARE @TABLENAME TABLE ( NAME VARCHAR(50) ) DECLARE @NAME VARCHAR(50) DECLARE @PK VARCHAR(50) INSERT INTO @TABLENAME ( NAME ) SELECT O.NAME FROM SYSOBJECTS O , SYSINDEXES I WHERE O.ID = I.ID AND O.XTYPE = 'U' AND I.INDID < 2 ORDER BY I.ROWS DESC , O.NAME WHILE EXISTS ( SELECT 1 FROM @TABLENAME ) BEGIN SELECT TOP 1 @NAME = NAME FROM @TABLENAME DELETE @TABLENAME WHERE NAME = @NAME DECLARE @OBJECTID INT SET @OBJECTID = OBJECT_ID(@NAME) SELECT @PK = COL_NAME(@OBJECTID, COLID) FROM SYSOBJECTS AS O INNER JOIN SYSINDEXES AS I ON I.NAME = O.NAME INNER JOIN SYSINDEXKEYS AS K ON K.INDID = I.INDID WHERE O.XTYPE = 'PK' AND PARENT_OBJ = @OBJECTID AND K.ID = @OBJECTID INSERT INTO @TABLEINFO ( NAME , SUMROWS , RESERVED , DATA , INDEX_SIZE , UNUSED ) EXEC SYS.SP_SPACEUSED @NAME UPDATE @TABLEINFO SET PK = @PK WHERE NAME = @NAME END SELECT F.NAME AS F_TABLE,ISNULL(P.TDESCRIPTION,F.NAME) AS F_TABLENAME, F.RESERVED AS F_SIZE, RTRIM(F.SUMROWS) AS F_SUM, F.PK AS F_PRIMARYKEY FROM @TABLEINFO F LEFT JOIN ( SELECT NAME = CASE WHEN A.COLORDER = 1 THEN D.NAME ELSE '' END , TDESCRIPTION = CASE WHEN A.COLORDER = 1 THEN ISNULL(F.VALUE, '') ELSE '' END FROM SYSCOLUMNS A LEFT JOIN SYSTYPES B ON A.XUSERTYPE = B.XUSERTYPE INNER JOIN SYSOBJECTS D ON A.ID = D.ID AND D.XTYPE = 'U' AND D.NAME <> 'DTPROPERTIES' LEFT JOIN SYS.EXTENDED_PROPERTIES F ON D.ID = F.MAJOR_ID WHERE A.COLORDER = 1 AND F.MINOR_ID = 0 ) P ON F.NAME = P.NAME WHERE 1 = 1 ORDER BY F_TABLE");
                    break;
                case "oracle":
                    sb.Append(@"SELECT DISTINCT COL.TABLE_NAME AS F_TABLE,TAB.COMMENTS AS F_TABLENAME,0 AS F_SIZE,NVL(T.NUM_ROWS,0)AS F_SUM,COLUMN_NAME AS F_PRIMARYKEY FROM USER_CONS_COLUMNS COL INNER JOIN USER_CONSTRAINTS CON ON CON.CONSTRAINT_NAME=COL.CONSTRAINT_NAME INNER JOIN USER_TAB_COMMENTS TAB ON TAB.TABLE_NAME=COL.TABLE_NAME INNER JOIN USER_TABLES T ON T.TABLE_NAME=COL.TABLE_NAME WHERE CON.CONSTRAINT_TYPE NOT IN('C','R')ORDER BY COL.TABLE_NAME");
                    break;
                case "mysql":
                    sb.Append(@"SELECT T1.*,(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.`COLUMNS`WHERE TABLE_SCHEMA=DATABASE()AND TABLE_NAME=T1.F_TABLE AND COLUMN_KEY='PRI')F_PRIMARYKEY FROM(SELECT TABLE_NAME F_TABLE,0 F_SIZE,TABLE_ROWS F_SUM,(SELECT IF(LENGTH(TRIM(TABLE_COMMENT))<1,TABLE_NAME,TABLE_COMMENT))F_TABLENAME FROM INFORMATION_SCHEMA.`TABLES`WHERE TABLE_SCHEMA=DATABASE())T1 ORDER BY T1.F_TABLE");
                    break;
                default:
                    throw new Exception("不支持");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 转换连接字符串
        /// </summary>
        /// <param name="dbLinkEntity"></param>
        /// <returns></returns>
        public string ToConnectionString(DbLinkEntity dbLinkEntity)
        {
            switch (dbLinkEntity.DbType.ToLower())
            {
                case "sqlserver":
                    return string.Format("Data Source={0},{4};Initial Catalog={1};User ID={2};Password={3};MultipleActiveResultSets=true", dbLinkEntity.Host, dbLinkEntity.ServiceName, dbLinkEntity.UserName, dbLinkEntity.Password, dbLinkEntity.Port);
                case "oracle":
                    return string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVER = DEDICATED)(SERVICE_NAME={2})));User Id={3};Password={4}", dbLinkEntity.Host, dbLinkEntity.Port.ToString(), dbLinkEntity.TableSpace, dbLinkEntity.UserName, dbLinkEntity.Password);
                case "mysql":
                    return string.Format("server={0};port={1};database={2};user={3};password={4};AllowLoadLocalInfile=true", dbLinkEntity.Host, dbLinkEntity.Port.ToString(), dbLinkEntity.ServiceName, dbLinkEntity.UserName, dbLinkEntity.Password);
                case "dm8":
                    return string.Format("server={0};port={1};database={2};User Id={3};PWD={4}", dbLinkEntity.Host, dbLinkEntity.Port.ToString(), dbLinkEntity.ServiceName, dbLinkEntity.UserName, dbLinkEntity.Password);
                case "kingbasees":
                    return string.Format("server={0};port={1};database={2};UID={3};PWD={4}", dbLinkEntity.Host, dbLinkEntity.Port.ToString(), dbLinkEntity.ServiceName, dbLinkEntity.UserName, dbLinkEntity.Password);
                case "postgresql":
                    return string.Format("server={0};port={1};Database={2};User Id={3};Password={4}", dbLinkEntity.Host, dbLinkEntity.Port.ToString(), dbLinkEntity.ServiceName, dbLinkEntity.UserName, dbLinkEntity.Password);
                default:
                    throw JNPFException.Oh(ErrorCode.D1505);
            }
        }

        /// <summary>
        /// 转换数据库类型
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        [NonAction]
        public SqlSugar.DbType ToDbType(string dbType)
        {
            switch (dbType.ToLower())
            {
                case "sqlserver":
                    return SqlSugar.DbType.SqlServer;
                case "mysql":
                    return SqlSugar.DbType.MySql;
                case "oracle":
                    return SqlSugar.DbType.Oracle;
                case "dm8":
                    return SqlSugar.DbType.Dm;
                case "kingbasees":
                    return SqlSugar.DbType.Kdbndp;
                case "postgresql":
                    return SqlSugar.DbType.PostgreSQL;
                default:
                    throw JNPFException.Oh(ErrorCode.D1505);
            }
        }
        /// <summary>
        /// 删除列长度（SqlSugar除了字符串其他不需要类型长度）
        /// </summary>
        /// <param name="dbColumnInfos"></param>
        private void DelDataLength(List<DbColumnInfo> dbColumnInfos)
        {
            foreach (var item in dbColumnInfos)
            {
                if (item.DataType != "varchar")
                {
                    item.Length = 0;
                }
                item.DataType = DataTypeConversion(item.DataType, db.CurrentConnectionConfig.DbType);
            }
        }

        /// <summary>
        /// 数据库数据类型转换
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="databaseType"></param>
        /// <returns></returns>
        private string DataTypeConversion(string dataType, SqlSugar.DbType databaseType)
        {
            if (databaseType.Equals(SqlSugar.DbType.Oracle))
            {
                switch (dataType)
                {
                    case "text":
                        return "CLOB";
                    case "decimal":
                        return "DECIMAL(38,38)";
                    case "datetime":
                        return "DATE";
                    case "bigint":
                        return "NUMBER";
                    default:
                        return dataType.ToUpper();
                }
            }
            else if (databaseType.Equals(SqlSugar.DbType.Dm))
            {
                return dataType.ToUpper();
            }
            else if (databaseType.Equals(SqlSugar.DbType.Kdbndp))
            {
                switch (dataType)
                {
                    case "int":
                        return "NUMBER";
                    case "datetime":
                        return "DATE";
                    case "bigint":
                        return "INT8";
                    default:
                        return dataType.ToUpper();
                }
            }
            else if (databaseType.Equals(SqlSugar.DbType.PostgreSQL))
            {
                switch (dataType)
                {
                    case "varchar":
                        return "varchar";
                    case "int":
                        return "NUMBER";
                    case "datetime":
                        return "DATE";
                    case "decimal":
                        return "DECIMAL";
                    case "bigint":
                        return "INT8";
                    case "text":
                        return "TEXT";
                    default:
                        return dataType;
                }
            }
            else
            {
                return dataType;
            }
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
                    column => column.ColumnName.ToLower(),
                    column => row[column]
                    )).ToList();
        }

        /// <summary>
        /// MySql创建表单+注释
        /// </summary>
        /// <param name="db">连接Db</param>
        /// <param name="tableModel">表</param>
        /// <param name="tableFieldList">字段</param>
        private async Task CreateTableMySql(DbTableModel tableModel, List<DbTableFieldModel> tableFieldList)
        {
            try
            {
                db.BeginTran();
                StringBuilder strSql = new StringBuilder();
                strSql.Append("CREATE TABLE `" + tableModel.table + "` (\r\n");
                foreach (var item in tableFieldList)
                {
                    if (item.primaryKey == 1 && item.allowNull == 1)
                        throw JNPFException.Oh(ErrorCode.D1509);
                    strSql.Append("  `" + item.field + "` " + item.dataType.ToUpper() + "");
                    if (item.dataType == "varchar" || item.dataType == "nvarchar" || item.dataType == "decimal")
                        strSql.Append(" (" + item.dataLength + ") ");
                    if (item.primaryKey == 1)
                    {
                        strSql.Append(" primary key ");
                    }
                    if (item.allowNull == 0)
                        strSql.Append(" NOT NULL ");
                    else
                        strSql.Append(" NULL ");
                    strSql.Append("COMMENT '" + item.fieldName + "'");
                    strSql.Append(",");
                }
                strSql.Remove(strSql.Length - 1, 1);
                strSql.Append("\r\n");
                strSql.Append(") COMMENT = '" + tableModel.tableName + "';");
                await db.Ado.ExecuteCommandAsync(strSql.ToString());
                db.CommitTran();
            }
            catch (Exception ex)
            {
                db.RollbackTran();
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// sqlsugar建表
        /// </summary>
        /// <param name="tableModel"></param>
        /// <param name="tableFieldList"></param>
        /// <returns></returns>
        private void CreateTable(DbTableModel tableModel, List<DbTableFieldModel> tableFieldList)
        {
            try
            {
                var cloumnList = tableFieldList.Adapt<List<DbColumnInfo>>();
                DelDataLength(cloumnList);
                var isOk = db.DbMaintenance.CreateTable(tableModel.table, cloumnList);
                db.DbMaintenance.AddTableRemark(tableModel.table, tableModel.tableName);
                foreach (var item in cloumnList)
                {
                    db.DbMaintenance.AddColumnRemark(item.DbColumnName, tableModel.table, item.ColumnDescription);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取表条数
        /// </summary>
        /// <param name="tableList"></param>
        private void GetTableCount(List<DatabaseTableListOutput> tableList)
        {
            foreach (var item in tableList)
            {
                try
                {
                    item.sum = db.Queryable<dynamic>().AS(item.table).Count();
                }
                catch (Exception ex)
                {
                    item.sum = 0;
                }
            }
        }

        private void ViewDataTypeConversion(List<TableFieldList> fields, SqlSugar.DbType databaseType)
        {
            foreach (var item in fields)
            {
                item.dataType = item.dataType.ToLower();
                if (item.dataType.Equals("string"))
                {
                    item.dataType = "varchar";
                    if (item.dataLength.ToInt() > 2000)
                    {
                        item.dataType = "text";
                        item.dataLength = "50";
                    }
                }
                else if (item.dataType.Equals("single"))
                {
                    item.dataType = "decimal";
                }
            }
        }


        #region 数据库类型转换
        private string OracleDataTypeConversion(string dataType)
        {
            switch (dataType)
            {
                case "varchar":
                    return "NVARCHAR2";
                case "int":
                    return "NUMBER";
                case "datetime":
                    return "DATE";
                case "decimal":
                    return "NUMBER";
                case "bigint":
                    return "NUMBER";
                case "text":
                    return "CLOB";
                default:
                    return dataType;
            }
        }

        private string DMDataTypeConversion(string dataType)
        {
            switch (dataType)
            {
                case "varchar":
                    return "VARCHAR";
                case "int":
                    return "INT";
                case "datetime":
                    return "DATETIME";
                case "decimal":
                    return "DECIMAL";
                case "bigint":
                    return "BIGINT";
                case "text":
                    return "TEXT";
                default:
                    return dataType;
            }
        }

        private string KingBaseDataTypeConversion(string dataType)
        {
            switch (dataType)
            {
                case "varchar":
                    return "VARCHAR";
                case "int":
                    return "NUMBER";
                case "datetime":
                    return "DATE";
                case "decimal":
                    return "DECIMAL";
                case "bigint":
                    return "INT8";
                case "text":
                    return "TEXT";
                default:
                    return dataType;
            }
        }

        private string PostgreDataTypeConversion(string dataType)
        {
            switch (dataType)
            {
                case "varchar":
                    return "varchar";
                case "int":
                    return "NUMBER";
                case "datetime":
                    return "DATE";
                case "decimal":
                    return "DECIMAL";
                case "bigint":
                    return "INT8";
                case "text":
                    return "TEXT";
                default:
                    return dataType;
            }
        }

        private string MysqlDataTypeConversion(string dataType)
        {
            switch (dataType)
            {
                case "varchar":
                    return "varchar";
                case "int":
                    return "int";
                case "datetime":
                    return "datetime";
                case "decimal":
                    return "decimal";
                case "bigint":
                    return "bigint";
                case "text":
                    return "text";
                default:
                    return dataType;
            }
        }

        private string SqlServerDataTypeConversion(string dataType)
        {
            switch (dataType)
            {
                case "varchar":
                    return "nvarchar";
                case "int":
                    return "int";
                case "datetime":
                    return "datetime";
                case "decimal":
                    return "decimal";
                case "bigint":
                    return "bigint";
                case "text":
                    return "text";
                default:
                    return dataType;
            }
        }
        #endregion
        #endregion
    }
}
