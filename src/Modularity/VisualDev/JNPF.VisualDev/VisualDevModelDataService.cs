using JNPF.Common.Configuration;
using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Helper;
using JNPF.Common.Model.NPOI;
using JNPF.DataEncryption;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.JsonSerialization;
using JNPF.System.Entitys.Model.Permission.User;
using JNPF.System.Interfaces.Common;
using JNPF.VisualDev.Entitys;
using JNPF.VisualDev.Entitys.Dto.VisualDevModelData;
using JNPF.VisualDev.Entitys.Entity;
using JNPF.VisualDev.Entitys.Model.VisualDevModelData;
using JNPF.VisualDev.Interfaces;
using JNPF.VisualDev.Run.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace JNPF.VisualDev
{
    /// <summary>
    /// 可视化开发基础
    /// </summary>
    [ApiDescriptionSettings(Tag = "VisualDev", Name = "OnlineDev", Order = 172)]
    [Route("api/visualdev/[controller]")]
    public class VisualDevModelDataService : IVisualDevModelDataService, IDynamicApiController, ITransient
    {
        private readonly IVisualDevService _visualDevService;
        private readonly IRunService _runService;
        private readonly IUserManager _userManager;
        private readonly IFileService _fileService;

        /// <summary>
        /// 初始化一个<see cref="VisualDevModelDataService"/>类型的新实例
        /// </summary>
        public VisualDevModelDataService(IVisualDevService visualDevService, IRunService runService, IUserManager userManager, IFileService fileService)
        {
            _visualDevService = visualDevService;
            _runService = runService;
            _userManager = userManager;
            _fileService = fileService;
        }

        #region Get

        /// <summary>
        /// 获取列表表单配置JSON
        /// </summary>
        /// <param name="modelId">主键id</param>
        /// <returns></returns>
        [HttpGet("{modelId}/Config")]
        public async Task<dynamic> GetData(string modelId)
        {
            var data = await _visualDevService.GetInfoById(modelId);
            return data.Adapt<VisualDevModelDataConfigOutput>();
        }

        /// <summary>
        /// 获取列表配置JSON
        /// </summary>
        /// <param name="modelId">主键id</param>
        /// <returns></returns>
        [HttpGet("{modelId}/ColumnData")]
        public async Task<dynamic> GetColumnData(string modelId)
        {
            var data = await _visualDevService.GetInfoById(modelId);
            return new { columnData = data.ColumnData };
        }

        /// <summary>
        /// 获取列表配置JSON
        /// </summary>
        /// <param name="modelId">主键id</param>
        /// <returns></returns>
        [HttpGet("{modelId}/FormData")]
        public async Task<dynamic> GetFormData(string modelId)
        {
            var data = await _visualDevService.GetInfoById(modelId);
            return new { formData = data.FormData };
        }

        /// <summary>
        /// 获取列表配置JSON
        /// </summary>
        /// <param name="modelId">主键id</param>
        /// <returns></returns>
        [HttpGet("{modelId}/FlowTemplate")]
        public async Task<dynamic> GetFlowTemplate(string modelId)
        {
            var data = await _visualDevService.GetInfoById(modelId);
            return new { flowTemplateJson = data.FlowTemplateJson };
        }

        /// <summary>
        /// 获取数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modelId"></param>
        /// <returns></returns>
        [HttpGet("{modelId}/{id}")]
        public async Task<dynamic> GetInfo(string id, string modelId)
        {
            //模板实体
            var templateEntity = await _visualDevService.GetInfoById(modelId);
            //有表
            if (!string.IsNullOrEmpty(templateEntity.Tables) && !"[]".Equals(templateEntity.Tables))
            {
                return new { id = id, data = await _runService.GetHaveTableInfo(id, templateEntity) };
            }
            //无表
            var entity = await _runService.GetInfo(id);
            string data = await _runService.GetIsNoTableInfo(templateEntity, entity.Data);
            return new { id = entity.Id, data = data };
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modelId"></param>
        /// <returns></returns>
        [HttpGet("{modelId}/{id}/DataChange")]
        public async Task<dynamic> GetDetails(string id, string modelId)
        {
            //模板实体
            var templateEntity = await _visualDevService.GetInfoById(modelId);
            //有表
            if (!string.IsNullOrEmpty(templateEntity.Tables) && !"[]".Equals(templateEntity.Tables))
            {
                return new { id = id, data = await _runService.GetHaveTableInfoDetails(id, templateEntity) };
            }
            //无表
            var entity = await _runService.GetInfo(id);
            string data = await _runService.GetIsNoTableInfoDetails(templateEntity, entity);
            return new { id = entity.Id, data = data };
        }

        #endregion

        #region Post

        /// <summary>
        /// 功能导出
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        [HttpPost("{modelId}/Actions/ExportData")]
        public async Task<dynamic> ActionsExportData(string modelId)
        {
            //模板实体
            var templateEntity = await _visualDevService.GetInfoById(modelId);
            var jsonStr = templateEntity.Serialize();
            return _fileService.Export(jsonStr, templateEntity.FullName);
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("Model/Actions/ImportData")]
        public async Task ActionsActionsImport(IFormFile file)
        {
            var josn = _fileService.Import(file);
            var templateEntity = josn.Deserialize<VisualDevEntity>();
            if (templateEntity == null || templateEntity.Type.IsNullOrEmpty())
                throw JNPFException.Oh(ErrorCode.D3006);
            else if (templateEntity.Type != 1)
                throw JNPFException.Oh(ErrorCode.D3009);
            if (!string.IsNullOrEmpty(templateEntity.Id) && await _visualDevService.GetDataExists(templateEntity.Id))
                throw JNPFException.Oh(ErrorCode.D1400);
            if(await _visualDevService.GetDataExists(templateEntity.EnCode, templateEntity.FullName))
                throw JNPFException.Oh(ErrorCode.D1400);
            await _visualDevService.CreateImportData(templateEntity);
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="modelId">主键id</param>
        /// <param name="input">分页查询条件</param>
        /// <returns></returns>
        [HttpPost("{modelId}/List")]
        public async Task<dynamic> List(string modelId, [FromBody] VisualDevModelListQueryInput input)
        {
            var templateEntity = await _visualDevService.GetInfoById(modelId);
            var realList = await _runService.GetListResult(templateEntity, input);
            return realList;
        }

        /// <summary>
        /// 创建数据
        /// </summary>
        /// <param name="modelId"></param>
        /// <param name="visualdevModelDataCrForm"></param>
        /// <returns></returns>
        [HttpPost("{modelId}")]
        public async Task Create(string modelId, [FromBody] VisualDevModelDataCrInput visualdevModelDataCrForm)
        {
            var templateEntity = await _visualDevService.GetInfoById(modelId);
            await _runService.Create(templateEntity, visualdevModelDataCrForm);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modelId"></param>
        /// <param name="visualdevModelDataUpForm"></param>
        /// <returns></returns>
        [HttpPut("{modelId}/{id}")]
        public async Task Update(string modelId, string id, [FromBody] VisualDevModelDataUpInput visualdevModelDataUpForm)
        {
            var templateEntity = await _visualDevService.GetInfoById(modelId);
            await _runService.Update(id, templateEntity, visualdevModelDataUpForm);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modelId"></param>
        /// <returns></returns>
        [HttpDelete("{modelId}/{id}")]
        public async Task Delete(string id, string modelId)
        {
            var templateEntity = await _visualDevService.GetInfoById(modelId);
            if (!string.IsNullOrEmpty(templateEntity.Tables) && !"[]".Equals(templateEntity.Tables))
            {
                await _runService.DelHaveTableInfo(id, templateEntity);
            }
            else
            {
                await _runService.DelIsNoTableInfo(id, templateEntity);
            }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="modelId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("batchDelete/{modelId}")]
        public async Task BatchDelete(string modelId, [FromBody] VisualDevModelDataBatchDelInput input)
        {
            var templateEntity = await _visualDevService.GetInfoById(modelId);
            if (!string.IsNullOrEmpty(templateEntity.Tables) && !"[]".Equals(templateEntity.Tables))
            {
                await _runService.BatchDelHaveTableData(input.ids, templateEntity);
            }
            else
            {
                await _runService.BatchDelIsNoTableData(input.ids, templateEntity);
            }
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        [HttpPost("{modelId}/Actions/Export")]
        public async Task<dynamic> Export(string modelId, [FromBody] VisualDevModelListQueryInput input)
        {
            var userInfo = await _userManager.GetUserInfo();
            var templateEntity = await _visualDevService.GetInfoById(modelId);
            List<VisualDevModelDataEntity> list = new List<VisualDevModelDataEntity>();
            var pageList = await _runService.GetListResult(templateEntity, input);
            List<Dictionary<string, object>> realList = pageList.list.ToObeject<List<Dictionary<string, object>>>();
            var output = ExcelCreateModel(templateEntity.FormData, realList, input.selectKey, userInfo);
            return output;
        }

        #endregion

        #region PublicMethod

        public VisualDevModelDataExportOutput ExcelCreateModel(string formData, List<Dictionary<string, object>> realList, List<string> keys, UserInfo userInfo)
        {
            List<ExcelTemplateModel> templateList = new List<ExcelTemplateModel>();
            VisualDevModelDataExportOutput output = new VisualDevModelDataExportOutput();
            FormDataModel formDataModel = TemplateKeywordsHelper.ReplaceKeywords(formData).ToObject<FormDataModel>();
            var modelList = _runService.TemplateDataConversion(formDataModel.fields);
            List<string> columnList = new List<string>();
            try
            {
                ExcelConfig excelconfig = new ExcelConfig();
                excelconfig.FileName = YitIdHelper.NextId().ToString() + ".xls";
                excelconfig.HeadFont = "微软雅黑";
                excelconfig.HeadPoint = 10;
                excelconfig.IsAllSizeColumn = true;
                excelconfig.ColumnModel = new List<ExcelColumnModel>();
                foreach (var item in keys)
                {
                    var excelColumn = modelList.Find(t => t.__vModel__ == item);
                    if (excelColumn != null)
                    {
                        excelconfig.ColumnModel.Add(new ExcelColumnModel() { Column = item, ExcelColumn = excelColumn.__config__.label });
                        columnList.Add(excelColumn.__config__.label); ;
                    }
                }
                var addPath = FileVariable.TemporaryFilePath + excelconfig.FileName;
                ExcelExportHelper<Dictionary<string, object>>.Export(realList, excelconfig, addPath, columnList);
                var fileName = _userManager.UserId + "|" + addPath + "|xls";
                output.name = excelconfig.FileName;
                output.url = "/api/File/Download?encryption=" + DESCEncryption.Encrypt(fileName, "JNPF");
                return output;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}