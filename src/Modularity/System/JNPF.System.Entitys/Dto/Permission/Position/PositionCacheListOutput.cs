using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.Permission.Position
{
    /// <summary>
    /// 岗位缓存列表输出
    /// </summary>
    [SuppressSniffer]
    public class PositionCacheListOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        public string fullName { get; set; }
    }
}
