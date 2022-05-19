using JNPF.Common.Util;
using Newtonsoft.Json;

namespace JNPF.VisualDev.Entitys.Dto.Portal
{
    /// <summary>
    /// 门户下拉框输出
    /// </summary>
    public class PortalSelectOutput : TreeModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [JsonIgnore]
        public string sortCode { get; set; }
        
        /// <summary>
        /// 有效标记
        /// </summary>
        [JsonIgnore]
        public int enabledMark { get; set; }

        /// <summary>
        /// 删除标记
        /// </summary>
        [JsonIgnore]
        public string deleteMark { get; set; }
    }
}
