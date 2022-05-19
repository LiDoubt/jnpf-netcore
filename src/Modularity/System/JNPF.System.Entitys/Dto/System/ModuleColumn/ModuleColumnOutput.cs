using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.ModuleColumn
{
    /// <summary>
    /// 功能列输出
    /// </summary>
    [SuppressSniffer]
    public class ModuleColumnOutput
    {
        /// <summary>
        /// 按钮主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 按钮名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 按钮编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 功能主键
        /// </summary>
        public string moduleId { get; set; }
    }
}
