using JNPF.Common.Util;
using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.Permission.Organize
{
    /// <summary>
    /// 机构树输出
    /// </summary>
    [SuppressSniffer]
    public class OrganizeTreeOutput : TreeModel
    {
        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
