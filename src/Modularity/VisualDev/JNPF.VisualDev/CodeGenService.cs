using JNPF.Common.Configuration;
using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Helper;
using JNPF.Common.Util;
using JNPF.DataEncryption;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.System;
using JNPF.ViewEngine;
using JNPF.VisualDev.Entitys;
using JNPF.VisualDev.Entitys.Dto.VisualDev;
using JNPF.VisualDev.Entitys.Model.CodeGen;
using JNPF.VisualDev.Entitys.Model.VisualDevModelData;
using JNPF.VisualDev.Run.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace JNPF.VisualDev
{
    /// <summary>
    /// 可视化开发基础
    /// </summary>
    [ApiDescriptionSettings(Tag = "VisualDev", Name = "Generater", Order = 175)]
    [Route("api/visualdev/[controller]")]
    public class CodeGenService : IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<VisualDevEntity> _visualDevRepository;
        private readonly IViewEngine _viewEngine;
        private readonly IUserManager _userManager;
        private readonly IDbLinkService _dbLinkService;
        private readonly IDataBaseService _databaseService;
        private readonly IRunService _runService;
        private readonly IDictionaryDataService _dictionaryDataService;
        private int active = 1;

        /// <summary>
        /// 初始化一个<see cref="CodeGenService"/>类型的新实例
        /// </summary>
        public CodeGenService(ISqlSugarRepository<VisualDevEntity> visualDevRepository, IDataBaseService databaseService, IDbLinkService dbLinkService, IRunService runService, IViewEngine viewEngine, IUserManager userManager, IDictionaryDataService dictionaryDataService)
        {
            _visualDevRepository = visualDevRepository;
            _viewEngine = viewEngine;
            _userManager = userManager;
            _databaseService = databaseService;
            _dbLinkService = dbLinkService;
            _runService = runService;
            _dictionaryDataService = dictionaryDataService;
        }

        #region Get

        /// <summary>
        /// 获取命名空间
        /// </summary>
        [HttpGet("AreasName")]
        public dynamic GetAreasName()
        {
            List<string> areasName = new List<string>();
            if (KeyVariable.AreasName.Count > 0)
            {
                areasName = KeyVariable.AreasName;
            }
            return areasName;
        }

        #endregion

        #region Post

        /// <summary>
        /// 下载代码
        /// </summary>
        [HttpPost("{id}/Actions/DownloadCode")]
        public async Task<dynamic> DownloadCode(string id, [FromBody] DownloadCodeFormInput downloadCodeForm)
        {
            var userInfo = _userManager.GetUserInfo();
            var templateEntity = await _visualDevRepository.FirstOrDefaultAsync(v => v.Id == id && v.DeleteMark == null);
            _ = templateEntity ?? throw JNPFException.Oh(ErrorCode.COM1005);
            _ = templateEntity.Tables ?? throw JNPFException.Oh(ErrorCode.D2100);
            var model = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).ToObject<FormDataModel>();
            if (templateEntity.Type == 3)
            {
                downloadCodeForm.module = "WorkFlowForm";
            }
            model.className = downloadCodeForm.className.ToPascalCase();
            model.areasName = downloadCodeForm.module;
            string fileName = templateEntity.FullName;
            //判断子表名称
            var childTb = new List<string>();
            if (!downloadCodeForm.subClassName.IsNullOrEmpty())
            {
                childTb = new List<string>(downloadCodeForm.subClassName.Split(','));
            }
            //子表名称去重
            HashSet<string> set = new HashSet<string>(childTb);
            templateEntity.FormData = model.ToJson();
            bool result = childTb.Count == set.Count ? true : false;
            if (!result)
            {
                throw JNPFException.Oh(ErrorCode.D2101);
            }
            await this.TemplatesDataAggregation(fileName, templateEntity);
            string randPath = Path.Combine(FileVariable.GenerateCodePath, fileName);
            string downloadPath = randPath + ".zip";
            //判断是否存在同名称文件
            if (File.Exists(downloadPath))
            {
                File.Delete(downloadPath);
            }
            ZipFile.CreateFromDirectory(randPath, downloadPath);
            var downloadFileName = userInfo.Id + "|" + fileName + ".zip|codeGenerator";
            return new { name = fileName, url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(downloadFileName, "JNPF") };
        }

        /// <summary>
        /// 预览代码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="downloadCodeForm"></param>
        /// <returns></returns>
        [HttpPost("{id}/Actions/CodePreview")]
        public async Task<dynamic> CodePreview(string id, [FromBody] DownloadCodeFormInput downloadCodeForm)
        {
            var userInfo = _userManager.GetUserInfo();
            var templateEntity = await _visualDevRepository.FirstOrDefaultAsync(v => v.Id == id && v.DeleteMark == null);
            _ = templateEntity ?? throw JNPFException.Oh(ErrorCode.COM1005);
            _ = templateEntity.Tables ?? throw JNPFException.Oh(ErrorCode.D2100);
            var model = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).ToObject<FormDataModel>();
            model.className = downloadCodeForm.className.ToPascalCase();
            model.areasName = downloadCodeForm.module;
            string fileName = YitIdHelper.NextId().ToString();
            //判断子表名称
            var childTb = new List<string>();
            //子表名称去重
            HashSet<string> set = new HashSet<string>(childTb);
            templateEntity.FormData = model.ToJson();
            bool result = childTb.Count == set.Count ? true : false;
            if (!result)
            {
                throw JNPFException.Oh(ErrorCode.D2101);
            }
            await this.TemplatesDataAggregation(fileName, templateEntity);
            string randPath = Path.Combine(FileVariable.GenerateCodePath, fileName);
            var dataList = this.PriviewCode(randPath, templateEntity.Type.ToInt(), templateEntity.WebType.ToInt());
            if (dataList == null && dataList.Count == 0)
                throw JNPFException.Oh(ErrorCode.D2102);
            return new { list = dataList };
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 模板数据聚合
        /// </summary>
        /// <param name="fileName">生成ZIP文件名</param>
        /// <param name="templateEntity">模板实体</param>
        /// <returns></returns>
        public async Task TemplatesDataAggregation(string fileName, VisualDevEntity templateEntity)
        {
            var categoryName = (await _dictionaryDataService.GetInfo(templateEntity.Category)).EnCode;
            var funcTablePrimaryKey = string.Empty;
            var tableRelation = templateEntity.Tables.ToObject<List<DbTableRelationModel>>();
            var columnDesignModel = templateEntity.ColumnData.ToObject<ColumnDesignModel>();
            var formDataModel = templateEntity.FormData.ToObject<FormDataModel>();
            var controls = _runService.TemplateDataConversion(formDataModel.fields);

            var targetPathList = new List<string>();
            var templatePathList = new List<string>();
            //主表列配置
            var mainTableColumnConfigList = new List<TableColumnConfigModel>();

            #region 后端

            //后端文件生成
            foreach (var item in tableRelation)
            {
                int tableNum = 0;
                if (item.relationTable != "")
                {
                    //获取出子表控件值
                    var children = controls.Find(f => f.__vModel__.Contains("Field") && f.__config__.tableName.Equals(item.table));
                    if (children != null) controls = children.__config__.children;
                    tableNum++;
                }
                var tableName = item.table;
                var link = await _dbLinkService.GetInfo(templateEntity.DbLinkId);
                var table = _databaseService.GetFieldListByNoAsync(link, tableName);
                var tableColumnList = new List<TableColumnConfigModel>();
                foreach (var column in table)
                {
                    var field = column.field.Replace("F_", "").Replace("f_", "").ToPascalCase().LowerFirstChar();
                    var columnControlKey = controls.Find(c => c.__vModel__ == field);
                    tableColumnList.Add(new TableColumnConfigModel()
                    {
                        ColumnName = column.field.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                        Alias = column.field,
                        OriginalColumnName = column.field,
                        ColumnComment = column.fieldName,
                        DataType = column.dataType,
                        NetType = CodeGenUtil.ConvertDataType(column.dataType),
                        PrimaryKey = column.primaryKey.ToBool(),
                        QueryWhether = columnDesignModel != null ? GetIsColumnQueryWhether(columnDesignModel.searchList, field) : false,
                        QueryType = columnDesignModel != null ? GetColumnQueryType(columnDesignModel.searchList, field) : 0,
                        IsShow = columnDesignModel != null ? GetIsShowColumn(columnDesignModel.columnList, field) : false,
                        IsMultiple = GetIsMultipleColumn(controls, field),
                        jnpfKey = columnControlKey != null ? columnControlKey.__config__.jnpfKey : null,
                        Rule = columnControlKey != null ? columnControlKey.__config__.rule : null
                    });
                }
                if (item.relationTable == "")
                {
                    mainTableColumnConfigList = tableColumnList;
                }

                var IsUplpad = tableColumnList.Find(it => it.jnpfKey != null && (it.jnpfKey.Equals("uploadImg") || it.jnpfKey.Equals("uploadFz")));
                var IsExport = columnDesignModel != null && columnDesignModel.btnsList != null && templateEntity.WebType != 1 ? columnDesignModel.btnsList.Find(it => it.value == "download") : null;
                var IsMapper = tableColumnList.Find(it => it.jnpfKey != null && (it.jnpfKey.Equals("checkbox") || it.jnpfKey.Equals("cascader") || it.jnpfKey.Equals("address") || it.jnpfKey.Equals("uploadImg") || it.jnpfKey.Equals("uploadFz") || (it.jnpfKey.Equals("select") && it.IsMultiple == true)));
                var isBillRule = templateEntity.FormData.Contains("billRule");
                var isFlowId = tableColumnList.Find(it => it.ColumnName.ToLower().Equals("flowid"));
                var configModel = new CodeGenConfigModel();
                if (templateEntity.Type == 4 || templateEntity.Type == 5)
                {
                    //为生成功能方法模块添加数据
                    switch (templateEntity.Type)
                    {
                        case 4:
                            switch (templateEntity.WebType)
                            {
                                case 1:
                                    columnDesignModel = new ColumnDesignModel();
                                    columnDesignModel.btnsList = new List<ButtonConfigModel>();
                                    columnDesignModel.btnsList.Add(new ButtonConfigModel()
                                    {
                                        value = "add",
                                        icon = "el-icon-plus",
                                        label = "新增",
                                    });
                                    break;
                                case 2:
                                    break;
                                case 3:
                                    break;
                            }
                            break;
                        case 5:
                            switch (templateEntity.WebType)
                            {
                                case 1:
                                    columnDesignModel = new ColumnDesignModel();
                                    columnDesignModel.btnsList = new List<ButtonConfigModel>();
                                    columnDesignModel.btnsList.Add(new ButtonConfigModel()
                                    {
                                        value = "add",
                                        icon = "el-icon-plus",
                                        label = "新增",
                                    });
                                    break;
                                case 2:
                                case 3:
                                    columnDesignModel.btnsList = new List<ButtonConfigModel>();
                                    columnDesignModel.columnBtnsList = new List<ButtonConfigModel>();
                                    columnDesignModel.btnsList.Add(new ButtonConfigModel()
                                    {
                                        value = "add",
                                        icon = "el-icon-plus",
                                        label = "新增",
                                    });
                                    columnDesignModel.columnBtnsList.Add(new ButtonConfigModel()
                                    {
                                        value = "edit",
                                        icon = "el-icon-edit",
                                        label = "编辑",
                                    });
                                    columnDesignModel.columnBtnsList.Add(new ButtonConfigModel()
                                    {
                                        value = "remove",
                                        icon = "el-icon-delete",
                                        label = "删除",
                                    });
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    configModel = new CodeGenConfigModel()
                    {
                        NameSpace = formDataModel.areasName,
                        OriginalMainTableName = item.table,
                        PrimaryKey = tableColumnList.Find(t => t.PrimaryKey == true).ColumnName,
                        MainTable = item.table.ToPascalCase(),
                        BusName = templateEntity.FullName,
                        ClassName = formDataModel.className,
                        hasPage = columnDesignModel != null ? columnDesignModel.hasPage : false,
                        Function = GetCodeGenFunctionList(columnDesignModel.hasPage, columnDesignModel.btnsList, columnDesignModel.columnBtnsList, templateEntity.WebType.ToInt()),
                        TableField = tableColumnList,
                        TableRelations = GetCodeGenTableRelationList(tableRelation, item.table, link, controls.FindAll(it => it.__vModel__.Contains("tableField"))),
                    };
                    //为后续程序能正常运行还原数据
                    switch (templateEntity.Type)
                    {
                        case 4:
                            switch (templateEntity.WebType)
                            {
                                case 1:
                                    columnDesignModel = null;
                                    break;
                                case 2:
                                    break;
                                case 3:
                                    break;
                            }
                            break;
                        case 5:
                            switch (templateEntity.WebType)
                            {
                                case 1:
                                    columnDesignModel.btnsList = null;
                                    break;
                                case 2:
                                    columnDesignModel.btnsList = null;
                                    columnDesignModel.columnBtnsList = null;
                                    break;
                                case 3:
                                    columnDesignModel.btnsList = null;
                                    columnDesignModel.columnBtnsList = null;
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
                else if (templateEntity.Type == 3)
                {
                    configModel = new CodeGenConfigModel()
                    {
                        NameSpace = formDataModel.areasName,
                        OriginalMainTableName = item.table,
                        PrimaryKey = tableColumnList.Find(t => t.PrimaryKey == true).ColumnName,
                        MainTable = item.table.ToPascalCase(),
                        BusName = templateEntity.FullName,
                        ClassName = formDataModel.className,
                        hasPage = columnDesignModel != null ? columnDesignModel.hasPage : false,
                        Function = GetCodeGenFlowFunctionList(),
                        TableField = tableColumnList,
                        TableRelations = GetCodeGenTableRelationList(tableRelation, item.table, link, controls.FindAll(it => it.__vModel__.Contains("tableField"))),
                    };
                }
                funcTablePrimaryKey = tableColumnList.Find(t => t.PrimaryKey == true).ColumnName;
                var isChildTable = item.relationTable.IsNullOrEmpty() ? false : true;
                //Web表单&||app代码生成器
                if (templateEntity.Type == 4 || templateEntity.Type == 5)
                {
                    if (!isChildTable)
                    {
                        targetPathList = GetMainTableBackendTargetPathList(item, fileName, templateEntity.WebType.ToInt());
                        templatePathList = GetMainTableBackendTemplatePathList(templateEntity.WebType.ToInt());
                    }
                    else
                    {
                        targetPathList = GetRelationTableBackendTargetPathList(item, fileName, templateEntity.WebType.ToInt());
                        templatePathList = GetRelationTableBackendTemplatePathList(templateEntity.WebType.ToInt());
                    }
                }
                //流程表单
                else if (templateEntity.Type == 3)
                {
                    if (!isChildTable)
                    {
                        targetPathList = GetFlowMainTableBackendTargetPathList(item, fileName);
                        templatePathList = GetFlowMainTableBackendTemplatePathList();
                    }
                    else
                    {
                        targetPathList = GetFlowRelationTableBackendTargetPathList(item, fileName);
                        templatePathList = GetFlowRelationTableBackendTemplatePathList();
                    }
                }

                var defaultSidx = tableColumnList.Find(it => it.PrimaryKey == true).LowerColumnName;
                //生成后端文件
                for (var i = 0; i < templatePathList.Count; i++)
                {
                    var tContent = File.ReadAllText(templatePathList[i]);
                    var tResult = _viewEngine.RunCompileFromCached(tContent, new
                    {
                        BusName = configModel.BusName,
                        NameSpace = configModel.NameSpace,
                        PrimaryKey = configModel.PrimaryKey,
                        MainTable = configModel.MainTable,
                        OriginalMainTableName = configModel.OriginalMainTableName,
                        LowerMainTable = configModel.LowerMainTable,
                        ClassName = configModel.ClassName,
                        hasPage = configModel.hasPage,
                        Function = configModel.Function,
                        TableField = configModel.TableField,
                        TableRelations = configModel.TableRelations,
                        DefaultSidx = columnDesignModel != null && !string.IsNullOrEmpty(columnDesignModel.defaultSidx) ? columnDesignModel.defaultSidx : defaultSidx,
                        IsExport = IsExport == null ? false : true,
                        IsUplpad = IsUplpad == null ? false : true,
                        IsTableRelations = tableNum > 0 ? true : false,
                        ColumnField = IsExport == null ? null : GetMainTableColumnField(columnDesignModel.columnList),
                        IsMapper = IsMapper == null ? false : true,
                        IsBillRule = isBillRule,
                        DbLinkId = templateEntity.DbLinkId,
                        FlowId = templateEntity.Id,
                        WebType = templateEntity.WebType,
                        Type = templateEntity.Type,
                        IsMainTable = item.relationTable == "" ? true : false,
                        isFlowId = isFlowId == null ? true : false,
                        EnCode = templateEntity.EnCode,
                        useDataPermission = columnDesignModel != null ? columnDesignModel.useDataPermission : false,
                        SearchList = tableColumnList.FindAll(it => it.QueryType.Equals(1)).Count()
                    });
                    var dirPath = new DirectoryInfo(targetPathList[i]).Parent.FullName;
                    if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);
                    File.WriteAllText(targetPathList[i], tResult, Encoding.UTF8);
                }
                controls = _runService.TemplateDataConversion(formDataModel.fields);
            }

            #endregion

            #region 前端

            //前端文件生成
            //表单列
            Dictionary<string, List<string>> ListQueryControlsType = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> ListColumnControlsType = new Dictionary<string, List<string>>();


            #region 查询控件归类

            if (templateEntity.Type == 4)
            {
                var useInputList = new List<string>();
                useInputList.Add("comInput");
                useInputList.Add("textarea");
                useInputList.Add("JNPFText");
                useInputList.Add("billRule");
                ListQueryControlsType["inputList"] = useInputList;

                var useDateList = new List<string>();
                useDateList.Add("createTime");
                useDateList.Add("modifyTime");
                ListQueryControlsType["dateList"] = useDateList;

                var useSelectList = new List<string>();
                useSelectList.Add("select");
                useSelectList.Add("radio");
                useSelectList.Add("checkbox");
                ListQueryControlsType["selectList"] = useSelectList;

                var timePickerList = new List<string>();
                timePickerList.Add("time");
                ListQueryControlsType["timePickerList"] = timePickerList;

                var numRangeList = new List<string>();
                numRangeList.Add("numInput");
                numRangeList.Add("calculate");
                ListQueryControlsType["numRangeList"] = numRangeList;

                var datePickerList = new List<string>();
                datePickerList.Add("date");
                ListQueryControlsType["datePickerList"] = datePickerList;

                var userSelectList = new List<string>();
                userSelectList.Add("createUser");
                userSelectList.Add("modifyUser");
                userSelectList.Add("userSelect");
                ListQueryControlsType["userSelectList"] = userSelectList;

                var comSelectList = new List<string>();
                comSelectList.Add("currOrganize");
                comSelectList.Add("comSelect");
                ListQueryControlsType["comSelectList"] = comSelectList;

                var depSelectList = new List<string>();
                depSelectList.Add("currDept");
                depSelectList.Add("depSelect");
                ListQueryControlsType["depSelectList"] = depSelectList;

                var posSelectList = new List<string>();
                posSelectList.Add("currPosition");
                posSelectList.Add("posSelect");
                ListQueryControlsType["posSelectList"] = posSelectList;

                var useCascaderList = new List<string>();
                useCascaderList.Add("cascader");
                ListQueryControlsType["useCascaderList"] = useCascaderList;

                var JNPFAddressList = new List<string>();
                JNPFAddressList.Add("address");
                ListQueryControlsType["JNPFAddressList"] = JNPFAddressList;

                var treeSelectList = new List<string>();
                treeSelectList.Add("treeSelect");
                ListQueryControlsType["treeSelectList"] = treeSelectList;
            }
            else if (templateEntity.Type == 5)
            {
                var inputList = new List<string>();
                inputList.Add("comInput");
                inputList.Add("textarea");
                inputList.Add("JNPFText");
                inputList.Add("billRule");
                inputList.Add("modifyUser");
                inputList.Add("currOrganize");
                inputList.Add("currDept");
                inputList.Add("currPosition");
                inputList.Add("calculate");
                ListQueryControlsType["input"] = inputList;

                var numRangeList = new List<string>();
                numRangeList.Add("numInput");
                ListQueryControlsType["numRange"] = numRangeList;

                var switchList = new List<string>();
                switchList.Add("switch");
                ListQueryControlsType["switch"] = switchList;

                var selectList = new List<string>();
                selectList.Add("radio");
                selectList.Add("checkbox");
                selectList.Add("select");
                ListQueryControlsType["select"] = selectList;

                var cascaderList = new List<string>();
                cascaderList.Add("cascader");
                ListQueryControlsType["cascader"] = cascaderList;

                var timeList = new List<string>();
                timeList.Add("time");
                ListQueryControlsType["time"] = timeList;

                var dateList = new List<string>();
                dateList.Add("date");
                dateList.Add("createTime");
                dateList.Add("modifyTime");
                ListQueryControlsType["date"] = dateList;

                var comSelectList = new List<string>();
                comSelectList.Add("comSelect");
                ListQueryControlsType["comSelect"] = comSelectList;

                var depSelectList = new List<string>();
                depSelectList.Add("depSelect");
                ListQueryControlsType["depSelect"] = depSelectList;

                var posSelectList = new List<string>();
                posSelectList.Add("posSelect");
                ListQueryControlsType["posSelect"] = posSelectList;

                var userSelectList = new List<string>();
                userSelectList.Add("userSelect");
                ListQueryControlsType["userSelect"] = userSelectList;

                var treeSelectList = new List<string>();
                treeSelectList.Add("treeSelect");
                ListQueryControlsType["treeSelect"] = treeSelectList;

                var addressList = new List<string>();
                addressList.Add("address");
                ListQueryControlsType["address"] = addressList;
            }

            #endregion

            #region 列控件归类

            var columnList = new List<string>();
            columnList.Add("select");
            columnList.Add("radio");
            columnList.Add("checkbox");
            columnList.Add("treeSelect");
            columnList.Add("cascader");
            ListColumnControlsType["columnList"] = columnList;

            #endregion

            //表单全控件
            var formAllControlsList = GetFormAllControlsList(formDataModel.fields, formDataModel.gutter, true);

            //表单控件
            var formColimnList = new List<CodeGenFormColumnModel>();
            //列表主表控件Option
            var indnxListMainTableControlOption = GetFormAllControlsProps(formDataModel.fields, templateEntity.Type.ToInt(), true);
            foreach (var item in controls)
            {
                var config = item.__config__;
                switch (config.jnpfKey)
                {
                    case "table":
                        {
                            var childrenFormColimnList = new List<CodeGenFormColumnModel>();
                            foreach (var children in config.children)
                            {
                                var childrenConfig = children.__config__;
                                childrenFormColimnList.Add(new CodeGenFormColumnModel()
                                {
                                    Name = children.__vModel__.ToPascalCase(),
                                    OriginalName = children.__vModel__,
                                    jnpfKey = childrenConfig.jnpfKey,
                                    DataType = childrenConfig.dataType,
                                    DictionaryType = childrenConfig.dataType == "dictionary" ? childrenConfig.dictionaryType : (childrenConfig.dataType == "dynamic" ? childrenConfig.propsUrl : null),
                                    Format = children.format,
                                    Multiple = children.multiple,
                                    BillRule = childrenConfig.rule,
                                    Required = childrenConfig.required,
                                    Placeholder = childrenConfig.label,
                                    Range = children.range,
                                    RegList = childrenConfig.regList,
                                    DefaultValue = childrenConfig.defaultValue != null && childrenConfig.defaultValue.ToString() != "[]" && !string.IsNullOrEmpty(childrenConfig.defaultValue.ToString()) ? childrenConfig.defaultValue : null,
                                    Trigger = childrenConfig.trigger.ToJson() == "null" ? "blur" : (childrenConfig.trigger.ToJson().Contains("[") ? childrenConfig.trigger.ToJson() : childrenConfig.trigger.ToString()),
                                    ChildrenList = null
                                });
                            }
                            formColimnList.Add(new CodeGenFormColumnModel()
                            {
                                Name = config.tableName.ToPascalCase(),
                                Placeholder = config.label,
                                OriginalName = config.tableName,
                                jnpfKey = config.jnpfKey,
                                ChildrenList = childrenFormColimnList
                            });
                        }
                        break;
                    default:
                        {
                            var originalName = mainTableColumnConfigList.Find(it => it.LowerColumnName == item.__vModel__);
                            formColimnList.Add(new CodeGenFormColumnModel()
                            {
                                Name = item.__vModel__.ToPascalCase(),
                                OriginalName = originalName.Alias,
                                jnpfKey = config.jnpfKey,
                                DataType = config.dataType,
                                DictionaryType = config.dataType == "dictionary" ? config.dictionaryType : (config.dataType == "dynamic" ? config.propsUrl : null),
                                Format = item.format,
                                Multiple = item.multiple,
                                BillRule = config.rule,
                                Required = config.required,
                                Placeholder = config.label,
                                Range = item.range,
                                RegList = config.regList,
                                DefaultValue = config.defaultValue != null && config.defaultValue.ToString() != "[]" && !string.IsNullOrEmpty(config.defaultValue.ToString()) ? config.defaultValue : null,
                                Trigger = config.trigger.ToJson() == "null" ? "blur" : (config.trigger.ToJson().Contains("[") ? config.trigger.ToJson() : config.trigger.ToString()),
                                ChildrenList = null
                            });
                        }
                        break;
                }
            }

            //列表页查询
            var indexSearchColumnDesignList = new List<CodeGenConvIndexSearchColumnDesign>();
            //列表页按钮
            var indexListTopButtonList = new List<CodeGenConvIndexListTopButtonDesign>();
            //列表列按钮
            var indexListColumnButtonDesignList = new List<CodeGenConvIndexListTopButtonDesign>();
            //列表页列表
            var indexListColumnDesignList = new List<CodeGenConvIndexListColumnDesign>();
            if (templateEntity.WebType != 1 && templateEntity.Type != 3)
            {
                foreach (var item in columnDesignModel.searchList)
                {
                    var column = controls.Find(c => c.__vModel__ == item.__vModel__);
                    var queryControls = ListQueryControlsType.Where(q => q.Value.Contains(column.__config__.jnpfKey)).FirstOrDefault();
                    indexSearchColumnDesignList.Add(new CodeGenConvIndexSearchColumnDesign()
                    {
                        Name = item.__vModel__.ToPascalCase(),
                        Tag = item.__config__.tag,
                        Clearable = item.clearable ? "clearable " : "",
                        Format = column.format,
                        ValueFormat = column.valueformat,
                        Label = item.__config__.label,
                        QueryControlsKey = queryControls.Key != null ? queryControls.Key : null,
                        Props = column.__config__.props,
                        Index = columnDesignModel.searchList.IndexOf(item),
                        Type = column.type,
                        ShowAllLevels = column.showalllevels ? "true" : "false",
                        Level = column.level
                    });
                }
                //为生成功能方法模块添加数据
                switch (templateEntity.Type)
                {
                    case 5:
                        switch (templateEntity.WebType)
                        {
                            case 2:
                            case 3:
                                columnDesignModel.btnsList = new List<ButtonConfigModel>();
                                columnDesignModel.columnBtnsList = new List<ButtonConfigModel>();
                                break;
                        }
                        break;
                }
                foreach (var item in columnDesignModel.btnsList)
                {
                    indexListTopButtonList.Add(new CodeGenConvIndexListTopButtonDesign()
                    {
                        Type = columnDesignModel.btnsList.IndexOf(item) == 0 ? "primary" : "text",
                        Icon = item.icon,
                        Method = GetCodeGenConvIndexListTopButtonMethod(item.value),
                        Value = item.value,
                        Label = item.label
                    });
                }

                foreach (var item in columnDesignModel.columnBtnsList)
                {
                    indexListColumnButtonDesignList.Add(new CodeGenConvIndexListTopButtonDesign()
                    {
                        Type = item.value == "remove" ? "class=\"JNPF-table-delBtn\" " : "",
                        Icon = item.icon,
                        Method = GetCodeGenConvIndexListColumnButtonMethod(item.value, funcTablePrimaryKey),
                        Value = item.value,
                        Label = item.label,
                        Disabled = GetCodeGenWorkflowIndexListColumnButtonDisabled(item.value)
                    });
                }
                switch (templateEntity.Type)
                {
                    case 5:
                        switch (templateEntity.WebType)
                        {
                            case 2:
                            case 3:
                                columnDesignModel.btnsList = null;
                                columnDesignModel.columnBtnsList = null;
                                break;
                        }
                        break;
                }

                foreach (var item in columnDesignModel.columnList)
                {
                    var conversion = ListColumnControlsType.Where(q => q.Value.Contains(item.jnpfKey)).FirstOrDefault();
                    indexListColumnDesignList.Add(new CodeGenConvIndexListColumnDesign()
                    {
                        Name = item.prop.ToPascalCase(),
                        jnpfKey = item.jnpfKey,
                        Label = item.label,
                        Width = item.width == "0" ? "" : "width=\"" + item.width + "\" ",
                        Align = item.align,
                        IsAutomatic = conversion.Key == null ? false : true,
                        IsSort = item.sortable ? "sortable=\"custom\" " : ""
                    });
                }
            }

            var isBatchRemoveDel = indexListTopButtonList.Find(it => it.Value == "batchRemove");
            var isDownload = indexListTopButtonList.Find(it => it.Value == "download");
            var isRemoveDel = indexListColumnButtonDesignList.Find(it => it.Value == "remove");
            var isSort = columnDesignModel != null && columnDesignModel.columnList != null ? columnDesignModel.columnList.Find(it => it.sortable == true) : null;

            var convFromConfigModel = new CodeGenConvFormConfigModel()
            {
                NameSpace = formDataModel.areasName,
                ClassName = formDataModel.className,
                PrimaryKey = funcTablePrimaryKey,
                BusName = tableRelation.FirstOrDefault().tableName,
                FormList = formColimnList,
                PopupType = formDataModel.popupType,
                SearchColumnDesign = indexSearchColumnDesignList,
                AllColumnDesign = columnDesignModel,
                TopButtonDesign = indexListTopButtonList,
                ColumnDesign = indexListColumnDesignList,
                ColumnButtonDesign = indexListColumnButtonDesignList,
                TreeRelation = columnDesignModel != null && columnDesignModel.treeRelation != null ? columnDesignModel.treeRelation.Replace("F_", "").Replace("f_", "").ToPascalCase().LowerFirstChar() : null,
                IsExistQuery = columnDesignModel != null && columnDesignModel.searchList != null ? columnDesignModel.searchList.Select(it => it.__vModel__ == columnDesignModel.treeRelation).ToBool() : false,
                OptionsList = indnxListMainTableControlOption,
                IsBatchRemoveDel = isBatchRemoveDel == null ? false : true,
                IsDownload = isDownload == null ? false : true,
                IsRemoveDel = isRemoveDel == null ? false : true,
                FormAllContols = formAllControlsList,
                Type = columnDesignModel != null ? columnDesignModel.type : 0
            };
            targetPathList = new List<string>();
            templatePathList = new List<string>();

            var mianTable = tableRelation.Find(it => it.relationTable == "");
            //Web表单
            if (templateEntity.Type == 4)
            {
                targetPathList = GetFrontendTargetPathList(mianTable, fileName, templateEntity.WebType.ToInt());
                templatePathList = GetFrontendTemplatePathList(templateEntity.WebType.ToInt());
            }
            //app代码生成器
            else if (templateEntity.Type == 5)
            {
                targetPathList = GetAppFrontendTargetPathList(mianTable, fileName, templateEntity.WebType.ToInt());
                templatePathList = GetAppFrontendTemplatePathList(templateEntity.WebType.ToInt());
            }
            //流程表单
            else if (templateEntity.Type == 3)
            {
                targetPathList = GetFlowFrontendTargetPathList(mianTable, fileName);
                templatePathList = GetFlowFrontendTemplatePathList();
            }
            var flowTemplateJson = templateEntity.FlowTemplateJson != null ? templateEntity.FlowTemplateJson.ToJson() : null;
            if (tableRelation.Count > 1)
            {
                foreach (var item in tableRelation)
                {
                    if (item.relationTable != "")
                    {
                        var tableField = controls.Find(it => it.__config__.jnpfKey.Equals("table") && it.__config__.tableName.Equals(item.table));
                        if (flowTemplateJson != null)
                            flowTemplateJson = flowTemplateJson.Replace(tableField.__vModel__, item.table.ToPascalCase().LowerFirstChar());
                    }
                }
            }
            for (var i = 0; i < templatePathList.Count; i++)
            {
                var tContent = File.ReadAllText(templatePathList[i]);
                var tResult = _viewEngine.RunCompileFromCached(tContent, new
                {
                    BusName = convFromConfigModel.BusName,
                    ClassName = formDataModel.className,
                    NameSpace = convFromConfigModel.NameSpace,
                    PrimaryKey = convFromConfigModel.PrimaryKey.LowerFirstChar(),
                    FormDataModel = formDataModel,
                    FormList = formColimnList,
                    PopupType = formDataModel.popupType,
                    SearchColumnDesign = convFromConfigModel.SearchColumnDesign,
                    AllColumnDesign = convFromConfigModel.AllColumnDesign,
                    TopButtonDesign = convFromConfigModel.TopButtonDesign,
                    ColumnDesign = convFromConfigModel.ColumnDesign,
                    ColumnButtonDesign = convFromConfigModel.ColumnButtonDesign,
                    TreeRelation = convFromConfigModel.TreeRelation,
                    IsExistQuery = convFromConfigModel.IsExistQuery,
                    Type = convFromConfigModel.Type,
                    OptionsList = convFromConfigModel.OptionsList,
                    IsBatchRemoveDel = convFromConfigModel.IsBatchRemoveDel,
                    IsDownload = convFromConfigModel.IsDownload,
                    IsRemoveDel = convFromConfigModel.IsRemoveDel,
                    IsSort = isSort == null ? false : true,
                    FormAllContols = convFromConfigModel.FormAllContols,
                    DefaultSidx = columnDesignModel != null && columnDesignModel.defaultSidx != null ? columnDesignModel.defaultSidx : "",
                    EnCode = templateEntity.EnCode,
                    FlowId = templateEntity.Id,
                    FullName = templateEntity.FullName,
                    DbLinkId = templateEntity.DbLinkId,
                    FlowTemplateJson = flowTemplateJson,
                    Tables = templateEntity.Tables.ToJson(),
                    Category = categoryName,
                    MianTable = mianTable.table.ToPascalCase().LowerFirstChar(),
                    WebType = templateEntity.WebType,
                    HasPage = columnDesignModel != null ? columnDesignModel.hasPage : false,
                }, builderAction: builder =>
                {
                    builder.AddUsing("JNPF.JsonSerialization");
                    builder.AddUsing("JNPF.VisualDev.Entitys.Model.CodeGen");
                    builder.AddAssemblyReferenceByName("JNPF.VisualDev.Entitys");
                });
                var dirPath = new DirectoryInfo(targetPathList[i]).Parent.FullName;
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
                File.WriteAllText(targetPathList[i], tResult, Encoding.UTF8);
            }

            #endregion

        }

        /// <summary>
        /// 预览代码
        /// </summary>
        /// <param name="codePath"></param>
        /// <param name="type">1-Web设计,2-App设计,3-流程表单,4-Web表单,5-App表单</param>
        /// <param name="webType">页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）</param>
        /// <returns></returns>
        public List<Dictionary<string, object>> PriviewCode(string codePath, int type, int webType)
        {
            var dataList = FileHelper.GetAllFiles(codePath);
            var parentFolder = dataList.Find(it => it.FullName.Contains("html")).Directory.Name;
            List<Dictionary<string, string>> datas = new List<Dictionary<string, string>>();
            List<Dictionary<string, object>> allDatas = new List<Dictionary<string, object>>();
            foreach (var item in dataList)
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                FileStream fileStream = new FileStream(item.FullName, FileMode.Open);
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    var buffer = new Char[(int)reader.BaseStream.Length];
                    reader.Read(buffer, 0, (int)reader.BaseStream.Length);
                    var content = new string(buffer);
                    if ("cs".Equals(item.Extension.Replace(".", "")))
                    {
                        string fileName = item.FullName.ToLower();
                        if (fileName.Contains("listqueryinput") || fileName.Contains("crinput") || fileName.Contains("upinput") || fileName.Contains("upoutput") || fileName.Contains("listoutput") || fileName.Contains("infooutput"))
                        {
                            data.Add("folderName", "dto");
                        }
                        else if (fileName.Contains("mapper"))
                        {
                            data.Add("folderName", "mapper");
                        }
                        else if (fileName.Contains("entity"))
                        {
                            data.Add("folderName", "entity");
                        }
                        else
                        {
                            data.Add("folderName", "dotnet");
                        }
                        data.Add("fileName", item.Name);
                        //剔除"\0"特殊符号
                        data.Add("fileContent", content.Replace("\0", ""));
                        data.Add("fileType", item.Extension.Replace(".", ""));
                        datas.Add(data);
                    }
                    else if (".json".Equals(item.Extension))
                    {
                        data.Add("folderName", "json");
                        data.Add("id", YitIdHelper.NextId().ToString());
                        data.Add("fileName", item.Name);
                        //剔除"\0"特殊符号
                        data.Add("fileContent", content.Replace("\0", ""));
                        data.Add("fileType", item.Extension.Replace(".", ""));
                        datas.Add(data);

                    }
                    else if (".vue".Equals(item.Extension))
                    {
                        switch (type)
                        {
                            case 5:
                                data.Add("folderName", "app");
                                break;
                            default:
                                data.Add("folderName", "web");
                                break;
                        }
                        data.Add("id", YitIdHelper.NextId().ToString());
                        data.Add("fileName", item.Name);
                        //剔除"\0"特殊符号
                        data.Add("fileContent", content.Replace("\0", ""));
                        data.Add("fileType", item.Extension.Replace(".", ""));
                        datas.Add(data);
                    }
                }
            }
            //datas 集合去重
            var parent = datas.GroupBy(d => d["folderName"]).Select(d => d.First()).OrderByDescending(d => d["folderName"]).ToList();
            foreach (var item in parent)
            {
                Dictionary<string, object> dataMap = new Dictionary<string, object>();
                dataMap["fileName"] = item["folderName"];
                dataMap["id"] = YitIdHelper.NextId().ToString();
                dataMap["children"] = datas.FindAll(d => d["folderName"] == item["folderName"]);
                allDatas.Add(dataMap);
            }
            return allDatas;
        }

        #endregion

        #region PrivateMethod

        #region 后端主表

        /// <summary>
        /// 获取后端主表模板文件路径集合
        /// </summary>
        /// <param name="webType">页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）</param>
        /// <returns></returns>
        private List<string> GetMainTableBackendTemplatePathList(int webType)
        {
            var templatePath = Path.Combine(App.WebHostEnvironment.WebRootPath, "Template");
            switch (webType)
            {
                case 1:
                    return new List<string>()
                    {
                        Path.Combine(templatePath , "Service.cs.vm"),
                        Path.Combine(templatePath , "IService.cs.vm"),
                        Path.Combine(templatePath , "Entity.cs.vm"),
                        Path.Combine(templatePath , "CrInput.cs.vm"),
                        Path.Combine(templatePath , "Mapper.cs.vm"),
                    };
                case 2:
                    return new List<string>()
                    {
                        Path.Combine(templatePath , "Service.cs.vm"),
                        Path.Combine(templatePath , "IService.cs.vm"),
                        Path.Combine(templatePath , "Entity.cs.vm"),
                        Path.Combine(templatePath , "Mapper.cs.vm"),
                        Path.Combine(templatePath , "CrInput.cs.vm"),
                        Path.Combine(templatePath , "UpInput.cs.vm"),
                        Path.Combine(templatePath , "ListQueryInput.cs.vm"),
                        Path.Combine(templatePath , "InfoOutput.cs.vm"),
                        Path.Combine(templatePath , "ListOutput.cs.vm")
                    };
                case 3:
                    return new List<string>()
                    {
                        Path.Combine(templatePath , "WorkflowService.cs.vm"),
                        Path.Combine(templatePath , "IService.cs.vm"),
                        Path.Combine(templatePath , "Entity.cs.vm"),
                        Path.Combine(templatePath , "Mapper.cs.vm"),
                        Path.Combine(templatePath , "CrInput.cs.vm"),
                        Path.Combine(templatePath , "UpInput.cs.vm"),
                        Path.Combine(templatePath , "ListQueryInput.cs.vm"),
                        Path.Combine(templatePath , "InfoOutput.cs.vm"),
                        Path.Combine(templatePath , "ListOutput.cs.vm")
                    };
            }
            return null;
        }

        /// <summary>
        /// 获取后端主表生成文件路径
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="fileName"></param>
        /// <param name="webType">页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）</param>
        /// <returns></returns>
        private List<string> GetMainTableBackendTargetPathList(DbTableRelationModel tableInfo, string fileName, int webType)
        {
            var backendPath = Path.Combine(FileVariable.GenerateCodePath, fileName, "Net");
            var tableName = tableInfo.table.ToPascalCase();
            var servicePath = Path.Combine(backendPath, "Controller", tableName + "Service.cs");
            var iservicePath = Path.Combine(backendPath, "Controller", "I" + tableName + "Service.cs");
            var entityPath = Path.Combine(backendPath, "Models", "Entity", tableName + "Entity.cs");
            var mapperPath = Path.Combine(backendPath, "Models", "Mapper", tableName + "Mapper.cs");
            var inputCrPath = Path.Combine(backendPath, "Models", "Dto", tableName + "CrInput.cs");
            var inputUpPath = Path.Combine(backendPath, "Models", "Dto", tableName + "UpInput.cs");
            var inputListQueryPath = Path.Combine(backendPath, "Models", "Dto", tableName + "ListQueryInput.cs");
            var outputInfoPath = Path.Combine(backendPath, "Models", "Dto", tableName + "InfoOutput.cs");
            var outputListPath = Path.Combine(backendPath, "Models", "Dto", tableName + "ListOutput.cs");
            switch (webType)
            {
                case 1:
                    return new List<string>()
                    {
                        servicePath,
                        iservicePath,
                        entityPath,
                        inputCrPath,
                        mapperPath
                    };
                case 2:
                    return new List<string>()
                    {
                        servicePath,
                        iservicePath,
                        entityPath,
                        mapperPath,
                        inputCrPath,
                        inputUpPath,
                        inputListQueryPath,
                        outputInfoPath,
                        outputListPath
                    };
                case 3:
                    return new List<string>()
                    {
                        servicePath,
                        iservicePath,
                        entityPath,
                        mapperPath,
                        inputCrPath,
                        inputUpPath,
                        inputListQueryPath,
                        outputInfoPath,
                        outputListPath
                    };
            }
            return null;
        }

        /// <summary>
        /// 获取流程后端主表模板文件路径集合
        /// </summary>
        /// <returns></returns>
        private List<string> GetFlowMainTableBackendTemplatePathList()
        {
            var templatePath = Path.Combine(App.WebHostEnvironment.WebRootPath, "Template");
            return new List<string>()
            {
                Path.Combine(templatePath , "WorkFlowFormService.cs.vm"),
                Path.Combine(templatePath , "IService.cs.vm"),
                Path.Combine(templatePath , "Entity.cs.vm"),
                Path.Combine(templatePath , "Mapper.cs.vm"),
                Path.Combine(templatePath , "CrInput.cs.vm"),
                Path.Combine(templatePath , "UpInput.cs.vm"),
                Path.Combine(templatePath , "InfoOutput.cs.vm")
            };
        }

        /// <summary>
        /// 获取流程后端主表生成文件路径
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private List<string> GetFlowMainTableBackendTargetPathList(DbTableRelationModel tableInfo, string fileName)
        {
            var backendPath = Path.Combine(FileVariable.GenerateCodePath, fileName, "Net");
            var tableName = tableInfo.table.ToPascalCase();
            var servicePath = Path.Combine(backendPath, "Controller", tableName + "Service.cs");
            var iservicePath = Path.Combine(backendPath, "Controller", "I" + tableName + "Service.cs");
            var entityPath = Path.Combine(backendPath, "Models", "Entity", tableName + "Entity.cs");
            var mapperPath = Path.Combine(backendPath, "Models", "Mapper", tableName + "Mapper.cs");
            var inputCrPath = Path.Combine(backendPath, "Models", "Dto", tableName + "CrInput.cs");
            var inputUpPath = Path.Combine(backendPath, "Models", "Dto", tableName + "UpInput.cs");
            var outputInfoPath = Path.Combine(backendPath, "Models", "Dto", tableName + "InfoOutput.cs");
            return new List<string>()
            {
                servicePath,
                iservicePath,
                entityPath,
                mapperPath,
                inputCrPath,
                inputUpPath,
                outputInfoPath
            };
        }

        #endregion

        #region 后端关系表

        /// <summary>
        /// 获取后端主表模板文件路径集合
        /// </summary>
        /// <param name="webType">页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）</param>
        /// <returns></returns>
        private List<string> GetRelationTableBackendTemplatePathList(int webType)
        {
            var templatePath = Path.Combine(App.WebHostEnvironment.WebRootPath, "Template");
            switch (webType)
            {
                case 1:
                    return new List<string>()
                    {
                        Path.Combine(templatePath , "Entity.cs.vm"),
                        Path.Combine(templatePath , "CrInput.cs.vm")
                    };
                case 2:
                case 3:
                    return new List<string>()
                    {
                        Path.Combine(templatePath , "Entity.cs.vm"),
                        Path.Combine(templatePath , "Mapper.cs.vm"),
                        Path.Combine(templatePath , "CrInput.cs.vm"),
                        Path.Combine(templatePath , "UpInput.cs.vm"),
                        Path.Combine(templatePath , "InfoOutput.cs.vm")
                    };
            }
            return null;
        }

        /// <summary>
        /// 获取后端关系表生成文件路径
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="fileName"></param>
        /// <param name="webType">页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）</param>
        /// <returns></returns>
        private List<string> GetRelationTableBackendTargetPathList(DbTableRelationModel tableInfo, string fileName, int webType)
        {
            var backendPath = Path.Combine(FileVariable.GenerateCodePath, fileName, "Net");
            var tableName = tableInfo.table.ToPascalCase();
            var entityPath = Path.Combine(backendPath, "Models", "Entity", tableName + "Entity.cs");
            var mapperPath = Path.Combine(backendPath, "Models", "Mapper", tableName + "Mapper.cs");
            var inputCrPath = Path.Combine(backendPath, "Models", "Dto", tableName + "CrInput.cs");
            var inputUpPath = Path.Combine(backendPath, "Models", "Dto", tableName + "UpInput.cs");
            var outputInfoPath = Path.Combine(backendPath, "Models", "Dto", tableName + "InfoOutput.cs");

            switch (webType)
            {
                case 1:
                    return new List<string>()
                    {
                        entityPath,
                        inputCrPath
                    };
                case 2:
                    return new List<string>()
                    {
                        entityPath,
                        mapperPath,
                        inputCrPath,
                        inputUpPath,
                        outputInfoPath
                    };
                case 3:
                    return new List<string>()
                    {
                        entityPath,
                        mapperPath,
                        inputCrPath,
                        inputUpPath,
                        outputInfoPath
                    };
            }
            return null;
        }

        /// <summary>
        /// 获取流程主表模板文件路径集合
        /// </summary>
        /// <returns></returns>
        private List<string> GetFlowRelationTableBackendTemplatePathList()
        {
            var templatePath = Path.Combine(App.WebHostEnvironment.WebRootPath, "Template");
            return new List<string>()
            {
                Path.Combine(templatePath , "Entity.cs.vm"),
                Path.Combine(templatePath , "Mapper.cs.vm"),
                Path.Combine(templatePath , "CrInput.cs.vm"),
                Path.Combine(templatePath , "UpInput.cs.vm"),
                Path.Combine(templatePath , "InfoOutput.cs.vm")
            };
        }

        /// <summary>
        /// 获取流程后端关系表生成文件路径
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private List<string> GetFlowRelationTableBackendTargetPathList(DbTableRelationModel tableInfo, string fileName)
        {
            var backendPath = Path.Combine(FileVariable.GenerateCodePath, fileName, "Net");
            var tableName = tableInfo.table.ToPascalCase();
            var entityPath = Path.Combine(backendPath, "Models", "Entity", tableName + "Entity.cs");
            var mapperPath = Path.Combine(backendPath, "Models", "Mapper", tableName + "Mapper.cs");
            var inputCrPath = Path.Combine(backendPath, "Models", "Dto", tableName + "CrInput.cs");
            var inputUpPath = Path.Combine(backendPath, "Models", "Dto", tableName + "UpInput.cs");
            var outputInfoPath = Path.Combine(backendPath, "Models", "Dto", tableName + "InfoOutput.cs");
            return new List<string>()
            {
                entityPath,
                mapperPath,
                inputCrPath,
                inputUpPath,
                outputInfoPath
            };
        }

        #endregion

        #region 前端页面

        /// <summary>
        /// 获取前端页面模板文件路径集合
        /// </summary>
        /// <param name="webType">页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）</param>
        /// <returns></returns>
        private List<string> GetFrontendTemplatePathList(int webType)
        {
            var templatePath = Path.Combine(App.WebHostEnvironment.WebRootPath, "Template");
            switch (webType)
            {
                case 1:
                    return new List<string>()
                    {
                        Path.Combine(templatePath , "Form.vue.vm")
                    };
                case 2:
                    return new List<string>()
                    {
                        Path.Combine(templatePath , "index.vue.vm"),
                        Path.Combine(templatePath , "Form.vue.vm"),
                        Path.Combine(templatePath , "ExportBox.vue.vm")
                    };
                case 3:
                    return new List<string>()
                    {
                        Path.Combine(templatePath , "WorkflowIndex.vue.vm"),
                        Path.Combine(templatePath , "WorkflowForm.vue.vm"),
                        Path.Combine(templatePath , "ExportBox.vue.vm"),
                        Path.Combine(templatePath , "ExportJson.json.vm")
                    };
                default:
                    break;
            }
            return null;
        }

        /// <summary>
        /// 设置前端页面生成文件路径
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="fileName"></param>
        /// <param name="webType">页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）</param>
        /// <returns></returns>
        private List<string> GetFrontendTargetPathList(DbTableRelationModel tableInfo, string fileName, int webType)
        {
            var frontendPath = Path.Combine(FileVariable.GenerateCodePath, fileName, "Net");
            var tableName = tableInfo.table.ToPascalCase().LowerFirstChar();
            var indexPath = Path.Combine(frontendPath, "html", tableName, "index.vue");
            var formPath = Path.Combine(frontendPath, "html", tableName, "Form.vue");
            var exportBoxPath = Path.Combine(frontendPath, "html", tableName, "ExportBox.vue");
            var exportJsonPath = Path.Combine(frontendPath, "json", "ExportJson.json");
            switch (webType)
            {
                case 1:
                    return new List<string>()
                    {
                        indexPath
                    };
                case 2:
                    return new List<string>()
                    {
                        indexPath,
                        formPath,
                        exportBoxPath
                    };
                case 3:
                    return new List<string>()
                    {
                        indexPath,
                        formPath,
                        exportBoxPath,
                        exportJsonPath
                    };
            }
            return null;
        }

        /// <summary>
        /// 获取App前端页面模板文件路径集合
        /// </summary>
        /// <param name="webType"></param>
        /// <returns></returns>
        private List<string> GetAppFrontendTemplatePathList(int webType)
        {
            var templatePath = Path.Combine(App.WebHostEnvironment.WebRootPath, "Template");
            switch (webType)
            {
                case 1:
                    return new List<string>()
                    {
                        Path.Combine(templatePath , "appForm.vue.vm")
                    };
                case 2:
                    return new List<string>()
                    {
                        Path.Combine(templatePath , "appIndex.vue.vm"),
                        Path.Combine(templatePath , "appForm.vue.vm")
                    };
                case 3:
                    return new List<string>()
                    {
                        Path.Combine(templatePath , "appWorkflowIndex.vue.vm"),
                        Path.Combine(templatePath , "appWorkflowForm.vue.vm"),
                        Path.Combine(templatePath , "ExportJson.json.vm")
                    };
                default:
                    break;
            }
            return null;
        }

        /// <summary>
        /// 设置App前端页面生成文件路径
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="fileName"></param>
        /// <param name="webType">页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）</param>
        /// <returns></returns>
        private List<string> GetAppFrontendTargetPathList(DbTableRelationModel tableInfo, string fileName, int webType)
        {
            var frontendPath = Path.Combine(FileVariable.GenerateCodePath, fileName, "Net");
            var tableName = tableInfo.table.ToPascalCase().LowerFirstChar();
            var indexPath = Path.Combine(frontendPath, "html", tableName, "index.vue");
            var formPath = Path.Combine(frontendPath, "html", tableName, "form.vue");
            var exportJsonPath = Path.Combine(frontendPath, "json", "ExportJson.json");
            switch (webType)
            {
                case 1:
                    return new List<string>()
                    {
                        indexPath
                    };
                case 2:
                    return new List<string>()
                    {
                        indexPath,
                        formPath,
                    };
                case 3:
                    return new List<string>()
                    {
                        Path.Combine(frontendPath, "html", "app","index", "index.vue"),
                        Path.Combine(frontendPath, "html", "app","form", "index.vue"),
                        exportJsonPath
                    };
            }
            return null;
        }

        /// <summary>
        /// 获取流程前端页面模板文件路径集合
        /// </summary>
        /// <returns></returns>
        private List<string> GetFlowFrontendTemplatePathList()
        {
            var templatePath = Path.Combine(App.WebHostEnvironment.WebRootPath, "Template");
            return new List<string>()
            {
                Path.Combine(templatePath , "WorkflowForm.vue.vm"),
                Path.Combine(templatePath , "appWorkflowForm.vue.vm")
            };
        }

        /// <summary>
        /// 设置流程前端页面生成文件路径
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private List<string> GetFlowFrontendTargetPathList(DbTableRelationModel tableInfo, string fileName)
        {
            var frontendPath = Path.Combine(FileVariable.GenerateCodePath, fileName, "Net");
            var tableName = tableInfo.table.ToPascalCase().LowerFirstChar();
            var indexPath = Path.Combine(frontendPath, "html", "PC", tableName, "index.vue");
            var indexAppPath = Path.Combine(frontendPath, "html", "APP", tableName, "index.vue");
            return new List<string>()
            {
                indexPath,
                indexAppPath
            };
        }

        #endregion

        /// <summary>
        /// 获取主表字段名
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string GetMainTableColumnField(List<IndexGridFieldModel> list)
        {
            StringBuilder columnSb = new StringBuilder();
            foreach (var item in list)
            {
                columnSb.AppendFormat("{{\\\"value\\\":\\\"{0}\\\",\\\"field\\\":\\\"{1}\\\"}},", item.label, item.prop);
            }
            return columnSb.ToString();
        }

        /// <summary>
        /// 获取是否查询列
        /// </summary>
        /// <param name="searchList"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        private bool GetIsColumnQueryWhether(List<FieldsModel> searchList, string alias)
        {
            var column = searchList.Find(s => s.__vModel__ == alias);
            return column == null ? false : true;
        }

        /// <summary>
        /// 获取列查询类型
        /// </summary>
        /// <param name="searchList"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        private int GetColumnQueryType(List<FieldsModel> searchList, string alias)
        {
            var column = searchList.Find(s => s.__vModel__ == alias);
            return column == null ? 0 : column.searchType;
        }

        /// <summary>
        /// 获取是否展示列
        /// </summary>
        /// <param name="columnList"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        private bool GetIsShowColumn(List<IndexGridFieldModel> columnList, string alias)
        {
            var column = columnList.Find(s => s.prop == alias);
            return column == null ? false : true;
        }

        /// <summary>
        /// 获取是否多选
        /// </summary>
        /// <param name="columnList"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public bool GetIsMultipleColumn(List<FieldsModel> columnList, string alias)
        {
            var column = columnList.Find(s => s.__vModel__ == alias);
            if (column != null)
            {
                return column.multiple;
            }
            return false;
        }

        /// <summary>
        /// 获取代码生成方法列表
        /// </summary>
        /// <param name="hasPage">是否分页</param>
        /// <param name="btnsList">头部按钮</param>
        /// <param name="columnBtnsList">列表按钮</param>
        /// <param name="webType">页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）</param>
        /// <returns></returns>
        private List<CodeGenFunctionModel> GetCodeGenFunctionList(bool hasPage, List<ButtonConfigModel> btnsList, List<ButtonConfigModel> columnBtnsList, int webType)
        {
            List<CodeGenFunctionModel> functionList = new List<CodeGenFunctionModel>();
            switch (webType)
            {
                case 1:
                    functionList.Add(new CodeGenFunctionModel()
                    {
                        FullName = "add",
                        IsInterface = true
                    });
                    break;
                default:
                    //信息
                    functionList.Add(new CodeGenFunctionModel()
                    {
                        FullName = "info",
                        IsInterface = true
                    });
                    if (!hasPage)
                        functionList.Add(new CodeGenFunctionModel()
                        {
                            FullName = "noPage",
                            IsInterface = true
                        });
                    else
                        functionList.Add(new CodeGenFunctionModel()
                        {
                            FullName = "page",
                            IsInterface = true
                        });

                    btnsList.ForEach(b =>
                    {
                        if (b.value == "download" && !hasPage)
                            functionList.Add(new CodeGenFunctionModel()
                            {
                                FullName = "page",
                                IsInterface = false
                            });
                        else if (b.value == "download" && hasPage)
                            functionList.Add(new CodeGenFunctionModel()
                            {
                                FullName = "noPage",
                                IsInterface = false
                            });
                        functionList.Add(new CodeGenFunctionModel()
                        {
                            FullName = b.value,
                            IsInterface = true
                        });
                    });
                    columnBtnsList.ForEach(c =>
                    {
                        functionList.Add(new CodeGenFunctionModel()
                        {
                            FullName = c.value,
                            IsInterface = true
                        });
                    });
                    break;
            }
            return functionList;
        }

        /// <summary>
        /// 获取代码生成流程方法列表
        /// </summary>
        /// <returns></returns>
        private List<CodeGenFunctionModel> GetCodeGenFlowFunctionList()
        {
            List<CodeGenFunctionModel> functionList = new List<CodeGenFunctionModel>();
            //信息
            functionList.Add(new CodeGenFunctionModel()
            {
                FullName = "info",
                IsInterface = true
            });
            functionList.Add(new CodeGenFunctionModel()
            {
                FullName = "add",
                IsInterface = true
            });
            functionList.Add(new CodeGenFunctionModel()
            {
                FullName = "edit",
                IsInterface = true
            });
            functionList.Add(new CodeGenFunctionModel()
            {
                FullName = "remove",
                IsInterface = true
            });
            return functionList;
        }

        /// <summary>
        /// 获取代码生成表关系列表
        /// </summary>
        /// <param name="tableRelation">全部表列表</param>
        /// <param name="currentTable">当前表</param>
        /// <param name="link">连接ID</param>
        /// <param name="fieldList">子表控件</param>
        /// <returns></returns>
        private List<CodeGenTableRelationsModel> GetCodeGenTableRelationList(List<DbTableRelationModel> tableRelation, string currentTable, DbLinkEntity link, List<FieldsModel> fieldList)
        {
            List<CodeGenTableRelationsModel> tableRelationsList = new List<CodeGenTableRelationsModel>();
            var relationTable = tableRelation.Find(t => t.table.Equals(currentTable)).relationTable;
            if (!relationTable.IsNotEmptyOrNull())
            {
                tableRelation.ForEach(t =>
                {
                    List<TableColumnConfigModel> tableColumnConfigList = new List<TableColumnConfigModel>();
                    if (t.relationTable.IsNotEmptyOrNull())
                    {
                        var table = _databaseService.GetFieldListByNoAsync(link, t.table);
                        var field = fieldList.Find(it => it.__config__.tableName.Equals(t.table));
                        if (field != null)
                        {
                            foreach (var column in table)
                            {
                                var columnName = column.field.Replace("F_", "").Replace("f_", "").ToPascalCase();
                                var control = field.__config__.children.Find(it => it.__vModel__.Equals(columnName.LowerFirstChar()));
                                tableColumnConfigList.Add(new TableColumnConfigModel()
                                {
                                    ColumnName = columnName,
                                    Alias = column.field,
                                    OriginalColumnName = column.field,
                                    ColumnComment = column.fieldName,
                                    DataType = column.dataType,
                                    NetType = CodeGenUtil.ConvertDataType(column.dataType),
                                    PrimaryKey = column.primaryKey.ToBool(),
                                    IsMultiple = GetIsMultipleColumn(fieldList, column.field),
                                    jnpfKey = control != null ? control.__config__.jnpfKey : null,
                                    Rule = control != null ? control.__config__.rule : null
                                });
                            }
                        }
                        tableRelationsList.Add(new CodeGenTableRelationsModel()
                        {
                            TableName = t.table.ToPascalCase(),
                            PrimaryKey = table.Find(t => t.primaryKey == 1).field.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                            TableField = t.tableField.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                            RelationField = t.relationField.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                            TableComment = t.tableName,
                            ChilderColumnConfigList = tableColumnConfigList
                        });
                    }
                });
            }
            return tableRelationsList;
        }

        /// <summary>
        /// 获取代码生成常规Index列表头部按钮方法
        /// </summary>
        /// <returns></returns>
        private string GetCodeGenConvIndexListTopButtonMethod(string value)
        {
            var method = string.Empty;
            switch (value)
            {
                case "add":
                    method = "addOrUpdateHandle()";
                    break;
                case "download":
                    method = "exportData()";
                    break;
                case "batchRemove":
                    method = "handleBatchRemoveDel()";
                    break;
                default:
                    break;
            }
            return method;
        }

        /// <summary>
        /// 获取代码生成常规Index列表列按钮方法
        /// </summary>
        /// <returns></returns>
        private string GetCodeGenConvIndexListColumnButtonMethod(string value, string primaryKey)
        {
            var method = string.Empty;
            switch (value)
            {
                case "edit":
                    method = $"addOrUpdateHandle(scope.row.{primaryKey.LowerFirstChar()})";
                    break;
                case "remove":
                    method = $"handleDel(scope.row.{primaryKey.LowerFirstChar()})";
                    break;
                case "detail":
                    method = $"addOrUpdateHandle(scope.row.{primaryKey.LowerFirstChar()},true)";
                    break;
                default:
                    break;
            }
            return method;
        }

        /// <summary>
        /// 获取代码生成流程Index列表列按钮是否禁用
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetCodeGenWorkflowIndexListColumnButtonDisabled(string value)
        {
            var disabled = string.Empty;
            switch (value)
            {
                case "edit":
                    disabled = ":disabled = '[1, 2, 5].indexOf(scope.row.flowState) > -1' ";
                    break;
                case "remove":
                    disabled = ":disabled = '[1, 2, 3, 5].indexOf(scope.row.flowState) > -1' ";
                    break;
            }
            return disabled;
        }

        /// <summary>
        /// 获取常规index列表控件Option
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private string GetCodeGenConvIndexListControlOption(string name, List<Dictionary<string, object>> options)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"{name}Options:");
            sb.Append("[");
            var list = options.ToObject<List<Dictionary<string, object>>>();
            foreach (var valueItem in list)
            {
                sb.Append("{");
                foreach (var items in valueItem)
                {
                    sb.Append($"\"{items.Key}\":{items.Value.ToJson()},");
                }
                sb = new StringBuilder(sb.ToString().TrimEnd(','));
                sb.Append("},");
            }
            sb = new StringBuilder(sb.ToString().TrimEnd(','));
            sb.Append("],");

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private string GetCodeGenConvIndexListControlOption(string name, List<object> options)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"{name}Options:{options.ToJson()},");

            return sb.ToString();
        }

        /// <summary>
        /// 获取表单全部控件列表
        /// </summary>
        /// <param name="fieldList"></param>
        /// <param name="gutter"></param>
        /// <param name="isMain">是否主循环</param>
        /// <returns></returns>
        private List<CodeGenFormAllControlsDesign> GetFormAllControlsList(List<FieldsModel> fieldList, int gutter, bool isMain = false)
        {
            List<CodeGenFormAllControlsDesign> list = new List<CodeGenFormAllControlsDesign>();
            foreach (var item in fieldList)
            {
                var config = item.__config__;
                switch (config.jnpfKey)
                {
                    //栅格布局
                    case "row":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                jnpfKey = config.jnpfKey,
                                Span = config.span,
                                Gutter = gutter,
                                Children = GetFormAllControlsList(config.children, gutter)
                            });
                        }
                        break;
                    //表格
                    case "table":
                        {
                            List<CodeGenFormAllControlsDesign> childrenTableList = new List<CodeGenFormAllControlsDesign>();
                            foreach (var children in config.children)
                            {
                                var childrenConfig = children.__config__;
                                childrenTableList.Add(new CodeGenFormAllControlsDesign()
                                {
                                    jnpfKey = childrenConfig.jnpfKey,
                                    Name = children.__vModel__,
                                    OriginalName = children.__vModel__,
                                    Style = !string.IsNullOrEmpty(children.type) ? $":style='{children.style.ToJson()}' " : "",
                                    Placeholder = children.placeholder != null ? $"placeholder=\"{children.placeholder}\" " : "",
                                    Clearable = children.clearable ? "clearable " : "",
                                    Readonly = children.@readonly ? "readonly " : "",
                                    Disabled = children.disabled ? "disabled " : "",
                                    IsDisabled = item.disabled ? "disabled " : $":disabled=\"judgeWrite('{config.tableName.ToPascalCase().LowerFirstChar()}')\" ",
                                    ShowWordLimit = children.showwordlimit ? "show-word-limit " : "",
                                    Type = children.type != null ? $"type=\"{children.type}\" " : "",
                                    Format = children.format != null ? $"format=\"{children.format}\" " : "",
                                    ValueFormat = children.valueformat != null ? $"value-format=\"{children.valueformat}\" " : "",
                                    AutoSize = children.autosize != null ? $":autosize='{children.autosize.ToJson()}' " : "",
                                    Multiple = children.multiple ? $"multiple " : "",
                                    Size = childrenConfig.optionType != null ? (childrenConfig.optionType == "default" ? "" : $"size=\"{children.size}\" ") : "",
                                    Label = childrenConfig.label,
                                    Props = childrenConfig.props,
                                    Tag = childrenConfig.tag,
                                    MainProps = children.props != null ? $":props=\"{children.__vModel__}Props\" " : "",
                                    Options = children.options != null ? $":options=\"{children.__vModel__}Options\" " : "",
                                    ShowAllLevels = children.showalllevels ? "show-all-levels " : "",
                                    Separator = !string.IsNullOrEmpty(children.separator) ? $"separator=\"{children.separator}\" " : "",
                                    Required = childrenConfig.required ? "required " : "",
                                    Step = children.step != 0 ? $":step=\"{children.step}\" " : "",
                                    Max = item.max != 0 ? $":max=\"{item.max}\" " : "",
                                    Min = item.min != 0 ? $":min=\"{item.min}\" " : "",
                                    ColumnWidth = childrenConfig.columnWidth != null ? $"width='{childrenConfig.columnWidth}' " : null
                                });
                            }
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                jnpfKey = config.jnpfKey,
                                Name = item.__vModel__.ToPascalCase(),
                                OriginalName = config.tableName.ToPascalCase().LowerFirstChar(),
                                Span = config.span,
                                ShowText = config.showTitle,
                                Label = config.label,
                                ChildTableName = config.tableName.ToPascalCase(),
                                Children = childrenTableList
                            });
                        }
                        break;
                    //卡片
                    case "card":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                jnpfKey = config.jnpfKey,
                                OriginalName = item.__vModel__,
                                Shadow = item.shadow,
                                Children = GetFormAllControlsList(config.children, gutter),
                                Span = config.span,
                                Content = item.header
                            });
                        }
                        break;
                    //分割线
                    case "divider":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                jnpfKey = config.jnpfKey,
                                OriginalName = item.__vModel__,
                                Span = config.span,
                                Contentposition = item.contentposition,
                                Default = item.__slot__.@default
                            });
                        }
                        break;
                    //折叠面板
                    case "collapse":
                        {
                            //先加为了防止 children下 还有折叠面板
                            List<CodeGenFormAllControlsDesign> childrenCollapseList = new List<CodeGenFormAllControlsDesign>();
                            foreach (var children in config.children)
                            {
                                childrenCollapseList.Add(new CodeGenFormAllControlsDesign()
                                {
                                    Title = children.title,
                                    Name = children.name,
                                    Gutter = gutter,
                                    Children = GetFormAllControlsList(children.__config__.children, gutter)
                                });
                            }
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                jnpfKey = config.jnpfKey,
                                Accordion = item.accordion ? "true" : "false",
                                Name = "active" + active++,
                                Active = config.active.ToObject<List<string>>().ToJson(),
                                Children = childrenCollapseList,
                                Span = config.span
                            });
                        }
                        break;
                    //tab标签
                    case "tab":
                        {
                            //先加为了防止 children下 还有折叠面板
                            List<CodeGenFormAllControlsDesign> childrenCollapseList = new List<CodeGenFormAllControlsDesign>();
                            foreach (var children in config.children)
                            {
                                childrenCollapseList.Add(new CodeGenFormAllControlsDesign()
                                {
                                    Title = children.title,
                                    Gutter = gutter,
                                    Children = GetFormAllControlsList(children.__config__.children, gutter)
                                });
                            }
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                jnpfKey = config.jnpfKey,
                                Type = item.type,
                                TabPosition = item.tabPosition,
                                Name = "active" + active++,
                                Active = config.active.ToString(),
                                Children = childrenCollapseList,
                                Span = config.span
                            });
                        }
                        break;
                    //分组标题
                    case "groupTitle":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                jnpfKey = config.jnpfKey,
                                Span = config.span,
                                Contentposition = item.contentposition,
                                Content = item.content
                            });
                        }
                        break;
                    //文本
                    case "JNPFText":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                jnpfKey = config.jnpfKey,
                                Span = config.span,
                                DefaultValue = config.defaultValue,
                                TextStyle = item.textStyle != null ? item.textStyle.ToJson() : "",
                                Style = item.style.ToJson()
                            });
                        }
                        break;
                    //常规
                    default:
                        {
                            string vModel = string.Empty;
                            string name = item.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase();
                            var Model = name.LowerFirstChar();
                            switch (config.jnpfKey)
                            {
                                default:
                                    vModel = $"v-model=\"dataForm.{Model}\" ";
                                    break;
                            }
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                Name = item.__vModel__,
                                OriginalName = item.__vModel__,
                                jnpfKey = config.jnpfKey,
                                Style = item.style != null ? $":style='{item.style.ToJson()}' " : "",
                                Type = !string.IsNullOrEmpty(item.type) ? $"type='{item.type}' " : "",
                                Span = config.span,
                                Clearable = item.clearable ? "clearable " : "",
                                Readonly = item.@readonly ? "readonly " : "",
                                Required = config.required ? "required " : "",
                                Placeholder = !string.IsNullOrEmpty(item.placeholder) ? $"placeholder=\"{item.placeholder}\" " : "",
                                Disabled = item.disabled ? "disabled " : "",
                                IsDisabled = item.disabled ? "disabled " : $":disabled=\"judgeWrite('{ item.__vModel__}')\" ",
                                ShowWordLimit = item.showwordlimit ? "show-word-limit " : "",
                                Format = !string.IsNullOrEmpty(item.format) ? $"format=\"{item.format}\" " : "",
                                ValueFormat = !string.IsNullOrEmpty(item.valueformat) ? $"value-format=\"{item.valueformat}\" " : "",
                                AutoSize = item.autosize != null && item.autosize.ToJson() != "null" ? $":autosize='{item.autosize.ToJson()}' " : "",
                                Multiple = item.multiple ? $"multiple " : "",
                                IsRange = item.isrange ? "is-range " : "",
                                Props = config.props,
                                MainProps = item.props != null ? $":props=\"{Model}Props\" " : "",
                                OptionType = config.optionType == "default" ? "" : "-button",
                                Size = !string.IsNullOrEmpty(config.optionType) ? (config.optionType == "default" ? "" : $"size=\"{item.size}\" ") : "",
                                PrefixIcon = !string.IsNullOrEmpty(item.prefixicon) ? $"prefix-icon=\"{item.prefixicon}\" " : "",
                                SuffixIcon = !string.IsNullOrEmpty(item.suffixicon) ? $"suffix-icon=\"{item.suffixicon}\" " : "",
                                MaxLength = !string.IsNullOrEmpty(item.maxlength) ? $"maxlength=\"{item.maxlength}\" " : "",
                                Step = item.step != 0 ? $":step=\"{item.step}\" " : "",
                                StepStrictly = item.stepstrictly ? "step-strictly " : "",
                                ControlsPosition = !string.IsNullOrEmpty(item.controlsposition) ? $"controls-position=\"{item.controlsposition}\" " : "",
                                ShowChinese = item.showChinese ? "showChinese " : "",
                                ShowPassword = item.showPassword ? "show-password " : "",
                                Filterable = item.filterable ? "filterable " : "",
                                ShowAllLevels = item.showalllevels ? "show-all-levels " : "",
                                Separator = !string.IsNullOrEmpty(item.separator) ? $"separator=\"{item.separator}\" " : "",
                                RangeSeparator = !string.IsNullOrEmpty(item.rangeseparator) ? $"range-separator=\"{item.rangeseparator}\" " : "",
                                StartPlaceholder = !string.IsNullOrEmpty(item.startplaceholder) ? $"start-placeholder=\"{item.startplaceholder}\" " : "",
                                EndPlaceholder = !string.IsNullOrEmpty(item.endplaceholder) ? $"end-placeholder=\"{item.endplaceholder}\" " : "",
                                PickerOptions = item.pickeroptions != null && item.pickeroptions.ToJson() != "null" ? $":picker-options='{item.pickeroptions.ToJson()}' " : "",
                                Options = item.options != null ? $":options=\"{item.__vModel__}Options\" " : "",
                                Max = item.max != 0 ? $":max=\"{item.max}\" " : "",
                                AllowHalf = item.allowhalf ? "allow-half " : "",
                                ShowTexts = item.showtext ? $"show-text " : "",
                                ShowScore = item.showScore ? $"show-score " : "",
                                ShowAlpha = item.showalpha ? $"show-alpha " : "",
                                ColorFormat = !string.IsNullOrEmpty(item.colorformat) ? $"color-format=\"{item.colorformat}\" " : "",
                                ActiveText = !string.IsNullOrEmpty(item.activetext) ? $"active-text=\"{item.activetext}\" " : "",
                                InactiveText = !string.IsNullOrEmpty(item.inactivetext) ? $"inactive-text=\"{item.inactivetext}\" " : "",
                                ActiveColor = !string.IsNullOrEmpty(item.activecolor) ? $"active-color=\"{item.activecolor}\" " : "",
                                InactiveColor = !string.IsNullOrEmpty(item.inactivecolor) ? $"inactive-color=\"{item.inactivecolor}\" " : "",
                                IsSwitch = config.jnpfKey == "switch" ? $":active-value=\"{item.activevalue}\" :inactive-value=\"{item.inactivevalue}\" " : "",
                                Min = item.min != 0 ? $":min=\"{item.min}\" " : "",
                                ShowStops = item.showstops ? $"show-stops " : "",
                                Range = item.range ? $"range " : "",
                                Accept = !string.IsNullOrEmpty(item.accept) ? $"accept=\"{item.accept}\" " : "",
                                ShowTip = item.showTip ? $"showTip " : "",
                                FileSize = item.fileSize != 0 ? $":fileSize=\"{item.fileSize}\" " : "",
                                SizeUnit = !string.IsNullOrEmpty(item.sizeUnit) ? $"sizeUnit=\"{item.sizeUnit}\" " : "",
                                Limit = item.limit != 0 ? $":limit=\"{item.limit}\" " : "",
                                Contentposition = !string.IsNullOrEmpty(item.contentposition) ? $"content-position=\"{item.contentposition}\" " : "",
                                ButtonText = !string.IsNullOrEmpty(item.buttonText) ? $"buttonText=\"{item.buttonText}\" " : "",
                                Level = item.level != 0 ? $":level=\"{item.level}\" " : "",
                                ActionText = !string.IsNullOrEmpty(item.actionText) ? $"actionText=\"{item.actionText}\" " : "",
                                Shadow = !string.IsNullOrEmpty(item.shadow) ? $"shadow=\"{item.shadow}\" " : "",
                                Content = !string.IsNullOrEmpty(item.content) ? $"content=\"{item.content}\" " : "",
                                NoShow = config.noShow ? "v-if=\"false\" " : "",
                                Label = config.label,
                                vModel = vModel,
                                Prepend = item.__slot__ != null && !string.IsNullOrEmpty(item.__slot__.prepend) ? item.__slot__.prepend : null,
                                Append = item.__slot__ != null && !string.IsNullOrEmpty(item.__slot__.append) ? item.__slot__.append : null,
                                Tag = config.tag,
                                Count = item.max
                            });
                        }
                        break;
                }
            }
            //还原为后面方法做准备
            if (isMain)
                active = 1;
            return list;
        }

        /// <summary>
        /// 生成表单Children无限级
        /// </summary>
        /// <param name="childrenList"></param>
        /// <param name="gutter"></param>
        /// <returns></returns>
        private List<CodeGenFormAllControlsDesign> GetFormChildrenControlsList(List<FieldsModel> childrenList, int gutter)
        {
            List<CodeGenFormAllControlsDesign> list = new List<CodeGenFormAllControlsDesign>();
            foreach (var item in childrenList)
            {
                var config = item.__config__;
                switch (config.jnpfKey)
                {
                    //栅格布局
                    case "row":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                jnpfKey = config.jnpfKey,
                                Span = config.span,
                                Gutter = gutter,
                                Children = GetFormChildrenControlsList(config.children, gutter)
                            });
                        }
                        break;
                    //表格
                    case "table":
                        {
                            List<CodeGenFormAllControlsDesign> childrenTableList = new List<CodeGenFormAllControlsDesign>();
                            foreach (var children in config.children)
                            {
                                var childrenConfig = children.__config__;
                                childrenTableList.Add(new CodeGenFormAllControlsDesign()
                                {
                                    jnpfKey = childrenConfig.jnpfKey,
                                    Name = children.__vModel__,
                                    Style = !string.IsNullOrEmpty(children.type) ? $":style='{children.style.ToJson()}' " : "",
                                    Placeholder = children.placeholder != null ? $"placeholder=\"{children.placeholder}\" " : "",
                                    Clearable = children.clearable ? "clearable " : "",
                                    Readonly = children.@readonly ? "readonly " : "",
                                    Disabled = children.disabled ? "disabled " : "",
                                    ShowWordLimit = children.showwordlimit ? "show-word-limit " : "",
                                    Type = children.type != null ? $"type=\"{children.type}\" " : "",
                                    Format = children.format != null ? $"format=\"{children.format}\" " : "",
                                    ValueFormat = children.valueformat != null ? $"value-format=\"{children.valueformat}\" " : "",
                                    AutoSize = children.autosize != null ? $":autosize='{children.autosize.ToJson()}' " : "",
                                    Multiple = children.multiple ? $"multiple " : "",
                                    Size = childrenConfig.optionType != null ? (childrenConfig.optionType == "default" ? "" : $"size=\"{children.size}\" ") : "",
                                    Label = childrenConfig.label,
                                    Props = childrenConfig.props,
                                    Tag = childrenConfig.tag,
                                    MainProps = children.props != null ? $":props=\"{children.__vModel__}Props\"" : "",
                                    Options = children.options != null ? $":options=\"{children.__vModel__}Options\" " : "",
                                    ShowAllLevels = children.showalllevels ? "show-all-levels " : "",
                                    Separator = !string.IsNullOrEmpty(children.separator) ? $"separator=\"{children.separator}\" " : "",
                                    Required = childrenConfig.required ? "required " : "",
                                    Step = children.step != 0 ? $":step=\"{children.step}\" " : "",
                                });
                            }
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                jnpfKey = config.jnpfKey,
                                Span = config.span,
                                ShowText = config.showTitle,
                                Label = config.label,
                                ChildTableName = item.__config__.tableName.ToPascalCase(),
                                Children = childrenTableList
                            });
                        }
                        break;
                    //卡片
                    case "card":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                jnpfKey = config.jnpfKey,
                                Shadow = item.shadow,
                                Children = GetFormChildrenControlsList(config.children, gutter),
                                Span = config.span
                            });
                        }
                        break;
                    //分割线
                    case "divider":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                jnpfKey = config.jnpfKey,
                                Span = config.span,
                                Contentposition = item.contentposition,
                                Default = item.__slot__.@default
                            });
                        }
                        break;
                    //折叠面板
                    case "collapse":
                        {
                            //先加为了防止 children下 还有折叠面板
                            List<CodeGenFormAllControlsDesign> childrenCollapseList = new List<CodeGenFormAllControlsDesign>();
                            foreach (var children in config.children)
                            {
                                childrenCollapseList.Add(new CodeGenFormAllControlsDesign()
                                {
                                    Title = children.title,
                                    Name = children.name,
                                    Gutter = gutter,
                                    Children = GetFormChildrenControlsList(children.__config__.children, gutter)
                                });
                            }
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                jnpfKey = config.jnpfKey,
                                Name = "active" + active++,
                                Accordion = item.accordion ? "true" : "false",
                                Active = config.active.ToObject<List<string>>().ToJson(),
                                Children = childrenCollapseList,
                                Span = config.span,
                            });
                        }
                        break;
                    //tab标签
                    case "tab":
                        {
                            //先加为了防止 children下 还有折叠面板
                            List<CodeGenFormAllControlsDesign> childrenCollapseList = new List<CodeGenFormAllControlsDesign>();
                            foreach (var children in config.children)
                            {
                                childrenCollapseList.Add(new CodeGenFormAllControlsDesign()
                                {
                                    Title = children.title,
                                    Gutter = gutter,
                                    Children = GetFormChildrenControlsList(children.__config__.children, gutter)
                                });
                            }
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                jnpfKey = config.jnpfKey,
                                Type = item.type,
                                TabPosition = item.tabPosition,
                                Name = "active" + active++,
                                Active = config.active.ToString(),
                                Children = childrenCollapseList,
                                Span = config.span
                            });
                        }
                        break;
                    //分组标题
                    case "groupTitle":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                jnpfKey = config.jnpfKey,
                                Span = config.span,
                                Contentposition = item.contentposition,
                                Content = item.content
                            });
                        }
                        break;
                    //文本
                    case "JNPFText":
                        {
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                jnpfKey = config.jnpfKey,
                                Span = config.span,
                                DefaultValue = config.defaultValue,
                                TextStyle = item.textStyle != null ? item.textStyle.ToJson() : "",
                                Style = item.style.ToJson()
                            });
                        }
                        break;
                    //常规
                    default:
                        {
                            string vModel = string.Empty;
                            string name = item.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase();
                            var Model = name.LowerFirstChar();
                            switch (config.jnpfKey)
                            {
                                default:
                                    vModel = $"v-model=\"dataForm.{Model}\" ";
                                    break;
                            }
                            list.Add(new CodeGenFormAllControlsDesign()
                            {
                                Name = item.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                jnpfKey = config.jnpfKey,
                                Style = item.style != null ? $":style='{item.style.ToJson()}' " : "",
                                Type = !string.IsNullOrEmpty(item.type) ? $"type='{item.type}' " : "",
                                Span = config.span,
                                Clearable = item.clearable ? "clearable " : "",
                                Readonly = item.@readonly ? "readonly " : "",
                                Required = config.required ? "required " : "",
                                Placeholder = !string.IsNullOrEmpty(item.placeholder) ? $"placeholder=\"{item.placeholder}\" " : "",
                                Disabled = item.disabled ? "disabled " : "",
                                ShowWordLimit = item.showwordlimit ? "show-word-limit " : "",
                                Format = !string.IsNullOrEmpty(item.format) ? $"format=\"{item.format}\" " : "",
                                ValueFormat = !string.IsNullOrEmpty(item.valueformat) ? $"value-format=\"{item.valueformat}\" " : "",
                                AutoSize = item.autosize != null && item.autosize.ToJson() != "null" ? $":autosize='{item.autosize.ToJson()}' " : "",
                                Multiple = item.multiple ? $"multiple " : "",
                                IsRange = item.isrange ? "is-range " : "",
                                Props = config.props,
                                MainProps = item.props != null ? $":props=\"{Model}Props\" " : "",
                                OptionType = config.optionType == "default" ? "" : "-button",
                                Size = !string.IsNullOrEmpty(config.optionType) ? (config.optionType == "default" ? "" : $"size=\"{item.size}\" ") : "",
                                PrefixIcon = !string.IsNullOrEmpty(item.prefixicon) ? $"prefix-icon=\"{item.prefixicon}\" " : "",
                                SuffixIcon = !string.IsNullOrEmpty(item.suffixicon) ? $"suffix-icon=\"{item.suffixicon}\" " : "",
                                MaxLength = !string.IsNullOrEmpty(item.maxlength) ? $"maxlength=\"{item.maxlength}\" " : "",
                                Step = item.step != 0 ? $":step=\"{item.step}\" " : "",
                                StepStrictly = item.stepstrictly ? "step-strictly " : "",
                                ControlsPosition = !string.IsNullOrEmpty(item.controlsposition) ? $"controls-position=\"{item.controlsposition}\" " : "",
                                ShowChinese = item.showChinese ? "showChinese " : "",
                                ShowPassword = item.showPassword ? "show-password " : "",
                                Filterable = item.filterable ? "filterable " : "",
                                ShowAllLevels = item.showalllevels ? "show-all-levels " : "",
                                Separator = !string.IsNullOrEmpty(item.separator) ? $"separator=\"{item.separator}\" " : "",
                                RangeSeparator = !string.IsNullOrEmpty(item.rangeseparator) ? $"range-separator=\"{item.rangeseparator}\" " : "",
                                StartPlaceholder = !string.IsNullOrEmpty(item.startplaceholder) ? $"start-placeholder=\"{item.startplaceholder}\" " : "",
                                EndPlaceholder = !string.IsNullOrEmpty(item.endplaceholder) ? $"end-placeholder=\"{item.endplaceholder}\" " : "",
                                PickerOptions = item.pickeroptions != null && item.pickeroptions.ToJson() != "null" ? $":picker-options='{item.pickeroptions.ToJson()}' " : "",
                                Options = item.options != null ? $":options=\"{item.__vModel__}Options\" " : "",
                                Max = item.max != 0 ? $":max=\"{item.max}\" " : "",
                                AllowHalf = item.allowhalf ? "allow-half " : "",
                                ShowTexts = item.showtext ? $"show-text " : "",
                                ShowScore = item.showScore ? $"show-score " : "",
                                ShowAlpha = item.showalpha ? $"show-alpha " : "",
                                ColorFormat = !string.IsNullOrEmpty(item.colorformat) ? $"color-format=\"{item.colorformat}\" " : "",
                                ActiveText = !string.IsNullOrEmpty(item.activetext) ? $"active-text=\"{item.activetext}\" " : "",
                                InactiveText = !string.IsNullOrEmpty(item.inactivetext) ? $"inactive-text=\"{item.inactivetext}\" " : "",
                                ActiveColor = !string.IsNullOrEmpty(item.activecolor) ? $"active-color=\"{item.activecolor}\" " : "",
                                InactiveColor = !string.IsNullOrEmpty(item.inactivecolor) ? $"inactive-color=\"{item.inactivecolor}\" " : "",
                                IsSwitch = config.jnpfKey == "switch" ? $":active-value=\"{item.activevalue}\" :inactive-value=\"{item.inactivevalue}\" " : "",
                                Min = item.min != 0 ? $":min=\"{item.min}\" " : "",
                                ShowStops = item.showstops ? $"show-stops " : "",
                                Range = item.range ? $"range " : "",
                                Accept = !string.IsNullOrEmpty(item.accept) ? $"accept=\"{item.accept}\" " : "",
                                ShowTip = item.showTip ? $"showTip " : "",
                                FileSize = item.fileSize != 0 ? $":fileSize=\"{item.fileSize}\" " : "",
                                SizeUnit = !string.IsNullOrEmpty(item.sizeUnit) ? $"sizeUnit=\"{item.sizeUnit}\" " : "",
                                Limit = item.limit != 0 ? $":limit=\"{item.limit}\" " : "",
                                Contentposition = !string.IsNullOrEmpty(item.contentposition) ? $"content-position=\"{item.contentposition}\" " : "",
                                ButtonText = !string.IsNullOrEmpty(item.buttonText) ? $"buttonText=\"{item.buttonText}\" " : "",
                                Level = item.level != 0 ? $":level=\"{item.level}\" " : "",
                                ActionText = !string.IsNullOrEmpty(item.actionText) ? $"actionText=\"{item.actionText}\" " : "",
                                Shadow = !string.IsNullOrEmpty(item.shadow) ? $"shadow=\"{item.shadow}\" " : "",
                                Content = !string.IsNullOrEmpty(item.content) ? $"content=\"{item.content}\" " : "",
                                NoShow = config.noShow ? "v-if=\"false\" " : "",
                                Label = config.label,
                                vModel = vModel,
                                Prepend = item.__slot__ != null && !string.IsNullOrEmpty(item.__slot__.prepend) ? item.__slot__.prepend : null,
                                Append = item.__slot__ != null && !string.IsNullOrEmpty(item.__slot__.append) ? item.__slot__.append : null,
                                Tag = config.tag,
                                Count = item.max
                            });
                        }
                        break;
                }
            }
            return list;
        }

        /// <summary>
        /// 获取表单全部控件选项配置
        /// </summary>
        /// <param name="fieldList"></param>
        /// <param name="type">1-Web设计,2-App设计,3-流程表单,4-Web表单,5-App表单</param>
        /// <param name="isMain">是否主循环</param>
        /// <returns></returns>
        private List<CodeGenConvIndexListControlOptionDesign> GetFormAllControlsProps(List<FieldsModel> fieldList, int type, bool isMain = false)
        {
            List<CodeGenConvIndexListControlOptionDesign> list = new List<CodeGenConvIndexListControlOptionDesign>();
            foreach (var item in fieldList)
            {
                var config = item.__config__;
                switch (config.jnpfKey)
                {
                    //卡片
                    case "card":
                    //栅格布局
                    case "row":
                        {
                            list.AddRange(GetFormAllControlsProps(config.children, type));
                        }
                        break;
                    //表格
                    case "table":
                        {
                            for (int i = 0; i < config.children.Count; i++)
                            {
                                var childrenConfig = config.children[i].__config__;
                                switch (childrenConfig.jnpfKey)
                                {
                                    case "select":
                                        {
                                            switch (childrenConfig.dataType)
                                            {
                                                //静态数据
                                                case "static":
                                                    list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                    {
                                                        jnpfKey = childrenConfig.jnpfKey,
                                                        Name = config.children[i].__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                                        DictionaryType = childrenConfig.dataType == "dictionary" ? childrenConfig.dictionaryType : (childrenConfig.dataType == "dynamic" ? childrenConfig.propsUrl : null),
                                                        DataType = childrenConfig.dataType,
                                                        IsStatic = true,
                                                        IsIndex = false,
                                                        IsProps = type == 5 || type == 3 ? true : false,
                                                        Props = $"{{\"label\":\"{childrenConfig.props.label}\",\"value\":\"{childrenConfig.props.value}\"}}",
                                                        IsChildren = true,
                                                        Content = GetCodeGenConvIndexListControlOption(config.children[i].__vModel__, config.children[i].__slot__.options)
                                                    });
                                                    break;
                                                default:
                                                    list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                    {
                                                        jnpfKey = childrenConfig.jnpfKey,
                                                        Name = config.children[i].__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                                        DictionaryType = childrenConfig.dataType == "dictionary" ? childrenConfig.dictionaryType : (childrenConfig.dataType == "dynamic" ? childrenConfig.propsUrl : null),
                                                        DataType = childrenConfig.dataType,
                                                        IsStatic = false,
                                                        IsIndex = false,
                                                        IsProps = type == 5 || type == 3 ? true : false,
                                                        Props = $"{{\"label\":\"{childrenConfig.props.label}\",\"value\":\"{childrenConfig.props.value}\"}}",
                                                        IsChildren = true,
                                                        Content = $"{config.children[i].__vModel__}Options : [],"
                                                    });
                                                    break;
                                            }
                                        }
                                        break;
                                    case "treeSelect":
                                    case "cascader":
                                        {
                                            switch (childrenConfig.dataType)
                                            {
                                                case "static":
                                                    list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                    {
                                                        jnpfKey = childrenConfig.jnpfKey,
                                                        Name = config.children[i].__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                                        DictionaryType = childrenConfig.dataType == "dictionary" ? childrenConfig.dictionaryType : (childrenConfig.dataType == "dynamic" ? childrenConfig.propsUrl : null),
                                                        DataType = childrenConfig.dataType,
                                                        IsStatic = true,
                                                        IsIndex = false,
                                                        IsProps = true,
                                                        IsChildren = true,
                                                        Props = config.children[i].props.props.ToJson(),
                                                        Content = GetCodeGenConvIndexListControlOption(config.children[i].__vModel__, config.children[i].options)
                                                    });
                                                    break;
                                                default:
                                                    list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                    {
                                                        jnpfKey = childrenConfig.jnpfKey,
                                                        Name = config.children[i].__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                                        DictionaryType = childrenConfig.dataType == "dictionary" ? childrenConfig.dictionaryType : (childrenConfig.dataType == "dynamic" ? childrenConfig.propsUrl : null),
                                                        DataType = childrenConfig.dataType,
                                                        IsStatic = false,
                                                        IsIndex = false,
                                                        IsProps = true,
                                                        IsChildren = true,
                                                        Props = config.children[i].props.props.ToJson(),
                                                        Content = $"{config.children[i].__vModel__}Options : [],"
                                                    });
                                                    break;
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            //foreach (var children in config.children)
                            //{
                            //    var childrenConfig = children.__config__;
                            //    if (childrenConfig.jnpfKey == "select")
                            //    {
                            //        switch (childrenConfig.dataType)
                            //        {

                            //        }
                            //        break;

                            //    }
                            //}
                        }
                        break;
                    //折叠面板
                    case "collapse":
                        {
                            StringBuilder title = new StringBuilder("[");
                            foreach (var children in config.children)
                            {
                                title.Append($"{{title:\"{children.title}\"}},");
                                list.AddRange(GetFormAllControlsProps(children.__config__.children, type));
                            }
                            title.Remove(title.Length - 1, 1);
                            title.Append("]");
                            list.Add(new CodeGenConvIndexListControlOptionDesign()
                            {
                                jnpfKey = config.jnpfKey,
                                Name = "active" + active++,
                                IsStatic = true,
                                IsIndex = false,
                                IsProps = false,
                                IsChildren = false,
                                Content = config.active.ToObject<List<string>>().ToJson(),
                                Title = title.ToString()
                            });
                        }
                        break;
                    //tab标签
                    case "tab":
                        {
                            StringBuilder title = new StringBuilder("[");
                            foreach (var children in config.children)
                            {
                                title.Append($"{{title:\"{children.title}\"}},");
                                list.AddRange(GetFormAllControlsProps(children.__config__.children, type));
                            }
                            title.Remove(title.Length - 1, 1);
                            title.Append("]");
                            list.Add(new CodeGenConvIndexListControlOptionDesign()
                            {
                                jnpfKey = config.jnpfKey,
                                Name = "active" + active++,
                                IsStatic = true,
                                IsIndex = false,
                                IsProps = false,
                                IsChildren = false,
                                Content = config.active.ToString(),
                                Title = title.ToString()
                            });
                        }
                        break;
                    //分组标题
                    case "groupTitle":
                    //分割线
                    case "divider":
                    //文本
                    case "JNPFText":
                        break;
                    //常规
                    default:
                        {
                            switch (config.jnpfKey)
                            {
                                //复选框
                                case "checkbox":
                                //下拉框多选
                                case "select":
                                //单选框
                                case "radio":
                                    {
                                        switch (config.dataType)
                                        {
                                            case "static":
                                                list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                {
                                                    jnpfKey = config.jnpfKey,
                                                    Name = item.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                                    DictionaryType = config.dataType == "dictionary" ? config.dictionaryType : (config.dataType == "dynamic" ? config.propsUrl : null),
                                                    DataType = config.dataType,
                                                    IsStatic = true,
                                                    IsIndex = true,
                                                    IsProps = type == 5 || type == 3 ? true : false,
                                                    Props = $"{{\"label\":\"{config.props.label}\",\"value\":\"{config.props.value}\"}}",
                                                    IsChildren = false,
                                                    Content = GetCodeGenConvIndexListControlOption(item.__vModel__, item.__slot__.options)
                                                });
                                                break;
                                            default:
                                                list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                {
                                                    jnpfKey = config.jnpfKey,
                                                    Name = item.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                                    DictionaryType = config.dataType == "dictionary" ? config.dictionaryType : (config.dataType == "dynamic" ? config.propsUrl : null),
                                                    DataType = config.dataType,
                                                    IsStatic = false,
                                                    IsIndex = true,
                                                    IsProps = type == 5 || type == 3 ? true : false,
                                                    Props = $"{{\"label\":\"{config.props.label}\",\"value\":\"{config.props.value}\"}}",
                                                    IsChildren = false,
                                                    Content = $"{item.__vModel__}Options : [],"
                                                });
                                                break;
                                        }
                                    }
                                    break;
                                case "treeSelect":
                                case "cascader":
                                    {
                                        switch (config.dataType)
                                        {
                                            case "static":
                                                list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                {
                                                    jnpfKey = config.jnpfKey,
                                                    Name = item.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                                    DictionaryType = config.dataType == "dictionary" ? config.dictionaryType : (config.dataType == "dynamic" ? config.propsUrl : null),
                                                    DataType = config.dataType,
                                                    IsStatic = true,
                                                    IsIndex = true,
                                                    IsProps = true,
                                                    IsChildren = false,
                                                    Props = item.props.props.ToJson(),
                                                    Content = GetCodeGenConvIndexListControlOption(item.__vModel__, item.options)
                                                });
                                                break;
                                            default:
                                                list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                {
                                                    jnpfKey = config.jnpfKey,
                                                    Name = item.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                                    DictionaryType = config.dataType == "dictionary" ? config.dictionaryType : (config.dataType == "dynamic" ? config.propsUrl : null),
                                                    DataType = config.dataType,
                                                    IsStatic = false,
                                                    IsIndex = true,
                                                    IsProps = true,
                                                    IsChildren = false,
                                                    Props = item.props.props.ToJson(),
                                                    Content = $"{item.__vModel__}Options : [],"
                                                });
                                                break;
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
            //还原为后面方法做准备
            if (isMain)
                active = 1;
            return list;
        }

        /// <summary>
        /// 表单子表控件选项配置
        /// </summary>
        /// <param name="childrenFormData"></param>
        /// <returns></returns>
        private List<CodeGenConvIndexListControlOptionDesign> GetFormChildrenControlsProps(List<FieldsModel> childrenFormData)
        {
            List<CodeGenConvIndexListControlOptionDesign> list = new List<CodeGenConvIndexListControlOptionDesign>();
            foreach (var item in childrenFormData)
            {
                var config = item.__config__;
                switch (config.jnpfKey)
                {
                    //栅格布局
                    case "row":
                        {
                            list.AddRange(GetFormChildrenControlsProps(config.children));
                        }
                        break;
                    //表格
                    case "table":
                        {
                            foreach (var children in config.children)
                            {
                                var childrenConfig = children.__config__;
                                if (childrenConfig.jnpfKey == "select")
                                {
                                    switch (config.dataType)
                                    {
                                        //静态数据
                                        case "static":
                                            list.Add(new CodeGenConvIndexListControlOptionDesign()
                                            {
                                                jnpfKey = config.jnpfKey,
                                                Name = children.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                                DictionaryType = childrenConfig.dataType == "dictionary" ? childrenConfig.dictionaryType : (childrenConfig.dataType == "dynamic" ? childrenConfig.propsUrl : null),
                                                DataType = childrenConfig.dataType,
                                                IsStatic = true,
                                                IsIndex = false,
                                                IsProps = false,
                                                IsChildren = true,
                                                Content = GetCodeGenConvIndexListControlOption(children.__vModel__, children.__slot__.options)
                                            });
                                            break;
                                        default:
                                            list.Add(new CodeGenConvIndexListControlOptionDesign()
                                            {
                                                jnpfKey = config.jnpfKey,
                                                Name = children.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                                DictionaryType = childrenConfig.dataType == "dictionary" ? childrenConfig.dictionaryType : (childrenConfig.dataType == "dynamic" ? childrenConfig.propsUrl : null),
                                                DataType = childrenConfig.dataType,
                                                IsStatic = false,
                                                IsIndex = false,
                                                IsProps = false,
                                                IsChildren = true,
                                                Content = $"{children.__vModel__}Options : [],"
                                            });
                                            break;
                                    }
                                    break;

                                }
                            }
                        }
                        break;
                    //卡片
                    case "card":
                        {
                            list.AddRange(GetFormChildrenControlsProps(config.children));
                        }
                        break;
                    //折贴面板
                    case "collapse":
                        {
                            foreach (var children in config.children)
                            {
                                list.AddRange(GetFormChildrenControlsProps(children.__config__.children));
                            }
                            list.Add(new CodeGenConvIndexListControlOptionDesign()
                            {
                                jnpfKey = config.jnpfKey,
                                Name = "active" + active++,
                                IsStatic = true,
                                IsIndex = false,
                                IsProps = false,
                                IsChildren = false,
                                Content = config.active.ToObject<List<string>>().ToJson()
                            });
                        }
                        break;
                    //tab标签
                    case "tab":
                        {
                            foreach (var children in config.children)
                            {
                                list.AddRange(GetFormChildrenControlsProps(children.__config__.children));
                            }
                            list.Add(new CodeGenConvIndexListControlOptionDesign()
                            {
                                jnpfKey = config.jnpfKey,
                                Name = "active" + active++,
                                IsStatic = true,
                                IsIndex = false,
                                IsProps = false,
                                IsChildren = false,
                                Content = config.active.ToString()
                            });
                        }
                        break;
                    //文本
                    case "JNPFText":
                        {

                        }
                        break;
                    //分割线
                    case "divider":
                        {

                        }
                        break;
                    //分组标题
                    case "groupTitle":
                        {

                        }
                        break;
                    default:
                        {
                            switch (config.jnpfKey)
                            {
                                //复选框
                                case "checkbox":
                                //下拉框多选
                                case "select":
                                //单选框
                                case "radio":
                                    {
                                        switch (config.dataType)
                                        {
                                            case "static":
                                                {
                                                    list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                    {
                                                        jnpfKey = config.jnpfKey,
                                                        Name = item.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                                        DictionaryType = config.dataType == "dictionary" ? config.dictionaryType : (config.dataType == "dynamic" ? config.propsUrl : null),
                                                        DataType = config.dataType,
                                                        IsStatic = true,
                                                        IsIndex = true,
                                                        IsProps = false,
                                                        IsChildren = false,
                                                        Content = GetCodeGenConvIndexListControlOption(item.__vModel__, item.__slot__.options)
                                                    });
                                                }
                                                break;
                                            default:
                                                list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                {
                                                    jnpfKey = config.jnpfKey,
                                                    Name = item.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                                    DictionaryType = config.dataType == "dictionary" ? config.dictionaryType : (config.dataType == "dynamic" ? config.propsUrl : null),
                                                    DataType = config.dataType,
                                                    IsStatic = false,
                                                    IsProps = false,
                                                    IsChildren = false,
                                                    IsIndex = true,
                                                    Content = $"{item.__vModel__}Options : [],"
                                                });
                                                break;
                                        }
                                    }
                                    break;
                                case "treeSelect":
                                case "cascader":
                                    {
                                        switch (config.dataType)
                                        {
                                            case "static":
                                                {
                                                    list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                    {
                                                        jnpfKey = config.jnpfKey,
                                                        Name = item.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                                        DictionaryType = config.dataType == "dictionary" ? config.dictionaryType : (config.dataType == "dynamic" ? config.propsUrl : null),
                                                        DataType = config.dataType,
                                                        IsStatic = true,
                                                        IsIndex = true,
                                                        IsProps = true,
                                                        IsChildren = false,
                                                        Props = item.props.props.ToJson(),
                                                        Content = GetCodeGenConvIndexListControlOption(item.__vModel__, item.options)
                                                    });
                                                }
                                                break;
                                            default:
                                                list.Add(new CodeGenConvIndexListControlOptionDesign()
                                                {
                                                    jnpfKey = config.jnpfKey,
                                                    Name = item.__vModel__.Replace("F_", "").Replace("f_", "").ToPascalCase(),
                                                    DictionaryType = config.dataType == "dictionary" ? config.dictionaryType : (config.dataType == "dynamic" ? config.propsUrl : null),
                                                    DataType = config.dataType,
                                                    IsStatic = false,
                                                    IsProps = true,
                                                    IsChildren = false,
                                                    IsIndex = true,
                                                    Props = item.props.props.ToJson(),
                                                    Content = $"{item.__vModel__}Options : [],"
                                                });
                                                break;
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
            return list;
        }

        #endregion
    }
}