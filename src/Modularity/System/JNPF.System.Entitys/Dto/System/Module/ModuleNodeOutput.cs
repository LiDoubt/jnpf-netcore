using JNPF.Common.Util;
using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.Module
{
    /// <summary>
    /// 功能节点输出
    /// </summary>
    [SuppressSniffer]
    public class ModuleNodeOutput : TreeModel
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 菜单编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string icon { get; set; }

        /// <summary>
        /// 菜单分类【1-类别、2-页面】
        /// </summary>
        public int? type { get; set; }

        /// <summary>
        /// 菜单地址
        /// </summary>
        public string urlAddress { get; set; }

        /// <summary>
        /// 链接目标
        /// </summary>
        public string linkTarget { get; set; }

        /// <summary>
        /// 菜单分类：Web、App
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        public string propertyJson { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
