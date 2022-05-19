using JNPF.Dependency;
using JNPF.VisualDev.Entitys;
using JNPF.VisualDev.Entitys.Dto.VisualDevModelData;
using JNPF.VisualDev.Run.Interfaces;
using JNPF.Common.Helper;
using JNPF.VisualDev.Entitys.Model.VisualDevModelData;
using JNPF.Common.Core.Manager;
using JNPF.System.Interfaces.System;
using JNPF.System.Interfaces.Permission;
using JNPF.JsonSerialization;
using JNPF.VisualDev.Entitys.Entity;
using JNPF.System.Entitys.Model.System.DataBase;
using JNPF.Common.Extension;
using JNPF.Common.Filter;
using JNPF.Common.Const;
using JNPF.VisualDev.Entitys.Enum.VisualDevModelData;
using JNPF.System.Entitys.System;
using JNPF.FriendlyException;
using JNPF.Common.Enum;
using Newtonsoft.Json.Linq;
using JNPF.WorkFlow.Interfaces.FlowTask;
using JNPF.WorkFlow.Interfaces.FlowTask.Repository;
using JNPF.Common.Model;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using SqlSugar;
using Yitter.IdGenerator;
using JNPF.ClayObject.Extensions;
using Newtonsoft.Json;

namespace JNPF.VisualDev.Core
{
    /// <summary>
    /// 在线开发运行服务
    /// </summary>
    public class RunService : IRunService, ITransient
    {
        private readonly ISqlSugarRepository<VisualDevModelDataEntity> _visualDevModelDataRepository;
        private readonly ISqlSugarRepository<VisualDevEntity> _visualDevRepository;
        private readonly IBillRullService _billRuleService;
        private readonly IOrganizeService _organizeService;
        private readonly IDepartmentService _departmentService;
        private readonly IUsersService _userService;
        private readonly IPositionService _positionService;
        private readonly IDataBaseService _databaseService;
        private readonly IDbLinkService _dbLinkService;
        private readonly ISysCacheService _sysCacheService;
        private readonly IDictionaryDataService _dictionaryDataService;
        private readonly IDataInterfaceService _dataInterfaceService;
        private readonly IDictionaryTypeService _dictionaryTypeService;
        private readonly IProvinceService _provinceService;
        private readonly IUserManager _userManager; // 用户管理

        /// <summary>
        /// 初始化一个<see cref="RunService"/>类型的新实例
        /// </summary>
        public RunService(ISqlSugarRepository<VisualDevModelDataEntity> visualDevModelDataRepository, ISqlSugarRepository<VisualDevEntity> visualDevRepository, IUserManager userManager, IBillRullService billRuleService, IOrganizeService organizeService, IUsersService userService, IPositionService positionService, IDataBaseService databaseService, IDbLinkService dbLinkService, ISysCacheService sysCacheService, IDictionaryDataService dictionaryDataService, IDataInterfaceService dataInterfaceService, IDictionaryTypeService dictionaryTypeService, IProvinceService provinceService, IDepartmentService departmentService)
        {
            _visualDevModelDataRepository = visualDevModelDataRepository;
            _visualDevRepository = visualDevRepository;
            _userManager = userManager;
            _billRuleService = billRuleService;
            _organizeService = organizeService;
            _userService = userService;
            _positionService = positionService;
            _databaseService = databaseService;
            _dbLinkService = dbLinkService;
            _sysCacheService = sysCacheService;
            _dictionaryDataService = dictionaryDataService;
            _dataInterfaceService = dataInterfaceService;
            _dictionaryTypeService = dictionaryTypeService;
            _provinceService = provinceService;
            _departmentService = departmentService;
        }

        #region Get

        /// <summary>
        /// 列表数据处理
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PageResult<Dictionary<string, object>>> GetListResult(VisualDevEntity entity, VisualDevModelListQueryInput input, string actionType = "List")
        {
            var realList = new PageResult<Dictionary<string, object>>();
            realList.list = new List<Dictionary<string, object>>();
            var list = new List<VisualDevModelDataEntity>();

            FormDataModel formDataModel = TemplateKeywordsHelper.ReplaceKeywords(entity.FormData).Deserialize<FormDataModel>();
            List<FieldsModel> formData = formDataModel.fields;

            //先模板数据缓存 解开子表无限children
            formData = TemplateCacheDataConversion(formData);
            Dictionary<string, object> templateData = await GetVisualDevTemplateData(entity.Id, formData);

            formData = formDataModel.fields;
            //剔除布局控件
            formData = TemplateDataConversion(formData);

            var columnData = TemplateKeywordsHelper.ReplaceKeywords(entity.ColumnData).Deserialize<ColumnDesignModel>();

            //列表主键
            var primaryKey = "F_Id";

            if (!string.IsNullOrEmpty(entity.Tables) && !"[]".Equals(entity.Tables))
            {
                List<TableModel> mapList = entity.Tables.ToString().Deserialize<List<TableModel>>();
                string mainTable = mapList.Find(m => m.relationTable == "").table;
                var tableList = new List<DbTableFieldModel>();
                var link = await _dbLinkService.GetInfo(entity.DbLinkId);
                tableList = _databaseService.GetFieldListByNoAsync(link, mainTable);
                var mainPrimary = tableList.Find(t => t.primaryKey == 1);
                primaryKey = mainPrimary.field;
                StringBuilder feilds = new StringBuilder();
                tableList.ForEach(item =>
                {
                    feilds.AppendFormat("{0},", item.field);
                });
                if (columnData.columnList.Count > 0)
                {
                    feilds = new StringBuilder(feilds.ToString().TrimEnd(','));
                }
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat("select {0} from {1}", feilds, mainTable);
                realList = _databaseService.GetInterFaceData(link, sql.ToString(), input, columnData, input.menuId);
                list = GetTableDataList(realList.list.ToList(), primaryKey);
            }
            else
            {
                list = await GetList(entity.Id);
            }

            input.sidx = string.IsNullOrEmpty(input.sidx) ? (columnData.defaultSidx == "" ? primaryKey : columnData.defaultSidx) : input.sidx;
            //关键字过滤
            if (list.Count > 0)
            {
                //将查询的关键字json转成Dictionary
                Dictionary<string, object> keywordJsonDic = string.IsNullOrEmpty(input.json) ? null : input.json.Deserialize<Dictionary<string, object>>();
                //将关键字查询传输的id转换成名称
                Dictionary<string, object> keyAndList = new Dictionary<string, object>();
                if ((!string.IsNullOrEmpty(entity.Tables) && "[]".Equals(entity.Tables)) || string.IsNullOrEmpty(entity.Tables))
                {
                    list = GetNoTableFilteringData(list, keywordJsonDic, formData);
                    keyAndList = await GetKeyData(formData, templateData, null, list, columnData, actionType, entity.WebType.ToInt(), primaryKey);
                    realList.list = GetQueryDataConversion(keyAndList["list"].Serialize().Deserialize<List<VisualDevModelDataEntity>>());
                    //关键字过滤
                    //realList.list = GetQueryFilteringData(keyAndList["keyJsonMap"].Serialize().Deserialize<Dictionary<string, object>>(), keyAndList["list"].Serialize().Deserialize<List<VisualDevModelDataEntity>>(), columnData);
                }
                else
                {
                    keyAndList = await GetKeyData(formData, templateData, null, list, columnData, actionType, entity.WebType.ToInt(), primaryKey);
                    realList.list = GetQueryDataConversion(keyAndList["list"].Serialize().Deserialize<List<VisualDevModelDataEntity>>());
                }
                if (input.sort == "desc")
                {
                    realList.list = realList.list.OrderByDescending(x =>
                    {
                        var dic = x as IDictionary<string, object>;
                        dic.GetOrAdd(input.sidx, () => null);
                        return dic[input.sidx];
                    }).ToList();
                }
                else
                {
                    realList.list = realList.list.OrderBy(x =>
                    {
                        var dic = x as IDictionary<string, object>;
                        dic.GetOrAdd(input.sidx, () => null);
                        return dic[input.sidx];
                    }).ToList();
                }
            }
            if (input.dataType == "0")
            {
                if (!string.IsNullOrEmpty(entity.Tables) && !"[]".Equals(entity.Tables))
                {
                }
                else
                {
                    realList.pagination = new PageResult();
                    realList.pagination.total = realList.list.Count;
                    realList.pagination.pageSize = input.pageSize;
                    realList.pagination.pageIndex = input.currentPage;
                    realList.list = realList.list.Skip(input.pageSize * (input.currentPage - 1)).Take(input.pageSize).ToList();
                }
                //分组表格
                if (columnData.type == 3)
                {
                    var groupList = columnData.columnList.Where(p => p.prop == columnData.groupField).ToList();
                    var exceptList = columnData.columnList.Except(groupList).FirstOrDefault();
                    //分组数据
                    Dictionary<string, List<Dictionary<string, object>>> groupDic = new Dictionary<string, List<Dictionary<string, object>>>();
                    foreach (var item in realList.list)
                    {
                        if (item.ContainsKey(columnData.groupField))
                        {
                            var groupDicKey = item[columnData.groupField] is null ? "" : item[columnData.groupField].ToString();
                            if (!groupDic.ContainsKey(groupDicKey))
                            {
                                groupDic.Add(groupDicKey, new List<Dictionary<string, object>>()); //初始化
                            }
                            item.Remove(columnData.groupField);
                            groupDic[groupDicKey].Add(item);
                        }
                        else
                        {
                            var groupDicKey = "null";
                            if (!groupDic.ContainsKey(groupDicKey))
                            {
                                groupDic.Add(groupDicKey, new List<Dictionary<string, object>>()); //初始化
                            }
                            groupDic[groupDicKey].Add(item);
                        }

                    }
                    List<Dictionary<string, object>> realGroupDic = new List<Dictionary<string, object>>();
                    foreach (var item in groupDic)
                    {
                        Dictionary<string, object> dataMap = new Dictionary<string, object>();
                        dataMap.Add("top", true);
                        dataMap.Add("id", YitIdHelper.NextId().ToString());
                        dataMap.Add("children", DateConver(item.Value));
                        dataMap.Add(exceptList.prop, item.Key);
                        realGroupDic.Add(dataMap);
                    }
                    realList.list = realGroupDic;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(entity.Tables) && !"[]".Equals(entity.Tables))
                {
                }
                else
                {
                    realList.pagination = new PageResult();
                    realList.pagination.total = realList.list.Count;
                    realList.pagination.pageSize = input.pageSize;
                    realList.pagination.pageIndex = input.currentPage;
                    realList.list = realList.list.ToList();
                }
            }
            realList.list = DateConver(realList.list);
            return realList;
        }

        /// <summary>
        /// 获取表主键
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<string> GetTablePrimary(VisualDevEntity entity)
        {
            var primaryKey = "F_Id";
            if (!string.IsNullOrEmpty(entity.Tables) && !"[]".Equals(entity.Tables))//
            {
                List<TableModel> mapList = entity.Tables.ToString().Deserialize<List<TableModel>>();
                string mainTable = mapList.Find(m => m.relationTable == "").table;
                var tableList = new List<DbTableFieldModel>();
                var link = await _dbLinkService.GetInfo(entity.DbLinkId);
                tableList = _databaseService.GetFieldListByNoAsync(link, mainTable);
                var mainPrimary = tableList.Find(t => t.primaryKey == 1);
                primaryKey = mainPrimary.field;
            }
            return primaryKey;
        }

        /// <summary>
        /// 获取模型数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<VisualDevModelDataEntity> GetInfo(string id)
        {
            return await _visualDevModelDataRepository.SingleAsync(m => m.Id == id);
        }

        /// <summary>
        /// 获取无表详情转换
        /// </summary>
        /// <param name="templateEntity">模板实体</param>
        /// <param name="data">真实数据</param>
        /// <returns></returns>
        public async Task<string> GetIsNoTableInfo(VisualDevEntity templateEntity, string data)
        {
            var formData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).ToObject<FormDataModel>();
            var modelList = formData.fields;
            //获取Redis缓存模板数据
            var templateData = await GetVisualDevTemplateData(templateEntity.Id, modelList);
            data = await GetSystemComponentsData(modelList, templateData, data);
            return data;
        }

        /// <summary>
        /// 获取有表详情转换
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">模板实体</param>
        /// <returns></returns>
        public async Task<string> GetHaveTableInfo(string id, VisualDevEntity templateEntity)
        {
            FormDataModel formData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).ToObject<FormDataModel>();
            List<FieldsModel> modelList = formData.fields.ToList<FieldsModel>();
            //剔除无限极
            modelList = TemplateDataConversion(modelList);
            List<TableModel> tableMapList = templateEntity.Tables.ToObject<List<TableModel>>();
            StringBuilder mainfeild = new StringBuilder();
            StringBuilder mainSql = new StringBuilder();
            var mainTable = tableMapList.Find(m => m.relationTable == "");
            var tableList = new List<DbTableFieldModel>();
            var link = await _dbLinkService.GetInfo(templateEntity.DbLinkId);
            tableList = _databaseService.GetFieldListByNoAsync(link, mainTable.table);
            var mainPrimary = tableList.Find(t => t.primaryKey == 1);
            mainSql.AppendFormat("select * from {0} where {2}='{1}';", mainTable.table, id, mainPrimary.field);
            var mainData = _databaseService.GetInterFaceData(link, mainSql.ToString()).Serialize().Deserialize<List<Dictionary<string, object>>>();
            List<Dictionary<string, object>> mainMap = GetTableDataInfoByTimeStamp(mainData, modelList, "update");
            //记录全部主表数据
            Dictionary<string, object> dataMap = mainMap.FirstOrDefault();
            Dictionary<string, object> newDataMap = new Dictionary<string, object>();
            foreach (var model in modelList)
            {
                if (!string.IsNullOrEmpty(model.__vModel__))
                {
                    if ("table".Equals(model.__config__.jnpfKey))
                    {
                        StringBuilder feilds = new StringBuilder();
                        List<FieldsModel> childModelList = model.__config__.children;
                        foreach (var childModel in childModelList)
                        {
                            feilds.Append(childModel.__vModel__ + ",");
                        }
                        if (childModelList.Count > 0)
                        {
                            feilds = new StringBuilder(feilds.ToString().TrimEnd(','));
                        }
                        //子表字段
                        string relationFeild = "";
                        //主表字段
                        string relationMainFeild = "";
                        string relationMainFeildValue = "";
                        //查询子表数据
                        StringBuilder childSql = new StringBuilder();
                        childSql.Append("select " + feilds + " from " + model.__config__.tableName + " where 1=1 ");
                        foreach (var tableMap in tableMapList)
                        {
                            if (tableMap.table.Equals(model.__config__.tableName))
                            {
                                relationFeild = tableMap.tableField;
                                relationMainFeild = tableMap.relationField;
                                if (dataMap.ContainsKey(relationMainFeild))
                                {
                                    relationMainFeildValue = dataMap[relationMainFeild].ToString();
                                    childSql.Append(@" And " + relationFeild + "='" + relationMainFeildValue + "'");
                                }
                                var childData = _databaseService.GetInterFaceData(link, childSql.ToString()).Serialize().Deserialize<List<Dictionary<string, object>>>();
                                var childTableData = GetTableDataInfoByTimeStamp(childData, model.__config__.children, "update");
                                if (childTableData.Count > 0)
                                {
                                    newDataMap[model.__vModel__] = childTableData;
                                }
                            }
                        }
                    }
                    else
                    {
                        mainfeild.Append(model.__vModel__ + ",");
                    }
                }
            }
            if (modelList.Count > 0)
            {
                mainfeild = new StringBuilder(mainfeild.ToString().TrimEnd(','));
            }
            int dicCount = newDataMap.Keys.Count;
            string[] strKey = new string[dicCount];
            newDataMap.Keys.CopyTo(strKey, 0);
            for (int i = 0; i < strKey.Length; i++)
            {
                var model = modelList.Where(m => m.__vModel__ == strKey[i]).FirstOrDefault();
                if (model != null)
                {
                    List<Dictionary<string, object>> tables = newDataMap[strKey[i]].ToObject<List<Dictionary<string, object>>>();
                    List<Dictionary<string, object>> newTables = new List<Dictionary<string, object>>();
                    foreach (var item in tables)
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        foreach (var value in item)
                        {
                            var child = model.__config__.children.Find(c => c.__vModel__ == value.Key);
                            if (child != null)
                            {
                                dic.Add(value.Key, value.Value);
                            }
                        }
                        newTables.Add(dic);
                    }
                    if (newTables.Count > 0)
                    {
                        newDataMap[strKey[i]] = newTables;
                    }
                }
            }
            foreach (var entryMap in dataMap)
            {
                if (entryMap.Value != null)
                {
                    var model = modelList.Where(m => m.__vModel__ == entryMap.Key.ToString()).FirstOrDefault();
                    if (model != null)
                    {
                        if (entryMap.Key.Equals(model.__vModel__))
                        {
                            if (mainfeild.ToString().Contains(entryMap.Key))
                            {
                                newDataMap[entryMap.Key] = entryMap.Value;
                            }
                        }
                    }
                }
            }
            return await GetTableInfoBySystemComponentsData(templateEntity.Id, modelList, newDataMap.Serialize());
        }

        /// <summary>
        /// 获取有表详情转换
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">模板实体</param>
        /// <returns></returns>
        public async Task<string> GetHaveTableInfoDetails(string id, VisualDevEntity templateEntity, bool isFlowTask = false)
        {
            FormDataModel formData = !isFlowTask ? TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).ToObject<FormDataModel>() : templateEntity.FormData.ToObject<FormDataModel>();
            List<FieldsModel> modelList = formData.fields;

            //先模板数据缓存 解开子表无限children
            modelList = TemplateCacheDataConversion(modelList);
            Dictionary<string, object> templateData = await GetVisualDevTemplateData(templateEntity.Id, modelList);
            //再将模板数据还原回原始结构
            modelList = formData.fields.ToList<FieldsModel>();

            var columnData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.ColumnData).Deserialize<ColumnDesignModel>();

            //解开模板样式控件
            modelList = TemplateDataConversion(modelList);
            List<TableModel> tableMapList = templateEntity.Tables.ToObject<List<TableModel>>();
            StringBuilder mainfeild = new StringBuilder();
            StringBuilder mainSql = new StringBuilder();
            var mainTable = tableMapList.Find(m => m.relationTable == "");
            var tableList = new List<DbTableFieldModel>();
            var link = await _dbLinkService.GetInfo(templateEntity.DbLinkId);
            tableList = _databaseService.GetFieldListByNoAsync(link, mainTable.table);
            var mainPrimary = tableList.Find(t => t.primaryKey == 1);
            mainSql.AppendFormat("select * from {0} where {2}='{1}';", mainTable.table, id, mainPrimary.field);
            var mainData = _databaseService.GetInterFaceData(link, mainSql.ToString()).ToObeject<List<Dictionary<string, object>>>();
            List<Dictionary<string, object>> mainMap = GetTableDataInfo(mainData, modelList, "detail");
            //记录全部主表数据
            Dictionary<string, object> dataMap = mainMap.FirstOrDefault();
            Dictionary<string, object> newDataMap = new Dictionary<string, object>();
            foreach (var model in modelList)
            {
                if (!string.IsNullOrEmpty(model.__vModel__))
                {
                    if ("table".Equals(model.__config__.jnpfKey))
                    {
                        StringBuilder feilds = new StringBuilder();
                        List<FieldsModel> childModelList = model.__config__.children;
                        foreach (var childModel in childModelList)
                        {
                            feilds.Append(childModel.__vModel__ + ",");
                        }
                        if (childModelList.Count > 0)
                        {
                            feilds = new StringBuilder(feilds.ToString().TrimEnd(','));
                        }
                        //子表字段
                        string relationFeild = "";
                        //主表字段
                        string relationMainFeild = "";
                        string relationMainFeildValue = "";
                        //查询子表数据
                        StringBuilder childSql = new StringBuilder();
                        childSql.Append("select " + feilds + " from " + model.__config__.tableName + " where 1=1 ");
                        foreach (var tableMap in tableMapList)
                        {
                            if (tableMap.table.Equals(model.__config__.tableName))
                            {
                                relationFeild = tableMap.tableField;
                                relationMainFeild = tableMap.relationField;
                                if (dataMap.ContainsKey(relationMainFeild))
                                {
                                    relationMainFeildValue = dataMap[relationMainFeild].ToString();
                                    childSql.Append(@" And " + relationFeild + "='" + relationMainFeildValue + "'");
                                }
                                var childData = _databaseService.GetInterFaceData(link, childSql.ToString()).Serialize().Deserialize<List<Dictionary<string, object>>>();
                                var childTableData = GetTableDataInfo(childData, model.__config__.children, "transition");
                                if (childTableData.Count > 0)
                                {
                                    newDataMap[model.__vModel__] = childTableData;
                                }
                            }
                        }
                    }
                    else
                    {
                        mainfeild.Append(model.__vModel__ + ",");
                    }
                }
            }
            if (modelList.Count > 0)
            {
                mainfeild = new StringBuilder(mainfeild.ToString().TrimEnd(','));
            }
            int dicCount = newDataMap.Keys.Count;
            string[] strKey = new string[dicCount];
            newDataMap.Keys.CopyTo(strKey, 0);
            for (int i = 0; i < strKey.Length; i++)
            {
                var model = modelList.Find(m => m.__vModel__ == strKey[i]);
                if (model != null)
                {
                    List<VisualDevModelDataEntity> childModelData = new List<VisualDevModelDataEntity>();
                    List<Dictionary<string, object>> tables = newDataMap[strKey[i]].ToObject<List<Dictionary<string, object>>>();
                    List<Dictionary<string, object>> newTables = new List<Dictionary<string, object>>();
                    foreach (var item in tables)
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        foreach (KeyValuePair<string, object> value in item)
                        {
                            var child = model.__config__.children.Find(c => c.__vModel__ == value.Key);
                            if (child != null && value.Value != null)
                            {
                                var jnpfKey = child.__config__.jnpfKey;
                                switch (jnpfKey)
                                {
                                    case "time":
                                        dic.Add(value.Key, value.Value);
                                        break;
                                    case "date":
                                        dic.Add(value.Key, Ext.TimeToTimeStamp(value.Value.ToDate()));
                                        break;
                                    default:
                                        dic.Add(value.Key, value.Value);
                                        break;
                                }

                            }
                        }
                        childModelData.Add(new VisualDevModelDataEntity()
                        {
                            Id = YitIdHelper.NextId().ToString(),
                            Data = dic.ToJson()
                        });
                    }
                    if (childModelData.Count > 0)
                    {
                        //将关键字查询传输的id转换成名称
                        Dictionary<string, object> childKeyAndList = await GetKeyData(model.__config__.children, templateData, null, childModelData, columnData);

                        foreach (var item in childKeyAndList["list"].ToObject<List<VisualDevModelDataEntity>>())
                        {
                            newTables.Add(item.Data.ToObject<Dictionary<string, object>>());
                        }

                        newDataMap[strKey[i]] = newTables;
                    }
                }
            }

            var entity = new VisualDevModelDataEntity()
            {
                Id = dataMap[mainPrimary.field].ToString(),
                Data = dataMap.ToJson()
            };
            List<VisualDevModelDataEntity> listEntity = new List<VisualDevModelDataEntity>();
            listEntity.Add(entity);
            //将关键字查询传输的id转换成名称
            Dictionary<string, object> keyAndList = await GetKeyData(modelList, templateData, null, listEntity, columnData, "detail");
            var convData = keyAndList["list"].ToObject<List<VisualDevModelDataEntity>>().FirstOrDefault().Data.ToObject<Dictionary<string, object>>();

            foreach (var entryMap in convData)
            {
                if (entryMap.Value != null)
                {
                    var model = modelList.Where(m => m.__vModel__.Contains(entryMap.Key.ToString())).FirstOrDefault();
                    if (model != null)
                    {
                        if (entryMap.Key.Equals(model.__vModel__))
                        {
                            if (mainfeild.ToString().Contains(entryMap.Key))
                            {
                                newDataMap[entryMap.Key] = entryMap.Value;
                            }
                        }
                    }
                    else
                    {
                        model = modelList.Where(m => m.__vModel__ == entryMap.Key.ToString().Replace("_id", "")).FirstOrDefault();
                        if (model != null)
                        {
                            newDataMap[entryMap.Key] = entryMap.Value;
                        }
                    }
                }
            }
            return newDataMap.ToJson();
        }

        /// <summary>
        /// 获取无表信息详情
        /// </summary>
        /// <param name="templateEntity">模板实体</param>
        /// <param name="data">真实数据</param>
        /// <returns></returns>
        public async Task<string> GetIsNoTableInfoDetails(VisualDevEntity templateEntity, VisualDevModelDataEntity data)
        {
            var formData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).ToObject<FormDataModel>();
            List<FieldsModel> modelList = formData.fields;

            //先模板数据缓存 解开子表无限children
            modelList = TemplateCacheDataConversion(modelList);
            Dictionary<string, object> templateData = await GetVisualDevTemplateData(templateEntity.Id, modelList);

            modelList = formData.fields;
            //剔除布局控件
            modelList = TemplateDataConversion(modelList);
            //模板分离子表模板
            var childTemplateList = modelList.FindAll(m => m.__config__.jnpfKey == "table");
            //删除模板内子表模板
            modelList.RemoveAll(x => x.__config__.jnpfKey == "table");

            var columnData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.ColumnData).Deserialize<ColumnDesignModel>();

            //获取真实数据
            var realData = data.Data.ToObject<Dictionary<string, object>>();
            //数据分离子表数据
            var childRealData = realData.Where(p => p.Key.Contains("tableField"));
            //获取关联表单
            var relationFormList = modelList.FindAll(m => m.__config__.jnpfKey == "relationForm");
            if (relationFormList.Count > 0)
            {
                foreach (var item in relationFormList)
                {
                    realData.Add(item.__vModel__ + "_id", null);
                }
            }
            Dictionary<string, object> newDataMap = new Dictionary<string, object>();
            //循环子表数据
            foreach (var item in childRealData)
            {
                realData.Remove(item.Key);
                List<VisualDevModelDataEntity> childModelData = new List<VisualDevModelDataEntity>();
                var childTemplate = childTemplateList.Find(c => c.__vModel__ == item.Key);
                List<Dictionary<string, object>> tables = item.Value.ToObject<List<Dictionary<string, object>>>();
                List<Dictionary<string, object>> newTables = new List<Dictionary<string, object>>();
                if (childTemplate != null)
                {
                    foreach (var childColumn in tables)
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        foreach (KeyValuePair<string, object> column in childColumn)
                        {
                            var child = childTemplate.__config__.children.Find(c => c.__vModel__ == column.Key);
                            if (child != null && column.Value != null)
                            {
                                dic.Add(column.Key, column.Value);
                            }
                        }
                        childModelData.Add(new VisualDevModelDataEntity()
                        {
                            Id = YitIdHelper.NextId().ToString(),
                            Data = dic.ToJson()
                        });
                    }
                    if (childModelData.Count > 0)
                    {
                        //将关键字查询传输的id转换成名称
                        Dictionary<string, object> childKeyAndList = await GetKeyData(childTemplate.__config__.children, templateData, null, childModelData, columnData);

                        foreach (var items in childKeyAndList["list"].ToObject<List<VisualDevModelDataEntity>>())
                        {
                            newTables.Add(items.Data.ToObject<Dictionary<string, object>>());
                        }

                        newDataMap[item.Key] = newTables;
                    }
                }
            }
            var modelData = new VisualDevModelDataEntity()
            {
                Id = data.Id.ToString(),
                Data = realData.ToJson()
            };
            List<VisualDevModelDataEntity> listEntity = new List<VisualDevModelDataEntity>();
            listEntity.Add(modelData);

            //将关键字查询传输的id转换成名称
            Dictionary<string, object> keyAndList = await GetKeyData(modelList, templateData, null, listEntity, columnData);
            var convData = keyAndList["list"].ToObject<List<VisualDevModelDataEntity>>().FirstOrDefault().Data.ToObject<Dictionary<string, object>>();

            foreach (var entryMap in convData)
            {
                if (entryMap.Value != null)
                {
                    var model = modelList.Where(m => m.__vModel__ == entryMap.Key.ToString()).FirstOrDefault();
                    if (model != null)
                    {
                        newDataMap[entryMap.Key] = entryMap.Value;
                    }
                    else
                    {
                        model = modelList.Where(m => m.__vModel__ == entryMap.Key.ToString().Replace("_id", "")).FirstOrDefault();
                        if (model != null)
                        {
                            newDataMap[entryMap.Key] = entryMap.Value;
                        }
                    }
                }
            }
            return newDataMap.ToJson();
        }

        #endregion

        #region Post

        /// <summary>
        /// 创建在线开发功能
        /// </summary>
        /// <param name="templateEntity">功能模板实体</param>
        /// <param name="dataInput">数据输入</param>
        /// <param name="isNewId">是否创建新ID</param>
        /// <returns></returns>
        public async Task Create(VisualDevEntity templateEntity, VisualDevModelDataCrInput dataInput)
        {
            if (!string.IsNullOrEmpty(templateEntity.Tables) && !"[]".Equals(templateEntity.Tables))
            {
                var mainId = YitIdHelper.NextId().ToString();
                var link = await _dbLinkService.GetInfo(templateEntity.DbLinkId);
                var haveTableSql = await CreateHaveTableSql(templateEntity, dataInput, mainId);

                try
                {
                    //开启事务
                    _visualDevModelDataRepository.Context.BeginTran();

                    //新增功能数据
                    await _databaseService.ExecuteSql(link, haveTableSql);

                    if (templateEntity.WebType == 3 && dataInput.status == 0)
                    {
                        var _flowTaskService = App.GetService<IFlowTaskService>();
                        await _flowTaskService.Submit(null, templateEntity.FlowId, mainId, null, 1, null, dataInput.data.Deserialize<JObject>(), 0, 0, false, true);
                    }

                    //关闭事务
                    _visualDevModelDataRepository.Context.CommitTran();
                }
                catch (Exception)
                {
                    _visualDevModelDataRepository.Context.RollbackTran();
                    throw;
                }
            }
            else
            {
                var allDataMap = dataInput.data.Deserialize<Dictionary<string, object>>();
                var formData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).Deserialize<FormDataModel>();
                var fieldsModelList = formData.fields;
                //剔除布局控件
                fieldsModelList = TemplateDataConversion(fieldsModelList);
                // 生成系统自动生成字段
                allDataMap = await GenerateFeilds(fieldsModelList, allDataMap, true);

                VisualDevModelDataEntity entity = new VisualDevModelDataEntity();
                entity.Data = allDataMap.Serialize();
                entity.VisualDevId = templateEntity.Id;
                try
                {
                    //开启事务
                    _visualDevModelDataRepository.Context.BeginTran();

                    //新增功能数据
                    var visualDevModelData = await _visualDevModelDataRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();

                    if (templateEntity.WebType == 3 && dataInput.status == 0)
                    {
                        var _flowTaskService = App.GetService<IFlowTaskService>();
                        await _flowTaskService.Submit(null, templateEntity.FlowId, visualDevModelData.Id, null, 1, null, dataInput.data.Deserialize<JObject>(), 0, 0, false, true);
                    }

                    //关闭事务
                    _visualDevModelDataRepository.Context.CommitTran();
                }
                catch (Exception)
                {
                    _visualDevModelDataRepository.Context.RollbackTran();
                    throw;
                }
            }
        }

        /// <summary>
        /// 创建有表SQL
        /// </summary>
        /// <param name="templateEntity"></param>
        /// <param name="dataInput"></param>
        /// <param name="mainId"></param>
        /// <returns></returns>
        public async Task<string> CreateHaveTableSql(VisualDevEntity templateEntity, VisualDevModelDataCrInput dataInput, string mainId)
        {
            var allDataMap = dataInput.data.Deserialize<Dictionary<string, object>>();
            var formData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).Deserialize<FormDataModel>();
            var fieldsModelList = formData.fields;
            //剔除布局控件
            fieldsModelList = TemplateDataConversion(fieldsModelList);
            // 生成系统自动生成字段
            allDataMap = await GenerateFeilds(fieldsModelList, allDataMap, true);

            //实例化模板信息
            var tableMapList = templateEntity.Tables.Deserialize<List<TableModel>>();
            //获取主表
            var mainTable = tableMapList.Find(t => t.relationTable == "");
            var tableList = new List<DbTableFieldModel>();
            var link = await _dbLinkService.GetInfo(templateEntity.DbLinkId);
            tableList = _databaseService.GetFieldListByNoAsync(link, mainTable.table);
            //分离主子表数据
            //先记录子表数据
            var childTableDataKey = allDataMap.Where(d => d.Key.Contains("tableField")).Select(d => d.Key).ToList();
            //记录主表数据
            var mainPrimary = tableList.Find(t => t.primaryKey == 1);
            var mainTableDataKey = allDataMap.Keys.Except(childTableDataKey).ToList();
            //主表字段集合
            StringBuilder mainFelid = new StringBuilder();
            List<string> mainFelidList = new List<string>();
            //主表查询语句
            StringBuilder mainSql = new StringBuilder();
            StringBuilder mainColumn = new StringBuilder();
            StringBuilder mainValues = new StringBuilder();
            //子表查询语句
            StringBuilder childSql = new StringBuilder();
            //拼接主表查询语句
            var mainFields = mainTable.fields;
            foreach (var item in mainTableDataKey)
            {
                var mainField = mainFields.Where(f => f.Field == item).FirstOrDefault();
                if (mainField != null)
                {
                    var allMap = allDataMap[item];
                    if (allMap != null && !string.IsNullOrEmpty(allMap.ToString()) && allMap.ToString() != "[]")
                    {
                        //Column部分
                        mainColumn.AppendFormat("{0},", item);
                        //Values部分
                        var value = TransformDataObject(allMap, item, fieldsModelList, "create");
                        mainValues.AppendFormat("'{0}',", value);
                    }
                }
            }
            //去除多余的,
            if (mainTableDataKey.Count > 0)
            {
                mainSql.AppendFormat("insert into {0} ({3}{1}) values('{4}'{2});", mainTable.table, (mainColumn.Length > 0 ? "," : "") + mainColumn.ToString().Trim(','), (mainValues.Length > 0 ? "," : "") + mainValues.ToString().Trim(','), mainPrimary.field, mainId);
            }
            //拼接子表sql
            foreach (var item in childTableDataKey)
            {
                //查找到该控件数据
                var objectData = allDataMap[item];
                var model = objectData.Serialize().Deserialize<List<Dictionary<string, object>>>();
                if (model.Count > 0)
                {
                    //利用key去找模板
                    var fieldsModel = fieldsModelList.Find(f => f.__vModel__ == item);
                    var fieldsConfig = fieldsModel.__config__;
                    model = GetTableDataListByDic(model, fieldsConfig.children);
                    StringBuilder childColumn = new StringBuilder();
                    StringBuilder childValues = new StringBuilder();
                    var childTable = tableMapList.Find(t => t.table == fieldsModel.__config__.tableName);
                    tableList = new List<DbTableFieldModel>();
                    tableList = _databaseService.GetFieldListByNoAsync(link, childTable.table);
                    var childPrimary = tableList.Find(t => t.primaryKey == 1);
                    foreach (var data in model)
                    {
                        if (data.Count > 0)
                        {
                            foreach (KeyValuePair<string, object> child in data)
                            {
                                if (child.Value != null && child.Value.ToString() != "[]" && child.Value.ToString() != "")
                                {
                                    //Column部分
                                    childColumn.AppendFormat("{0},", child.Key);
                                    //Values部分
                                    var value = TransformDataObject(child.Value, child.Key, fieldsConfig.children, "create");
                                    childValues.AppendFormat("'{0}',", value);
                                }
                            }
                            if (!string.IsNullOrEmpty(childColumn.ToString()))
                            {
                                childSql.AppendFormat("insert into {0}({6},{4},{1}) values('{3}','{5}',{2});", fieldsModel.__config__.tableName, childColumn.ToString().Trim(','), childValues.ToString().Trim(','), YitIdHelper.NextId().ToString(), childTable.tableField, mainId, childPrimary.field);
                            }
                            childColumn = new StringBuilder();
                            childValues = new StringBuilder();
                        }
                    }
                }
            }

            return mainSql.ToString() + childSql.ToString();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id">修改ID</param>
        /// <param name="visualdevEntity"></param>
        /// <param name="visualdevModelDataUpForm"></param>
        /// <returns></returns>
        public async Task Update(string id, VisualDevEntity templateEntity, VisualDevModelDataUpInput visualdevModelDataUpForm)
        {
            if (!string.IsNullOrEmpty(templateEntity.Tables) && !"[]".Equals(templateEntity.Tables))
            {
                var link = await _dbLinkService.GetInfo(templateEntity.DbLinkId);
                var haveTableSql = await UpdateHaveTableSql(templateEntity, visualdevModelDataUpForm, id);

                try
                {
                    //开启事务
                    _visualDevModelDataRepository.Context.BeginTran();

                    //修改功能数据
                    await _databaseService.ExecuteSql(link, haveTableSql);

                    if (templateEntity.WebType == 3 && visualdevModelDataUpForm.status == 0)
                    {
                        var _flowTaskRepository = App.GetService<IFlowTaskRepository>();
                        var _flowTaskService = App.GetService<IFlowTaskService>();
                        var taskEntity = await _flowTaskRepository.GetTaskInfo(id);
                        if (taskEntity == null)
                            await _flowTaskService.Submit(null, templateEntity.FlowId, id, null, 1, null, visualdevModelDataUpForm.data.Deserialize<JObject>(), 0, 0, false, true);
                        else
                            await _flowTaskService.Submit(id, templateEntity.FlowId, id, null, 1, null, visualdevModelDataUpForm.data.Deserialize<JObject>(), 0, 0, false, true);
                    }

                    //关闭事务
                    _visualDevModelDataRepository.Context.CommitTran();
                }
                catch (Exception)
                {
                    _visualDevModelDataRepository.Context.RollbackTran();
                    throw;
                }
            }
            else
            {
                Dictionary<string, object> allDataMap = visualdevModelDataUpForm.data.ToObject<Dictionary<string, object>>();
                FormDataModel formData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).ToObject<FormDataModel>();
                List<FieldsModel> fieldsModelList = formData.fields.ToObject<List<FieldsModel>>();
                fieldsModelList = TemplateDataConversion(fieldsModelList);
                //生成系统自动生成字段
                allDataMap = await GenerateFeilds(fieldsModelList, allDataMap, false);

                VisualDevModelDataEntity entity = new VisualDevModelDataEntity();
                entity.Data = allDataMap.Serialize();
                entity.VisualDevId = templateEntity.Id;
                entity.Id = id;
                try
                {
                    //开启事务
                    _visualDevModelDataRepository.Context.BeginTran();

                    //修改功能数据
                    await _visualDevModelDataRepository.Context.Updateable(entity).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();

                    if (templateEntity.WebType == 3 && visualdevModelDataUpForm.status == 0)
                    {
                        var _flowTaskRepository = App.GetService<IFlowTaskRepository>();
                        var _flowTaskService = App.GetService<IFlowTaskService>();
                        var taskEntity = await _flowTaskRepository.GetTaskInfo(id);
                        if (taskEntity == null)
                            await _flowTaskService.Submit(null, templateEntity.FlowId, id, null, 1, null, visualdevModelDataUpForm.data.Deserialize<JObject>(), 0, 0, true);
                        else
                            await _flowTaskService.Submit(id, templateEntity.FlowId, id, null, 1, null, visualdevModelDataUpForm.data.Deserialize<JObject>(), 0, 0, true);
                    }

                    //关闭事务
                    _visualDevModelDataRepository.Context.CommitTran();
                }
                catch (Exception)
                {
                    _visualDevModelDataRepository.Context.RollbackTran();
                    throw;
                }
            }
        }

        /// <summary>
        /// 修改有表SQL
        /// </summary>
        /// <param name="templateEntity"></param>
        /// <param name="visualdevModelDataUpForm"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> UpdateHaveTableSql(VisualDevEntity templateEntity, VisualDevModelDataUpInput visualdevModelDataUpForm, string id)
        {
            Dictionary<string, object> allDataMap = visualdevModelDataUpForm.data.ToObject<Dictionary<string, object>>();
            FormDataModel formData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).ToObject<FormDataModel>();
            List<FieldsModel> fieldsModelList = formData.fields.ToObject<List<FieldsModel>>();
            fieldsModelList = TemplateDataConversion(fieldsModelList);
            //生成系统自动生成字段
            allDataMap = await GenerateFeilds(fieldsModelList, allDataMap, false);

            List<TableModel> tableMapList = templateEntity.Tables.ToObject<List<TableModel>>();
            //循环表
            var mainTable = tableMapList.Where(t => t.relationTable == "").FirstOrDefault();
            var tableList = new List<DbTableFieldModel>();
            var link = await _dbLinkService.GetInfo(templateEntity.DbLinkId);
            tableList = _databaseService.GetFieldListByNoAsync(link, mainTable.table);
            var mainPrimary = tableList.Find(t => t.primaryKey == 1);
            StringBuilder delMain = new StringBuilder();
            delMain.AppendFormat("delete from {0} where {2}='{1}';", mainTable.table, id, mainPrimary.field);
            StringBuilder allDelSql = new StringBuilder();
            allDelSql.AppendFormat("{0}", delMain.ToString());
            StringBuilder queryMain = new StringBuilder();
            queryMain.AppendFormat("select * from {0} where {2}='{1}';", mainTable.table, id, mainPrimary.field);
            var mainData = _databaseService.GetInterFaceData(link, queryMain.ToString()).Serialize().Deserialize<List<Dictionary<string, object>>>();
            Dictionary<string, object> mainMap = GetTableDataInfo(mainData, fieldsModelList, "List").FirstOrDefault();
            if (tableMapList.Count > 1)
            {
                //去除主表,剩余的为子表，再进行子表删除语句生成
                tableMapList.RemoveAt(0);
                foreach (var tableMap in tableMapList)
                {
                    //主表字段
                    //与主表关联字段
                    string relationField = tableMap.relationField;
                    var relationFieldValue = mainMap.Where(f => f.Key == relationField).FirstOrDefault();
                    //子表字段
                    string tableField = tableMap.tableField;
                    allDelSql.AppendFormat("delete from {0} where {1}='{2}';", tableMap.table, tableField, relationFieldValue.Value);
                }
            }
            string mainId = id;
            //分离主子表数据
            //先记录子表数据
            var childTableDataKey = allDataMap.Where(d => d.Key.Contains("tableField")).Select(d => d.Key).ToList();
            //记录主表数据
            var mainTableDataKey = allDataMap.Keys.Except(childTableDataKey).ToList();
            //实例化模板信息
            //主表字段集合
            StringBuilder mainFelid = new StringBuilder();
            List<string> mainFelidList = new List<string>();
            //主表查询语句
            StringBuilder mainSql = new StringBuilder();
            StringBuilder mainColumn = new StringBuilder();
            StringBuilder mainValues = new StringBuilder();
            //子表查询语句
            StringBuilder childSql = new StringBuilder();
            //拼接主表插入语句
            var mainFields = mainTable.fields;
            foreach (var item in mainTableDataKey)
            {
                var mainField = mainFields.Where(f => f.Field == item).FirstOrDefault();
                if (mainField != null)
                {
                    var allMap = allDataMap[item];
                    if (allMap != null && !string.IsNullOrEmpty(allMap.ToString()) && !allMap.ToString().Contains("[]"))
                    {
                        //Column部分
                        mainColumn.AppendFormat("{0},", item);
                        //Values部分
                        mainValues.AppendFormat("'{0}',", TransformDataObject(allMap, item, fieldsModelList, "create"));
                    }
                }
            }
            //去除多余的,
            if (mainTableDataKey.Count > 0)
            {
                mainSql.AppendFormat("insert into {0} ({3}{1}) values('{4}'{2});", mainTable.table, (mainColumn.Length > 0 ? "," : "") + mainColumn.ToString().Trim(','), (mainValues.Length > 0 ? "," : "") + mainValues.ToString().Trim(','), mainPrimary.field, mainId);
            }
            //拼接子表sql
            foreach (var item in childTableDataKey)
            {
                //查找到该控件数据
                var model = allDataMap[item].ToJson().ToList<Dictionary<string, object>>();
                if (model.Count > 0)
                {
                    //利用key去找模板
                    var fieldsModel = fieldsModelList.Find(f => f.__vModel__ == item);
                    var fieldsConfig = fieldsModel.__config__;
                    model = GetTableDataListByDic(model, fieldsConfig.children);
                    StringBuilder childColumn = new StringBuilder();
                    StringBuilder childValues = new StringBuilder();
                    var childTable = tableMapList.Where(t => t.table == fieldsConfig.tableName).FirstOrDefault();
                    tableList = new List<DbTableFieldModel>();
                    tableList = _databaseService.GetFieldListByNoAsync(link, childTable.table);
                    var childPrimary = tableList.Find(t => t.primaryKey == 1);
                    foreach (var data in model)
                    {
                        if (data.Count > 0)
                        {
                            foreach (KeyValuePair<string, object> child in data)
                            {
                                if (child.Value != null)
                                {
                                    //Column部分
                                    childColumn.AppendFormat("{0},", child.Key);
                                    //Values部分
                                    childValues.AppendFormat("'{0}',", TransformDataObject(child.Value, child.Key, fieldsConfig.children, "create"));
                                }
                            }
                            if (!string.IsNullOrEmpty(childColumn.ToString()))
                            {
                                childSql.AppendFormat("insert into {0}({6},{4},{1}) values('{3}','{5}',{2});", fieldsModel.__config__.tableName, childColumn.ToString().Trim(','), childValues.ToString().Trim(','), YitIdHelper.NextId().ToString(), childTable.tableField, mainId, childPrimary.field);
                            }
                            childColumn = new StringBuilder();
                            childValues = new StringBuilder();
                        }
                    }
                }
            }
            return allDelSql.ToString() + mainSql.ToString() + childSql.ToString();
        }

        #endregion

        #region PrivateMethod

        #region 拆解模板

        /// <summary>
        /// 模板缓存数据转换
        /// 专门为模板缓存数据，会将子表内的控件全部获取出来
        /// 适用场景缓存模板数据
        /// </summary>
        /// <returns></returns>
        private List<FieldsModel> TemplateCacheDataConversion(List<FieldsModel> fieldsModelList)
        {
            var template = new List<FieldsModel>();
            //将模板内的无限children解析出来
            //包含子表children
            foreach (var item in fieldsModelList)
            {
                var config = item.__config__;
                switch (config.jnpfKey)
                {
                    //栅格布局
                    case "row":
                        {
                            template.AddRange(TemplateCacheDataConversion(config.children));
                        }
                        break;
                    //表格
                    case "table":
                        {
                            template.AddRange(TemplateCacheDataConversion(config.children));
                        }
                        break;
                    //卡片
                    case "card":
                        {
                            template.AddRange(TemplateCacheDataConversion(config.children));
                        }
                        break;
                    //折叠面板
                    case "collapse":
                        {
                            foreach (var collapse in config.children)
                            {
                                template.AddRange(TemplateCacheDataConversion(collapse.__config__.children));
                            }
                        }
                        break;
                    //tab标签
                    case "tab":
                        {
                            foreach (var collapse in config.children)
                            {
                                template.AddRange(TemplateCacheDataConversion(collapse.__config__.children));
                            }
                        }
                        break;
                    //文本
                    case "JNPFText":
                        break;
                    //分割线
                    case "divider":
                        break;
                    //分组标题
                    case "groupTitle":
                        break;
                    default:
                        {
                            template.Add(item);
                        }
                        break;
                }
            }
            return template;
        }

        #endregion

        #region 解析模板数据

        /// <summary>
        /// 控制模板数据转换
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="fieldsModel">数据模板</param>
        /// <param name="actionType">操作类型(List-列表值,create-创建值,update-更新值,detail-详情值,transition-过渡值,query-查询)</param>
        /// <returns></returns>
        private object TemplateControlsDataConversion(object data, FieldsModel fieldsModel, string actionType = null)
        {
            try
            {
                object conversionData = new object();
                switch (fieldsModel.__config__.jnpfKey)
                {
                    #region 基础控件

                    //单行输入
                    case "comInput":
                        {
                            conversionData = string.IsNullOrEmpty(data.ToString()) ? null : data.ToString();
                        }
                        break;
                    //多行输入
                    case "textarea":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                    //数字输入
                    case "numInputc":
                        {
                            conversionData = data.ToInt();
                        }
                        break;
                    //金额输入
                    case "JNPFAmount":
                        {
                            conversionData = data.ToDecimal();
                        }
                        break;
                    //单选框组
                    case "radio":
                        {
                            conversionData = string.IsNullOrEmpty(data.ToString()) ? null : data.ToString();
                        }
                        break;
                    //多选框组
                    case "checkbox":
                        {
                            if (data.ToString().Contains("["))
                                conversionData = data.ToString().ToObject<List<string>>();
                            else
                                conversionData = data.ToString();
                        }
                        break;
                    //下拉选择
                    case "select":
                        {
                            switch (actionType)
                            {
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                case "List":
                                    {
                                        if (data.ToString().Contains(","))
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        if (fieldsModel.multiple && actionType != "query")
                                        {
                                            if (data.ToString().Contains(","))
                                            {
                                                conversionData = data.ToString().ToObject<List<string>>();
                                            }
                                            else
                                            {
                                                conversionData = string.Join(",", data.ToString().Split(',').ToArray());
                                            }
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    //时间选择
                    case "time":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", data);
                                    break;
                                case "create":
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", data);
                                    break;
                                case "detail":
                                    conversionData = data.ToString();
                                    break;
                                default:
                                    conversionData = data.ToString();
                                    break;
                            }
                        }
                        break;
                    //时间范围
                    case "timeRange":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    conversionData = data.ToString().ToObject<List<string>>();
                                    break;
                                case "create":
                                    conversionData = data.ToString().ToObject<List<string>>();
                                    break;
                                case "transition":
                                    conversionData = data;
                                    break;
                                case "update":
                                    conversionData = data.ToString().ToObject<List<string>>();
                                    break;
                            }
                        }
                        break;
                    //日期选择
                    case "date":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Ext.GetDateTime(data.ToString()));
                                    break;
                                case "create":
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Ext.GetDateTime(data.ToString()));
                                    break;
                                case "detail":
                                    conversionData = data.ToString();
                                    break;
                                case "update":
                                    conversionData = data;
                                    break;
                                default:
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Ext.GetDateTime(data.ToString()));
                                    break;
                            }
                        }
                        break;
                    //日期范围
                    case "dateRange":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    conversionData = data.ToString().ToObject<List<string>>();
                                    break;
                                case "transition":
                                    conversionData = data;
                                    break;
                                case "create":
                                    conversionData = data.ToString().ToObject<List<string>>();
                                    break;
                                case "update":
                                    conversionData = data.ToString().ToObject<List<string>>();
                                    break;
                            }
                        }
                        break;
                    //创建时间
                    case "createTime":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Ext.GetDateTime(data.ToString()));
                                    break;
                                case "create":
                                    conversionData = data.ToString();
                                    break;
                                case "update":
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Ext.GetDateTime(data.ToString()));
                                    break;
                                case "detail":
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", data);
                                    break;
                                default:
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", data);
                                    break;
                            }
                        }
                        break;
                    //修改时间
                    case "modifyTime":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Ext.GetDateTime(data.ToString()));
                                    break;
                                case "create":
                                    conversionData = data.ToString();
                                    break;
                                case "update":
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Ext.GetDateTime(data.ToString()));
                                    break;
                                default:
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", data);
                                    break;
                            }
                        }
                        break;
                    //文件上传
                    case "uploadFz":
                        {
                            if (data.ToString() != "[]")
                            {
                                conversionData = data.ToString().ToObject<List<FileControlsModel>>();
                            }
                            else
                            {
                                conversionData = null;
                            }
                        }
                        break;
                    //图片上传
                    case "uploadImg":
                        {
                            if (data.ToString() != "[]")
                            {
                                conversionData = data.ToString().ToObject<List<FileControlsModel>>();
                            }
                            else
                            {
                                conversionData = null;
                            }
                        }
                        break;
                    //颜色选择
                    case "colorPicker":
                        {
                            conversionData = string.IsNullOrEmpty(data.ToString()) ? null : data.ToString();
                        }
                        break;
                    //评分
                    case "rate":
                        {
                            conversionData = data.ToInt();
                        }
                        break;
                    //开关
                    case "switch":
                        {
                            conversionData = data.ToInt();
                        }
                        break;
                    //滑块
                    case "slider":
                        {
                            if (fieldsModel.range)
                            {
                                conversionData = data.ToString().ToObject<List<int>>();
                            }
                            else
                            {
                                conversionData = data.ToInt();
                            };
                        }
                        break;
                    ////文本
                    //case "JNPFText":
                    //    break;
                    //富文本
                    case "editor":
                        {
                            conversionData = string.IsNullOrEmpty(data.ToString()) ? null : data.ToString();
                        }
                        break;
                    //单据组件
                    case "billRule":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                    //省市区联动
                    case "address":
                        {
                            switch (actionType)
                            {
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                default:
                                    conversionData = data.ToString().ToObject<List<string>>();
                                    break;
                            }
                        }
                        break;
                    //创建人员
                    case "createUser":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                    //修改人员
                    case "modifyUser":
                        {
                            conversionData = string.IsNullOrEmpty(data.ToString()) ? null : data.ToString();
                        }
                        break;
                    //所属组织
                    case "currOrganize":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                    //所属部门
                    case "currDept":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                    //所属岗位
                    case "currPosition":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                    case "table":
                        {

                        }
                        break;
                    //级联
                    case "cascader":
                        switch (actionType)
                        {
                            case "transition":
                                {
                                    conversionData = data;
                                }
                                break;
                            default:
                                {
                                    if (fieldsModel.props.props.multiple)
                                    {
                                        conversionData = data.ToString().ToObject<List<List<string>>>();
                                    }
                                    else
                                    {
                                        conversionData = data.ToString().ToObject<List<string>>();
                                    }
                                }
                                break;
                        }
                        break;
                    default:
                        conversionData = data.ToString();
                        break;

                    #endregion

                    #region 高级控件

                    //公司组件
                    case "comSelect":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "create":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "update":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            var list = data.ToString().ToObject<List<string>>();
                                            conversionData = string.Join(",", list.ToArray());
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "detail":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                default:
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    //部门组件
                    case "depSelect":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "create":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "update":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            var list = data.ToString().ToObject<List<string>>();
                                            conversionData = string.Join(",", list.ToArray());
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "detail":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                default:
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    //岗位组件
                    case "posSelect":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "create":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "update":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            var list = data.ToString().ToObject<List<string>>();
                                            conversionData = string.Join(",", list.ToArray());
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "detail":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                default:
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    //用户组件
                    case "userSelect":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "create":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "update":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            var list = data.ToString().ToObject<List<string>>();
                                            conversionData = string.Join(",", list.ToArray());
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "detail":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                default:
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    //树形选择
                    case "treeSelect":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "create":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "update":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            var list = data.ToString().ToObject<List<string>>();
                                            conversionData = string.Join(",", list.ToArray());
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "detail":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                default:
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    //弹窗选择
                    case "popupSelect":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                        #endregion

                }
                return conversionData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 转换对应数据Object
        /// </summary>
        /// <param name="dataValue">数据值</param>
        /// <param name="dataKey">数据Key</param>
        /// <param name="fieldsModelList">模板</param>
        /// <param name="actionType">操作类型(List-列表值,create-创建值,detail-详情值,transition-过渡值)</param>
        /// <returns></returns>
        private string TransformDataObject(object dataValue, string dataKey, List<FieldsModel> fieldsModelList, string actionType = null)
        {
            StringBuilder sb = new StringBuilder();
            //根据KEY查找模板
            var model = fieldsModelList.Find(f => f.__vModel__ == dataKey);
            if (model != null)
            {
                switch (model.__config__.jnpfKey)
                {
                    //文件上传
                    case "uploadFz":
                        {
                            var fileList = TemplateControlsDataConversion(dataValue, model);
                            if (fileList != null)
                            {
                                sb.AppendFormat("{0}", fileList.Serialize());
                            }
                        }
                        break;
                    //图片上传
                    case "uploadImg":
                        {
                            var fileList = TemplateControlsDataConversion(dataValue, model);
                            if (fileList != null)
                            {
                                sb.AppendFormat("{0}", fileList.Serialize());
                            }
                        }
                        break;
                    //省市区联动
                    case "address":
                        {
                            sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                        }
                        break;
                    //树形选择
                    case "treeSelect":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model));
                            }
                        }
                        break;
                    //公司组件
                    case "comSelect":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model));
                            }
                        }
                        break;
                    //部门组件
                    case "depSelect":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model));
                            }
                        }
                        break;
                    //岗位组件
                    case "posSelect":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model));
                            }
                        }
                        break;
                    //用户组件
                    case "userSelect":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model));
                            }
                        }
                        break;
                    //滑块
                    case "slider":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model));
                            }
                        }
                        break;
                    //时间范围
                    case "timeRange":
                        {
                            sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType).Serialize());
                        }
                        break;
                    //日期范围
                    case "dateRange":
                        {
                            sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType).Serialize());
                        }
                        break;
                    //下拉选择
                    case "select":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model));
                            }
                        }
                        break;
                    //复选框
                    case "checkbox":
                        {
                            sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                        }
                        break;
                    //级联
                    case "cascader":
                        {
                            sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                        }
                        break;
                    //日期选择
                    case "date":
                        {
                            try
                            {
                                DateTime.Parse(dataValue.ToString());
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(Ext.TimeToTimeStamp(Convert.ToDateTime(dataValue)), model, actionType));
                            }
                            catch
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType));
                            }
                        }
                        break;
                    default:
                        sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType));
                        break;
                }
            }
            if (dataKey == "F_Id")
            {
                sb.AppendFormat("{0}", dataValue.ToString());
            }
            return sb.ToString().TrimStart('"').TrimEnd('"');
        }

        #endregion

        #region 缓存模板数据

        /// <summary>
        /// 获取可视化开发模板可缓存数据
        /// </summary>
        /// <param name="moldelId">模型id</param>
        /// <param name="formData">模板数据结构</param>
        /// <returns></returns>
        private async Task<Dictionary<string, object>> GetVisualDevTemplateData(string moldelId, List<FieldsModel> formData)
        {
            Dictionary<string, object> templateData = new Dictionary<string, object>();
            var cacheKey = CommonConst.VISUALDEV + _userManager.TenantId + "_" + moldelId;
            if (_sysCacheService.Exists(cacheKey))
            {
                templateData = _sysCacheService.Get(cacheKey).Deserialize<Dictionary<string, object>>();
            }
            else
            {
                foreach (var model in formData)
                {
                    if (model != null && model.__vModel__ != null)
                    {
                        ConfigModel configModel = model.__config__;
                        string fieldName1 = model.__vModel__;
                        string type = configModel.jnpfKey;
                        switch (type)
                        {
                            //单选框
                            case JnpfKeyConst.RADIO:
                                {
                                    if (vModelType.DICTIONARY.GetDescription() == configModel.dataType)
                                    {
                                        var dictionaryDataEntityList = string.IsNullOrEmpty(configModel.dictionaryType) ? null : await _dictionaryDataService.GetList(configModel.dictionaryType);
                                        List<Dictionary<string, string>> dictionaryDataList = new List<Dictionary<string, string>>();
                                        foreach (var item in dictionaryDataEntityList)
                                        {
                                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                                            dictionary.Add(item.Id, item.FullName);
                                            dictionaryDataList.Add(dictionary);
                                        }
                                        templateData.Add(fieldName1, dictionaryDataList);
                                    }
                                    if (vModelType.STATIC.GetDescription() == configModel.dataType)
                                    {
                                        var optionList = model.__slot__.options;
                                        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                        foreach (var item in optionList)
                                        {
                                            Dictionary<string, string> option = new Dictionary<string, string>();
                                            option.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                            list.Add(option);
                                        }
                                        templateData.Add(fieldName1, list);
                                    }
                                    if (vModelType.DYNAMIC.GetDescription() == configModel.dataType)
                                    {
                                        //获取远端数据
                                        var dynamic = await _dataInterfaceService.GetInfo(model.__config__.propsUrl);
                                        if (1.Equals(dynamic.DataType))
                                        {
                                            var linkEntity = await _dbLinkService.GetInfo(dynamic.DBLinkId);
                                            var dt = _databaseService.GetInterFaceData(linkEntity, dynamic.Query);
                                            List<Dictionary<string, object>> dynamicDataList = dt.Serialize().Deserialize<List<Dictionary<string, object>>>();
                                            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                            foreach (var item in dynamicDataList)
                                            {
                                                Dictionary<string, string> dynamicDic = new Dictionary<string, string>();
                                                dynamicDic.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                                list.Add(dynamicDic);
                                            }
                                            templateData.Add(fieldName1, list);
                                        }
                                    }
                                }
                                break;
                            //下拉框
                            case JnpfKeyConst.SELECT:
                                {
                                    if (vModelType.DICTIONARY.GetDescription() == configModel.dataType)
                                    {
                                        List<DictionaryDataEntity> dictionaryDataEntityList = string.IsNullOrEmpty(configModel.dictionaryType) ? null : await _dictionaryDataService.GetList(configModel.dictionaryType);
                                        List<Dictionary<string, string>> dictionaryDataList = new List<Dictionary<string, string>>();
                                        foreach (var item in dictionaryDataEntityList)
                                        {
                                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                                            dictionary.Add(item.Id, item.FullName);
                                            dictionaryDataList.Add(dictionary);
                                        }
                                        templateData.Add(fieldName1, dictionaryDataList);
                                    }
                                    else if (vModelType.STATIC.GetDescription() == configModel.dataType)
                                    {
                                        var optionList = model.__slot__.options;
                                        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                        foreach (var item in optionList)
                                        {
                                            Dictionary<string, string> option = new Dictionary<string, string>();
                                            option.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                            list.Add(option);
                                        }
                                        templateData.Add(fieldName1, list);
                                    }
                                    else if (vModelType.DYNAMIC.GetDescription() == configModel.dataType)
                                    {
                                        //获取远端数据
                                        DataInterfaceEntity dynamic = await _dataInterfaceService.GetInfo(model.__config__.propsUrl);
                                        if (1.Equals(dynamic.DataType))
                                        {
                                            var linkEntity = await _dbLinkService.GetInfo(dynamic.DBLinkId);
                                            var dt = _databaseService.GetInterFaceData(linkEntity, dynamic.Query);
                                            List<Dictionary<string, object>> dynamicDataList = dt.Serialize().Deserialize<List<Dictionary<string, object>>>();
                                            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                            foreach (var item in dynamicDataList)
                                            {
                                                Dictionary<string, string> dynamicDic = new Dictionary<string, string>();
                                                dynamicDic.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                                list.Add(dynamicDic);
                                            }
                                            templateData.Add(fieldName1, list);
                                        }
                                    }
                                }
                                break;
                            //复选框
                            case JnpfKeyConst.CHECKBOX:
                                {
                                    if (vModelType.DICTIONARY.GetDescription() == configModel.dataType)
                                    {
                                        List<DictionaryDataEntity> dictionaryDataEntityList = string.IsNullOrEmpty(configModel.dictionaryType) ? null : await _dictionaryDataService.GetList(configModel.dictionaryType);
                                        List<Dictionary<string, string>> dictionaryDataList = new List<Dictionary<string, string>>();
                                        foreach (var item in dictionaryDataEntityList)
                                        {
                                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                                            dictionary.Add(item.Id, item.FullName);
                                            dictionaryDataList.Add(dictionary);
                                        }
                                        templateData.Add(fieldName1, dictionaryDataList);
                                    }
                                    if (vModelType.STATIC.GetDescription() == configModel.dataType)
                                    {
                                        var optionList = model.__slot__.options;
                                        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                        foreach (var item in optionList)
                                        {
                                            Dictionary<string, string> option = new Dictionary<string, string>();
                                            option.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                            list.Add(option);
                                        }
                                        templateData.Add(fieldName1, list);
                                    }
                                    if (vModelType.DYNAMIC.GetDescription() == configModel.dataType)
                                    {
                                        //获取远端数据
                                        DataInterfaceEntity dynamic = await _dataInterfaceService.GetInfo(model.__config__.propsUrl);
                                        if (1.Equals(dynamic.DataType))
                                        {
                                            var linkEntity = await _dbLinkService.GetInfo(dynamic.DBLinkId);
                                            var dt = _databaseService.GetInterFaceData(linkEntity, dynamic.Query);
                                            List<Dictionary<string, object>> dynamicDataList = dt.Serialize().Deserialize<List<Dictionary<string, object>>>();
                                            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                            foreach (var item in dynamicDataList)
                                            {
                                                Dictionary<string, string> dynamicDic = new Dictionary<string, string>();
                                                dynamicDic.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                                list.Add(dynamicDic);
                                            }
                                            templateData.Add(fieldName1, list);
                                        }
                                    }
                                }
                                break;
                            //树形选择
                            case JnpfKeyConst.TREESELECT:
                                {
                                    if (vModelType.DICTIONARY.GetDescription() == configModel.dataType)
                                    {
                                        List<DictionaryDataEntity> dictionaryDataEntityList = await _dictionaryDataService.GetList();
                                        List<Dictionary<string, string>> dictionaryDataList = new List<Dictionary<string, string>>();
                                        foreach (var item in dictionaryDataEntityList)
                                        {
                                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                                            dictionary.Add(item.Id, item.FullName);
                                            dictionaryDataList.Add(dictionary);
                                        }
                                        templateData.Add(fieldName1, dictionaryDataList);
                                    }
                                    else if (vModelType.STATIC.GetDescription() == configModel.dataType)
                                    {
                                        var props = model.props.props;
                                        var optionList = GetTreeOptions(model.options, props);
                                        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                        foreach (var item in optionList)
                                        {
                                            Dictionary<string, string> option = new Dictionary<string, string>();
                                            option.Add(item.value, item.label);
                                            list.Add(option);
                                        }
                                        templateData.Add(fieldName1, list);
                                    }
                                    else if (vModelType.DYNAMIC.GetDescription() == configModel.dataType)
                                    {
                                        //获取远端数据
                                        DataInterfaceEntity dynamic = await _dataInterfaceService.GetInfo(model.__config__.propsUrl);
                                        if (1.Equals(dynamic.DataType))
                                        {
                                            var linkEntity = await _dbLinkService.GetInfo(dynamic.DBLinkId);
                                            var dt = _databaseService.GetInterFaceData(linkEntity, dynamic.Query);
                                            List<Dictionary<string, object>> dynamicDataList = dt.Serialize().Deserialize<List<Dictionary<string, object>>>();
                                            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                            foreach (var item in dynamicDataList)
                                            {
                                                Dictionary<string, string> dynamicDic = new Dictionary<string, string>();
                                                dynamicDic.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                                list.Add(dynamicDic);
                                            }
                                            templateData.Add(fieldName1, list);
                                        }
                                    }
                                }
                                break;
                            //公司
                            case JnpfKeyConst.COMSELECT:
                                {
                                    var com_organizeEntityList = await _organizeService.GetCompanyListAsync();
                                    List<Dictionary<string, string>> com_organizeList = new List<Dictionary<string, string>>();
                                    foreach (var item in com_organizeEntityList)
                                    {
                                        Dictionary<string, string> com_organize = new Dictionary<string, string>();
                                        com_organize.Add(item.Id, item.FullName);
                                        com_organizeList.Add(com_organize);
                                    }
                                    templateData.Add(fieldName1, com_organizeList);
                                }
                                break;
                            //部门
                            case JnpfKeyConst.DEPSELECT:
                                {
                                    var dep_organizeEntityList = await _departmentService.GetListAsync();
                                    List<Dictionary<string, string>> dep_organizeList = new List<Dictionary<string, string>>();
                                    foreach (var item in dep_organizeEntityList)
                                    {
                                        Dictionary<string, string> dep_organize = new Dictionary<string, string>();
                                        dep_organize.Add(item.Id, item.FullName);
                                        dep_organizeList.Add(dep_organize);
                                    }
                                    templateData.Add(fieldName1, dep_organizeList);
                                }
                                break;
                            //岗位
                            case JnpfKeyConst.POSSELECT:
                                {
                                    var positionEntityList = await _positionService.GetListAsync();
                                    List<Dictionary<string, string>> positionList = new List<Dictionary<string, string>>();
                                    foreach (var item in positionEntityList)
                                    {
                                        Dictionary<string, string> position = new Dictionary<string, string>();
                                        position.Add(item.Id, item.FullName);
                                        positionList.Add(position);
                                    }
                                    templateData.Add(fieldName1, positionList);
                                }
                                break;
                            //用户
                            case JnpfKeyConst.USERSELECT:
                                {
                                    var userEntityList = await _userService.GetList();
                                    List<Dictionary<string, string>> userList = new List<Dictionary<string, string>>();
                                    foreach (var item in userEntityList)
                                    {
                                        Dictionary<string, string> user = new Dictionary<string, string>();
                                        user.Add(item.Id, item.RealName + "/" + item.Account);
                                        userList.Add(user);
                                    }
                                    templateData.Add(fieldName1, userList);
                                }
                                break;
                            //数据字典
                            case JnpfKeyConst.DICTIONARY:
                                {
                                    var dictionaryTypeEntityLists = await _dictionaryTypeService.GetList();
                                    List<Dictionary<string, string>> dictionaryTypeList = new List<Dictionary<string, string>>();
                                    foreach (var item in dictionaryTypeEntityLists)
                                    {
                                        Dictionary<string, string> dictionaryType = new Dictionary<string, string>();
                                        dictionaryType.Add(item.Id, item.FullName);
                                        dictionaryTypeList.Add(dictionaryType);
                                    }
                                    templateData.Add(fieldName1, dictionaryTypeList);
                                }
                                break;
                            //省市区
                            case JnpfKeyConst.ADDRESS:
                                {
                                    var addressEntityList = await _provinceService.GetList();
                                    List<Dictionary<string, string>> addressList = new List<Dictionary<string, string>>();
                                    foreach (var item in addressEntityList)
                                    {
                                        Dictionary<string, string> address = new Dictionary<string, string>();
                                        address.Add(item.Id, item.FullName);
                                        addressList.Add(address);
                                    }
                                    templateData.Add(fieldName1, addressList);
                                }
                                break;
                            //级联选择
                            case JnpfKeyConst.CASCADER:
                                {
                                    if (vModelType.STATIC.GetDescription() == configModel.dataType)
                                    {
                                        var props = model.props.props;
                                        var optionList = GetTreeOptions(model.options, props);
                                        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                        foreach (var item in optionList)
                                        {
                                            Dictionary<string, string> option = new Dictionary<string, string>();
                                            option.Add(item.value, item.label);
                                            list.Add(option);
                                        }
                                        templateData.Add(fieldName1, list);
                                    }
                                }
                                break;
                        }
                    }
                }
                //缓存2分钟
                _sysCacheService.Set(cacheKey, templateData, TimeSpan.FromMinutes(2));
            }
            return templateData;
        }

        #endregion

        #region 系统组件生成与解析

        /// <summary>
        /// 生成系统自动生成字段
        /// </summary>
        /// <param name="fieldsModelList">模板数据</param>
        /// <param name="allDataMap">真实数据</param>
        /// <param name="IsCreate">创建与修改标识 true创建 false 修改</param>
        /// <returns></returns>
        public async Task<Dictionary<string, object>> GenerateFeilds(List<FieldsModel> fieldsModelList, Dictionary<string, object> allDataMap, bool IsCreate)
        {
            int dicCount = allDataMap.Keys.Count;
            string[] strKey = new string[dicCount];
            allDataMap.Keys.CopyTo(strKey, 0);
            for (int i = 0; i < strKey.Length; i++)
            {
                //根据KEY查找模板
                var model = fieldsModelList.Find(f => f.__vModel__ == strKey[i]);
                if (model != null)
                {
                    //如果模板jnpfKey为table为子表数据
                    if ("table".Equals(model.__config__.jnpfKey) && allDataMap[strKey[i]] != null)
                    {
                        List<FieldsModel> childFieldsModelList = model.__config__.children;
                        var objectData = allDataMap[strKey[i]];
                        List<Dictionary<string, object>> childAllDataMapList = objectData.Serialize().Deserialize<List<Dictionary<string, object>>>();
                        List<Dictionary<string, object>> newChildAllDataMapList = new List<Dictionary<string, object>>();
                        foreach (var childmap in childAllDataMapList)
                        {
                            var newChildData = new Dictionary<string, object>();
                            foreach (KeyValuePair<string, object> item in childmap)
                            {
                                var childFieldsModel = childFieldsModelList.Where(c => c.__vModel__ == item.Key).FirstOrDefault();
                                if (childFieldsModel != null)
                                {
                                    var userInfo = await _userManager.GetUserInfo();
                                    if (childFieldsModel.__vModel__.Equals(item.Key))
                                    {
                                        string jnpfKeyType = childFieldsModel.__config__.jnpfKey;
                                        switch (jnpfKeyType)
                                        {
                                            case "billRule":
                                                if (IsCreate)
                                                {
                                                    string billNumber = await _billRuleService.GetBillNumber(childFieldsModel.__config__.rule);
                                                    if (!"单据规则不存在".Equals(billNumber))
                                                    {
                                                        newChildData[item.Key] = billNumber;
                                                    }
                                                    else
                                                    {
                                                        newChildData[item.Key] = "";
                                                    }
                                                }
                                                else
                                                {
                                                    newChildData[item.Key] = childmap[item.Key];
                                                }
                                                break;
                                            case "createUser":
                                                if (IsCreate)
                                                {
                                                    newChildData[item.Key] = userInfo.userId;
                                                }
                                                break;
                                            case "createTime":
                                                if (IsCreate)
                                                {
                                                    newChildData[item.Key] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                                                }
                                                break;
                                            case "modifyUser":
                                                if (!IsCreate)
                                                {
                                                    newChildData[item.Key] = userInfo.userId;
                                                }
                                                break;
                                            case "modifyTime":
                                                if (!IsCreate)
                                                {
                                                    newChildData[item.Key] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                                                }
                                                break;
                                            case "currPosition":
                                                var userEntity = _userService.GetInfoByUserId(userInfo.userId);
                                                if (!string.IsNullOrEmpty(userEntity.PositionId))
                                                {
                                                    var positionEntity = await _positionService.GetInfoById(userEntity.PositionId.Split(",").FirstOrDefault());
                                                    if (positionEntity != null)
                                                    {
                                                        newChildData[item.Key] = positionEntity.Id;
                                                    }
                                                }
                                                else
                                                {
                                                    newChildData[item.Key] = "";
                                                }
                                                break;
                                            case "currOrganize":
                                                {
                                                    if (userInfo.organizeId != null)
                                                    {
                                                        newChildData[item.Key] = userInfo.organizeId;
                                                    }
                                                    else
                                                    {
                                                        newChildData[item.Key] = "";
                                                    }
                                                }
                                                break;
                                            default:
                                                newChildData[item.Key] = childmap[item.Key];
                                                break;
                                        }
                                    }
                                }
                            }
                            newChildAllDataMapList.Add(newChildData);
                            allDataMap[strKey[i]] = newChildAllDataMapList;
                        }
                    }
                    else
                    {
                        var userInfo = await _userManager.GetUserInfo();
                        if (model.__vModel__.Equals(strKey[i]))
                        {
                            string jnpfKeyType = model.__config__.jnpfKey;
                            switch (jnpfKeyType)
                            {
                                case "billRule":
                                    if (IsCreate)
                                    {
                                        string billNumber = await _billRuleService.GetBillNumber(model.__config__.rule);
                                        if (!"单据规则不存在".Equals(billNumber))
                                        {
                                            allDataMap[strKey[i]] = billNumber;
                                        }
                                        else
                                        {
                                            allDataMap[strKey[i]] = "";
                                        }
                                    }
                                    break;
                                case "createUser":
                                    {
                                        if (IsCreate)
                                        {
                                            allDataMap[strKey[i]] = userInfo.userId;
                                        }
                                        else
                                        {
                                            allDataMap[strKey[i]] = await _userService.GetUserIdByRealName(allDataMap[strKey[i]].ToString());
                                        }
                                    }
                                    break;
                                case "createTime":
                                    if (IsCreate)
                                    {
                                        allDataMap[strKey[i]] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                                    }
                                    break;
                                case "modifyUser":
                                    if (!IsCreate)
                                    {
                                        allDataMap[strKey[i]] = userInfo.userId;
                                    }
                                    break;
                                case "modifyTime":
                                    if (!IsCreate)
                                    {
                                        allDataMap[strKey[i]] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                                    }
                                    break;
                                case "currPosition":
                                    var userEntity = _userService.GetInfoByUserId(userInfo.userId);
                                    if (!string.IsNullOrEmpty(userEntity.PositionId))
                                    {
                                        var positionEntity = await _positionService.GetInfoById(userEntity.PositionId.Split(",").FirstOrDefault());
                                        if (positionEntity != null)
                                        {
                                            allDataMap[strKey[i]] = positionEntity.Id;
                                        }
                                    }
                                    else
                                    {
                                        allDataMap[strKey[i]] = "";
                                    }
                                    break;
                                case "currOrganize":
                                    {
                                        if (userInfo.organizeId != null)
                                        {
                                            allDataMap[strKey[i]] = userInfo.organizeId;
                                        }
                                        else
                                        {
                                            allDataMap[strKey[i]] = "";
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            return allDataMap;
        }

        /// <summary>
        /// 将系统组件生成的数据转换为数据
        /// </summary>
        /// <param name="formData">数据库模板数据</param>
        /// <param name="templateData">模板真实数据</param>
        /// <param name="entity">真实数据</param>
        /// <returns></returns>
        private async Task<string> GetSystemComponentsData(List<FieldsModel> formData, Dictionary<string, object> templateData, string modelData)
        {
            //剔除无限极
            formData = TemplateDataConversion(formData);
            //数据库保存的F_Data
            Dictionary<string, object> dataMap = modelData.ToObject<Dictionary<string, object>>();
            int dicCount = dataMap.Keys.Count;
            string[] strKey = new string[dicCount];
            dataMap.Keys.CopyTo(strKey, 0);
            //自动生成的数据不在模板数据内
            var record = dataMap.Keys.Except(templateData.Keys).ToList();
            foreach (var key in record)
            {
                if (dataMap[key] != null)
                {
                    var dataValue = dataMap[key].ToString();
                    if (!string.IsNullOrEmpty(dataValue))
                    {
                        var model = formData.Where(f => f.__vModel__ == key).FirstOrDefault();
                        if (model != null)
                        {
                            ConfigModel configModel = model.__config__;
                            string type = configModel.jnpfKey;
                            switch (type)
                            {
                                //case "currDept":
                                //    {
                                //        var deptMapList = await _departmentService.GetListAsync();
                                //        dataMap[key] = deptMapList.Find(o => o.Id.Equals(dataMap[key].ToString())).FullName;
                                //    }
                                //    break;
                                case "createUser":
                                    {
                                        var createUser = await _userService.GetInfoByUserIdAsync(dataMap[key].ToString());
                                        if (createUser != null)
                                            dataMap[key] = createUser.RealName;
                                    }
                                    break;
                                case "modifyUser":
                                    {
                                        var modifyUser = await _userService.GetInfoByUserIdAsync(dataMap[key].ToString());
                                        if (modifyUser != null)
                                            dataMap[key] = modifyUser.RealName;
                                    }
                                    break;
                                case "currPosition":
                                    {
                                        var mapList = await _positionService.GetListAsync();
                                        dataMap[key] = mapList.Where(p => p.Id == dataMap[key].ToString()).FirstOrDefault().FullName;
                                    }
                                    break;
                                case "currOrganize":
                                    {
                                        var orgMapList = await _organizeService.GetListAsync();
                                        dataMap[key] = orgMapList.Find(o => o.Id.Equals(dataMap[key].ToString())).FullName;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            return dataMap.Serialize();
        }

        #endregion

        #region 转换数据

        /// <summary>
        /// 将关键字key查询传输的id转换成名称，还有动态数据id成名称
        /// </summary>
        /// <param name="formData">数据库模板数据</param>
        /// <param name="templateData">模板真实数据</param>
        /// <param name="keywordJsonDic">查询数据</param>
        /// <param name="list">真实数据</param>
        /// <param name="webType">表单类型1-纯表单、2-普通表单、3-工作流表单</param>
        /// <param name="primaryKey">数据主键</param>
        /// <returns></returns>
        private async Task<Dictionary<string, object>> GetKeyData(List<FieldsModel> formData, Dictionary<string, object> templateData, Dictionary<string, object> keywordJsonDic, List<VisualDevModelDataEntity> list, ColumnDesignModel columnDesign = null, string actionType = "List", int webType = 2, string primaryKey = "F_Id")
        {
            List<string> redisList = new List<string>();

            //转换数据
            var convData = new Dictionary<string, object>();
            //转换列表数据
            foreach (var entity in list)
            {
                //数据库保存的F_Data
                Dictionary<string, object> dataMap = entity.Data.Deserialize<Dictionary<string, object>>();
                if (webType == 3)
                {
                    IFlowTaskRepository _flowTaskService = null;
                    if (_flowTaskService == null)
                    {
                        _flowTaskService = App.GetService<IFlowTaskRepository>();
                    }
                    var flowTask = await _flowTaskService.GetTaskInfo(entity.Id);
                    if (flowTask != null)
                    {
                        dataMap["flowState"] = flowTask.Status;
                    }
                }
                int dicCount = dataMap.Keys.Count;
                string[] strKey = new string[dicCount];
                dataMap.Keys.CopyTo(strKey, 0);
                for (int i = 0; i < strKey.Length; i++)
                {
                    if (!(dataMap[strKey[i]] is null))
                    {
                        var form = formData.Where(f => f.__vModel__ == strKey[i]).FirstOrDefault();
                        if (form != null)
                        {
                            if (form.__vModel__.Contains(form.__config__.jnpfKey + "Field"))
                            {
                                dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], form);
                            }
                            else
                            {
                                dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], form, actionType);
                            }
                            var templateValue = templateData.Where(t => t.Key.Equals(strKey[i])).FirstOrDefault();
                            //转换后的数据值
                            var dataDicValue = dataMap[strKey[i]];
                            if (templateValue.Key != null && !(dataDicValue is null) && dataDicValue.ToString() != "[]")
                            {
                                var jnpfKey = form.__config__.jnpfKey;
                                var moreValue = dataDicValue as IEnumerable<object>;
                                //不是List数据直接赋值
                                if (moreValue == null)
                                {
                                    jnpfKey = "";
                                }
                                switch (jnpfKey)
                                {
                                    case JnpfKeyConst.COMSELECT:
                                        {
                                            StringBuilder addName = new StringBuilder();
                                            List<Dictionary<string, string>> dicAdd = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                            foreach (var item in moreValue)
                                            {
                                                var comData = dicAdd.Where(a => a.ContainsKey(item.ToString())).FirstOrDefault();
                                                if (comData != null)
                                                {
                                                    addName.Append(comData[item.ToString()] + ",");
                                                }
                                            }
                                            if (addName.Length != 0)
                                            {
                                                dataMap[strKey[i]] = addName.ToString().TrimEnd(',');
                                            }
                                        }
                                        break;
                                    case JnpfKeyConst.DEPSELECT:
                                        {
                                            StringBuilder addName = new StringBuilder();
                                            List<Dictionary<string, string>> dicAdd = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                            foreach (var item in moreValue)
                                            {
                                                var comData = dicAdd.Where(a => a.ContainsKey(item.ToString())).FirstOrDefault();
                                                if (comData != null)
                                                {
                                                    addName.Append(comData[item.ToString()] + ",");
                                                }
                                            }
                                            if (addName.Length != 0)
                                            {
                                                dataMap[strKey[i]] = addName.ToString().TrimEnd(',');
                                            }
                                        }
                                        break;
                                    case JnpfKeyConst.ADDRESS:
                                        {
                                            StringBuilder addName = new StringBuilder();
                                            List<Dictionary<string, string>> dicAdd = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                            foreach (var item in moreValue)
                                            {
                                                var addressData = dicAdd.Where(a => a.ContainsKey(item.ToString())).FirstOrDefault();
                                                if (addressData != null)
                                                {
                                                    addName.Append(addressData[item.ToString()] + "/");
                                                }
                                            }
                                            if (addName.Length != 0)
                                            {
                                                dataMap[strKey[i]] = addName.ToString().TrimEnd('/');
                                            }
                                        }
                                        break;
                                    case JnpfKeyConst.CASCADER:
                                        {
                                            StringBuilder addName = new StringBuilder();
                                            List<Dictionary<string, string>> dicAdd = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                            if (form.props.props.multiple)
                                            {
                                                foreach (var item in moreValue)
                                                {
                                                    StringBuilder sb = new StringBuilder();
                                                    foreach (var items in item.Serialize().Deserialize<List<string>>())
                                                    {
                                                        var cascaderData = dicAdd.Where(c => c.ContainsKey(items.ToString())).FirstOrDefault();
                                                        if (cascaderData != null)
                                                        {
                                                            sb.Append(cascaderData[items.ToString()] + "/");
                                                        }
                                                    }
                                                    if (sb.Length != 0)
                                                    {
                                                        addName.AppendFormat("{0}", sb.ToString().TrimEnd('/') + ",");
                                                    }
                                                }
                                                if (addName.Length != 0)
                                                {
                                                    dataMap[strKey[i]] = addName.ToString().TrimEnd(',');
                                                }
                                            }
                                            else
                                            {
                                                foreach (var item in moreValue)
                                                {
                                                    var cascaderData = dicAdd.Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                                    if (cascaderData != null)
                                                    {
                                                        addName.Append(cascaderData[item.ToString()] + "/");
                                                    }
                                                }
                                                if (addName.Length != 0)
                                                {
                                                    dataMap[strKey[i]] = addName.ToString().TrimEnd('/');
                                                }
                                            }
                                        }
                                        break;
                                    case JnpfKeyConst.CHECKBOX:
                                        {
                                            StringBuilder addName = new StringBuilder();
                                            List<Dictionary<string, string>> dicAdd = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                            foreach (var item in moreValue)
                                            {
                                                var cascaderData = dicAdd.Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                                if (cascaderData != null)
                                                {
                                                    addName.Append(cascaderData[item.ToString()] + ",");
                                                }
                                            }
                                            if (addName.Length != 0)
                                            {
                                                dataMap[strKey[i]] = addName.ToString().TrimEnd(',');
                                            }
                                        }
                                        break;
                                    case JnpfKeyConst.USERSELECT:
                                        {
                                            StringBuilder userName = new StringBuilder();
                                            List<Dictionary<string, string>> dicUser = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                            foreach (var item in moreValue)
                                            {
                                                var cascaderData = dicUser.Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                                if (cascaderData != null)
                                                {
                                                    userName.Append(cascaderData[item.ToString()] + ",");
                                                }
                                            }
                                            if (userName.Length != 0)
                                            {
                                                dataMap[strKey[i]] = userName.ToString().TrimEnd(',');
                                            }
                                        }
                                        break;
                                    case JnpfKeyConst.POSSELECT:
                                        {
                                            StringBuilder posName = new StringBuilder();
                                            List<Dictionary<string, string>> dicPos = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                            foreach (var item in moreValue)
                                            {
                                                var cascaderData = dicPos.Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                                if (cascaderData != null)
                                                {
                                                    posName.Append(cascaderData[item.ToString()] + ",");
                                                }
                                            }
                                            if (posName.Length != 0)
                                            {
                                                dataMap[strKey[i]] = posName.ToString().TrimEnd(',');
                                            }
                                        }
                                        break;
                                    case JnpfKeyConst.SELECT:
                                        {
                                            StringBuilder selectName = new StringBuilder();
                                            List<Dictionary<string, string>> dicSelect = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                            foreach (var item in moreValue)
                                            {
                                                var cascaderData = dicSelect.Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                                if (cascaderData != null)
                                                {
                                                    selectName.Append(cascaderData[item.ToString()] + ",");
                                                }
                                            }
                                            if (selectName.Length != 0)
                                            {
                                                dataMap[strKey[i]] = selectName.ToString().TrimEnd(',');
                                            }
                                        }
                                        break;
                                    case JnpfKeyConst.TREESELECT:
                                        {
                                            StringBuilder treeSelectName = new StringBuilder();
                                            List<Dictionary<string, string>> treeSelectDic = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                            foreach (var item in moreValue)
                                            {
                                                var cascaderData = treeSelectDic.Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                                if (cascaderData != null)
                                                {
                                                    treeSelectName.Append(cascaderData[item.ToString()] + ",");
                                                }
                                            }
                                            if (treeSelectName.Length != 0)
                                            {
                                                dataMap[strKey[i]] = treeSelectName.ToString().TrimEnd(',');
                                            }
                                        }
                                        break;
                                    default:
                                        {
                                            var convertList = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                            var convertData = convertList.Where(t => t.ContainsKey(dataMap[strKey[i]].ToString())).FirstOrDefault();
                                            if (convertData != null)
                                            {
                                                dataMap[strKey[i]] = convertData.Values.FirstOrDefault().ToString();
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
                //转换模板没记录的数据
                var record = dataMap.Keys.Except(templateData.Keys).ToList();
                //针对有表的模板去除"rownum,主键"
                record.RemoveAll(r => { return r == "ROWNUM"; });
                record.RemoveAll(r => { return r == primaryKey; });
                foreach (var key in record)
                {
                    if (!(dataMap[key] is null) && dataMap[key].ToString() != "")
                    {
                        var dataValue = dataMap[key];
                        var model = formData.Where(f => f.__vModel__ == key).FirstOrDefault();
                        if (model != null)
                        {
                            ConfigModel configModel = model.__config__;
                            string type = configModel.jnpfKey;
                            switch (type)
                            {
                                //switch开关
                                case "switch":
                                    {
                                        dataMap[key] = dataMap[key].ToInt() == 0 ? "关" : "开";
                                    }
                                    break;
                                //时间范围
                                case JnpfKeyConst.TIMERANGE:
                                    {
                                        List<string> jsonArray = new List<string>();
                                        jsonArray = dataValue.Serialize().Deserialize<List<string>>();
                                        string value1 = string.Format("{0:" + model.format + "}", Convert.ToDateTime(jsonArray.First()));
                                        string value2 = string.Format("{0:" + model.format + "}", Convert.ToDateTime(jsonArray.Last()));
                                        jsonArray.Clear();
                                        jsonArray.Add(value1 + "至");
                                        jsonArray.Add(value2);
                                        dataMap[key] = jsonArray;
                                    }
                                    break;
                                //日期选择
                                case JnpfKeyConst.DATE:
                                    {
                                        string value = string.Empty;
                                        if (!string.IsNullOrEmpty(model.format))
                                        {
                                            value = string.Format("{0:" + model.format + "}", Convert.ToDateTime(dataMap[key].ToString()));
                                        }
                                        else
                                        {
                                            switch (model.type)
                                            {
                                                case "date":
                                                    value = string.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(dataMap[key].ToString()));
                                                    break;
                                                default:
                                                    value = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Convert.ToDateTime(dataMap[key].ToString()));
                                                    break;
                                            }
                                        }
                                        dataMap[key] = value;
                                    }
                                    break;
                                //日期选择
                                case JnpfKeyConst.TIME:
                                    {
                                        string value = string.Empty;
                                        if (!string.IsNullOrEmpty(model.format))
                                        {
                                            value = string.Format("{0:" + model.format + "}", Convert.ToDateTime(dataMap[key].ToString()));
                                        }
                                        else
                                        {
                                            value = dataMap[key].ToString();
                                        }
                                        dataMap[key] = value;
                                    }
                                    break;
                                //日期范围
                                case JnpfKeyConst.DATERANGE:
                                    {
                                        List<string> jsonArray = new List<string>();
                                        jsonArray = dataValue.Serialize().Deserialize<List<string>>();
                                        string value1 = string.Format("{0:" + model.format + "}", Ext.GetDateTime(jsonArray.First()));
                                        string value2 = string.Format("{0:" + model.format + "}", Ext.GetDateTime(jsonArray.Last()));
                                        jsonArray.Clear();
                                        jsonArray.Add(value1 + "至");
                                        jsonArray.Add(value2);
                                        dataMap[key] = jsonArray;
                                    }
                                    break;
                                case "currDept":
                                    {
                                        var orgMapList = await _organizeService.GetListAsync();
                                        foreach (var organizeEntity in orgMapList)
                                        {
                                            if (dataMap[key].ToString().Equals(organizeEntity.Id))
                                            {
                                                dataMap[key] = organizeEntity.FullName;
                                            }
                                        }
                                    }
                                    break;
                                case "createUser":
                                    {
                                        var userCreEntity = await _userService.GetInfoByUserIdAsync(dataMap[key].ToString());
                                        if (userCreEntity != null)
                                        {
                                            dataMap[key] = userCreEntity.RealName;
                                        }
                                        else
                                        {
                                            dataMap[key] = dataMap[key];
                                        }
                                    }
                                    break;
                                case "createTime":
                                    {
                                        string value = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Convert.ToDateTime(dataMap[key].ToString()));
                                        dataMap[key] = value;
                                    }
                                    break;
                                case "modifyUser":
                                    {
                                        var userModEntity = await _userService.GetInfoByUserIdAsync(dataMap[key].ToString());
                                        if (userModEntity != null)
                                        {
                                            dataMap[key] = userModEntity.RealName;
                                        }
                                        else
                                        {
                                            dataMap[key] = dataMap[key];
                                        }
                                    }
                                    break;
                                case "modifyTime":
                                    {
                                        string value = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Convert.ToDateTime(dataMap[key].ToString()));
                                        dataMap[key] = value;
                                    }
                                    break;
                                case "currPosition":
                                    {
                                        var mapList = await _positionService.GetListAsync();
                                        var position = mapList.Where(p => p.Id == dataMap[key].ToString()).FirstOrDefault();
                                        if (position != null)
                                        {
                                            dataMap[key] = position.FullName;
                                        }
                                    }
                                    break;
                                case "currOrganize":
                                    {
                                        var orgMapList = await _organizeService.GetListAsync();
                                        foreach (var organizeEntity in orgMapList)
                                        {
                                            if (dataMap[key].ToString().Equals(organizeEntity.Id))
                                            {
                                                dataMap[key] = organizeEntity.FullName;
                                            }
                                        }
                                    }
                                    break;
                                //级联
                                case "cascader":
                                    {
                                        switch (configModel.dataType)
                                        {
                                            case "dictionary":
                                                {
                                                    List<DictionaryDataEntity> data = new List<DictionaryDataEntity>();
                                                    var cacheKey = CommonConst.VISUALDEV + _userManager.TenantId + "_" + configModel.dictionaryType;
                                                    if (_sysCacheService.Exists(cacheKey))
                                                    {
                                                        data = _sysCacheService.Get(cacheKey).Deserialize<List<DictionaryDataEntity>>();
                                                    }
                                                    else
                                                    {
                                                        data = await _dictionaryDataService.GetList(configModel.dictionaryType);
                                                        _sysCacheService.Set(cacheKey, data);
                                                        redisList.Add(cacheKey);
                                                    }
                                                    List<string> dataList = dataMap[key].Serialize().Deserialize<List<string>>();
                                                    List<string> cascaderList = new List<string>();
                                                    foreach (var items in dataList)
                                                    {
                                                        var cascader = data.Find(d => d.Id == items);
                                                        if (cascader != null)
                                                        {
                                                            cascaderList.Add(cascader.FullName);
                                                        }
                                                    }
                                                    dataMap[key] = cascaderList;
                                                }
                                                break;
                                            case "dynamic":
                                                {
                                                    //获取远端数据
                                                    DataInterfaceEntity dataEntity = await _dataInterfaceService.GetInfo(configModel.propsUrl);
                                                    switch (dataEntity.DataType)
                                                    {
                                                        //SQL数据
                                                        case 1:
                                                            {
                                                                var linkEntity = await _dbLinkService.GetInfo(dataEntity.DBLinkId);
                                                                var dt = _databaseService.GetInterFaceData(linkEntity, dataEntity.Query);
                                                                //List<Dictionary<string, object>> dynamicDataList = dt.Serialize().Deserialize<List<Dictionary<string, object>>>();
                                                                //List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                                                //foreach (var item in dynamicDataList)
                                                                //{
                                                                //    Dictionary<string, string> dynamicDic = new Dictionary<string, string>();
                                                                //    dynamicDic.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                                                //    list.Add(dynamicDic);
                                                                //}
                                                                //templateData.Add(fieldName1, list);
                                                            }
                                                            break;
                                                        //静态数据
                                                        case 2:
                                                            {
                                                                List<Dictionary<string, string>> dynamicList = new List<Dictionary<string, string>>();
                                                                var value = model.props.props.value;
                                                                var label = model.props.props.label;
                                                                var children = model.props.props.children;
                                                                JToken dynamicDataList = JValue.Parse(dataEntity.Query);
                                                                foreach (var data in dynamicDataList)
                                                                {
                                                                    Dictionary<string, string> dic = new Dictionary<string, string>();
                                                                    dic[value] = data.Value<string>(value);
                                                                    dic[label] = data.Value<string>(label);
                                                                    dynamicList.Add(dic);
                                                                    if (data.Value<object>(children) != null && data.Value<object>(children).ToString() != "")
                                                                    {
                                                                        dynamicList.AddRange(GetDynamicInfiniteData(data.Value<object>(children).ToString(), model.props.props));
                                                                    }
                                                                }
                                                                List<string> dataList = dataMap[key].Serialize().Deserialize<List<string>>();
                                                                List<string> cascaderList = new List<string>();
                                                                foreach (var items in dataList)
                                                                {
                                                                    var vara = dynamicList.Where(a => a.ContainsValue(items)).FirstOrDefault();
                                                                    if (vara != null)
                                                                    {
                                                                        cascaderList.Add(vara[label]);
                                                                    }
                                                                }
                                                                dataMap[key] = cascaderList;
                                                            }
                                                            break;
                                                        //Api数据
                                                        case 3:
                                                            {

                                                            }
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    break;
                                //关联表单
                                case JnpfKeyConst.RELATIONFORM:
                                    {
                                        List<Dictionary<string, object>> relationFormDataList = new List<Dictionary<string, object>>();

                                        var redisName = CommonConst.VISUALDEV + _userManager.TenantId + "_" + configModel.jnpfKey + "_" + model.modelId;
                                        if (_sysCacheService.Exists(redisName))
                                        {
                                            relationFormDataList = _sysCacheService.Get(redisName).Deserialize<List<Dictionary<string, object>>>();
                                        }
                                        else
                                        {
                                            //根据可视化功能ID获取该模板全部数据
                                            var relationFormModel = await _visualDevRepository.FirstOrDefaultAsync(v => v.Id == model.modelId);
                                            FormDataModel relationFormData = relationFormModel.FormData.Deserialize<FormDataModel>();
                                            //多虑多余控件
                                            var newFields = this.TemplateDataConversion(relationFormData.fields);
                                            var newFieLdsModelList = newFields.FindAll(x => model.relationField.Equals(x.__vModel__));
                                            VisualDevModelListQueryInput listQueryInput = new VisualDevModelListQueryInput
                                            {
                                                dataType = "1",
                                                sidx = columnDesign.defaultSidx,
                                                sort = columnDesign.sort
                                            };
                                            relationFormDataList = (await GetListResult(relationFormModel, listQueryInput)).list.ToList();
                                            _sysCacheService.Set(redisName, relationFormDataList);
                                            redisList.Add(redisName);
                                        }
                                        var relationFormRealData = relationFormDataList.Where(it => it["id"].Equals(dataMap[key])).FirstOrDefault();
                                        if (relationFormRealData != null && relationFormRealData.Count > 0)
                                        {
                                            dataMap[key + "_id"] = relationFormRealData["id"];
                                            dataMap[key] = relationFormRealData[model.relationField];
                                        }
                                    }
                                    break;
                                case JnpfKeyConst.POPUPSELECT:
                                    {
                                        var popupsList = (await _dataInterfaceService.GetData(model.interfaceId)).Serialize().Deserialize<List<Dictionary<string, string>>>();
                                        var specificData = popupsList.Where(it => it.ContainsKey(model.propsValue) && it.ContainsValue(dataMap[key].ToString())).FirstOrDefault();
                                        if (specificData != null)
                                        {
                                            var showField = model.columnOptions.First();
                                            dataMap[key] = specificData[showField.value];
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
                //为数据实时性，接口请求完后删除Redis缓存
                if (redisList != null)
                {
                    foreach (var items in redisList)
                    {
                        _sysCacheService.Del(items);
                    }
                }
                entity.Data = dataMap.Serialize();
            }
            //转换查询数据
            if (keywordJsonDic != null)
            {
                int dicCount = keywordJsonDic.Keys.Count;
                string[] strKey = new string[dicCount];
                keywordJsonDic.Keys.CopyTo(strKey, 0);
                for (int i = 0; i < strKey.Length; i++)
                {
                    var form = formData.Where(f => f.__vModel__ == strKey[i]).FirstOrDefault();
                    //范围查询跳过数据转换
                    var scopeList = new List<string>();
                    scopeList.Add(JnpfKeyConst.TIMERANGE);
                    scopeList.Add(JnpfKeyConst.DATE);
                    scopeList.Add(JnpfKeyConst.TIME);
                    scopeList.Add(JnpfKeyConst.DATERANGE);
                    scopeList.Add("createTime");
                    scopeList.Add("modifyTime");
                    scopeList.Add("numInput");
                    var jnpfKey = form.__config__.jnpfKey;
                    var isScope = scopeList.Find(s => s.Equals(jnpfKey));
                    if (form != null && isScope == null)
                    {
                        if (form.__vModel__.Contains(form.__config__.jnpfKey + "Field"))
                        {
                            keywordJsonDic[strKey[i]] = TemplateControlsDataConversion(keywordJsonDic[strKey[i]], form);
                        }
                        else
                        {
                            keywordJsonDic[strKey[i]] = TemplateControlsDataConversion(keywordJsonDic[strKey[i]], form, "query");
                        }
                        var templateValue = templateData.Where(t => t.Key.Equals(strKey[i])).FirstOrDefault();
                        //转换后的数据值
                        var dataDicValue = keywordJsonDic[strKey[i]];
                        if (templateValue.Key != null && !(dataDicValue is null) && dataDicValue.ToString() != "[]")
                        {
                            var moreValue = dataDicValue as IEnumerable<object>;
                            //不是List数据直接赋值
                            if (moreValue == null)
                            {
                                jnpfKey = "";
                            }
                            switch (jnpfKey)
                            {
                                case JnpfKeyConst.COMSELECT:
                                    {
                                        StringBuilder addName = new StringBuilder();
                                        List<Dictionary<string, string>> dicAdd = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                        foreach (var item in moreValue)
                                        {
                                            var comData = dicAdd.Where(a => a.ContainsKey(item.ToString())).FirstOrDefault();
                                            if (comData != null)
                                            {
                                                addName.Append(comData[item.ToString()] + ",");
                                            }
                                        }
                                        if (addName.Length != 0)
                                        {
                                            keywordJsonDic[strKey[i]] = addName.ToString().TrimEnd(',');
                                        }
                                    }
                                    break;
                                case JnpfKeyConst.DEPSELECT:
                                    {
                                        StringBuilder addName = new StringBuilder();
                                        List<Dictionary<string, string>> dicAdd = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                        foreach (var item in moreValue)
                                        {
                                            var comData = dicAdd.Where(a => a.ContainsKey(item.ToString())).FirstOrDefault();
                                            if (comData != null)
                                            {
                                                addName.Append(comData[item.ToString()] + ",");
                                            }
                                        }
                                        if (addName.Length != 0)
                                        {
                                            keywordJsonDic[strKey[i]] = addName.ToString().TrimEnd(',');
                                        }
                                    }
                                    break;
                                case JnpfKeyConst.ADDRESS:
                                    {
                                        StringBuilder addName = new StringBuilder();
                                        List<Dictionary<string, string>> dicAdd = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                        foreach (var item in moreValue)
                                        {
                                            var addressData = dicAdd.Where(a => a.ContainsKey(item.ToString())).FirstOrDefault();
                                            if (addressData != null)
                                            {
                                                addName.Append(addressData[item.ToString()] + "/");
                                            }
                                        }
                                        if (addName.Length != 0)
                                        {
                                            keywordJsonDic[strKey[i]] = addName.ToString().TrimEnd('/');
                                        }
                                    }
                                    break;
                                case JnpfKeyConst.CASCADER:
                                    {
                                        StringBuilder addName = new StringBuilder();
                                        List<Dictionary<string, string>> dicAdd = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                        if (form.props.props.multiple)
                                        {
                                            foreach (var item in moreValue)
                                            {
                                                StringBuilder sb = new StringBuilder();
                                                foreach (var items in item.Serialize().Deserialize<List<string>>())
                                                {
                                                    var cascaderData = dicAdd.Where(c => c.ContainsKey(items.ToString())).FirstOrDefault();
                                                    if (cascaderData != null)
                                                    {
                                                        sb.Append(cascaderData[items.ToString()] + "/");
                                                    }
                                                }
                                                if (sb.Length != 0)
                                                {
                                                    addName.AppendFormat("{0}", sb.ToString().TrimEnd('/') + ",");
                                                }
                                            }
                                            if (addName.Length != 0)
                                            {
                                                keywordJsonDic[strKey[i]] = addName.ToString().TrimEnd(',');
                                            }
                                        }
                                        else
                                        {
                                            foreach (var item in moreValue)
                                            {
                                                var cascaderData = dicAdd.Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                                if (cascaderData != null)
                                                {
                                                    addName.Append(cascaderData[item.ToString()] + "/");
                                                }
                                            }
                                            if (addName.Length != 0)
                                            {
                                                keywordJsonDic[strKey[i]] = addName.ToString().TrimEnd('/');
                                            }
                                        }
                                    }
                                    break;
                                case JnpfKeyConst.CHECKBOX:
                                    {
                                        StringBuilder addName = new StringBuilder();
                                        List<Dictionary<string, string>> dicAdd = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                        foreach (var item in moreValue)
                                        {
                                            var cascaderData = dicAdd.Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                            if (cascaderData != null)
                                            {
                                                addName.Append(cascaderData[item.ToString()] + ",");
                                            }
                                        }
                                        if (addName.Length != 0)
                                        {
                                            keywordJsonDic[strKey[i]] = addName.ToString().TrimEnd(',');
                                        }
                                    }
                                    break;
                                case JnpfKeyConst.USERSELECT:
                                    {
                                        StringBuilder userName = new StringBuilder();
                                        List<Dictionary<string, string>> dicUser = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                        foreach (var item in moreValue)
                                        {
                                            var cascaderData = dicUser.Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                            if (cascaderData != null)
                                            {
                                                userName.Append(cascaderData[item.ToString()] + ",");
                                            }
                                        }
                                        if (userName.Length != 0)
                                        {
                                            keywordJsonDic[strKey[i]] = userName.ToString().TrimEnd(',');
                                        }
                                    }
                                    break;
                                case JnpfKeyConst.POSSELECT:
                                    {
                                        StringBuilder posName = new StringBuilder();
                                        List<Dictionary<string, string>> dicPos = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                        foreach (var item in moreValue)
                                        {
                                            var cascaderData = dicPos.Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                            if (cascaderData != null)
                                            {
                                                posName.Append(cascaderData[item.ToString()] + ",");
                                            }
                                        }
                                        if (posName.Length != 0)
                                        {
                                            keywordJsonDic[strKey[i]] = posName.ToString().TrimEnd(',');
                                        }
                                    }
                                    break;
                                case JnpfKeyConst.SELECT:
                                    {
                                        StringBuilder selectName = new StringBuilder();
                                        List<Dictionary<string, string>> dicSelect = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                        foreach (var item in moreValue)
                                        {
                                            var cascaderData = dicSelect.Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                            if (cascaderData != null)
                                            {
                                                selectName.Append(cascaderData[item.ToString()] + ",");
                                            }
                                        }
                                        if (selectName.Length != 0)
                                        {
                                            keywordJsonDic[strKey[i]] = selectName.ToString().TrimEnd(',');
                                        }
                                    }
                                    break;
                                case JnpfKeyConst.TREESELECT:
                                    {
                                        StringBuilder treeSelectName = new StringBuilder();
                                        List<Dictionary<string, string>> treeSelectDic = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                        foreach (var item in moreValue)
                                        {
                                            var cascaderData = treeSelectDic.Where(c => c.ContainsKey(item.ToString())).FirstOrDefault();
                                            if (cascaderData != null)
                                            {
                                                treeSelectName.Append(cascaderData[item.ToString()] + ",");
                                            }
                                        }
                                        if (treeSelectName.Length != 0)
                                        {
                                            keywordJsonDic[strKey[i]] = treeSelectName.ToString().TrimEnd(',');
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        var convertList = templateValue.Value.Serialize().Deserialize<List<Dictionary<string, string>>>();
                                        var convertData = convertList.Where(t => t.ContainsKey(keywordJsonDic[strKey[i]].ToString())).FirstOrDefault();
                                        if (convertData != null)
                                        {
                                            keywordJsonDic[strKey[i]] = convertData.Values.FirstOrDefault().ToString();
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
                //转换查询条件没记录的数据
                var record = keywordJsonDic.Keys.Except(templateData.Keys).ToList();
                foreach (var key in record)
                {
                    var model = formData.Where(f => f.__vModel__ == key).FirstOrDefault();
                    ConfigModel configModel = model.__config__;
                    string type = configModel.jnpfKey;
                    switch (type)
                    {
                        //时间范围
                        case JnpfKeyConst.TIMERANGE:
                            {
                                List<string> jsonArray = keywordJsonDic[key].Serialize().Deserialize<List<string>>();
                                string value1 = string.Format("{0:HH:mm:ss}", jsonArray[0].ToDate());
                                string value2 = string.Format("{0:HH:mm:ss}", jsonArray[1].ToDate());
                                jsonArray.Clear();
                                jsonArray.Add(value1 + "至");
                                jsonArray.Add(value2);
                                keywordJsonDic[key] = jsonArray;
                            }
                            break;
                        //日期选择
                        case JnpfKeyConst.DATE:
                            {
                                var jsonArray = keywordJsonDic[key].Serialize().Deserialize<List<string>>();
                                string value1 = string.Format("{0:" + model.format + "}", Ext.GetDateTime(jsonArray[0]));
                                string value2 = string.Format("{0:" + model.format + "}", Ext.GetDateTime(jsonArray[1]));
                                jsonArray.Clear();
                                jsonArray.Add(value1 + "至");
                                jsonArray.Add(value2 + "至");
                                keywordJsonDic[key] = jsonArray;
                            }
                            break;
                        //日期范围
                        case JnpfKeyConst.DATERANGE:
                            {
                                List<string> jsonArray = keywordJsonDic[key].Serialize().Deserialize<List<string>>();
                                string value1 = string.Format("{0:yyyy-MM-dd}", Ext.GetDateTime(jsonArray[0]));
                                string value2 = string.Format("{0:yyyy-MM-dd}", Ext.GetDateTime(jsonArray[1]));
                                jsonArray.Clear();
                                jsonArray.Add(value1 + "至");
                                jsonArray.Add(value2);
                                keywordJsonDic[key] = jsonArray;
                            }
                            break;
                        case "createUser":
                            {
                                var userEntity = _userService.GetInfoByUserId(keywordJsonDic[key].ToString());
                                keywordJsonDic[key] = userEntity.RealName;
                            }
                            break;
                        case "createTime":
                            {
                                var jsonArray = keywordJsonDic[key].ToObeject<List<string>>();
                                string value1 = string.Format("{0:yyyy-MM-dd 00:00:00}", Ext.GetDateTime(jsonArray[0]));
                                string value2 = string.Format("{0:yyyy-MM-dd 23:59:59}", Ext.GetDateTime(jsonArray[1]));
                                jsonArray.Clear();
                                jsonArray.Add(value1 + "至");
                                jsonArray.Add(value2 + "至");
                                keywordJsonDic[key] = jsonArray;
                            }
                            break;
                        case "modifyUser":
                            {
                                var userEntity = _userService.GetInfoByUserId(keywordJsonDic[key].ToString());
                                keywordJsonDic[key] = userEntity.RealName;
                            }
                            break;
                        case "modifyTime":
                            {
                                var jsonArray = keywordJsonDic[key].ToObeject<List<string>>();
                                string value1 = string.Format("{0:yyyy-MM-dd 00:00:00}", Ext.GetDateTime(jsonArray[0]));
                                string value2 = string.Format("{0:yyyy-MM-dd 23:59:59}", Ext.GetDateTime(jsonArray[1]));
                                jsonArray.Clear();
                                jsonArray.Add(value1 + "至");
                                jsonArray.Add(value2 + "至");
                                keywordJsonDic[key] = jsonArray;
                            }
                            break;
                        case "currDept":
                            {
                                var currDept = _departmentService.GetDepName(keywordJsonDic[key].ToString());
                                keywordJsonDic[key] = currDept;
                            }
                            break;
                        case "currPosition":
                            {
                                var currPosition = _positionService.GetName(keywordJsonDic[key].ToString());
                                keywordJsonDic[key] = currPosition;
                            }
                            break;
                        case "cascader":
                            {
                                switch (configModel.dataType)
                                {
                                    case "dictionary":
                                        {
                                            List<DictionaryDataEntity> data = new List<DictionaryDataEntity>();
                                            var cacheKey = CommonConst.VISUALDEV + _userManager.TenantId + "_" + configModel.dictionaryType;
                                            if (_sysCacheService.Exists(cacheKey))
                                            {
                                                data = _sysCacheService.Get(cacheKey).Deserialize<List<DictionaryDataEntity>>();
                                            }
                                            else
                                            {
                                                data = await _dictionaryDataService.GetList(configModel.dictionaryType);
                                                _sysCacheService.Set(cacheKey, data);
                                                redisList.Add(cacheKey);
                                            }
                                            List<string> dataList = keywordJsonDic[key].Serialize().Deserialize<List<string>>();
                                            List<string> cascaderList = new List<string>();
                                            foreach (var items in dataList)
                                            {
                                                var cascader = data.Find(d => d.Id == items);
                                                if (cascader != null)
                                                {
                                                    cascaderList.Add(cascader.FullName);
                                                }
                                            }
                                            keywordJsonDic[key] = cascaderList;
                                        }
                                        break;
                                    case "dynamic":
                                        {
                                            //获取远端数据
                                            //DataInterfaceEntity dynamic = await dataInterfaceService.GetInfo(configModel.propsUrl);
                                            //if (1.Equals(dynamic.DataType))
                                            //{
                                            //    var linkEntity = await dbLinkservice.GetInfo(dynamic.DBLinkId);
                                            //    var dt = await databaseService.GetInterFaceData(linkEntity, dynamic.Query);
                                            //    List<Dictionary<string, object>> dynamicDataList = UtilConvert.DataTableToDicList(dt);
                                            //    List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                            //    foreach (var item in dynamicDataList)
                                            //    {
                                            //        Dictionary<string, string> dynamicDic = new Dictionary<string, string>();
                                            //        dynamicDic.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                            //        list.Add(dynamicDic);
                                            //    }
                                            //    templateData.Add(fieldName1, list);
                                            //}
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }

                            break;
                    }
                }
            }
            Dictionary<string, object> map = new Dictionary<string, object>();
            map[vModelType.LIST.GetDescription()] = list;
            map[vModelType.KEYJSONMAP.GetDescription()] = keywordJsonDic;
            return map;
        }

        #endregion

        #region 数据转换与筛选

        /// <summary>
        /// 无表的数据筛选
        /// </summary>
        /// <param name="list"></param>
        /// <param name="keyJsonMap"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        private List<VisualDevModelDataEntity> GetNoTableFilteringData(List<VisualDevModelDataEntity> list, Dictionary<string, object> keyJsonMap, List<FieldsModel> formData)
        {
            List<VisualDevModelDataEntity> realList = new List<VisualDevModelDataEntity>();
            foreach (var entity in list)
            {
                Dictionary<String, object> query = keyJsonMap;
                Dictionary<String, object> realEntity = entity.Data.Deserialize<Dictionary<string, object>>();
                if (query != null && query.Count != 0)
                {
                    int m = 0;
                    int dicCount = query.Keys.Count;
                    string[] strKey = new string[dicCount];
                    query.Keys.CopyTo(strKey, 0);
                    for (int i = 0; i < strKey.Length; i++)
                    {
                        var keyValue = keyJsonMap[strKey[i]];
                        var queryEntity = realEntity.Where(e => e.Key == strKey[i]).FirstOrDefault();
                        var model = formData.Where(f => f.__vModel__ == strKey[i]).FirstOrDefault();
                        if (queryEntity.Value != null && !string.IsNullOrWhiteSpace(keyValue.ToString()))
                        {
                            var realValue = queryEntity.Value;
                            var type = model.__config__.jnpfKey;
                            switch (type)
                            {
                                case JnpfKeyConst.DATE:
                                    {
                                        List<String> queryTime = keyValue.ToObeject<List<string>>();
                                        int formatType = 0;
                                        if (model.format == "yyyy-MM")
                                        {
                                            formatType = 1;
                                        }
                                        else if (model.format == "yyyy")
                                        {
                                            formatType = 2;
                                        }
                                        string value1 = string.Format("{0:yyyy-MM-dd}", Ext.GetDateTime(queryTime.First()));
                                        string value2 = string.Format("{0:yyyy-MM-dd}", Ext.GetDateTime(queryTime.Last()));
                                        if (Ext.IsInTimeRange(Ext.GetDateTime(realValue.ToString()), value1, value2, formatType))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case JnpfKeyConst.TIME:
                                    {
                                        List<String> queryTime = keyValue.ToObeject<List<string>>();
                                        if (Ext.IsInTimeRange(realValue.ToDate(), queryTime.First(), queryTime.Last(), 3))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case "createTime":
                                    {
                                        List<String> dayTime1 = keyValue.ToObeject<List<string>>();
                                        string value1 = string.Format("{0:yyyy-MM-dd 00:00:00}", Ext.GetDateTime(dayTime1.First()));
                                        string value2 = string.Format("{0:yyyy-MM-dd 23:59:59}", Ext.GetDateTime(dayTime1.Last()));
                                        if (Ext.IsInTimeRange(Convert.ToDateTime(realValue), value1, value2))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case "modifyTime":
                                    {
                                        List<String> dayTime1 = keyValue.ToObeject<List<string>>();
                                        string value1 = string.Format("{0:yyyy-MM-dd 00:00:00}", Ext.GetDateTime(dayTime1.First()));
                                        string value2 = string.Format("{0:yyyy-MM-dd 23:59:59}", Ext.GetDateTime(dayTime1.Last()));
                                        if (!string.IsNullOrEmpty(realValue.ToString()))
                                        {
                                            if (Ext.IsInTimeRange(Convert.ToDateTime(realValue), value1, value2))
                                            {
                                                realEntity["id"] = entity.Id;
                                                m++;
                                            }
                                        }
                                    }
                                    break;
                                case "numInput":
                                    {
                                        List<string> numArray = keyValue.ToObeject<List<string>>();
                                        var numA = numArray.First().ToInt();
                                        var numB = numArray.Last() == null ? Int64.MaxValue : numArray.Last().ToInt();
                                        var numC = realValue.ToInt();
                                        if (numC >= numA && numC <= numB)
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case "calculate":
                                    {
                                        List<string> numArray = keyValue.ToObeject<List<string>>();
                                        var numA = numArray.First().ToInt();
                                        var numB = numArray.Last() == null ? Int64.MaxValue : numArray.Last().ToInt();
                                        var numC = realValue.ToInt();
                                        if (numC >= numA && numC <= numB)
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        if (model.searchType == 2)
                                        {
                                            if (realValue.ToString().Contains(keyValue.ToString()))
                                            {
                                                realEntity["id"] = entity.Id;
                                                m++;
                                            }
                                        }
                                        else if (model.searchType == 1)
                                        {
                                            //多选时为模糊查询
                                            if (model.multiple || type == "checkbox")
                                            {
                                                if (realValue.ToString().Contains(keyValue.ToString()))
                                                {
                                                    realEntity["id"] = entity.Id;
                                                    m++;
                                                }
                                            }
                                            else
                                            {
                                                if (realValue.ToString().Equals(keyValue.ToString()))
                                                {
                                                    realEntity["id"] = entity.Id;
                                                    m++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (realValue.ToString().Contains(keyValue.ToString()))
                                            {
                                                realEntity["id"] = entity.Id;
                                                m++;
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        if (m == dicCount)
                        {
                            realList.Add(entity);
                        }
                    }
                }
                else
                {
                    realList.Add(entity);
                }
            }
            return realList;
        }

        /// <summary>
        /// 查询过滤数据
        /// </summary>
        /// <param name="keyJsonMap"></param>
        /// <param name="list"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetQueryFilteringData(Dictionary<string, object> keyJsonMap, List<VisualDevModelDataEntity> list, ColumnDesignModel columnDesignModel)
        {
            List<Dictionary<string, object>> realList = new List<Dictionary<string, object>>();
            foreach (var entity in list)
            {
                Dictionary<String, object> query = keyJsonMap;
                Dictionary<String, object> realEntity = entity.Data.Deserialize<Dictionary<String, object>>();
                var formData = columnDesignModel.searchList;
                if (query != null && query.Count != 0)
                {
                    //添加关键词全匹配计数，全符合条件则添加
                    int m = 0;
                    int dicCount = query.Keys.Count;
                    string[] strKey = new string[dicCount];
                    query.Keys.CopyTo(strKey, 0);
                    for (int i = 0; i < strKey.Length; i++)
                    {
                        var keyValue = keyJsonMap[strKey[i]];
                        var queryEntity = realEntity.Where(e => e.Key == strKey[i]).FirstOrDefault();
                        var model = formData.Where(f => f.__vModel__ == strKey[i]).FirstOrDefault();
                        if (queryEntity.Value != null && !string.IsNullOrWhiteSpace(keyValue.ToString()))
                        {
                            var realValue = queryEntity.Value;
                            var type = model.__config__.jnpfKey;
                            switch (type)
                            {
                                case JnpfKeyConst.DATERANGE:
                                    {
                                        List<String> dayTime1 = keyValue.Serialize().Deserialize<List<string>>();
                                        List<String> dayTime2 = realValue.Serialize().Deserialize<List<string>>();
                                        var newList1 = new List<string> { dayTime1[0].TrimEnd('至') };
                                        var newList2 = new List<string> { dayTime2[0].TrimEnd('至') };
                                        dayTime1.RemoveRange(0, 1);
                                        dayTime2.RemoveRange(0, 1);
                                        dayTime1.InsertRange(0, newList1);
                                        dayTime2.InsertRange(0, newList2);
                                        bool cont1 = Ext.timeCalendar(dayTime2[0], dayTime1[0], dayTime1[1]);
                                        bool cont2 = Ext.timeCalendar(dayTime2[1], dayTime1[0], dayTime1[1]);
                                        if (cont1 || cont2)
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case JnpfKeyConst.DATE:
                                    {
                                        List<String> queryTime = keyValue.ToObeject<List<string>>();
                                        if (Ext.IsInTimeRange(realValue.ToDate(), queryTime[0].TrimEnd('至'), queryTime[1].TrimEnd('至')))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case JnpfKeyConst.TIME:
                                    {
                                        List<String> queryTime = keyValue.ToObeject<List<string>>();
                                        if (Ext.IsInTimeRange(realValue.ToDate(), queryTime[0], queryTime[1]))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case JnpfKeyConst.TIMERANGE:
                                    {
                                        List<String> dayTime1 = keyValue.Serialize().Deserialize<List<string>>();
                                        List<String> dayTime2 = realValue.Serialize().Deserialize<List<string>>();
                                        if (dayTime2 != null)
                                        {
                                            var newList1 = new List<string> { dayTime1[0].TrimEnd('至') };
                                            var newList2 = new List<string> { dayTime2[0].TrimEnd('至') };
                                            dayTime1.RemoveRange(0, 1);
                                            dayTime2.RemoveRange(0, 1);
                                            dayTime1.InsertRange(0, newList1);
                                            dayTime2.InsertRange(0, newList2);
                                            bool cont1 = Ext.timeCalendar(dayTime2[0], dayTime1[0], dayTime1[1]);
                                            bool cont2 = Ext.timeCalendar(dayTime2[1], dayTime1[0], dayTime1[1]);
                                            if (cont1 || cont2)
                                            {
                                                realEntity["id"] = entity.Id;
                                                m++;
                                            }
                                        }
                                    }
                                    break;
                                case "createTime":
                                    {
                                        List<String> dayTime1 = keyValue.ToObeject<List<string>>();
                                        if (Ext.IsInTimeRange(realValue.ToDate(), dayTime1[0].TrimEnd('至'), dayTime1.Last().TrimEnd('至')))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case "modifyTime":
                                    {
                                        List<String> dayTime1 = keyValue.ToObeject<List<string>>();
                                        if (Ext.IsInTimeRange(realValue.ToDate(), dayTime1[0].TrimEnd('至'), dayTime1.Last().TrimEnd('至')))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case "numInput":
                                    {
                                        List<string> numArray = keyValue.ToObeject<List<string>>();

                                        var numA = numArray.First().ToInt();
                                        var numB = numArray.Last() == null ? Int64.MaxValue : numArray.Last().ToInt();
                                        var numC = realValue.ToInt();
                                        if (numC >= numA && numC <= numB)
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        if (model.searchType == 2)
                                        {
                                            if (realValue.ToString().Contains(keyValue.ToString()))
                                            {
                                                realEntity["id"] = entity.Id;
                                                m++;
                                            }
                                        }
                                        else if (model.searchType == 1)
                                        {
                                            //多选时为模糊查询
                                            if (model.multiple || type == "checkbox")
                                            {
                                                if (realValue.ToString().Contains(keyValue.ToString()))
                                                {
                                                    realEntity["id"] = entity.Id;
                                                    m++;
                                                }
                                            }
                                            else
                                            {
                                                if (realValue.ToString().Equals(keyValue.ToString()))
                                                {
                                                    realEntity["id"] = entity.Id;
                                                    m++;
                                                }
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        if (m == dicCount)
                        {
                            realList.Add(realEntity);
                        }
                    }
                }
                else
                {
                    realEntity["id"] = entity.Id;
                    realList.Add(realEntity);
                }
            }
            return realList;
        }

        /// <summary>
        /// 查询转换格式
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetQueryDataConversion(List<VisualDevModelDataEntity> list)
        {
            List<Dictionary<string, object>> realList = new List<Dictionary<string, object>>();
            foreach (var entity in list)
            {
                Dictionary<String, object> realEntity = entity.Data.Deserialize<Dictionary<String, object>>();
                realEntity["id"] = entity.Id;
                realList.Add(realEntity);
            }
            return realList;
        }

        #endregion

        /// <summary>
        /// 获取无表列表数据
        /// </summary>
        /// <param name="modelId">模板ID</param>
        /// <returns></returns>
        private async Task<List<VisualDevModelDataEntity>> GetList(string modelId)
        {
            return await _visualDevModelDataRepository.Where(m => m.VisualDevId == modelId && m.DeleteMark == null).ToListAsync();
        }

        /// <summary>
        /// options无限级
        /// </summary>
        /// <returns></returns>
        private List<OptionsModel> GetTreeOptions(List<object> model, PropsBeanModel props)
        {
            List<OptionsModel> options = new List<OptionsModel>();
            foreach (var item in model)
            {
                OptionsModel option = new OptionsModel();
                var dicObject = item.Serialize().Deserialize<Dictionary<string, object>>();
                option.label = dicObject[props.label].ToString();
                option.value = dicObject[props.value].ToString();
                if (dicObject.ContainsKey(props.children))
                {
                    var children = dicObject[props.children].Serialize().Deserialize<List<object>>();
                    options.AddRange(GetTreeOptions(children, props));
                }
                options.Add(option);
            }
            return options;
        }

        /// <summary>
        /// 获取动态无限级数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="props"></param>
        /// <returns></returns>
        private List<Dictionary<string, string>> GetDynamicInfiniteData(string data, PropsBeanModel props)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            var value = props.value;
            var label = props.label;
            var children = props.children;
            JToken dynamicDataList = JValue.Parse(data);
            foreach (var info in dynamicDataList)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic[value] = info.Value<string>(value);
                dic[label] = info.Value<string>(label);
                list.Add(dic);
                if (info.Value<object>(children) != null && info.Value<object>(children).ToString() != "")
                {
                    list.AddRange(GetDynamicInfiniteData(info.Value<object>(children).ToString(), props));
                }
            }
            return list;
        }

        /// <summary>
        /// 将有表转无表
        /// </summary>
        /// <param name="dicList"></param>
        /// <param name="mainPrimary"></param>
        /// <returns></returns>
        private List<VisualDevModelDataEntity> GetTableDataList(List<Dictionary<string, object>> dicList, string mainPrimary)
        {
            List<VisualDevModelDataEntity> list = new List<VisualDevModelDataEntity>();
            foreach (var dataMap in dicList)
            {
                VisualDevModelDataEntity entity = new VisualDevModelDataEntity();
                //需要将小写的转为大写
                entity.Data = dataMap.ToJson();
                entity.Id = dataMap[mainPrimary].ToString();
                list.Add(entity);
            }
            return list;
        }

        /// <summary>
        /// 获取有表单条数据
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetTableDataInfo(List<Dictionary<string, object>> dataList, List<FieldsModel> fieldsModels, string actionType)
        {
            //转换表字符串成数组
            foreach (var dataMap in dataList)
            {
                int dicCount = dataMap.Keys.Count;
                string[] strKey = new string[dicCount];
                dataMap.Keys.CopyTo(strKey, 0);
                for (int i = 0; i < strKey.Length; i++)
                {
                    var dataValue = dataMap[strKey[i]];
                    if (dataValue != null && !string.IsNullOrEmpty(dataValue.ToString()) && !dataValue.ToString().Contains("[]"))
                    {
                        var model = fieldsModels.Find(f => f.__vModel__ == strKey[i]);
                        if (model != null)
                        {
                            switch (model.__config__.jnpfKey)
                            {
                                //日期选择
                                case "date":
                                    {
                                        dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], model, "List");
                                    }
                                    break;
                                //创建时间
                                case "createTime":
                                    {
                                        dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], model, "List");
                                    }
                                    break;
                                //修改选择
                                case "modifyTime":
                                    {
                                        dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], model, "List");
                                    }
                                    break;
                                default:
                                    dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], model, actionType);
                                    break;
                            }

                        }
                    }
                }
            }
            return dataList;
        }

        /// <summary>
        /// 转换字典列表内是否有时间戳
        /// </summary>
        /// <param name="dicList"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetTableDataListByDic(List<Dictionary<string, object>> dicList, List<FieldsModel> fieldsModels)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (var dataMap in dicList)
            {
                int dicCount = dataMap.Keys.Count;
                string[] strKey = new string[dicCount];
                dataMap.Keys.CopyTo(strKey, 0);
                for (int i = 0; i < strKey.Length; i++)
                {
                    var dataValue = dataMap[strKey[i]];
                    if (dataValue != null && !string.IsNullOrEmpty(dataValue.ToString()))
                    {
                        var model = fieldsModels.Find(f => f.__vModel__ == strKey[i]);
                        if (model != null)
                        {
                            dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], model, "transition");
                        }
                    }
                }
                list.Add(dataMap);
            }
            return list;
        }

        /// <summary>
        /// 获取有表单条数据 转时间戳
        /// </summary>
        /// <param name="main"></param>
        /// <param name="fieldsModels"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetTableDataInfoByTimeStamp(List<Dictionary<string, object>> dataList, List<FieldsModel> fieldsModels, string actionType)
        {
            //转换表字符串成数组
            foreach (var dataMap in dataList)
            {
                int dicCount = dataMap.Keys.Count;
                string[] strKey = new string[dicCount];
                dataMap.Keys.CopyTo(strKey, 0);
                for (int i = 0; i < strKey.Length; i++)
                {
                    var dataValue = dataMap[strKey[i]];
                    if (dataValue != null && !string.IsNullOrEmpty(dataValue.ToString()) && !dataValue.ToString().Contains("[]"))
                    {
                        var model = fieldsModels.Find(f => f.__vModel__ == strKey[i]);
                        if (model != null)
                        {
                            dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], model, actionType);
                        }
                    }
                }
            }
            return dataList;
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 模板数据转换
        /// </summary>
        /// <param name="fieldsModelList"></param>
        /// <returns></returns>
        public List<FieldsModel> TemplateDataConversion(List<FieldsModel> fieldsModelList)
        {
            var template = new List<FieldsModel>();
            //将模板内的无限children解析出来
            //不包含子表children
            foreach (var item in fieldsModelList)
            {
                var config = item.__config__;
                switch (config.jnpfKey)
                {
                    //栅格布局
                    case "row":
                        {
                            template.AddRange(TemplateDataConversion(config.children));
                        }
                        break;
                    //表格
                    case "table":
                        {
                            template.Add(item);
                        }
                        break;
                    //卡片
                    case "card":
                        {
                            template.AddRange(TemplateDataConversion(config.children));
                        }
                        break;
                    //折叠面板
                    case "collapse":
                        {
                            foreach (var collapse in config.children)
                            {
                                template.AddRange(TemplateDataConversion(collapse.__config__.children));
                            }
                        }
                        break;
                    case "tab":
                        foreach (var collapse in config.children)
                        {
                            template.AddRange(TemplateDataConversion(collapse.__config__.children));
                        }
                        break;
                    //文本
                    case "JNPFText":
                        break;
                    //分割线
                    case "divider":
                        break;
                    //分组标题
                    case "groupTitle":
                        break;
                    default:
                        {
                            template.Add(item);
                        }
                        break;
                }
            }
            return template;
        }

        /// <summary>
        /// 获取有表详情系统组件数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modelList"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> GetTableInfoBySystemComponentsData(string id, List<FieldsModel> modelList, string data)
        {
            //获取Redis缓存模板数据
            var templateData = await GetVisualDevTemplateData(id, modelList);
            data = await GetSystemComponentsData(modelList, templateData, data);
            return data;
        }

        /// <summary>
        /// 删除无表数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DelIsNoTableInfo(string id, VisualDevEntity templateEntity)
        {
            var ids = new List<string>();
            ids.Add(id);
            if (templateEntity.WebType == 3)
            {
                Scoped.Create((_, scope) =>
                {
                    var services = scope.ServiceProvider;

                    var _flowTaskRepository = App.GetService<IFlowTaskRepository>(services);
                    var notAllow = _flowTaskRepository.GetAllowDeleteFlowTaskList(ids);
                    ids = ids.Except(notAllow).ToList();
                });
            }
            id = ids.FirstOrDefault();
            if (id != null)
            {
                var entity = await _visualDevModelDataRepository.FirstOrDefaultAsync(v => v.Id == id && v.DeleteMark == null);
                _ = entity ?? throw JNPFException.Oh(ErrorCode.COM1007);

                try
                {
                    //开启事务
                    _visualDevModelDataRepository.Context.BeginTran();

                    //删除无表表数据
                    await _visualDevModelDataRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).UpdateColumns(it => new { it.DeleteMark, it.DeleteTime, it.DeleteUserId }).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();

                    if (templateEntity.WebType == 3)
                    {
                        Scoped.Create((_, scope) =>
                        {
                            var services = scope.ServiceProvider;
                            var _flowTaskRepository = App.GetService<IFlowTaskRepository>(services);
                            var entity = _flowTaskRepository.GetTaskInfoNoAsync(id);
                            if (entity != null)
                                _flowTaskRepository.DeleteTaskNoAsync(entity);
                        });
                    }

                    //关闭事务
                    _visualDevModelDataRepository.Context.CommitTran();
                }
                catch (Exception)
                {
                    _visualDevModelDataRepository.Context.RollbackTran();
                    throw;
                }
            }
        }

        /// <summary>
        /// 批量删除无表数据
        /// </summary>
        /// <param name="ids">ID数组</param>
        /// <returns></returns>
        public async Task BatchDelIsNoTableData(List<string> ids, VisualDevEntity templateEntity)
        {
            ids = ids.Where(i => i != null).ToList();
            if (templateEntity.WebType == 3)
            {
                Scoped.Create((_, scope) =>
                {
                    var services = scope.ServiceProvider;

                    var _flowTaskRepository = App.GetService<IFlowTaskRepository>(services);
                    var notAllow = _flowTaskRepository.GetAllowDeleteFlowTaskList(ids);
                    ids = ids.Except(notAllow).ToList();
                });
            }
            if (ids.Count > 0)
            {
                var dataList = await _visualDevModelDataRepository.Entities.In(r => r.Id, ids.Where(i => i != null).ToArray()).Where(v => v.DeleteMark == null).ToListAsync();

                try
                {
                    //开启事务
                    _visualDevModelDataRepository.Context.BeginTran();

                    //删除有表数据
                    await _visualDevModelDataRepository.Context.Updateable(dataList).UpdateColumns(it => new { it.DeleteMark, it.DeleteTime, it.DeleteUserId }).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();

                    if (templateEntity.WebType == 3)
                    {
                        Scoped.Create((_, scope) =>
                        {
                            var services = scope.ServiceProvider;

                            var _flowTaskRepository = App.GetService<IFlowTaskRepository>(services);
                            ids.ForEach(it =>
                            {
                                var entity = _flowTaskRepository.GetTaskInfoNoAsync(it);
                                if (entity != null)
                                    _flowTaskRepository.DeleteTaskNoAsync(entity);
                            });
                        });
                    }

                    //关闭事务
                    _visualDevModelDataRepository.Context.CommitTran();
                }
                catch (Exception)
                {
                    _visualDevModelDataRepository.Context.RollbackTran();
                    throw;
                }

            }
        }

        /// <summary>
        /// 删除有表信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="templateEntity">模板实体</param>
        /// <returns></returns>
        public async Task DelHaveTableInfo(string id, VisualDevEntity templateEntity)
        {
            var ids = new List<string>();
            ids.Add(id);
            if (templateEntity.WebType == 3)
            {
                Scoped.Create((_, scope) =>
                {
                    var services = scope.ServiceProvider;

                    var _flowTaskRepository = App.GetService<IFlowTaskRepository>(services);
                    var notAllow = _flowTaskRepository.GetAllowDeleteFlowTaskList(ids);
                    ids = ids.Except(notAllow).ToList();
                });
            }
            id = ids.FirstOrDefault();
            if (ids.Count > 0)
            {
                List<TableModel> tableMapList = templateEntity.Tables.ToObject<List<TableModel>>();
                //获取详情模板
                FormDataModel formDataModel = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).ToObject<FormDataModel>();
                List<FieldsModel> formData = formDataModel.fields;

                var tableList = new List<DbTableFieldModel>();
                //获取主表
                var mainTable = tableMapList.Find(t => t.relationTable == "");

                var link = await _dbLinkService.GetInfo(templateEntity.DbLinkId);
                tableList = _databaseService.GetFieldListByNoAsync(link, mainTable.table);
                //获取主键
                var mainPrimary = tableList.Find(t => t.primaryKey == 1);
                //拼接语句
                //主表删除语句
                StringBuilder delMain = new StringBuilder();
                //总删除语句
                StringBuilder allDelSql = new StringBuilder();
                //查询主表信息语句
                StringBuilder queryMain = new StringBuilder();
                //子表删除语句
                StringBuilder childSql = new StringBuilder();
                delMain.AppendFormat("delete from {0} where {1} = '{2}';", mainTable.table, mainPrimary.field, id);
                allDelSql.Append(delMain);
                queryMain.AppendFormat("select * from {0} where {1}='{2}';", tableMapList.FirstOrDefault().table, mainPrimary.field, id);
                var mainData = _databaseService.GetInterFaceData(link, queryMain.ToString()).Serialize().Deserialize<List<Dictionary<string, object>>>();
                List<Dictionary<string, object>> mainMapList = GetTableDataInfo(mainData, formData, null);
                if (mainMapList.Count > 0)
                {
                    if (tableMapList.Count > 1)
                    {
                        //去除主表
                        tableMapList.Remove(mainTable);
                        foreach (var tableMap in tableMapList)
                        {
                            //主表字段
                            string relationField = tableMap.relationField;
                            string relationFieldValue = mainMapList.First()[relationField].ToString();
                            //子表字段
                            string tableField = tableMap.tableField;
                            childSql.AppendFormat("delete from {0} where {1} = '{2}';", tableMap.table, tableField, relationFieldValue);
                            allDelSql.Append(childSql);
                        }
                    }

                    try
                    {
                        //开启事务
                        _visualDevModelDataRepository.Context.BeginTran();

                        //删除有表数据
                        await _databaseService.ExecuteSql(link, allDelSql.ToString());

                        if (templateEntity.WebType == 3)
                        {
                            Scoped.Create((_, scope) =>
                            {
                                var services = scope.ServiceProvider;
                                var _flowTaskRepository = App.GetService<IFlowTaskRepository>(services);
                                var entity = _flowTaskRepository.GetTaskInfoNoAsync(id);
                                if (entity != null)
                                    _flowTaskRepository.DeleteTaskNoAsync(entity);
                            });
                        }

                        //关闭事务
                        _visualDevModelDataRepository.Context.CommitTran();
                    }
                    catch (Exception)
                    {
                        _visualDevModelDataRepository.Context.RollbackTran();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 批量删除有表数据
        /// </summary>
        /// <param name="ids">id数组</param>
        /// <param name="templateEntity">模板实体</param>
        /// <returns></returns>
        public async Task BatchDelHaveTableData(List<string> ids, VisualDevEntity templateEntity)
        {
            if (templateEntity.WebType == 3)
            {
                Scoped.Create((_, scope) =>
                {
                    var services = scope.ServiceProvider;

                    var _flowTaskRepository = App.GetService<IFlowTaskRepository>(services);
                    var notAllow = _flowTaskRepository.GetAllowDeleteFlowTaskList(ids);
                    ids = ids.Except(notAllow).ToList();
                });
            }
            if (ids.Count > 0)
            {
                List<TableModel> tableMapList = templateEntity.Tables.ToObject<List<TableModel>>();
                //获取详情模板
                FormDataModel formDataModel = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).ToObject<FormDataModel>();
                List<FieldsModel> formData = formDataModel.fields;

                var tableList = new List<DbTableFieldModel>();
                //获取主表
                var mainTable = tableMapList.Find(t => t.relationTable == "");

                var link = await _dbLinkService.GetInfo(templateEntity.DbLinkId);
                tableList = _databaseService.GetFieldListByNoAsync(link, mainTable.table);
                //获取主键
                var mainPrimary = tableList.Find(t => t.primaryKey == 1);
                //拼接语句
                //主表删除语句
                StringBuilder delMain = new StringBuilder();
                //总删除语句
                StringBuilder allDelSql = new StringBuilder();
                //查询主表信息语句
                StringBuilder queryMain = new StringBuilder();
                //子表删除语句
                StringBuilder childSql = new StringBuilder();
                delMain.AppendFormat("delete from {0} where {1} in ('{2}');", mainTable.table, mainPrimary.field, string.Join("','", ids.ToArray()));
                allDelSql.Append(delMain);
                queryMain.AppendFormat("select * from {0} where {1} in ('{2}');", tableMapList.FirstOrDefault().table, mainPrimary.field, string.Join("','", ids.ToArray()));
                var mainData = _databaseService.GetInterFaceData(link, queryMain.ToString()).Serialize().Deserialize<List<Dictionary<string, object>>>();
                List<Dictionary<string, object>> mainMapList = GetTableDataInfo(mainData, formData, null);
                if (mainMapList.Count > 0)
                {
                    if (tableMapList.Count > 1)
                    {
                        //去除主表
                        tableMapList.Remove(mainTable);
                        foreach (var tableMap in tableMapList)
                        {
                            //主表字段
                            string relationField = tableMap.relationField;
                            string relationFieldValue = mainMapList.First()[relationField].ToString();
                            //子表字段
                            string tableField = tableMap.tableField;
                            childSql.AppendFormat("delete from {0} where {1} = '{2}';", tableMap.table, tableField, relationFieldValue);
                            allDelSql.Append(childSql);
                        }
                    }

                    try
                    {
                        //开启事务
                        _visualDevModelDataRepository.Context.BeginTran();

                        //删除有表数据
                        await _databaseService.ExecuteSql(link, allDelSql.ToString());

                        if (templateEntity.WebType == 3)
                        {
                            Scoped.Create((_, scope) =>
                            {
                                var services = scope.ServiceProvider;

                                var _flowTaskRepository = App.GetService<IFlowTaskRepository>(services);
                                ids.ForEach(it =>
                                {
                                    var entity = _flowTaskRepository.GetTaskInfoNoAsync(it);
                                    if (entity != null)
                                        _flowTaskRepository.DeleteTaskNoAsync(entity);
                                });
                            });
                        }

                        //关闭事务
                        _visualDevModelDataRepository.Context.CommitTran();
                    }
                    catch (Exception)
                    {
                        _visualDevModelDataRepository.Context.RollbackTran();
                        throw;
                    }
                }
            }
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
        #endregion
    }
}