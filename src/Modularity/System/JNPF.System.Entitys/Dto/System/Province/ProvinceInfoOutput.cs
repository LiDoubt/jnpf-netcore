using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.Province
{
    [SuppressSniffer]
    public class ProvinceInfoOutput
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 区域编号
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int enabledMark { get; set; }
        /// <summary>
        /// 区域名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 区域上级
        /// </summary>
        public string parentId { get; set; }
        /// <summary>
        /// 上级名称
        /// </summary>
        public string parentName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
