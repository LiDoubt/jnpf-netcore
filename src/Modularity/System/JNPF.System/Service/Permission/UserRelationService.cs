using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.System.Entitys.Dto.Permission.UserRelation;
using JNPF.System.Entitys.Permission;
using JNPF.System.Interfaces.Permission;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JNPF.System.Service.Permission
{
    /// <summary>
    /// 业务实现：用户关系
    /// </summary>
    [ApiDescriptionSettings(Tag = "Permission", Name = "UserRelation", Order = 169)]
    [Route("api/permission/[controller]")]
    public class UserRelationService : IUserRelationService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<UserRelationEntity> _userRelationRepository;
        private readonly ISqlSugarRepository<UserEntity> _userRepository;

        /// <summary>
        /// 初始化一个<see cref="UserRelationService"/>类型的新实例
        /// </summary>
        /// <param name="userRelationRepository"></param>
        /// <param name="userRepository"></param>
        public UserRelationService(ISqlSugarRepository<UserRelationEntity> userRelationRepository, ISqlSugarRepository<UserEntity> userRepository)
        {
            _userRelationRepository = userRelationRepository;
            _userRepository = userRepository;
        }

        #region Get

        /// <summary>
        /// 获取岗位/角色成员列表
        /// </summary>
        /// <param name="objectId">岗位id或角色id</param>
        /// <returns></returns>
        [HttpGet("{objectId}")]
        public async Task<dynamic> GetListByObjectId(string objectId)
        {
            var data = await _userRelationRepository.Where(u => u.ObjectId == objectId).Select(s => s.UserId).ToListAsync();
            return new { ids = data };
        }

        #endregion 

        #region Post

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="objectId">功能主键</param>
        /// <param name="input">参数</param>
        /// <returns></returns>
        [HttpPost("{objectId}")]
        public async Task Create(string objectId, [FromBody] UserRelationCrInput input)
        {
            var oldUserIds = await _userRelationRepository.Where(u => u.ObjectId.Equals(objectId) && u.ObjectType.Equals(input.objectType)).Select(s => s.UserId).ToListAsync();
            try
            {
                //开启事务
                _userRelationRepository.Ado.BeginTran();
                //清空原有数据
                await _userRelationRepository.DeleteAsync(u => u.ObjectId.Equals(objectId) && u.ObjectType.Equals(input.objectType));
                //创建新数据
                var dataList = new List<UserRelationEntity>();
                input.userIds.ForEach(item =>
                {
                    dataList.Add(new UserRelationEntity()
                    {
                        UserId = item,
                        ObjectType = input.objectType,
                        ObjectId = objectId,
                        SortCode = input.userIds.IndexOf(item)
                    });
                });
                if (dataList.Count > 0)
                {
                    await _userRelationRepository.Context.Insertable(dataList).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
                }
                // 修改用户
                // 计算旧用户数组与新用户数组差
                var addList = input.userIds.Except(oldUserIds).ToList();
                var delList = oldUserIds.Except(input.userIds).ToList();
                //处理新增用户岗位
                if (addList.Count > 0)
                {
                    var addUserList = await _userRepository.Entities.In(u => u.Id, addList.ToArray()).ToListAsync();
                    addUserList.ForEach(item =>
                    {
                        if (input.objectType.Equals("Position"))
                        {
                            var idList = string.IsNullOrEmpty(item.PositionId) ? new List<string>() : item.PositionId.Split(',').ToList();
                            idList.Add(objectId);
                            item.PositionId = string.Join(",", idList.ToArray()).TrimStart(',').TrimEnd(',');
                        }
                        else if (input.objectType.Equals("Role"))
                        {
                            var idList = string.IsNullOrEmpty(item.RoleId) ? new List<string>() : item.RoleId.Split(',').ToList();
                            idList.Add(objectId);
                            item.RoleId = string.Join(",", idList.ToArray()).TrimStart(',').TrimEnd(',');
                        }
                    });
                    await _userRepository.Context.Updateable(addUserList).UpdateColumns(it => new { it.RoleId, it.PositionId }).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
                }

                //出来移除用户
                if (delList.Count > 0)
                {
                    var delUserList = await _userRepository.Entities.In(u => u.Id, delList.ToArray()).ToListAsync();
                    delUserList.ForEach(item =>
                    {
                        if (input.objectType.Equals("Position"))
                        {
                            var idList = item.PositionId.Split(',').ToList();
                            idList.RemoveAll(x => x == objectId);
                            _userRepository.Context.Updateable<UserEntity>().SetColumns(it => new UserEntity()
                            {
                                PositionId = string.Join(",", idList.ToArray()).TrimStart(',').TrimEnd(','),
                                LastModifyTime = SqlFunc.GetDate(),
                                LastModifyUserId = item.Id
                            }).Where(it => it.Id == item.Id).ExecuteCommand();
                        }
                        else if (input.objectType.Equals("Role"))
                        {
                            var idList = item.RoleId.Split(',').ToList();
                            idList.RemoveAll(x => x == objectId);
                            _userRepository.Context.Updateable<UserEntity>().SetColumns(it => new UserEntity()
                            {
                                RoleId = string.Join(",", idList.ToArray()).TrimStart(',').TrimEnd(','),
                                LastModifyTime = SqlFunc.GetDate(),
                                LastModifyUserId = item.Id
                            }).Where(it => it.Id == item.Id).ExecuteCommand();
                        }
                    });
                }
                _userRelationRepository.Ado.CommitTran();
            }
            catch (Exception)
            {
                _userRelationRepository.Ado.RollbackTran();
                throw;
            }
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 创建用户岗位关系
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="ids">岗位ID</param>
        /// <returns></returns>
        [NonAction]
        public List<UserRelationEntity> CreateByPosition(string userId, string ids)
        {
            List<UserRelationEntity> userRelationList = new List<UserRelationEntity>();
            if (!ids.IsNullOrEmpty())
            {
                var position = new List<string>(ids.Split(','));
                position.ForEach(item =>
                {
                    var entity = new UserRelationEntity();
                    entity.ObjectType = "Position";
                    entity.ObjectId = item;
                    entity.SortCode = position.IndexOf(item);
                    entity.UserId = userId;
                    userRelationList.Add(entity);
                });
            }
            return userRelationList;
        }

        /// <summary>
        /// 创建用户角色关系
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="ids">角色ID</param>
        /// <returns></returns>
        [NonAction]
        public List<UserRelationEntity> CreateByRole(string userId, string ids)
        {
            List<UserRelationEntity> userRelationList = new List<UserRelationEntity>();
            if (!ids.IsNullOrEmpty())
            {
                var position = new List<string>(ids.Split(','));
                position.ForEach(item =>
                {
                    var entity = new UserRelationEntity();
                    entity.ObjectType = "Role";
                    entity.ObjectId = item;
                    entity.SortCode = position.IndexOf(item);
                    entity.UserId = userId;
                    userRelationList.Add(entity);
                });
            }
            return userRelationList;
        }

        /// <summary>
        /// 创建用户关系
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [NonAction]
        public async Task Create(List<UserRelationEntity> input)
        {
            try
            {
                //开启事务
                _userRelationRepository.Ado.BeginTran();

                await _userRelationRepository.Context.Insertable(input).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();

                _userRelationRepository.Ado.CommitTran();
            }
            catch (Exception)
            {
                _userRelationRepository.Ado.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task Delete(string id)
        {
            try
            {
                //开启事务
                _userRelationRepository.Ado.BeginTran();

                await _userRelationRepository.DeleteAsync(u => u.UserId == id);

                _userRelationRepository.Ado.CommitTran();
            }
            catch (Exception)
            {
                _userRelationRepository.Ado.RollbackTran();
                throw JNPFException.Oh(ErrorCode.D5003);
            }
        }

        /// <summary>
        /// 根据用户主键获取列表
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetListByUserId(string userId)
        {
            return await _userRelationRepository.Where(m => m.UserId == userId).OrderBy(o => o.CreatorTime).ToListAsync();
        }

        /// <summary>
        /// 获取岗位
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<string>> GetPositionId(string userId)
        {
            var data = await _userRelationRepository.Where(m => m.UserId == userId && m.ObjectType == "Position").OrderBy(o => o.CreatorTime).ToListAsync();
            return data.Select(m => m.ObjectId).ToList();
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objId"></param>
        /// <returns></returns>
        [NonAction]
        public List<string> GetUserId(string type, string objId)
        {
            var data = _userRelationRepository.Entities.Where(x => x.ObjectId == objId && x.ObjectType == type).Select(x => x.UserId).Distinct().ToList();
            return data;
        }
        #endregion
    }
}
