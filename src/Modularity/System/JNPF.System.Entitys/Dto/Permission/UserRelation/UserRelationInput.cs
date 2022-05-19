using JNPF.Dependency;
using System.Collections.Generic;

namespace JNPF.System.Entitys.Dto.Permission.UserRelation
{
    /// <summary>
    /// 用户关系输入
    /// </summary>
    [SuppressSniffer]
    public class UserRelationInput
    {
        /// <summary>
        /// 用户列表
        /// </summary>
        public List<string> userId { get; set; }
    }
}
