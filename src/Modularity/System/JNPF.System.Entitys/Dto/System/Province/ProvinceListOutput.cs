using JNPF.Common.Util;
using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.Province
{
    [SuppressSniffer]
    public class ProvinceListOutput : TreeModel
    {
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
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
