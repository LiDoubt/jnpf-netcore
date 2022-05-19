using JNPF.Dependency;
using System.Collections.Generic;

namespace JNPF.System.Entitys.Dto.Permission.Authorize
{
    /// <summary>
    /// 权限模块输入
    /// </summary>
    [SuppressSniffer]
    public class AuthorizeModelInput
    {
        /// <summary>
        /// 项目类型
        /// </summary>
        public string itemType { get; set; }

        /// <summary>
        /// 对象类型
        /// </summary>
        public string objectType { get; set; }

        /// <summary>
        /// 对象ID
        /// </summary>
        public List<string> objectId { get; set; }
    }
}
