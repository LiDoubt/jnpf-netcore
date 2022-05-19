using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.ModuleDataAuthorize
{
    /// <summary>
    /// 功能权限数据列表输出
    /// </summary>
    [SuppressSniffer]
    public class ModuleDataAuthorizeListOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 字段注释
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
    }
}
