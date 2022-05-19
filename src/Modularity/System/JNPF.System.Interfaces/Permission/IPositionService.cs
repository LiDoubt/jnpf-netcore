using JNPF.System.Entitys.Permission;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces.Permission
{
    /// <summary>
    /// 业务契约：岗位管理
    /// </summary>
    public interface IPositionService
    {
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id">获取信息</param>
        /// <returns></returns>
        Task<PositionEntity> GetInfoById(string id);

        /// <summary>
        /// 获取岗位列表
        /// </summary>
        /// <returns></returns>
        Task<List<PositionEntity>> GetListAsync();

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        string GetName(string ids);
    }
}
