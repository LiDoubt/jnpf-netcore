using JNPF.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.System.Entitys.Dto.System.ModuleDataAuthorizeScheme
{
    /// <summary>
    /// 功能权限数据计划信息输出
    /// </summary>
    [SuppressSniffer]
    public class ModuleDataAuthorizeSchemeInfoOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 菜单id
        /// </summary>
        public string moduleId { get; set; }

        /// <summary>
        /// 方案名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 方案对象
        /// </summary>
        public string conditionJson { get; set; }

        /// <summary>
        /// 过滤条件
        /// </summary>
        public string conditionText { get; set; }
    }
}
