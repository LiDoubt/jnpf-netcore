using JNPF.Common.Core.Manager;
using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Filter;
using JNPF.Common.Util;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.System.Entitys.Dto.Permission.Position;
using JNPF.System.Entitys.Permission;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.Permission;
using JNPF.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JNPF.System.Service.Permission
{
    /// <summary>
    /// 业务实现：岗位管理
    /// 版 本：V3.2.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021.06.07 
    /// </summary>
    [ApiDescriptionSettings(Tag = "Permission", Name = "Position", Order = 162)]
    [Route("api/Permission/[controller]")]
    public class PositionService : IPositionService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<PositionEntity> _positionRepository;
        private readonly ISqlSugarRepository<UserRelationEntity> _userRelationRepository;
        private readonly ISysCacheService _sysCacheService;
        private readonly IOrganizeService _organizeService;
        private readonly IUserManager _userManager;

        /// <summary>
        /// 初始化一个<see cref="PositionService"/>类型的新实例
        /// </summary>
        public PositionService(ISqlSugarRepository<PositionEntity> positionRepository, ISqlSugarRepository<UserRelationEntity> userRelationRepository, IOrganizeService organizeService, ISysCacheService sysCacheService, IUserManager userManager)
        {
            _organizeService = organizeService;
            _userRelationRepository = userRelationRepository;
            _positionRepository = positionRepository;
            _sysCacheService = sysCacheService;
            _userManager = userManager;
        }

        #region GET

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery] PositionListQuery input)
        {
            var pageInput = input.Adapt<PageInputBase>();
            var organizeIds = new List<string>();
            if (!input.organizeId.IsNullOrEmpty())
                organizeIds = await _organizeService.GetSubsidiary(input.organizeId);
            var data = await _positionRepository.Context.Queryable<PositionEntity, OrganizeEntity, DictionaryDataEntity>(
                (a, b, c) => new JoinQueryInfos(JoinType.Left, b.Id == a.OrganizeId, JoinType.Left, a.Type == c.EnCode
                && c.DictionaryTypeId == "dae93f2fd7cd4df999d32f8750fa6a1e"))
                .Select((a, b, c) => new PositionListOutput
                {
                    id = a.Id,
                    fullName = a.FullName,
                    enCode = a.EnCode,
                    type = c.FullName,
                    department = b.FullName,
                    enabledMark = a.EnabledMark,
                    creatorTime = a.CreatorTime,
                    description = a.Description,
                    sortCode = a.SortCode,
                    organizeId = a.OrganizeId,
                    deleteMark = a.DeleteMark
                }).MergeTable()
                //组织机构
                .WhereIF(organizeIds.Any(), u => organizeIds.Contains(u.organizeId))
                //关键字（名称、编码）
                .WhereIF(!pageInput.keyword.IsNullOrEmpty(), u => u.fullName.Contains(pageInput.keyword) || u.enCode.Contains(pageInput.keyword))
                .Where(p => p.deleteMark == null).OrderBy(t => t.sortCode).ToPagedListAsync(pageInput.currentPage, pageInput.pageSize);
            return PageResult<PositionListOutput>.SqlSugarPageResult(data);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("All")]
        public async Task<dynamic> GetList()
        {
            var data = await _positionRepository.Context.Queryable<PositionEntity, OrganizeEntity, DictionaryDataEntity>((a, b, c) => new JoinQueryInfos(JoinType.Left, b.Id == a.OrganizeId, JoinType.Left, a.Type == c.EnCode && c.DictionaryTypeId == "dae93f2fd7cd4df999d32f8750fa6a1e"))
                .Select((a, b, c) => new { Id = a.Id, FullName = a.FullName, EnCode = a.EnCode, Type = c.FullName, Department = b.FullName, EnabledMark = a.EnabledMark, CreatorTime = a.CreatorTime, Description = a.Description, SortCode = a.SortCode, OrganizeId = a.OrganizeId, DeleteMark = a.DeleteMark }).MergeTable()
                .Where(p => p.DeleteMark == null && p.EnabledMark.Equals(1)).OrderBy(t => t.SortCode).Select<PositionListOutput>().ToListAsync();
            return new { list = data };
        }

        /// <summary>
        /// 获取下拉框（公司+部门+岗位）
        /// </summary>
        /// <returns></returns>
        [HttpGet("Selector")]
        public async Task<dynamic> GetSelector()
        {
            var organizeList = await _organizeService.GetListAsync();
            var positionList = await _positionRepository.Where(t => t.EnabledMark.Equals(1) && t.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
            var treeList = new List<PositionSelectorOutput>();
            organizeList.ForEach(item =>
            {
                var icon = "";
                if (item.Category.Equals("department"))
                {
                    icon = "icon-ym icon-ym-tree-department1";
                }
                else
                {
                    icon = "icon-ym icon-ym-tree-organization3";
                }
                treeList.Add(
                    new PositionSelectorOutput
                    {
                        id = item.Id,
                        parentId = item.ParentId,
                        fullName = item.FullName,
                        enabledMark = item.EnabledMark,
                        icon = icon,
                        type = item.Category,
                        sortCode = item.SortCode
                    });
            });
            positionList.ForEach(item =>
            {
                treeList.Add(
                    new PositionSelectorOutput
                    {
                        id = item.Id,
                        parentId = item.OrganizeId,
                        fullName = item.FullName,
                        enabledMark = item.EnabledMark,
                        icon = "icon-ym icon-ym-tree-position1",
                        type = "position",
                        sortCode = item.SortCode
                    });
            });
            return new { list = treeList.ToTree("-1") };
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var entity = await _positionRepository.SingleAsync(p => p.Id == id);
            var output = entity.Adapt<PositionInfoOutput>();
            return output;
        }

        #endregion

        #region POST

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create([FromBody] PositionCrInput input)
        {
            var user = await _userManager.GetUserInfo();
            if (!user.dataScope.Any(it => it.organizeId == input.organizeId && it.Add == true) && !user.isAdministrator)
            {
                throw JNPFException.Oh(ErrorCode.D1013);
            }
            if (await _positionRepository.AnyAsync(p => p.OrganizeId == input.organizeId && p.FullName == input.fullName && p.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D6005);
            if (await _positionRepository.AnyAsync(p => p.OrganizeId == input.organizeId && p.EnCode == input.enCode && p.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D6000);
            var entity = input.Adapt<PositionEntity>();
            var isOk = await _positionRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();
            _ = isOk ?? throw JNPFException.Oh(ErrorCode.D6001);
            await _sysCacheService.DelPosition(_userManager.UserId);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var user = await _userManager.GetUserInfo();
            var entity = await _positionRepository.SingleAsync(p => p.Id == id && p.DeleteMark == null);
            if (!user.dataScope.Any(it => it.organizeId == entity.OrganizeId && it.Delete == true) && !user.isAdministrator)
            {
                throw JNPFException.Oh(ErrorCode.D1013);
            }
            // 岗位下有用户不能删
            if (await _userRelationRepository.AnyAsync(u => u.ObjectType == "Position" && u.ObjectId == id))
                throw JNPFException.Oh(ErrorCode.D6007);
            var isOk = await _positionRepository.Context.Updateable(entity).IgnoreColumns(it => new { it.DeleteMark, it.DeleteTime, it.DeleteUserId }).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
            if (!(isOk > 0))
                throw JNPFException.Oh(ErrorCode.D6002);
            await _sysCacheService.DelPosition(_userManager.UserId);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(string id, [FromBody] PositionUpInput input)
        {
            var user = await _userManager.GetUserInfo();
            var oldEntity = await _positionRepository.SingleAsync(it => it.Id == id);
            if (oldEntity.OrganizeId != input.organizeId && !user.dataScope.Any(it => it.organizeId == oldEntity.OrganizeId && it.Edit == true) && !user.isAdministrator)
                throw JNPFException.Oh(ErrorCode.D1013);
            if (!user.dataScope.Any(it => it.organizeId == input.organizeId && it.Edit == true) && !user.isAdministrator)
                throw JNPFException.Oh(ErrorCode.D1013);
            if (await _positionRepository.AnyAsync(p => p.OrganizeId == input.organizeId && p.FullName == input.fullName && p.DeleteMark == null && p.Id != id))
                throw JNPFException.Oh(ErrorCode.D6005);
            if (await _positionRepository.AnyAsync(p => p.OrganizeId == input.organizeId && p.EnCode == input.enCode && p.DeleteMark == null && p.Id != id))
                throw JNPFException.Oh(ErrorCode.D6000);
            var entity = input.Adapt<PositionEntity>();
            var isOk = await _positionRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0))
                throw JNPFException.Oh(ErrorCode.D6003);
            await _sysCacheService.DelPosition(_userManager.UserId);
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpPut("{id}/Actions/State")]
        public async Task UpdateState(string id)
        {
            var user = await _userManager.GetUserInfo();
            if (!user.dataScope.Any(it => it.organizeId == id && it.Add == true) && !user.isAdministrator)
            {
                throw JNPFException.Oh(ErrorCode.D1013);
            }
            if (!await _positionRepository.AnyAsync(r => r.Id == id && r.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.D6006);

            var isOk = await _positionRepository.Context.Updateable<PositionEntity>().SetColumns(it => new PositionEntity()
            {
                EnabledMark = SqlFunc.IIF(it.EnabledMark == 1, 0, 1),
                LastModifyUserId = _userManager.UserId,
                LastModifyTime = SqlFunc.GetDate()
            }).Where(it => it.Id == id).ExecuteCommandAsync();
            if (!(isOk > 0))
                throw JNPFException.Oh(ErrorCode.D6004);
            await _sysCacheService.DelPosition(_userManager.UserId);
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id">获取信息</param>
        /// <returns></returns>
        [NonAction]
        public async Task<PositionEntity> GetInfoById(string id)
        {
            return await _positionRepository.SingleAsync(p => p.Id == id);
        }

        /// <summary>
        /// 获取岗位列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<PositionEntity>> GetListAsync()
        {
            return await _positionRepository.Entities.Where(u => u.DeleteMark == null).ToListAsync();
        }

        /// <summary>
        /// 名称
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [NonAction]
        public string GetName(string ids)
        {
            if (ids.IsNullOrEmpty())
            {
                return "";
            }
            var idList = ids.Split(",").ToList();
            var nameList = new List<string>();
            var roleList = _positionRepository.Entities.Where(x => x.DeleteMark == null && x.EnabledMark == 1).ToList();
            foreach (var item in idList)
            {
                var info = roleList.Find(x => x.Id == item);
                if (info != null && info.FullName.IsNotEmptyOrNull())
                {
                    nameList.Add(info.FullName);
                }
            }
            var name = string.Join(",", nameList);
            return name;
        }
        #endregion
    }
}
