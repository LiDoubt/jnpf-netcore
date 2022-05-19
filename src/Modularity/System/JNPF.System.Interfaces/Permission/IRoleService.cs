namespace JNPF.System.Interfaces.Permission
{
    /// <summary>
    /// 业务契约：角色信息
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// 名称
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        string GetName(string ids);
    }
}
