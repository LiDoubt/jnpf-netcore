using JNPF.Common.Core.Manager;
using JNPF.Common.Extension;
using JNPF.Common.Helper;
using JNPF.Common.Util;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.System.Entitys.Dto.Permission.Authorize;
using JNPF.System.Entitys.Enum;
using JNPF.System.Entitys.Model.Permission.Authorize;
using JNPF.System.Entitys.Permission;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.Permission;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JNPF.System.Service.Permission
{
    /// <summary>
    /// 业务实现：操作权限
    /// </summary>
    [ApiDescriptionSettings(Tag = "Permission", Name = "Authority", Order = 170)]
    [Route("api/permission/[controller]")]
    public class AuthorizeService : IAuthorizeService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<UserEntity> _userRepository;
        private readonly ISqlSugarRepository<AuthorizeEntity> _authorizeRepository; //权限操作表仓储
        private readonly ISqlSugarRepository<ModuleEntity> _moduleRepository;
        private readonly ISqlSugarRepository<ModuleButtonEntity> _moduleButtonRepository;
        private readonly ISqlSugarRepository<ModuleColumnEntity> _moduleColumnRepository;
        private readonly ISqlSugarRepository<ModuleFormEntity> _moduleFormRepository;
        private readonly ISqlSugarRepository<ModuleDataAuthorizeSchemeEntity> _moduleDataAuthorizeSchemeRepository;
        private readonly ISqlSugarRepository<RoleEntity> _roleRepository;

        private readonly IUserManager _userManager;

        /// <summary>
        /// 初始化一个<see cref="AuthorizeService"/>类型的新实例
        /// </summary>
        public AuthorizeService(ISqlSugarRepository<UserEntity> userRepository, ISqlSugarRepository<AuthorizeEntity> authorizeRepository, ISqlSugarRepository<ModuleEntity> moduleRepository, ISqlSugarRepository<ModuleButtonEntity> moduleButtonRepository, ISqlSugarRepository<ModuleColumnEntity> moduleColumnRepository, ISqlSugarRepository<ModuleFormEntity> moduleFormRepository, ISqlSugarRepository<ModuleDataAuthorizeSchemeEntity> moduleDataAuthorizeSchemeRepository, IUserManager userManager, ISqlSugarRepository<RoleEntity> roleRepository)
        {
            _userRepository = userRepository;
            _authorizeRepository = authorizeRepository;
            _moduleRepository = moduleRepository;
            _moduleButtonRepository = moduleButtonRepository;
            _moduleColumnRepository = moduleColumnRepository;
            _moduleFormRepository = moduleFormRepository;
            _moduleDataAuthorizeSchemeRepository = moduleDataAuthorizeSchemeRepository;
            _userManager = userManager;
            _roleRepository = roleRepository;
        }

        #region Get

        /// <summary>
        /// 获取功能权限数据
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        [HttpGet("Model/{itemId}/{objectType}")]
        public async Task<dynamic> GetModelList(string itemId, string objectType)
        {
            var ids = await _authorizeRepository.Where(a => a.ItemId == itemId && a.ObjectType == objectType).Select(s => s.ObjectId).ToListAsync();
            return new { ids };
        }

        #endregion

        #region Post

        /// <summary>
        /// 权限数据
        /// </summary>
        /// <param name="objectId">对象主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("Data/{objectId}/Values")]
        public async Task<dynamic> GetDataValues(string objectId, [FromBody] AuthorizeDataQuery input)
        {
            var output = new AuthorizeDataOutput();
            var authorizeData = new AuthorizeModel();
            var userId = _userManager.UserId;
            var isAdmin = _userManager.IsAdministrator;

            var menuList = await this.GetCurrentUserModuleAuthorize(userId, isAdmin);
            if (menuList.Any(it => it.Category.Equals("App")))
            {
                menuList.Where(it => it.Category.Equals("App") && it.ParentId.Equals("-1")).ToList().ForEach(it =>
                {
                    it.ParentId = "1";
                });
                menuList.Add(new ModuleEntity()
                {
                    Id = "1",
                    FullName = "app菜单",
                    Icon = "ym-custom ym-custom-cellphone",
                    ParentId = "-1",
                    Category = "App",
                    Type = 1,
                    SortCode = 99999
                });
            }
            var moduleButtonList = await this.GetCurrentUserButtonAuthorize(userId, isAdmin);
            var moduleColumnList = await this.GetCurrentUserColumnAuthorize(userId, isAdmin);
            var moduleFormList = await this.GetCurrentUserFormAuthorize(userId, isAdmin);
            var moduleDataSchemeList = await this.GetCurrentUserResourceAuthorize(userId, isAdmin);

            authorizeData.ModuleList = menuList.Adapt<List<AuthorizeModuleModel>>();
            authorizeData.ButtonList = moduleButtonList.Adapt<List<AuthorizeModuleButtonModel>>();
            authorizeData.ColumnList = moduleColumnList.Adapt<List<AuthorizeModuleColumnModel>>();
            authorizeData.FormList = moduleFormList.Adapt<List<AuthorizeModuleFormModel>>();
            authorizeData.ResourceList = moduleDataSchemeList.Adapt<List<AuthorizeModuleResourceModel>>();

            #region 已勾选的权限id

            var authorizeList = await this.GetAuthorizeListByObjectId(objectId);
            var checkModuleList = authorizeList.Where(o => o.ItemType.Equals("module")).Select(m => m.ItemId).ToList();
            var checkButtonList = authorizeList.Where(o => o.ItemType.Equals("button")).Select(m => m.ItemId).ToList();
            var checkColumnList = authorizeList.Where(o => o.ItemType.Equals("column")).Select(m => m.ItemId).ToList();
            var checkFormList = authorizeList.Where(o => o.ItemType.Equals("form")).Select(m => m.ItemId).ToList();
            var checkResourceList = authorizeList.Where(o => o.ItemType.Equals("resource")).Select(m => m.ItemId).ToList();

            #endregion

            var moduleList = new List<ModuleEntity>();
            var childNodesIds = new List<string>();
            switch (input.type)
            {
                case "module":
                    var authorizeDataModuleList = authorizeData.ModuleList.Adapt<List<AuthorizeDataModelOutput>>();
                    GetOutPutResult(ref output, authorizeDataModuleList, checkModuleList);
                    return output;
                case "button":
                    if (string.IsNullOrEmpty(input.moduleIds))
                    {
                        return output;
                    }
                    else
                    {
                        var moduleIdList = new List<string>(input.moduleIds.Split(","));
                        moduleIdList.ForEach(ids =>
                        {
                            var moduleEntity = menuList.Find(m => m.Id == ids);
                            if (moduleEntity != null)
                            {
                                moduleList.Add(moduleEntity);
                            }
                        });
                        //勾选的菜单末级节点菜单id集合
                        childNodesIds = GetChildNodesId(moduleList);
                    }
                    output = GetButton(moduleList, moduleButtonList, childNodesIds, checkButtonList);
                    return output;
                case "column":
                    if (string.IsNullOrEmpty(input.moduleIds))
                    {
                        return output;
                    }
                    else
                    {
                        var moduleIdList = new List<string>(input.moduleIds.Split(","));
                        moduleIdList.ForEach(ids =>
                        {
                            var moduleEntity = menuList.Find(m => m.Id == ids);
                            if (moduleEntity != null)
                            {
                                moduleList.Add(moduleEntity);
                            }
                        });
                        //子节点菜单id集合
                        childNodesIds = GetChildNodesId(moduleList);
                    }
                    output = GetColumn(moduleList, moduleColumnList, childNodesIds, checkColumnList);
                    return output;
                case "form":
                    if (string.IsNullOrEmpty(input.moduleIds))
                    {
                        return output;
                    }
                    else
                    {
                        var moduleIdList = new List<string>(input.moduleIds.Split(","));
                        moduleIdList.ForEach(ids =>
                        {
                            var moduleEntity = menuList.Find(m => m.Id == ids);
                            if (moduleEntity != null)
                            {
                                moduleList.Add(moduleEntity);
                            }
                        });
                        //子节点菜单id集合
                        childNodesIds = GetChildNodesId(moduleList);
                    }
                    output = GetForm(moduleList, moduleFormList, childNodesIds, checkFormList);
                    return output;
                case "resource":
                    if (string.IsNullOrEmpty(input.moduleIds))
                    {
                        return output;
                    }
                    else
                    {
                        var moduleIdList = new List<string>(input.moduleIds.Split(","));
                        moduleIdList.ForEach(ids =>
                        {
                            var moduleEntity = menuList.Find(m => m.Id == ids);
                            if (moduleEntity != null)
                            {
                                moduleList.Add(moduleEntity);
                            }
                        });
                        //子节点菜单id集合
                        childNodesIds = GetChildNodesId(moduleList);
                    }
                    output = GetResource(moduleList, moduleDataSchemeList, childNodesIds, checkResourceList);
                    return output;
                default:
                    return output;
            }
        }

        /// <summary>
        /// 设置或更新岗位/角色/用户权限
        /// </summary>
        /// <param name="objectId">参数</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("Data/{objectId}")]
        public async Task UpdateData(string objectId, [FromBody] AuthorizeDataUpInput input)
        {
            input.button = input.button.Except(input.module).ToList();
            input.column = input.column.Except(input.module).ToList();
            input.resource = input.resource.Except(input.module).ToList();
            var authorizeList = new List<AuthorizeEntity>();
            AddAuthorizeEntity(ref authorizeList, input.module, objectId, input.objectType, "module");
            AddAuthorizeEntity(ref authorizeList, input.button, objectId, input.objectType, "button");
            AddAuthorizeEntity(ref authorizeList, input.column, objectId, input.objectType, "column");
            AddAuthorizeEntity(ref authorizeList, input.form, objectId, input.objectType, "form");
            AddAuthorizeEntity(ref authorizeList, input.resource, objectId, input.objectType, "resource");
            try
            {
                //开启事务
                _authorizeRepository.Ado.BeginTran();
                //删除除了门户外的相关权限
                await _authorizeRepository.DeleteAsync(a => a.ObjectId == objectId && !a.ItemType.Equals("portal"));

                if (authorizeList.Count > 0)
                {
                    //新增权限
                    await _authorizeRepository.Context.Insertable(authorizeList).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
                }
                _authorizeRepository.Ado.CommitTran();
            }
            catch (Exception)
            {
                _authorizeRepository.Ado.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 批量设置权限
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("Data/Batch")]
        public async Task BatchData([FromBody] AuthorizeDataBatchInput input)
        {
            // 计算按钮、列表、资源三个集合内不包含菜单ID的差
            input.button = input.button.Except(input.module).ToList();
            input.column = input.column.Except(input.module).ToList();
            input.resource = input.resource.Except(input.module).ToList();
            // 拼装权限集合
            var authorizeItemList = new List<AuthorizeEntity>();
            var authorizeObejctList = new List<AuthorizeEntity>();
            BatchAddAuthorizeEntity(ref authorizeItemList, input.module, "module", true);
            BatchAddAuthorizeEntity(ref authorizeItemList, input.button, "button", true);
            BatchAddAuthorizeEntity(ref authorizeItemList, input.column, "column", true);
            BatchAddAuthorizeEntity(ref authorizeItemList, input.resource, "resource", true);
            BatchAddAuthorizeEntity(ref authorizeObejctList, input.positionIds, "Position", false);
            BatchAddAuthorizeEntity(ref authorizeObejctList, input.roleIds, "Role", false);
            BatchAddAuthorizeEntity(ref authorizeObejctList, input.userIds, "User", false);
            var data = new List<AuthorizeEntity>();
            SeveBatch(ref data, authorizeObejctList, authorizeItemList);
            // 获取已有权限集合
            var existingRoleData = await _authorizeRepository.Entities.In(a => a.ObjectId, input.roleIds).Where(a => a.ObjectType.Equals("Role")).ToListAsync();
            // 计算新增菜单集合与已有权限集合差
            data = data.Except(existingRoleData).ToList();
            //数据不为空添加
            if (data.Count > 0)
            {
                try
                {
                    //开启事务
                    _authorizeRepository.Ado.BeginTran();

                    //新增权限
                    var num = await _authorizeRepository.Context.Insertable(data).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();


                    _authorizeRepository.Ado.CommitTran();
                }
                catch (Exception)
                {
                    _authorizeRepository.Ado.RollbackTran();
                    throw;
                }
            }
        }

        /// <summary>
        /// 设置/更新功能权限
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("Model/{itemId}")]
        public async Task UpdateModel(string itemId, [FromBody] AuthorizeModelInput input)
        {
            var authorizeList = new List<AuthorizeEntity>();
            //角色ID不为空
            if (input.objectId.Count > 0)
            {
                input.objectId.ForEach(item =>
                {
                    var entity = new AuthorizeEntity();
                    entity.ItemId = itemId;
                    entity.ItemType = input.itemType;
                    entity.ObjectId = item;
                    entity.ObjectType = input.objectType;
                    entity.SortCode = input.objectId.IndexOf(item);
                    authorizeList.Add(entity);
                });
                try
                {
                    //开启事务
                    _authorizeRepository.Ado.BeginTran();
                    //删除除了门户外的相关权限
                    await _authorizeRepository.DeleteAsync(a => a.ItemId == itemId);
                    //新增权限
                    await _authorizeRepository.Context.Insertable(authorizeList).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();

                    _authorizeRepository.Ado.CommitTran();
                }
                catch (Exception)
                {
                    _authorizeRepository.Ado.RollbackTran();
                    throw;
                }
            }
            else
            {
                //删除除了门户外的相关权限
                await _authorizeRepository.DeleteAsync(a => a.ItemId == itemId);
            }
        }

        #endregion

        #region PrivateMethod

        /// <summary>
        /// 添加权限接口参数组装
        /// </summary>
        /// <param name="list">返回参数</param>
        /// <param name="itemIds">权限数据id</param>
        /// <param name="objectId">对象ID</param>
        /// <param name="objectType">分类</param>
        /// <param name="itemType">权限分类</param>
        private void AddAuthorizeEntity(ref List<AuthorizeEntity> list, List<string> itemIds, string objectId, string objectType, string itemType)
        {
            foreach (var item in itemIds)
            {
                var entity = new AuthorizeEntity();
                entity.ItemId = item;
                entity.ObjectId = objectId;
                entity.ItemType = itemType;
                entity.ObjectType = objectType;
                entity.SortCode = itemIds.IndexOf(item);
                list.Add(entity);
            }
        }

        /// <summary>
        /// 批量添加权限接口参数组装
        /// </summary>
        /// <param name="list">返回参数</param>
        /// <param name="ids">来源数据</param>
        /// <param name="type">来源类型</param>
        /// <param name="isData">是否是权限数据</param>
        private void BatchAddAuthorizeEntity(ref List<AuthorizeEntity> list, List<string> ids, string type, bool isData)
        {
            if (ids != null && ids.Count != 0)
            {
                if (isData)
                {
                    foreach (var item in ids)
                    {
                        var entity = new AuthorizeEntity();
                        entity.ItemId = item;
                        entity.ItemType = type;
                        list.Add(entity);
                    }
                }
                else
                {
                    foreach (var item in ids)
                    {
                        var entity = new AuthorizeEntity();
                        entity.ObjectId = item;
                        entity.ObjectType = type;
                        list.Add(entity);
                    }
                }
            }
        }

        /// <summary>
        /// 保存批量权限
        /// </summary>
        /// <param name="list">返回list</param>
        /// <param name="objectList">对象数据</param>
        /// <param name="authorizeList">权限数据</param>
        private void SeveBatch(ref List<AuthorizeEntity> list, List<AuthorizeEntity> objectList, List<AuthorizeEntity> authorizeList)
        {
            foreach (var objectItem in objectList)
            {
                foreach (AuthorizeEntity entityItem in authorizeList)
                {
                    var entity = new AuthorizeEntity();
                    entity.ItemId = entityItem.ItemId;
                    entity.ItemType = entityItem.ItemType;
                    entity.ObjectId = objectItem.ObjectId;
                    entity.ObjectType = objectItem.ObjectType;
                    entity.SortCode = authorizeList.IndexOf(entityItem);
                    list.Add(entity);
                }
            }
        }

        /// <summary>
        /// 返回参数处理
        /// </summary>
        /// <param name="output">返回参数</param>
        /// <param name="list">返回参数数据</param>
        /// <param name="checkList">已勾选的id</param>
        /// <param name="parentId"></param>
        private void GetOutPutResult(ref AuthorizeDataOutput output, List<AuthorizeDataModelOutput> list, List<string> checkList, string parentId = "-1")
        {
            output.all = list.Select(l => l.id).ToList();
            output.ids = checkList;
            output.list = list.OrderBy(x => x.sortCode).ToList().ToTree(parentId);
        }

        /// <summary>
        /// 获取子节点菜单id
        /// </summary>
        /// <param name="moduleEntitiesList"></param>
        /// <returns></returns>
        private List<string> GetChildNodesId(List<ModuleEntity> moduleEntitiesList)
        {
            var ids = moduleEntitiesList.Select(m => m.Id).ToList();
            var pids = moduleEntitiesList.Select(m => m.ParentId).ToList();
            var childNodesIds = ids.Where(x => !pids.Contains(x) && moduleEntitiesList.Find(m => m.Id == x).ParentId != "-1").ToList();
            return childNodesIds.Union(ids).ToList();
        }

        /// <summary>
        /// 过滤菜单权限数据
        /// </summary>
        /// <param name="childNodesIds">其他权限数据菜单id集合</param>
        /// <param name="moduleList">勾选菜单权限数据</param>
        /// <param name="output">返回值</param>
        private void GetParentsModuleList(List<string> childNodesIds, List<ModuleEntity> moduleList, ref List<AuthorizeDataModelOutput> output)
        {
            //获取有其他权限的菜单末级节点id
            var authorizeModuleData = moduleList.Adapt<List<AuthorizeDataModelOutput>>();
            foreach (var item in childNodesIds)
            {
                GteModuleListById(item, authorizeModuleData, output);
            }
            output = output.Distinct().ToList();
        }

        /// <summary>
        /// 根据菜单id递归获取authorizeDataOutputModel的父级菜单
        /// </summary>
        /// <param name="id">菜单id</param>
        /// <param name="authorizeDataOutputModel">选中菜单集合</param>
        /// <param name="output">返回数据</param>
        private void GteModuleListById(string id, List<AuthorizeDataModelOutput> authorizeDataOutputModel, List<AuthorizeDataModelOutput> output)
        {
            var data = authorizeDataOutputModel.Find(l => l.id == id);
            if (data != null)
            {
                if (data.parentId != "-1")
                {
                    if (!output.Contains(data))
                    {
                        output.Add(data);
                    }
                    GteModuleListById(data.parentId, authorizeDataOutputModel, output);
                }
                else
                {
                    if (!output.Contains(data))
                    {
                        output.Add(data);
                    }
                }
            }
        }

        /// <summary>
        /// 按钮权限
        /// </summary>
        /// <param name="moduleList">选中的菜单</param>
        /// <param name="moduleButtonList">所有的菜单</param>
        /// <param name="childNodesIds"></param>
        /// <param name="checkList"></param>
        /// <returns></returns>
        private AuthorizeDataOutput GetButton(List<ModuleEntity> moduleList, List<ModuleButtonEntity> moduleButtonList, List<string> childNodesIds, List<string> checkList)
        {
            var output = new AuthorizeDataOutput();
            var buttonList = new List<ModuleButtonEntity>();
            childNodesIds.ForEach(ids =>
            {
                var buttonEntity = moduleButtonList.FindAll(m => m.ModuleId == ids);
                if (buttonEntity.Count != 0)
                {
                    buttonEntity.ForEach(bt =>
                    {
                        bt.Icon = "";
                        if (bt.ParentId.Equals("-1"))
                        {
                            bt.ParentId = ids;
                        }
                    });
                    buttonList = buttonList.Union(buttonEntity).ToList();
                }
            });
            var authorizeDataButtonList = buttonList.Adapt<List<AuthorizeDataModelOutput>>();
            var authorizeDataModuleList = new List<AuthorizeDataModelOutput>();
            //末级菜单id集合
            var moduleIds = buttonList.Select(b => b.ModuleId).ToList().Distinct().ToList();
            GetParentsModuleList(moduleIds, moduleList, ref authorizeDataModuleList);
            var list = authorizeDataModuleList.Union(authorizeDataButtonList).ToList();
            GetOutPutResult(ref output, list, checkList);
            return output;
        }

        /// <summary>
        /// 列表权限
        /// </summary>
        /// <param name="moduleList"></param>
        /// <param name="moduleColumnEntity"></param>
        /// <param name="childNodesIds"></param>
        /// <param name="checkList"></param>
        /// <returns></returns>
        private AuthorizeDataOutput GetColumn(List<ModuleEntity> moduleList, List<ModuleColumnEntity> moduleColumnEntity, List<string> childNodesIds, List<string> checkList)
        {
            var output = new AuthorizeDataOutput();
            var columnList = new List<ModuleColumnEntity>();
            childNodesIds.ForEach(ids =>
            {
                var columnEntity = moduleColumnEntity.FindAll(m => m.ModuleId == ids);
                if (columnEntity.Count != 0)
                {
                    columnEntity.ForEach(bt =>
                    {
                        bt.ParentId = ids;
                    });
                    columnList = columnList.Union(columnEntity).ToList();
                }
            });
            var authorizeDataColumnList = columnList.Adapt<List<AuthorizeDataModelOutput>>();
            var authorizeDataModuleList = new List<AuthorizeDataModelOutput>();
            var moduleIds = columnList.Select(b => b.ModuleId).ToList().Distinct().ToList();
            GetParentsModuleList(moduleIds, moduleList, ref authorizeDataModuleList);
            var list = authorizeDataModuleList.Union(authorizeDataColumnList).ToList();
            GetOutPutResult(ref output, list, checkList);
            return output;
        }

        /// <summary>
        /// 表单权限
        /// </summary>
        /// <returns></returns>
        private AuthorizeDataOutput GetForm(List<ModuleEntity> moduleList, List<ModuleFormEntity> moduleFormEntity, List<string> childNodesIds, List<string> checkList)
        {
            var output = new AuthorizeDataOutput();
            var formList = new List<ModuleFormEntity>();
            childNodesIds.ForEach(ids =>
            {
                var formEntity = moduleFormEntity.FindAll(m => m.ModuleId == ids);
                if (formEntity.Count != 0)
                {
                    formEntity.ForEach(bt =>
                    {
                        bt.ParentId = ids;
                    });
                    formList = formList.Union(formEntity).ToList();
                }
            });
            var authorizeDataFormList = formList.Adapt<List<AuthorizeDataModelOutput>>();
            var authorizeDataModuleList = new List<AuthorizeDataModelOutput>();
            var moduleIds = formList.Select(b => b.ModuleId).ToList().Distinct().ToList();
            GetParentsModuleList(moduleIds, moduleList, ref authorizeDataModuleList);
            var list = authorizeDataModuleList.Union(authorizeDataFormList).ToList();
            GetOutPutResult(ref output, list, checkList);
            return output;
        }

        /// <summary>
        /// 数据权限
        /// </summary>
        /// <param name="moduleList"></param>
        /// <param name="moduleResourceEntity"></param>
        /// <param name="childNodesIds"></param>
        /// <param name="checkList"></param>
        /// <returns></returns>
        private AuthorizeDataOutput GetResource(List<ModuleEntity> moduleList, List<ModuleDataAuthorizeSchemeEntity> moduleResourceEntity, List<string> childNodesIds, List<string> checkList)
        {
            var moduleIds = new List<string>();
            var output = new AuthorizeDataOutput();
            var authorizeDataResourceList = new List<AuthorizeDataModelOutput>();
            childNodesIds.ForEach(ids =>
            {
                var resourceEntity = moduleResourceEntity.FindAll(m => m.ModuleId == ids);
                if (resourceEntity.Count != 0)
                {
                    moduleIds.Add(ids);
                    var entity = resourceEntity.Adapt<List<AuthorizeDataModelOutput>>();

                    entity.ForEach(e =>
                    {
                        e.parentId = ids;
                    });
                    authorizeDataResourceList = authorizeDataResourceList.Union(entity).ToList();
                }
            });
            var authorizeDataModuleList = new List<AuthorizeDataModelOutput>();
            GetParentsModuleList(moduleIds, moduleList, ref authorizeDataModuleList);
            var list = authorizeDataModuleList.Union(authorizeDataResourceList).ToList();
            GetOutPutResult(ref output, list, checkList);
            return output;
        }

        private ConditionalModel GetConditionalModel(SearchMethod expressType, string fieldName, string fieldValue, string dataType = "string")
        {
            switch (expressType)
            {
                //like
                case SearchMethod.Contains:
                    return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.Like, FieldValue = fieldValue };
                //等于
                case SearchMethod.Equal:
                    switch (dataType)
                    {
                        case "Double":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.Equal, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(double)) };
                        case "Int32":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.Equal, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(int)) };
                        default:
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.Equal, FieldValue = fieldValue };
                    }
                //不等于
                case SearchMethod.NotEqual:
                    switch (dataType)
                    {
                        case "Double":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.NoEqual, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(double)) };
                        case "Int32":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.NoEqual, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(int)) };
                        default:
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.NoEqual, FieldValue = fieldValue };
                    }
                //小于
                case SearchMethod.LessThan:
                    switch (dataType)
                    {
                        case "Double":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.LessThan, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(double)) };
                        case "Int32":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.LessThan, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(int)) };
                        default:
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.LessThan, FieldValue = fieldValue };
                    }
                //小于等于
                case SearchMethod.LessThanOrEqual:
                    switch (dataType)
                    {
                        case "Double":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.LessThan, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(double)) };
                        case "Int32":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.LessThan, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(int)) };
                        default:
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = fieldValue };
                    }
                //大于
                case SearchMethod.GreaterThan:
                    switch (dataType)
                    {
                        case "Double":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.GreaterThan, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(double)) };
                        case "Int32":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.GreaterThan, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(int)) };
                        default:
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.GreaterThan, FieldValue = fieldValue };
                    }
                //大于等于
                case SearchMethod.GreaterThanOrEqual:
                    switch (dataType)
                    {
                        case "Double":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(double)) };
                        case "Int32":
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = fieldValue, FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(int)) };
                        default:
                            return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = fieldValue };
                    }
                //包含
                case SearchMethod.In:
                    return new ConditionalModel() { FieldName = fieldName, ConditionalType = ConditionalType.In, FieldValue = fieldValue };
            }
            return new ConditionalModel();
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 当前用户模块权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="isAdmin">是否超管</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ModuleEntity>> GetCurrentUserModuleAuthorize(string userId, bool isAdmin)
        {
            var output = new List<ModuleEntity>();
            if (!isAdmin)
            {
                var role = _userRepository.FirstOrDefault(u => u.Id == userId && u.DeleteMark == null).RoleId;
                var roleArray = role.Split(',');
                var roleId = await _roleRepository.Entities.In(r => r.Id, roleArray).Where(r => r.EnabledMark.Equals(1) && r.DeleteMark == null).Select(r => r.Id).ToListAsync();
                var items = await _authorizeRepository.Entities.In(a => a.ObjectId, roleId).Where(a => a.ItemType == "module").GroupBy(it => new { it.ItemId }).Select(it => new { it.ItemId }).ToListAsync();
                if (items.Count == 0) return output;
                output = await _moduleRepository.Entities.In(m => m.Id, items.Select(it => it.ItemId).ToArray()).Where(a => a.EnabledMark.Equals(1) && a.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
            }
            else
            {
                output = await _moduleRepository.Where(a => a.EnabledMark.Equals(1) && a.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
            }
            return output;
        }

        /// <summary>
        /// 当前用户模块按钮权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="isAdmin">是否超管</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ModuleButtonEntity>> GetCurrentUserButtonAuthorize(string userId, bool isAdmin)
        {
            var output = new List<ModuleButtonEntity>();
            if (!isAdmin)
            {
                var role = _userRepository.FirstOrDefault(u => u.Id == userId && u.DeleteMark == null).RoleId;
                var roleArray = role.Split(',');
                var roleId = await _roleRepository.Entities.In(r => r.Id, roleArray).Where(r => r.EnabledMark.Equals(1) && r.DeleteMark == null).Select(r => r.Id).ToListAsync();
                var items = await _authorizeRepository.Entities.In(a => a.ObjectId, roleId).Where(a => a.ItemType == "button").GroupBy(it => new { it.ItemId }).Select(it => new { it.ItemId }).ToListAsync();
                if (items.Count == 0) return output;
                output = await _moduleButtonRepository.Entities.In(m => m.Id, items.Select(it => it.ItemId).ToArray()).Where(a => a.EnabledMark.Equals(1) && a.DeleteMark == null).OrderBy(o => o.SortCode)
                    .Mapper(a =>
                    {
                        a.ParentId = a.ParentId.Equals("-1") ? a.ModuleId : a.ParentId;
                    }).ToListAsync();
            }
            else
            {
                output = await _moduleButtonRepository.Where(a => a.EnabledMark.Equals(1) && a.DeleteMark == null).OrderBy(o => o.SortCode)
                    .Mapper(a =>
                    {
                        a.ParentId = a.ParentId.Equals("-1") ? a.ModuleId : a.ParentId;
                    }).ToListAsync();
            }
            return output;
        }

        /// <summary>
        /// 当前用户模块列权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="isAdmin">是否超管</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ModuleColumnEntity>> GetCurrentUserColumnAuthorize(string userId, bool isAdmin)
        {
            var output = new List<ModuleColumnEntity>();
            if (!isAdmin)
            {
                var role = _userRepository.FirstOrDefault(u => u.Id == userId && u.DeleteMark == null).RoleId;
                var roleArray = role.Split(',');
                var roleId = await _roleRepository.Entities.In(r => r.Id, roleArray).Where(r => r.EnabledMark.Equals(1) && r.DeleteMark == null).Select(r => r.Id).ToListAsync();
                var items = await _authorizeRepository.Entities.In(a => a.ObjectId, roleId).Where(a => a.ItemType == "column").GroupBy(it => new { it.ItemId }).Select(it => new { it.ItemId }).ToListAsync();
                if (items.Count == 0) return output;
                output = await _moduleColumnRepository.Entities.In(m => m.Id, items.Select(it => it.ItemId).ToArray()).Where(a => a.EnabledMark.Equals(1) && a.DeleteMark == null).OrderBy(o => o.SortCode)
                    .Mapper(a =>
                    {
                        a.ParentId = a.ModuleId;
                    }).ToListAsync();
            }
            else
            {
                output = await _moduleColumnRepository.Where(a => a.EnabledMark.Equals(1) && a.DeleteMark == null).OrderBy(o => o.SortCode)
                    .Mapper(a =>
                    {
                        a.ParentId = a.ModuleId;
                    }).ToListAsync();
            }
            return output;
        }

        /// <summary>
        /// 当前用户模块表单权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="isAdmin">是否超管</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ModuleFormEntity>> GetCurrentUserFormAuthorize(string userId, bool isAdmin)
        {
            var output = new List<ModuleFormEntity>();
            if (!isAdmin)
            {
                var role = _userRepository.FirstOrDefault(u => u.Id == userId && u.DeleteMark == null).RoleId;
                var roleArray = role.Split(',');
                var roleId = await _roleRepository.Entities.In(r => r.Id, roleArray).Where(r => r.EnabledMark.Equals(1) && r.DeleteMark == null).Select(r => r.Id).ToListAsync();
                var items = await _authorizeRepository.Entities.In(a => a.ObjectId, roleId).Where(a => a.ItemType == "form").GroupBy(it => new { it.ItemId }).Select(it => new { it.ItemId }).ToListAsync();
                if (items.Count == 0) return output;
                output = await _moduleFormRepository.Entities.In(m => m.Id, items.Select(it => it.ItemId).ToArray()).Where(a => a.EnabledMark.Equals(1) && a.DeleteMark == null).OrderBy(o => o.SortCode)
                    .Mapper(a =>
                    {
                        a.ParentId = a.ModuleId;
                    }).ToListAsync();
            }
            else
            {
                output = await _moduleFormRepository.Where(a => a.EnabledMark.Equals(1) && a.DeleteMark == null).OrderBy(o => o.SortCode)
                    .Mapper(a =>
                    {
                        a.ParentId = a.ModuleId;
                    }).ToListAsync();
            }
            return output;
        }

        /// <summary>
        /// 当前用户模块权限资源
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="isAdmin">是否超管</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ModuleDataAuthorizeSchemeEntity>> GetCurrentUserResourceAuthorize(string userId, bool isAdmin)
        {
            var output = new List<ModuleDataAuthorizeSchemeEntity>();
            if (!isAdmin)
            {
                var role = _userRepository.FirstOrDefault(u => u.Id == userId && u.DeleteMark == null).RoleId;
                var roleArray = role.Split(',');
                var roleId = await _roleRepository.Entities.In(r => r.Id, roleArray).Where(r => r.EnabledMark.Equals(1) && r.DeleteMark == null).Select(r => r.Id).ToListAsync();
                var items = await _authorizeRepository.Entities.In(a => a.ObjectId, roleId).Where(a => a.ItemType == "resource").GroupBy(it => new { it.ItemId }).Select(it => new { it.ItemId }).ToListAsync();
                if (items.Count == 0) return output;
                output = await _moduleDataAuthorizeSchemeRepository.Entities.In(m => m.Id, items.Select(it => it.ItemId).ToArray()).Where(a => a.EnabledMark.Equals(1) && a.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
            }
            else
            {
                output = await _moduleDataAuthorizeSchemeRepository.Where(a => a.EnabledMark.Equals(1) && a.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
            }
            return output;
        }

        /// <summary>
        /// 获取权限项ids
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <param name="itemType">项类型</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<string>> GetAuthorizeItemIds(string roleId, string itemType)
        {
            var data = await _authorizeRepository.Where(a => a.ObjectId == roleId && a.ItemType == itemType).GroupBy(it => new { it.ItemId }).Select(it => new { it.ItemId }).ToListAsync();
            return data.Select(it => it.ItemId).ToList();
        }

        /// <summary>
        /// 是否存在权限资源
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> GetIsExistModuleDataAuthorizeScheme(string[] ids)
        {
            return await _moduleDataAuthorizeSchemeRepository.AnyAsync(m => ids.Contains(m.Id) && m.DeleteMark == null);
        }

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="objectId">对象主键</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<AuthorizeEntity>> GetAuthorizeListByObjectId(string objectId)
        {
            return await _authorizeRepository.Where(a => a.ObjectId == objectId).ToListAsync();
        }

        /// <summary>
        /// 获取数据条件
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<IConditionalModel>> GetConditionAsync<T>(string moduleId) where T : new()
        {
            var userInfo = await _userManager.GetUserInfo();
            var conModels = new List<IConditionalModel>();
            var roleId = _roleRepository.Entities.In(r => r.Id, userInfo.roleIds).Where(r => r.EnabledMark.Equals(1) && r.DeleteMark == null).Select(r => r.Id).ToList();
            var items = _authorizeRepository.Entities.In(a => a.ObjectId, roleId).Where(a => a.ItemType == "resource").GroupBy(it => new { it.ItemId }).Select(it => new { it.ItemId }).ToList();
            if (items.Count == 0) return conModels;
            var resourceList = _moduleDataAuthorizeSchemeRepository.Entities.In(it => it.Id, items.Select(it => it.ItemId).ToArray()).Where(it => it.ModuleId == moduleId && it.DeleteMark == null).ToList();
            foreach (var item in resourceList)
            {
                var conditionModelList = JsonHelper.ToList<AuthorizeModuleResourceConditionModel>(item.ConditionJson);
                foreach (var conditionItem in conditionModelList)
                {
                    foreach (var fieldItem in conditionItem.Groups)
                    {
                        var itemField = fieldItem.Field.Replace("F_", "").Replace("f_", "").LowerFirstChar();
                        var itemValue = fieldItem.Value;
                        var itemMethod = (SearchMethod)Enum.Parse(typeof(SearchMethod), fieldItem.Op);
                        switch (itemValue.ToString())
                        {
                            //当前用户
                            case "@userId":
                                {
                                    switch (conditionItem.Logic)
                                    {
                                        case "and":
                                            conModels.Add(new ConditionalCollections()
                                            {
                                                ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>() {
                                                    new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, GetConditionalModel(itemMethod, itemField, userInfo.userId))
                                                }
                                            });
                                            break;
                                        case "or":
                                            conModels.Add(new ConditionalCollections()
                                            {
                                                ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>() {
                                                    new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(itemMethod, itemField, userInfo.userId))
                                                }
                                            });
                                            break;
                                    }
                                }
                                break;
                            //当前用户集下属
                            case "@userAraSubordinates":
                                {
                                    switch (conditionItem.Logic)
                                    {
                                        case "and":
                                            conModels.Add(new ConditionalCollections()
                                            {
                                                ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>() {
                                                   new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, GetConditionalModel(itemMethod, itemField, userInfo.userId)),
                                                   new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(SearchMethod.In, itemField, string.Join(",", userInfo.subordinates)))
                                                }
                                            });
                                            break;
                                        case "or":
                                            conModels.Add(new ConditionalCollections()
                                            {
                                                ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>() {
                                                   new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(itemMethod, itemField, userInfo.userId)),
                                                   new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(SearchMethod.In, itemField, string.Join(",", userInfo.subordinates)))
                                                }
                                            });
                                            break;
                                    }
                                }
                                break;
                            //当前组织
                            case "@organizeId":
                                {
                                    if (!string.IsNullOrEmpty(userInfo.organizeId))
                                        switch (conditionItem.Logic)
                                        {
                                            case "and":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, GetConditionalModel(itemMethod, itemField, userInfo.organizeId))
                                                    }
                                                });
                                                break;
                                            case "or":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(itemMethod, itemField, userInfo.organizeId))
                                                    }
                                                });
                                                break;
                                        }
                                }
                                break;
                            //当前组织及子组织
                            case "@organizationAndSuborganization":
                                {
                                    if (!string.IsNullOrEmpty(userInfo.organizeId))
                                        switch (conditionItem.Logic)
                                        {
                                            case "and":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, GetConditionalModel(itemMethod, itemField, userInfo.organizeId)),
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(SearchMethod.In, itemField, string.Join(",", userInfo.subsidiary)))
                                                    }
                                                });
                                                break;
                                            case "or":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(itemMethod, itemField, userInfo.organizeId)),
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(SearchMethod.In, itemField, string.Join(",", userInfo.subsidiary)))
                                                    }
                                                });
                                                break;
                                        }
                                }
                                break;
                            default:
                                {
                                    if (!string.IsNullOrEmpty(itemValue))
                                        switch (conditionItem.Logic)
                                        {
                                            case "and":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, GetConditionalModel(itemMethod, itemField, itemValue, fieldItem.Type))
                                                    }
                                                });
                                                break;
                                            case "or":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(itemMethod, itemField, itemValue, fieldItem.Type))
                                                    }
                                                });
                                                break;
                                        }
                                }
                                break;
                        }
                    }
                }
            }
            if (resourceList.Count == 0)
            {
                conModels.Add(new ConditionalCollections()
                {
                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                    {
                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, new ConditionalModel() { FieldName = "1", ConditionalType = ConditionalType.Equal, FieldValue = "0", FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(int)) })
                    }
                });
            }
            return conModels;
        }

        /// <summary>
        /// 获取数据条件(在线开发专用)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public List<IConditionalModel> GetCondition<T>(string moduleId) where T : new()
        {
            var userInfo = _userManager.GetUserInfo().Result;
            var conModels = new List<IConditionalModel>();
            var roleId = _roleRepository.Entities.In(r => r.Id, userInfo.roleIds).Where(r => r.EnabledMark.Equals(1) && r.DeleteMark == null).Select(r => r.Id).ToList();
            var items = _authorizeRepository.Entities.In(a => a.ObjectId, roleId).Where(a => a.ItemType == "resource").GroupBy(it => new { it.ItemId }).Select(it => new { it.ItemId }).ToList();
            if (items.Count == 0) return conModels;
            var resourceList = _moduleDataAuthorizeSchemeRepository.Entities.In(it => it.Id, items.Select(it => it.ItemId).ToArray()).Where(it => it.ModuleId == moduleId && it.DeleteMark == null).ToList();
            foreach (var item in resourceList)
            {
                var conditionModelList = JsonHelper.ToList<AuthorizeModuleResourceConditionModel>(item.ConditionJson);
                foreach (var conditionItem in conditionModelList)
                {
                    foreach (var fieldItem in conditionItem.Groups)
                    {
                        var itemField = fieldItem.Field;
                        var itemValue = fieldItem.Value;
                        var itemMethod = (SearchMethod)Enum.Parse(typeof(SearchMethod), fieldItem.Op);

                        switch (itemValue.ToString())
                        {
                            //当前用户
                            case "@userId":
                                {
                                    switch (conditionItem.Logic)
                                    {
                                        case "and":
                                            conModels.Add(new ConditionalCollections()
                                            {
                                                ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>() {
                                                    new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, GetConditionalModel(itemMethod, itemField, userInfo.userId))
                                                }
                                            });
                                            break;
                                        case "or":
                                            conModels.Add(new ConditionalCollections()
                                            {
                                                ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>() {
                                                    new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(itemMethod, itemField, userInfo.userId))
                                                }
                                            });
                                            break;
                                    }
                                }
                                break;
                            //当前用户集下属
                            case "@userAraSubordinates":
                                {
                                    switch (conditionItem.Logic)
                                    {
                                        case "and":
                                            conModels.Add(new ConditionalCollections()
                                            {
                                                ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>() {
                                                   new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, GetConditionalModel(itemMethod, itemField, userInfo.userId)),
                                                   new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(SearchMethod.In, itemField, string.Join(",", userInfo.subordinates)))
                                                }
                                            });
                                            break;
                                        case "or":
                                            conModels.Add(new ConditionalCollections()
                                            {
                                                ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>() {
                                                   new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(itemMethod, itemField, userInfo.userId)),
                                                   new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(SearchMethod.In, itemField, string.Join(",", userInfo.subordinates)))
                                                }
                                            });
                                            break;
                                    }
                                }
                                break;
                            //当前组织
                            case "@organizeId":
                                {
                                    if (!string.IsNullOrEmpty(userInfo.organizeId))
                                        switch (conditionItem.Logic)
                                        {
                                            case "and":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, GetConditionalModel(itemMethod, itemField, userInfo.organizeId))
                                                    }
                                                });
                                                break;
                                            case "or":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(itemMethod, itemField, userInfo.organizeId))
                                                    }
                                                });
                                                break;
                                        }
                                }
                                break;
                            //当前组织及子组织
                            case "@organizationAndSuborganization":
                                {
                                    if (!string.IsNullOrEmpty(userInfo.organizeId))
                                        switch (conditionItem.Logic)
                                        {
                                            case "and":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, GetConditionalModel(itemMethod, itemField, userInfo.organizeId)),
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(SearchMethod.In, itemField, string.Join(",", userInfo.subsidiary)))
                                                    }
                                                });
                                                break;
                                            case "or":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(itemMethod, itemField, userInfo.organizeId)),
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(SearchMethod.In, itemField, string.Join(",", userInfo.subsidiary)))
                                                    }
                                                });
                                                break;
                                        }
                                }
                                break;
                            default:
                                {
                                    if (!string.IsNullOrEmpty(itemValue))
                                        switch (conditionItem.Logic)
                                        {
                                            case "and":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, GetConditionalModel(itemMethod, itemField, itemValue, fieldItem.Type))
                                                    }
                                                });
                                                break;
                                            case "or":
                                                conModels.Add(new ConditionalCollections()
                                                {
                                                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                                                    {
                                                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, GetConditionalModel(itemMethod, itemField, itemValue, fieldItem.Type))
                                                    }
                                                });
                                                break;
                                        }
                                }
                                break;
                        }
                    }
                }
            }
            if (resourceList.Count == 0)
            {
                conModels.Add(new ConditionalCollections()
                {
                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                    {
                        new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, new ConditionalModel() { FieldName = "1", ConditionalType = ConditionalType.Equal, FieldValue = "0", FieldValueConvertFunc = it => SqlSugar.UtilMethods.ChangeType2(it, typeof(int)) })
                    }
                });
            }
            return conModels;
        }

        #endregion
    }
}