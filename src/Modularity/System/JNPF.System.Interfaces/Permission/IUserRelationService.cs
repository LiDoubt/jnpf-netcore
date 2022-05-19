using JNPF.System.Entitys.Permission;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces.Permission
{
    /// <summary>
    /// 业务契约：用户关系
    /// </summary>
    public interface IUserRelationService
    {
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        Task Delete(string id);

        /// <summary>
        /// 创建用户岗位关系
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="ids">岗位ID</param>
        /// <returns></returns>
        List<UserRelationEntity> CreateByPosition(string userId, string ids);

        /// <summary>
        /// 创建用户角色关系
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="ids">角色ID</param>
        /// <returns></returns>
        List<UserRelationEntity> CreateByRole(string userId, string ids);

        /// <summary>
        /// 创建用户关系
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Create(List<UserRelationEntity> input);

        /// <summary>
        /// 根据用户主键获取列表
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        Task<dynamic> GetListByUserId(string userId);

        /// <summary>
        /// 获取岗位
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        Task<List<string>> GetPositionId(string userId);

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objId"></param>
        /// <returns></returns>
        List<string> GetUserId(string type, string objId);
    }
}
