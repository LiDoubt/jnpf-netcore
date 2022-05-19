using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.System.Entitys.Permission;
using JNPF.VisualDev.Entitys;
using JNPF.VisualDev.Entitys.Dto.VisualDev;
using JNPF.VisualDev.Interfaces;
using JNPF.VisualDev.Run.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Threading.Tasks;
using Mapster;
using System.Collections.Generic;
using System.Linq;
using JNPF.Common.Util;
using JNPF.System.Entitys.System;
using JNPF.VisualDev.Entitys.Model.VisualDevModelData;
using JNPF.WorkFlow.Entitys;
using System;
using JNPF.System.Interfaces.System;
using JNPF.VisualDev.Entitys.Dto.VisualDevModelData;
using JNPF.Common.Extension;
using JNPF.Common.Helper;
using JNPF.JsonSerialization;
using JNPF.WorkFlow.Interfaces.FlowTask;

namespace JNPF.VisualDev
{
    /// <summary>
    /// 可视化开发基础
    /// </summary>
    [ApiDescriptionSettings(Tag = "VisualDev", Name = "Base", Order = 171)]
    [Route("api/visualdev/[controller]")]
    public class VisualDevService : IVisualDevService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<VisualDevEntity> _visualDevRepository;
        private readonly IDictionaryDataService _dictionaryDataService;
        private readonly IRunService _runService;

        /// <summary>
        /// 初始化一个<see cref="VisualDevService"/>类型的新实例
        /// </summary>
        public VisualDevService(ISqlSugarRepository<VisualDevEntity> visualDevRepository, IRunService runService, IDictionaryDataService dictionaryDataService)
        {
            _visualDevRepository = visualDevRepository;
            _dictionaryDataService = dictionaryDataService;
            _runService = runService;
        }

        #region Get

        /// <summary>
        /// 获取功能列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] VisualDevListQueryInput input)
        {
            var data = await _visualDevRepository.Context.Queryable<VisualDevEntity, UserEntity, UserEntity>((a, b, c) => new JoinQueryInfos(JoinType.Left, b.Id == a.CreatorUserId, JoinType.Left, c.Id == a.LastModifyUserId))
                .WhereIF(!string.IsNullOrEmpty(input.keyword), a => a.FullName.Contains(input.keyword) || a.EnCode.Contains(input.keyword))
                .Where(a => a.DeleteMark == null && a.Type == input.type)
                .OrderBy(a => a.Category).OrderBy(a => a.SortCode)
                .Select((a, b, c) => new VisualDevListOutput { id = a.Id, fullName = a.FullName, enCode = a.EnCode, state = SqlFunc.ToInt32(a.State), type = SqlFunc.ToInt32(a.Type), tables = a.Tables, description = a.Description, category = a.Category, creatorTime = a.CreatorTime, creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account), lastModifyTime = a.LastModifyTime, lastModifyUser = SqlFunc.MergeString(c.RealName, SqlFunc.IIF(c.RealName == null, "", "/"), c.Account), deleteMark = a.DeleteMark, sortCode = a.SortCode, parentId = a.Category })
                .ToListAsync();
            var parentIds = data.Select(x => x.parentId).ToList().Distinct();
            var parentData = await _visualDevRepository.Context.Queryable<DictionaryDataEntity>()
                .Select(d => new { ParentId = "-1", FullName = d.FullName, Id = d.Id, DeleteMark = d.DeleteMark, state = d.EnabledMark }).MergeTable().Select<VisualDevListOutput>()
                .Where(d => parentIds.Contains(d.id) && d.deleteMark == null && d.state.Equals("1")).ToListAsync();
            var treeList = data.Union(parentData).ToList().ToTree("-1");
            return new { list = treeList };
        }

        /// <summary>
        /// 获取功能列表下拉框
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> GetSelector([FromQuery] VisualDevSelectorInput input)
        {
            var data = await _visualDevRepository.Where(v => v.Type == input.type && v.State == 1 && v.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
            var output = data.Adapt<List<VisualDevSelectorOutput>>();
            var parentIds = output.Select(x => x.parentId).ToList().Distinct();
            var pList = new List<VisualDevSelectorOutput>();
            var parentData = await _visualDevRepository.Context.Queryable<DictionaryDataEntity>().Where(d => parentIds.Contains(d.Id) && d.DeleteMark == null).ToListAsync();
            foreach (var item in parentData)
            {
                var pData = item.Adapt<VisualDevSelectorOutput>();
                pData.parentId = "-1";
                pList.Add(pData);
            }
            var treeList = output.Union(pList).ToList().ToTree("-1");
            return new { list = treeList };
        }

        /// <summary>
        /// 获取功能信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var data = await _visualDevRepository.FirstOrDefaultAsync(v => v.Id == id && v.DeleteMark == null);
            var output = data.Adapt<VisualDevInfoOutput>();
            return output;
        }

        /// <summary>
        /// 获取表单主表属性下拉框
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/FormDataFields")]
        public async Task<dynamic> GetFormDataFields(string id)
        {
            var templateEntity = await _visualDevRepository.FirstOrDefaultAsync(v => v.Id == id && v.DeleteMark == null);
            var formData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).Deserialize<FormDataModel>();
            //剔除多虑多余控件
            var newFields = _runService.TemplateDataConversion(formData.fields);
            var fieldsModels = newFields.FindAll(x => !"".Equals(x.__vModel__) && !"table".Equals(x.__config__.jnpfKey) && !"relationForm".Equals(x.__config__.jnpfKey));
            var output = fieldsModels.Select(x => new VisualDevFormDataFieldsOutput()
            {
                label = x.__config__.label,
                vmodel = x.__vModel__
            }).ToList();
            return new { list = output };
        }

        /// <summary>
        /// 获取表单主表属性列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/FieldDataSelect")]
        public async Task<dynamic> GetFieldDataSelect(string id)
        {
            var templateEntity = await _visualDevRepository.FirstOrDefaultAsync(v => v.Id == id);
            var formData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.FormData).Deserialize<FormDataModel>();
            var columnData = TemplateKeywordsHelper.ReplaceKeywords(templateEntity.ColumnData).Deserialize<ColumnDesignModel>();
            //获取值 无分页
            VisualDevModelListQueryInput listQueryInput = new VisualDevModelListQueryInput
            {
                dataType = "1",
                sidx = columnData.defaultSidx,
                sort = columnData.sort
            };
            var realList = (await _runService.GetListResult(templateEntity, listQueryInput)).list;
            return realList;
        }

        #endregion

        #region Post

        /// <summary>
        /// 新建功能信息
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] VisualDevCrInput input)
        {
            var entity = input.Adapt<VisualDevEntity>();
            try
            {
                //开启事务
                _visualDevRepository.Ado.BeginTran();

                if (input.webType == "3")
                {
                    var categoryData = await _dictionaryDataService.GetInfo(entity.Category);
                    var flowEngine = new FlowEngineEntity();
                    flowEngine.FlowTemplateJson = entity.FlowTemplateJson;
                    flowEngine.EnCode = "#visualDev" + entity.EnCode;
                    flowEngine.Type = 1;
                    flowEngine.FormType = 2;
                    flowEngine.FullName = entity.FullName;
                    flowEngine.Category = categoryData.EnCode;
                    flowEngine.VisibleType = 0;
                    flowEngine.Icon = "icon-ym icon-ym-node";
                    flowEngine.IconBackground = "#008cff";
                    flowEngine.Tables = entity.Tables;
                    flowEngine.DbLinkId = entity.DbLinkId;
                    flowEngine.FormTemplateJson = entity.FormData;
                    //添加流程引擎
                    var engineEntity = await _visualDevRepository.Context.Insertable<FlowEngineEntity>(flowEngine).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();
                    entity.FlowId = engineEntity.Id;
                    entity.Id = engineEntity.Id;
                }

                var visualDev = await _visualDevRepository.Context.Insertable(entity).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Create()).ExecuteReturnEntityAsync();

                //关闭事务
                _visualDevRepository.Ado.CommitTran();
            }
            catch (Exception)
            {
                _visualDevRepository.Ado.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 修改接口
        /// </summary>
        /// <param name="id">主键id</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] VisualDevUpInput input)
        {
            var entity = input.Adapt<VisualDevEntity>();
            entity.FlowId = entity.Id;
            try
            {
                //开启事务
                _visualDevRepository.Ado.BeginTran();

                if (input.webType == "3")
                {
                    var categoryData = await _dictionaryDataService.GetInfo(entity.Category);
                    var engineEntity = await _visualDevRepository.Context.Queryable<FlowEngineEntity>().FirstAsync(f => f.Id == entity.FlowId);
                    engineEntity.FlowTemplateJson = input.flowTemplateJson;
                    engineEntity.EnCode = "#visualDev" + entity.EnCode;
                    engineEntity.Type = 1;
                    engineEntity.FormType = 2;
                    engineEntity.FullName = entity.FullName;
                    engineEntity.Category = categoryData.EnCode;
                    engineEntity.VisibleType = 0;
                    engineEntity.Icon = "icon-ym icon-ym-node";
                    engineEntity.IconBackground = "#008cff";
                    engineEntity.Tables = entity.Tables;
                    engineEntity.DbLinkId = entity.DbLinkId;
                    engineEntity.FormTemplateJson = entity.FormData;
                    await _visualDevRepository.Context.Updateable<FlowEngineEntity>(engineEntity).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
                }

                await _visualDevRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();

                //关闭事务
                _visualDevRepository.Ado.CommitTran();
            }
            catch (Exception)
            {
                _visualDevRepository.Ado.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 删除接口
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _visualDevRepository.FirstOrDefaultAsync(v => v.Id == id && v.DeleteMark == null);
            await _visualDevRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPost("{id}/Actions/Copy")]
        public async Task ActionsCopy(string id)
        {
            var random = new Random().NextLetterAndNumberString(5);
            var entity = await _visualDevRepository.FirstOrDefaultAsync(v => v.Id == id && v.DeleteMark == null);
            entity.FullName = entity.FullName + ".副本"+ random;
            entity.EnCode = entity.EnCode + random;
            entity.State = 0;
            if (entity.WebType == 3)
            {
                var categoryData = await _dictionaryDataService.GetInfo(entity.Category);
                var flowEngine = new FlowEngineEntity();
                flowEngine.FlowTemplateJson = entity.FlowTemplateJson;
                flowEngine.EnCode = "#visualDev" + entity.EnCode;
                flowEngine.Type = 1;
                flowEngine.FormType = 2;
                flowEngine.FullName = entity.FullName;
                flowEngine.Category = categoryData.EnCode;
                flowEngine.VisibleType = 0;
                flowEngine.Icon = "icon-ym icon-ym-node";
                flowEngine.IconBackground = "#008cff";
                flowEngine.Tables = entity.Tables;
                flowEngine.DbLinkId = entity.DbLinkId;
                flowEngine.FormTemplateJson = entity.FormData;
                //添加流程引擎
                var engineEntity = await _visualDevRepository.Context.Insertable<FlowEngineEntity>(flowEngine).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();
                entity.FlowId = engineEntity.Id;
                entity.Id = engineEntity.Id;
                await _visualDevRepository.Context.Insertable(entity).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Create()).ExecuteCommandAsync();
            }
            else
            {
                await _visualDevRepository.Context.Insertable(entity).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
            }
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 获取功能信息
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<VisualDevEntity> GetInfoById(string id)
        {
            return await _visualDevRepository.FirstOrDefaultAsync(v => v.Id == id && v.DeleteMark == null);
        }

        /// <summary>
        /// 判断功能ID是否存在
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> GetDataExists(string id)
        {
            return await _visualDevRepository.AnyAsync(it => it.Id == id && it.DeleteMark == null);
        }

        /// <summary>
        /// 判断是否存在编码、名称相同的数据
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> GetDataExists(string enCode, string fullName)
        {
            return await _visualDevRepository.AnyAsync(it => it.EnCode == enCode && it.FullName == fullName && it.DeleteMark == null);
        }

        /// <summary>
        /// 新增导入数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [NonAction]
        public async Task CreateImportData(VisualDevEntity input)
        {
            try
            {
                //开启事务
                _visualDevRepository.Ado.BeginTran();

                if (input.WebType == 3)
                {
                    var categoryData = await _dictionaryDataService.GetInfo(input.Category);
                    var flowEngine = new FlowEngineEntity();
                    flowEngine.FlowTemplateJson = input.FlowTemplateJson;
                    flowEngine.EnCode = "#visualDev" + input.EnCode;
                    flowEngine.Type = 1;
                    flowEngine.FormType = 2;
                    flowEngine.FullName = input.FullName;
                    flowEngine.Category = categoryData.EnCode;
                    flowEngine.VisibleType = 0;
                    flowEngine.Icon = "icon-ym icon-ym-node";
                    flowEngine.IconBackground = "#008cff";
                    flowEngine.Tables = input.Tables;
                    flowEngine.DbLinkId = input.DbLinkId;
                    flowEngine.FormTemplateJson = input.FormData;
                    //添加流程引擎
                    var engineEntity = await _visualDevRepository.Context.Insertable<FlowEngineEntity>(flowEngine).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();
                    input.FlowId = engineEntity.Id;
                    input.Id = engineEntity.Id;
                }

                var visualDev = await _visualDevRepository.Context.Insertable(input).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();

                //关闭事务
                _visualDevRepository.Ado.CommitTran();
            }
            catch (Exception)
            {
                _visualDevRepository.Ado.RollbackTran();
                throw;
            }
        }

        #endregion
    }
}
