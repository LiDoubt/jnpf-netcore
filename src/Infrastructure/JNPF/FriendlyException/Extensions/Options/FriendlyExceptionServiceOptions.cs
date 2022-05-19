namespace JNPF.FriendlyException
{
    /// <summary>
    /// 友好异常服务配置选项
    /// </summary>
    public sealed class FriendlyExceptionServiceOptions
    {
        /// <summary>
        /// 是否启用全局友好异常
        /// </summary>
        public bool EnabledGlobalFriendlyException { get; set; } = true;
    }
}
