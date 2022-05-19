using JNPF.Common.Util;
using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.Permission.Authorize
{
    /// <summary>
    /// 权限数据输出模型
    /// </summary>
    [SuppressSniffer]
    public class AuthorizeDataModelOutput : TreeModel
    {
        /// <summary>
        ///  名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
