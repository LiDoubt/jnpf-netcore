using JNPF.Dependency;

namespace JNPF.DistributedIDGenerator
{
    /// <summary>
    /// 短 ID 生成配置选项
    /// </summary>
    [SuppressSniffer]
    public class GenerationOptions
    {
        /// <summary>
        /// 是否使用数字
        /// <para>默认 false</para>
        /// </summary>
        public bool UseNumbers { get; set; }

        /// <summary>
        /// 是否使用特殊字符
        /// <para>默认 true</para>
        /// </summary>
        public bool UseSpecialCharacters { get; set; } = true;

        /// <summary>
        /// 设置短 ID 长度
        /// </summary>
        public int Length { get; set; } = RandomHelpers.GenerateNumberInRange(Constants.MinimumAutoLength, Constants.MaximumAutoLength);
    }
}
