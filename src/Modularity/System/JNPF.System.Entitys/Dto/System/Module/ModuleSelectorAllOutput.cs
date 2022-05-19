using JNPF.Common.Util;
using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.Module
{
    /// <summary>
    /// 功能下拉框全部输出
    /// </summary>
    [SuppressSniffer]
    public class ModuleSelectorAllOutput : TreeModel
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 跳转地址
        /// </summary>
        public string urlAddress { get; set; }
        /// <summary>
        /// 扩展属性
        /// </summary>
        public string propertyJson { get; set; }
        /// <summary>
        /// 启用状态
        /// </summary>
        public int? enabledMark { get; set; }
    }
}
