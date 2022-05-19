using JNPF.Common.Util;
using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.Module
{
    /// <summary>
    /// 功能列表输出
    /// </summary>
    [SuppressSniffer]
    public class ModuleListOutput : TreeModel
    {
        /// <summary>
        /// 状态(1-可用,0-禁用)
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        ///  图标
        /// </summary>
        public string icon { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string urlAddress { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        public int? type { get; set; }

        /// <summary>
        /// 是否开启数据权限(1-开启,0-未开启)
        /// </summary>
        public int? isDataAuthorize { get; set; }

        /// <summary>
        /// 是否开启列表权限(1-开启,0-未开启)
        /// </summary>
        public int? isColumnAuthorize { get; set; }

        /// <summary>
        /// 是否开启按钮权限(1-开启,0-未开启)
        /// </summary>
        public int? isButtonAuthorize { get; set; }

        /// <summary>
        /// 是否开启表单权限(1-开启,0-未开启)
        /// </summary>
        public int? isFormAuthorize { get; set; }

        /// <summary>
        /// 排序，默认0
        /// </summary>
        public long? sortCode { get; set; }
    }
}
