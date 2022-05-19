using JNPF.Common.Util;
using JNPF.Dependency;
using Newtonsoft.Json;

namespace JNPF.System.Entitys.Dto.Permission.Organize
{
    /// <summary>
    /// 机构成员列表输出
    /// </summary>
    [SuppressSniffer]
    public class OrganizeMemberListOutput : TreeModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 有效标记
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; }

        [JsonIgnore]
        public long? SortCode { get; set; }

        [JsonIgnore]
        public string Account { get; set; }

        [JsonIgnore]
        public string RealName { get; set; }

        [JsonIgnore]
        public int? DeleteMark { get; set; }
    }
}
