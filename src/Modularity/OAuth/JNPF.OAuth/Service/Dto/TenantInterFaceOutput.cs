using JNPF.Dependency;

namespace JNPF.OAuth.Service.Dto
{
    /// <summary>
    /// 多租户网络连接输出
    /// </summary>
    [SuppressSniffer]
    public class TenantInterFaceOutput
    {
        /// <summary>
        /// DotNet
        /// </summary>
        public string dotnet { get; set; }
    }
}
