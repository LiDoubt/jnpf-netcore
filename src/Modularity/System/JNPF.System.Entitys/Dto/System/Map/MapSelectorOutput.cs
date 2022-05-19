using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.Map
{
    /// <summary>
    /// 地图下拉框
    /// </summary>
    [SuppressSniffer]
    public class MapSelectorOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 地图名称
        /// </summary>
        public string fullName { get; set; }
    }
}
