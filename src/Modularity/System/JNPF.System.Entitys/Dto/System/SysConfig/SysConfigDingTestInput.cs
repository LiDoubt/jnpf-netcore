using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.SysConfig
{
    /// <summary>
    /// 测试钉钉连接输入
    /// </summary>
    [SuppressSniffer]
    public class SysConfigDingTestInput
    {
        /// <summary>
        /// 企业号
        /// </summary>
        public string dingAgentId { get; set; }

        /// <summary>
        /// 应用凭证
        /// </summary>
        public string dingSynAppKey { get; set; }

        /// <summary>
        /// 凭证密钥
        /// </summary>
        public string dingSynAppSecret { get; set; }
    }
}
