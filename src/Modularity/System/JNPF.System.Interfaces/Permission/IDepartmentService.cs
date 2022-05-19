using JNPF.System.Entitys.Dto.Permission.Department;
using JNPF.System.Entitys.Permission;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces.Permission
{
    /// <summary>
    /// 业务契约：部门管理
    /// </summary>
    public interface IDepartmentService
    {
        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <returns></returns>
        Task<List<OrganizeEntity>> GetListAsync();

        /// <summary>
        /// 部门名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetDepName(string id);

        /// <summary>
        /// 公司名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetComName(string id);

        /// <summary>
        /// 公司id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetCompanyId(string id);
    }
}
