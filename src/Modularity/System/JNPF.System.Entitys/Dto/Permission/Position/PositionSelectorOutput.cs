using JNPF.Common.Util;
using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.Permission.Position
{
    /// <summary>
    /// 岗位下拉框输出
    /// </summary>
    [SuppressSniffer]
    public class PositionSelectorOutput : TreeModel
    {
        /// <summary>
        /// 岗位名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 岗位类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
