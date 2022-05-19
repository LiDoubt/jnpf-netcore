using JNPF.Dependency;
using System.Collections.Generic;

namespace JNPF.System.Entitys.Dto.Permission.Authorize
{
    /// <summary>
    /// 批量新增权限数据输入
    /// </summary>
    [SuppressSniffer]
    public class AuthorizeDataBatchInput
    {
        /// <summary>
        /// 角色ids
        /// </summary>
        public List<string> roleIds { get; set; }

        /// <summary>
        /// 按钮权限ids
        /// </summary>
        public List<string> button { get; set; }

        /// <summary>
        /// 列表权限ids
        /// </summary>
        public List<string> column { get; set; }

        /// <summary>
        /// 菜单权限ids
        /// </summary>
        public List<string> module { get; set; }

        /// <summary>
        /// 数据权限ids
        /// </summary>
        public List<string> resource { get; set; }

        /// <summary>
        /// 岗位ids
        /// </summary>
        public List<string> positionIds { get; set; }

        /// <summary>
        /// 用户ids
        /// </summary>
        public List<string> userIds { get; set; }
    }
}
