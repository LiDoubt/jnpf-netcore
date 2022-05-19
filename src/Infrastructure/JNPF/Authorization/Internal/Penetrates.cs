using JNPF.Dependency;

namespace JNPF.Authorization
{
    /// <summary>
    /// 常量、公共方法配置类
    /// </summary>
    [SuppressSniffer]
    internal static class Penetrates
    {
        /// <summary>
        /// 授权策略前缀
        /// </summary>
        internal const string AppAuthorizePrefix = "<JNPF.Authorization.AppAuthorizeRequirement>";
    }
}