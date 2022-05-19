using JNPF.Common.Util;
using JNPF.Dependency;

namespace JNPF.Apps.Entitys.Dto
{
    [SuppressSniffer]
    public class AppDataListAllOutput:TreeModel
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
        /// 链接地址
        /// </summary>
        public string urlAddress { get; set; }
        /// <summary>
        /// 菜单类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 扩展字段
        /// </summary>
        public string propertyJson { get; set; }
        /// <summary>
        /// 是否常用
        /// </summary>
        public bool isData { get; set; }
    }
}
