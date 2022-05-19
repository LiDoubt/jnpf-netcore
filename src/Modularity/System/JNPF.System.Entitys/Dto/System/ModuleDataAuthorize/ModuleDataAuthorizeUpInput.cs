using JNPF.Dependency;
using System.ComponentModel.DataAnnotations;

namespace JNPF.System.Entitys.Dto.System.ModuleDataAuthorize
{
    /// <summary>
    /// 功能权限数据修改输入
    /// </summary>
    [SuppressSniffer]
    public class ModuleDataAuthorizeUpInput : ModuleDataAuthorizeCrInput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
    }
}
