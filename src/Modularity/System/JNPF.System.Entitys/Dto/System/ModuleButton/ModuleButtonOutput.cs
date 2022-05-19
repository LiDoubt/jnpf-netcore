using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.ModuleButton
{
    /// <summary>
    /// 功能按钮输出
    /// </summary>
    [SuppressSniffer]
    public class ModuleButtonOutput
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
