using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.Map
{
    /// <summary>
    /// 地图修改
    /// </summary>
    [SuppressSniffer]
    public class MapUpInput : MapCrInput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
    }
}
