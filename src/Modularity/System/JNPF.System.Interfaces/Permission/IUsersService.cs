using JNPF.System.Entitys.Model.Permission.User;
using JNPF.System.Entitys.Permission;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces.Permission
{
    /// <summary>
    /// 业务契约：用户信息
    /// </summary>
    public interface IUsersService
    {
        /// <summary>
        /// 获取用户信息 根据用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        UserEntity GetInfoByUserId(string userId);

        /// <summary>
        /// 获取用户信息 根据用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<UserEntity> GetInfoByUserIdAsync(string userId);

        /// <summary>
        /// 获取用户信息 根据用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<UserInfo> GetUserInfo(string userId, string tenantId);

        /// <summary>
        /// 根据用户账户
        /// </summary>
        /// <param name="account">用户账户</param>
        /// <returns></returns>
        Task<UserEntity> GetInfoByAccount(string account);

        /// <summary>
        /// 获取用户信息 根据登录信息
        /// </summary>
        /// <param name="account">用户账户</param>
        /// <param name="password">用户密码</param>
        /// <returns></returns>
        Task<UserEntity> GetInfoByLogin(string account, string password);

        /// <summary>
        /// 根据用户姓名获取用户ID
        /// </summary>
        /// <param name="realName">用户姓名</param>
        /// <returns></returns>
        Task<string> GetUserIdByRealName(string realName);

        /// <summary>
        /// 获取下属
        /// </summary>
        /// <param name="managerId">主管Id</param>
        /// <returns></returns>
        Task<string[]> GetSubordinates(string managerId);

        /// <summary>
        /// 获取下属
        /// </summary>
        /// <param name="managerId">主管Id</param>
        /// <returns></returns>
        Task<List<string>> GetSubordinatesAsync(string managerId);

        /// <summary>
        /// 获取下属
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<List<string>> GetSubordinateId(string userId);

        /// <summary>
        /// 是否存在机构用户
        /// </summary>
        /// <param name="organizeId">机构ID</param>
        /// <returns></returns>
        Task<bool> ExistOrganizeUser(string organizeId);

        /// <summary>
        /// 获取用户名称
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<string> GetUserName(string userId);

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<UserEntity>> GetList();

        /// <summary>
        /// 用户岗位
        /// </summary>
        /// <param name="PositionIds"></param>
        /// <returns></returns>
        Task<List<PositionInfo>> GetPosition(string PositionIds);
    }
}
