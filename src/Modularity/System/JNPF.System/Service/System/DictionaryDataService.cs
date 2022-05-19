using JNPF.Common.Enum;
using JNPF.Common.Util;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.JsonSerialization;
using JNPF.System.Entitys.Dto.System.DictionaryData;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.Common;
using JNPF.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JNPF.System.Core.Service.DictionaryData
{
    /// <summary>
    /// 字典数据
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "DictionaryData", Order = 203)]
    [Route("api/system/[controller]")]
    public class DictionaryDataService : IDictionaryDataService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<DictionaryDataEntity> _dictionaryDataRepository;
        private readonly IDictionaryTypeService _dictionaryTypeService;
        private readonly IFileService _fileService;
        private readonly SqlSugarScope db;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionaryDataRepository"></param>
        /// <param name="dictionaryTypeService"></param>
        /// <param name="fileService"></param>
        public DictionaryDataService(ISqlSugarRepository<DictionaryDataEntity> dictionaryDataRepository, IDictionaryTypeService dictionaryTypeService, IFileService fileService)
        {
            _dictionaryDataRepository = dictionaryDataRepository;
            _dictionaryTypeService = dictionaryTypeService;
            _fileService = fileService;
            db = dictionaryDataRepository.Context;
        }

        #region GET
        /// <summary>
        /// 获取数据字典列表
        /// </summary>
        /// <param name="dictionaryTypeId">分类id</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("{dictionaryTypeId}")]
        public async Task<dynamic> GetList_Api(string dictionaryTypeId, [FromQuery] DictionaryDataListQuery input)
        {
            var data = await GetList(dictionaryTypeId);
            if ("1".Equals(input.isTree))
            {
                var treeList = data.Adapt<List<DictionaryDataTreeOutput>>();
                if (!string.IsNullOrEmpty(input.keyword))
                {
                    treeList = treeList.TreeWhere(t => t.enCode.Contains(input.keyword) || t.fullName.Contains(input.keyword), t => t.id, t => t.parentId);
                }
                return new { list = treeList.ToTree() };
            }
            else
            {
                if (!string.IsNullOrEmpty(input.keyword))
                {
                    data = data.FindAll(t => t.EnCode.Contains(input.keyword) || t.FullName.Contains(input.keyword));
                }
                var treeList = data.Adapt<List<DictionaryDataTreeOutput>>();
                return new { list = treeList };
            }
        }

        /// <summary>
        ///获取所有数据字典列表(分类+内容)
        /// </summary>
        /// <returns></returns>
        [HttpGet("All")]
        public async Task<dynamic> GetListAll()
        {
            var dictionaryData = (await GetList()).FindAll(x=>x.EnabledMark==1);
            var dictionaryType = await _dictionaryTypeService.GetList();
            var data = dictionaryType.Adapt<List<DictionaryDataAllListOutput>>();
            data.ForEach(dataall =>
            {
                if (dataall.isTree==1)
                {
                    dataall.dictionaryList = dictionaryData.FindAll(d => d.DictionaryTypeId == dataall.id).Adapt<List<DictionaryDataTreeOutput>>().ToTree();
                }
                else
                {
                    dataall.dictionaryList = dictionaryData.FindAll(d => d.DictionaryTypeId == dataall.id).Adapt<List<DictionaryDataListOutput>>();
                }
            });
            return new { list = data };

        }

        /// <summary>
        ///获取字典分类下拉框
        /// </summary>
        /// <returns></returns>
        [HttpGet("{dictionaryTypeId}/Selector/{id}")]
        public async Task<dynamic> GetSelector(string dictionaryTypeId,string id, string isTree)
        {
            var output = new List<DictionaryDataSelectorOutput>();
            var typeEntity = await _dictionaryTypeService.GetInfo(dictionaryTypeId);
            //顶级节点
            var dataEntity = typeEntity.Adapt<DictionaryDataSelectorOutput>();
            dataEntity.id = "0";
            dataEntity.parentId = "-1";
            output.Add(dataEntity);
            if ("1".Equals(isTree))
            {
                var dataList = (await GetList(dictionaryTypeId)).Adapt<List<DictionaryDataSelectorOutput>>();
                if (!id.Equals("0"))
                {
                    dataList.RemoveAll(x => x.id == id);
                }
                output = output.Union(dataList).ToList();
                return new { list = output.ToTree("-1") };
            }
            return new { list = output };
        }

        /// <summary>
        ///获取字典数据下拉框列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("{dictionaryTypeId}/Data/Selector")]
        public async Task<dynamic> GetDataSelector(string dictionaryTypeId)
        {
            try
            {
                var isTree = (await _dictionaryTypeService.GetInfo(dictionaryTypeId)).IsTree;
                var data = await GetList(dictionaryTypeId);
                var datalist = data.FindAll(d => d.EnabledMark == 1);
                var treeList = datalist.Adapt<List<DictionaryDataSelectorDataOutput>>();
                if (isTree == 1)
                {
                    var typeEntity = await _dictionaryTypeService.GetInfo(dictionaryTypeId);
                    //顶级节点
                    var dataEntity = typeEntity.Adapt<DictionaryDataSelectorDataOutput>();
                    dataEntity.id = "0";
                    dataEntity.parentId = "-1";
                    treeList.Add(dataEntity);
                    treeList = treeList.ToTree();
                }
                return new { list = treeList };
            }
            catch (Exception)
            {

                return new { list = new List<object>() };
            }
        }

        /// <summary>
        /// 获取按钮信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [HttpGet("{id}/Info")]
        public async Task<dynamic> GetInfo_Api(string id)
        {
            var data = await GetInfo(id);
            var output = data.Adapt<DictionaryDataInfoOutput>();
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
            var output = new DictionaryDataExportInput();
            await _dictionaryTypeService.GetListAllById(id, output.list);
            foreach (var item in output.list)
            {
                var modelList = await GetList(item.Id);
                output.modelList = output.modelList.Union(modelList).ToList();
            }
            var jsonStr = output.Serialize();
            return _fileService.Export(jsonStr, (await _dictionaryTypeService.GetInfo(id)).FullName);
        }
        #endregion

        #region Post
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Creater_Api([FromBody] DictionaryDataCrInput input)
        {
            if (await _dictionaryDataRepository.AnyAsync(x => x.EnCode == input.enCode && x.DictionaryTypeId == input.dictionaryTypeId && x.DeleteMark == null) || await _dictionaryDataRepository.AnyAsync(x => x.FullName == input.fullName && x.DictionaryTypeId == input.dictionaryTypeId && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D3003);
            var entity = input.Adapt<DictionaryDataEntity>();
            var isOk = await Create(entity);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update_Api(string id, [FromBody] DictionaryDataUpInput input)
        {
            if (await _dictionaryDataRepository.AnyAsync(x => x.EnCode == input.enCode && x.DictionaryTypeId == input.dictionaryTypeId && x.Id != id && x.DeleteMark == null) || await _dictionaryDataRepository.AnyAsync(x => x.Id != id && x.FullName == input.fullName && x.DictionaryTypeId == input.dictionaryTypeId && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D3003);
            var entity = input.Adapt<DictionaryDataEntity>();
            var isOk = await Update(entity);
            if (isOk < 0)
                throw JNPFException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete_Api(string id)
        {
            var entity = await _dictionaryDataRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
            if (entity == null)
                throw JNPFException.Oh(ErrorCode.D3004);
            if (await _dictionaryDataRepository.AnyAsync(o => o.ParentId.Equals(id) && o.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D3002);
            var isOk = await Delete(entity);
            if (isOk < 0)
                throw JNPFException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 更新字典状态
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/State")]
        public async Task ActionsState(string id)
        {
            var entity = await _dictionaryDataRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
            entity.EnabledMark = entity.EnabledMark == 0 ? 1 : 0;
            var isOk = await Update(entity);
            if (isOk < 0)
                throw JNPFException.Oh(ErrorCode.D1506);
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
            var model = josn.Deserialize<DictionaryDataExportInput>();
            if (model == null || model.list.Count == 0)
                throw JNPFException.Oh(ErrorCode.D3006);
            if (model.list.Find(x => x.ParentId == "-1") == null && !_dictionaryTypeService.IsExistParent(model.list))
            {
                throw JNPFException.Oh(ErrorCode.D3007);
            }
            await ImportData(model);
        }
        #endregion

        #region PulicMethod
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="dictionaryTypeId">类别主键</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<DictionaryDataEntity>> GetList(string dictionaryTypeId)
        {
            var entity = await _dictionaryTypeService.GetInfo(dictionaryTypeId);
            return await _dictionaryDataRepository.Entities.Where(d => d.DictionaryTypeId == entity.Id && d.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<DictionaryDataEntity>> GetList()
        {
            return await _dictionaryDataRepository.Entities.Where(d => d.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [NonAction]
        public async Task<DictionaryDataEntity> GetInfo(string id)
        {
            return await _dictionaryDataRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(DictionaryDataEntity entity)
        {
            return await _dictionaryDataRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(DictionaryDataEntity entity)
        {
            return await _dictionaryDataRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(DictionaryDataEntity entity)
        {
            return await _dictionaryDataRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns></returns>
        private async Task ImportData(DictionaryDataExportInput inputList)
        {
            try
            {
                #region 剔除重复数据
                var typeList = inputList.list.FindAll(x => !_dictionaryTypeService.IsExistType(x));
                var dataList = inputList.modelList.FindAll(x => !_dictionaryTypeService.IsExistData(x));
                #endregion

                #region 插入数据
                db.BeginTran();
                var typeStor = db.Storageable(typeList).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                typeStor.AsInsertable.ExecuteCommand(); //执行插入
                typeStor.AsUpdateable.ExecuteCommand(); //执行更新　
                var dataStor = db.Storageable(dataList).Saveable().ToStorage();//存在更新不存在插入 根据主键
                dataStor.AsInsertable.ExecuteCommand(); //执行插入
                dataStor.AsUpdateable.ExecuteCommand(); //执行更新　
                db.CommitTran();
                #endregion
            }
            catch (Exception ex)
            {

                db.RollbackTran();
            }
        }
        #endregion
    }
}
