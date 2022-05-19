using JNPF.Common.Util;
using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.ModuleButton
{
    /// <summary>
    /// 功能按钮列表输出
    /// </summary>
    [SuppressSniffer]
    public class ModuleButtonListOutput : TreeModel
    {
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
        /// 排序码
        /// </summary>
        public long? sortCode { get; set; }
    }
}
