using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.ModuleButton
{
    /// <summary>
    /// 功能按钮信息输出
    /// </summary>
    [SuppressSniffer]
    public class ModuleButtonInfoOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 上级ID
        /// </summary>
        public string parentId { get; set; }

        /// <summary>
        /// 按钮名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 按钮编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 按钮图标
        /// </summary>
        public string icon { get; set; }

        /// <summary>
        /// 按钮状态
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 按钮说明
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public long? sortCode { get; set; }
    }
}
