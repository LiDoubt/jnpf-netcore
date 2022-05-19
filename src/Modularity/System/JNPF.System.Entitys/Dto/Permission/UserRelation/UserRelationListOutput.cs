using JNPF.Dependency;
using Newtonsoft.Json;

namespace JNPF.System.Entitys.Dto.Permission.UserRelation
{
    /// <summary>
    /// 用户关系列表
    /// </summary>
    [SuppressSniffer]
    public class UserRelationListOutput
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 获取或设置 启用标识
        /// </summary>
        [JsonIgnore]
        public int enabledMark { get; set; }

        /// <summary>
        /// 获取或设置 删除标志
        /// </summary>
        [JsonIgnore]
        public int? deleteMark { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [JsonIgnore]
        public long sortCode { get; set; }
    }
}
