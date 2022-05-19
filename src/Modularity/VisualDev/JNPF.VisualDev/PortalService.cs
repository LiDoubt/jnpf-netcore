using JNPF.Common.Filter;
using JNPF.Common.Util;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.System.Entitys.Permission;
using JNPF.System.Entitys.System;
using JNPF.VisualDev.Entitys;
using JNPF.VisualDev.Entitys.Dto.Portal;
using JNPF.VisualDev.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using SqlSugar;
using System.Linq;
using System.Threading.Tasks;
using JNPF.Common.Core.Manager;
using JNPF.FriendlyException;
using JNPF.Common.Enum;
using System.Collections.Generic;
using JNPF.Common.Extension;
using JNPF.JsonSerialization;
using JNPF.System.Interfaces.Common;
using Microsoft.AspNetCore.Http;
using System;

namespace JNPF.VisualDev
{
    /// <summary>
    ///  业务实现：门户设计
    /// </summary>
    [ApiDescriptionSettings(Tag = "VisualDev", Name = "Portal", Order = 173)]
    [Route("api/visualdev/[controller]")]
    public class PortalService : IPortalService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<PortalEntity> _portalRepository;
        private readonly ISqlSugarRepository<DictionaryDataEntity> _dictionaryDataRepository;// 数据字典表仓储
        private readonly ISqlSugarRepository<AuthorizeEntity> _authorizeRepository; //权限操作表仓储
        private readonly ISqlSugarRepository<RoleEntity> _roleRepository;
        private readonly IUserManager _userManager;
        private readonly IFileService _fileService;

        /// <summary>
        /// 初始化一个<see cref="PortalService"/>类型的新实例
        /// </summary>
        public PortalService(ISqlSugarRepository<PortalEntity> portalRepository, ISqlSugarRepository<RoleEntity> roleRepository, ISqlSugarRepository<DictionaryDataEntity> dictionaryDataRepository, IUserManager userManager, ISqlSugarRepository<AuthorizeEntity> authorizeRepository, IFileService fileService)
        {
            _portalRepository = portalRepository;
            _dictionaryDataRepository = dictionaryDataRepository;
            _userManager = userManager;
            _authorizeRepository = authorizeRepository;
            _roleRepository = roleRepository;
            _fileService = fileService;
        }

        #region Get

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input">请求参数</param>
        /// <returns>返回列表</returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] KeywordInput input)
        {
            var portalList = await _portalRepository.Context.Queryable<PortalEntity, UserEntity, UserEntity>((a, b, c) => new JoinQueryInfos(JoinType.Left, b.Id == a.CreatorUserId, JoinType.Left, c.Id == a.LastModifyUserId))
               .WhereIF(!string.IsNullOrEmpty(input.keyword), a => a.FullName.Contains(input.keyword) || a.EnCode.Contains(input.keyword))
               .Where(a => a.DeleteMark == null)
               .OrderBy(a => a.SortCode)
               .Select((a, b, c) => new PortalListOutput { id = a.Id, fullName = a.FullName, enCode = a.EnCode, deleteMark = SqlFunc.ToString(a.DeleteMark), description = a.Description, creatorTime = a.CreatorTime, creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account), parentId = a.Category, lastModifyUser = SqlFunc.MergeString(c.RealName, SqlFunc.IIF(c.RealName == null, "", "/"), c.Account), lastModifyTime = SqlFunc.ToDate(a.LastModifyTime), enabledMark = a.EnabledMark, sortCode = SqlFunc.ToString(a.SortCode) })
               .ToListAsync();
            var parentIds = portalList.Select(x => x.parentId).ToList().Distinct();
            var treeList = await _dictionaryDataRepository.Where(d => parentIds.Contains(d.Id) && d.DeleteMark == null && d.EnabledMark.Equals(1))
                .Select(d => new PortalListOutput { id = d.Id, parentId = "0", enCode = "", fullName = d.FullName }).ToListAsync();
            return new { list = treeList.Union(portalList).ToList().ToTree("0") };
        }

        /// <summary>
        /// 获取门户侧边框列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> GetSelector([FromQuery] string type)
        {
            var userInfo = await _userManager.GetUserInfo();
            var data = new List<PortalSelectOutput>();
            if ("1".Equals(type) && !userInfo.isAdministrator)
            {
                var roleId = await _roleRepository.Entities.In(r => r.Id, userInfo.roleIds).Where(r => r.EnabledMark.Equals(1) && r.DeleteMark == null).Select(r => r.Id).ToListAsync();
                var items = await _authorizeRepository.Entities.In(a => a.ObjectId, roleId).Where(a => a.ItemType == "portal").GroupBy(it => new { it.ItemId }).Select(it => new { it.ItemId }).ToListAsync();
                if (items.Count != 0)
                    data = await _portalRepository.Entities.In(p => p.Id, items.Select(it => it.ItemId).ToArray()).Select(s => new PortalSelectOutput { id = s.Id, fullName = s.FullName, parentId = s.Category, enabledMark = SqlFunc.ToInt32(s.EnabledMark), sortCode = SqlFunc.ToString(s.SortCode), deleteMark = SqlFunc.ToString(s.DeleteMark) })
                        .MergeTable()
                        .Where(p => p.enabledMark.Equals(1) && p.deleteMark == null).OrderBy(p => p.sortCode).ToListAsync();
            }
            else
            {
                data = await _portalRepository.Entities.Select(s => new PortalSelectOutput { id = s.Id, fullName = s.FullName, parentId = s.Category, enabledMark = SqlFunc.ToInt32(s.EnabledMark), sortCode = SqlFunc.ToString(s.SortCode), deleteMark = SqlFunc.ToString(s.DeleteMark) })
                   .MergeTable().Where(p => p.enabledMark.Equals(1) && p.deleteMark == null).OrderBy(o => o.sortCode).ToListAsync();
            }
            var parentIds = data.Select(it => it.parentId).Distinct().ToList();
            var treeList = new List<PortalSelectOutput>();
            if (parentIds.Count() != 0)
                treeList = await _dictionaryDataRepository.Entities.In(it => it.Id, parentIds.ToArray()).Where(d => d.DeleteMark == null && d.EnabledMark.Equals(1)).Select(d => new PortalSelectOutput
                {
                    id = d.Id,
                    parentId = "0",
                    fullName = d.FullName,
                    sortCode = SqlFunc.ToString(d.SortCode)
                }).MergeTable().OrderBy(o => o.sortCode).ToListAsync();
            return new { list = treeList.Union(data).ToList().ToTree("0") };
        }

        /// <summary>
        /// 获取门户信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var data = await _portalRepository.SingleAsync(p => p.Id == id);
            var output = data.Adapt<PortalInfoOutput>();
            return output;
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/auth")]
        public async Task<dynamic> GetInfoAuth(string id)
        {
            var userInfo = await _userManager.GetUserInfo();
            if (userInfo.roleIds != null && !userInfo.isAdministrator)
            {
                var roleId = await _roleRepository.Entities.In(r => r.Id, userInfo.roleIds).Where(r => r.EnabledMark.Equals(1) && r.DeleteMark == null).Select(r => r.Id).ToListAsync();
                var items = await _authorizeRepository.Entities.In(a => a.ObjectId, roleId).Where(a => a.ItemType == "portal").GroupBy(it => new { it.ItemId }).Select(it => new { it.ItemId }).ToListAsync();
                if (items.Count == 0) return null;
                var entity = await _portalRepository.Entities.In(p => p.Id, items.Select(it => it.ItemId).ToArray()).SingleAsync(p => p.Id == id && p.EnabledMark.Equals(1) && p.DeleteMark == null);
                _ = entity ?? throw JNPFException.Oh(ErrorCode.COM1005);
                return new { formData = entity.FormData };
            }
            if (userInfo.isAdministrator)
            {
                var entity = await _portalRepository.SingleAsync(p => p.Id == id && p.EnabledMark.Equals(1) && p.DeleteMark == null);
                _ = entity ?? throw JNPFException.Oh(ErrorCode.COM1005);
                return new { formData = entity.FormData };
            }
            throw JNPFException.Oh(ErrorCode.D1900);
        }

        #endregion

        #region Post

        /// <summary>
        /// 门户导出
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        [HttpPost("{modelId}/Actions/ExportData")]
        public async Task<dynamic> ActionsExportData(string modelId)
        {
            //模板实体
            var templateEntity = await _portalRepository.SingleAsync(p => p.Id == modelId);
            var jsonStr = templateEntity.Serialize();
            return _fileService.Export(jsonStr, templateEntity.FullName);
        }

        /// <summary>
        /// 门户导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("Model/Actions/ImportData")]
        public async Task ActionsImportData(IFormFile file)
        {
            var josn = _fileService.Import(file);
            var templateEntity = josn.Deserialize<PortalEntity>();
            if (templateEntity == null)
                throw JNPFException.Oh(ErrorCode.D3006);
            if (templateEntity != null && templateEntity.FormData.IndexOf("layouyId") <= 0)
                throw JNPFException.Oh(ErrorCode.D3006);
            if (!string.IsNullOrEmpty(templateEntity.Id) && await _portalRepository.AnyAsync(it => it.Id == templateEntity.Id && it.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D1400);
            if (await _portalRepository.AnyAsync(it => it.EnCode == templateEntity.EnCode && it.FullName == templateEntity.FullName && it.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D1400);
            await _portalRepository.Context.Insertable(templateEntity).CallEntityMethod(m => m.Create()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 新建门户信息
        /// </summary>
        /// <param name="input">实体对象</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] PortalCrInput input)
        {
            var entity = input.Adapt<PortalEntity>();
            if (string.IsNullOrEmpty(entity.Category))
                throw JNPFException.Oh(ErrorCode.D1901);
            else if (string.IsNullOrEmpty(entity.FullName))
                throw JNPFException.Oh(ErrorCode.D1902);
            else if (string.IsNullOrEmpty(entity.EnCode))
                throw JNPFException.Oh(ErrorCode.D1903);
            else
                await _portalRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 修改接口
        /// </summary>
        /// <param name="id">主键id</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] PortalUpInput input)
        {
            var entity = input.Adapt<PortalEntity>();
            var isOk = await _portalRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除接口
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var entity = await _portalRepository.SingleAsync(p => p.Id == id && p.DeleteMark == null);
            _ = entity ?? throw JNPFException.Oh(ErrorCode.COM1005);
            var isOk = await _portalRepository.Context.Updateable(entity).UpdateColumns(it => new { it.DeleteMark, it.DeleteUserId, it.DeleteTime }).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPost("{id}/Actions/Copy")]
        public async Task ActionsCopy(string id)
        {
            var newEntity = new PortalEntity();
            var random = new Random().NextLetterAndNumberString(5);
            var entity = await _portalRepository.SingleAsync(p => p.Id == id && p.DeleteMark == null);
            newEntity.FullName = entity.FullName + ".副本" + random;
            newEntity.EnCode = entity.EnCode + random;
            newEntity.Category = entity.Category;
            newEntity.FormData = entity.FormData;
            newEntity.Description = entity.Description;
            newEntity.EnabledMark = 0;
            var isOk = await _portalRepository.Context.Insertable(newEntity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 设置默认门户
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}/Actions/SetDefault")]
        public async Task SetDefault(string id)
        {
            var userEntity = _userManager.User;
            _ = userEntity ?? throw JNPFException.Oh(ErrorCode.D5002);
            if (userEntity != null)
            {
                userEntity.PortalId = id;
                var isOk = await _portalRepository.Context.Updateable<UserEntity>().SetColumns(it => new UserEntity()
                {
                    PortalId = id,
                    LastModifyTime = SqlFunc.GetDate(),
                    LastModifyUserId = _userManager.UserId
                }).Where(it => it.Id == userEntity.Id).ExecuteCommandAsync();
                if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.D5014);
            }
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 获取默认
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<string> GetDefault()
        {
            var user = _userManager.User;
            if (!user.IsAdministrator.ToBool())
            {
                if (!string.IsNullOrEmpty(user.RoleId))
                {
                    var roleIds = user.RoleId.Split(',');
                    var roleId = await _roleRepository.Entities.In(r => r.Id, roleIds).Where(r => r.EnabledMark.Equals(1) && r.DeleteMark == null).Select(r => r.Id).ToListAsync();
                    var items = await _authorizeRepository.Entities.In(a => a.ObjectId, roleId).Where(a => a.ItemType == "portal").GroupBy(it => new { it.ItemId }).Select(it => new { it.ItemId }).ToListAsync();
                    if (items.Count == 0) return null;
                    var portalList = await _portalRepository.Entities.In(p => p.Id, items.Select(it => it.ItemId).ToArray()).Where(p => p.EnabledMark.Equals(1) && p.DeleteMark == null).OrderBy(o => o.SortCode).Select(s => s.Id).ToListAsync();
                    return portalList.FirstOrDefault();
                }
                return null;
            }
            else
            {
                var portalList = await _portalRepository.Entities.Where(p => p.EnabledMark.Equals(1) && p.DeleteMark == null).OrderBy(o => o.SortCode).Select(s => s.Id).ToListAsync();
                return portalList.FirstOrDefault();
            }
        }

        #endregion
    }
}