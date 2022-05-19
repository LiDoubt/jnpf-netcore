using JNPF.Common.Util;
using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.Permission.Department
{
    /// <summary>
    /// 部门下拉树输出
    /// </summary>
    [SuppressSniffer]
    public class DepartmentSelectorOutput : TreeModel
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 当前节点标识
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; } = "icon-ym icon-ym-tree-department1";

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
