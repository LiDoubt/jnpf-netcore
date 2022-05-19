using JNPF.System.Entitys.Dto.Permission.Organize;
using JNPF.System.Entitys.Dto.System.SysConfig;
using JNPF.System.Entitys.Permission;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces.System
{
    /// <summary>
    /// 第三方同步
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface ISynThirdInfoService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="thirdType"></param>
        /// <returns></returns>
        Task<dynamic> GetList(int thirdType);
        /// <summary>
        /// 企业微信同步组织
        /// </summary>
        /// <returns></returns>
        Task<dynamic> synAllOrganizeSysToQy();
        /// <summary>
        /// 企业微信同步用户
        /// </summary>
        /// <returns></returns>
        Task<dynamic> synAllUserSysToQy();
        /// <summary>
        /// 钉钉同步组织
        /// </summary>
        /// <returns></returns>
        Task<dynamic> synAllOrganizeSysToDing();
        /// <summary>
        /// 钉钉同步用户
        /// </summary>
        /// <returns></returns>
        Task<dynamic> synAllUserSysToDing();
        /// <summary>
        /// 组织同步
        /// </summary>
        /// <param name="thirdType"></param>
        /// <param name="dataType"></param>
        /// <param name="sysConfig"></param>
        /// <param name="orgList"></param>
        /// <returns></returns>
        Task SynDep(int thirdType, int dataType, SysConfigOutput sysConfig, List<OrganizeListOutput> orgList);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thirdType"></param>
        /// <param name="dataType"></param>
        /// <param name="sysConfig"></param>
        /// <param name="userList"></param>
        /// <returns></returns>
        Task SynUser(int thirdType, int dataType, SysConfigOutput sysConfig, List<UserEntity> userList);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="thirdType"></param>
        /// <param name="dataType"></param>
        /// <param name="sysConfig"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DelSynData(int thirdType, int dataType, SysConfigOutput sysConfig, string id);

        /// <summary>
        /// 根据系统主键获取第三方主键
        /// </summary>
        /// <param name="ids">系统主键</param>
        /// <param name="thirdType">第三方类型</param>
        /// <param name="dataType">数据类型</param>
        /// <returns></returns>
        Task<List<string>> GetThirdIdList(List<string> ids, int thirdType, int dataType);
    }
}
