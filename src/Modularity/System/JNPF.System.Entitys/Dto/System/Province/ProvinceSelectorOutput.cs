using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.Province
{
    [SuppressSniffer]
    public class ProvinceSelectorOutput
    {
        /// <summary>
        /// 是否为子节点
        /// </summary>
        public bool isLeaf { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }

    }
}
