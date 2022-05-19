using JNPF.Dependency;

namespace JNPF.Common.Filter
{
    /// <summary>
    /// 关键字输入
    /// </summary>
    [SuppressSniffer]
    public class KeywordInput
    {
        /// <summary>
        /// 查询关键字
        /// </summary>
        /// <example>测试</example>
        public string keyword { get; set; }
    }
}
