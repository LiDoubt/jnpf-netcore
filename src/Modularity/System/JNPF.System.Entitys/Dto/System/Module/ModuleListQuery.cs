using JNPF.Common.Filter;
using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.Module
{
    /// <summary>
    /// 功能列表查询
    /// </summary>
    [SuppressSniffer]
    public class ModuleListQuery : KeywordInput
    {
        /// <summary>
        /// 分类
        /// </summary>
        public string category { get; set; }
    }
}
