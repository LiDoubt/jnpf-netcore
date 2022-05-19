using JNPF.Dependency;
using System.ComponentModel.DataAnnotations;

namespace JNPF.System.Entitys.Dto.System.ModuleDataAuthorize
{
    /// <summary>
    /// 功能数据权限创建输入
    /// </summary>
    [SuppressSniffer]
    public class ModuleDataAuthorizeCrInput
    {
        /// <summary>
        /// 功能主键
        /// </summary>
        public string moduleId { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 字段注解
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 条件符号
        /// </summary>
        public string conditionSymbol { get; set; }

        /// <summary>
        /// 条件内容	
        /// </summary>
        public string conditionText { get; set; }

        /// <summary>
        /// 字段说明
        /// </summary>
        public string description { get; set; }
    }
}
