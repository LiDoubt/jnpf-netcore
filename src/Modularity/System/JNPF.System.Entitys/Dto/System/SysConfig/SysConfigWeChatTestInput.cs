using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.SysConfig
{
    /// <summary>
    /// 测试企业号连接输入
    /// </summary>
    [SuppressSniffer]
    public class SysConfigWeChatTestInput
    {
        /// <summary>
        /// 应用凭证
        /// </summary>
        public string qyhAgentId { get; set; }

        /// <summary>
        /// 凭证密钥
        /// </summary>
        public string qyhAgentSecret { get; set; }

        /// <summary>
        /// 企业号Id
        /// </summary>
        public string qyhCorpId { get; set; }

        /// <summary>
        /// 同步密钥
        /// </summary>
        public string qyhCorpSecret { get; set; }
    }
}
