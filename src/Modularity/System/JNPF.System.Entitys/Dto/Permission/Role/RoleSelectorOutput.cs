using JNPF.Common.Util;
using JNPF.Dependency;
using Newtonsoft.Json;

namespace JNPF.System.Entitys.Dto.Permission.Role
{
    /// <summary>
    /// 角色下拉输出
    /// </summary>
    [SuppressSniffer]
    public class RoleSelectorOutput : TreeModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 删除标记
        /// </summary>
        [JsonIgnore]
        public int? deleteMark { get; set; }

        /// <summary>
        /// 有效标记
        /// </summary>
        [JsonIgnore]
        public int? enabledMark { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [JsonIgnore]
        public long? sortCode { get; set; }
    }
}
