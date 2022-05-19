using JNPF.Common.Util;
using JNPF.Dependency;
using Newtonsoft.Json;
using System;

namespace JNPF.System.Entitys.Dto.Permission.Role
{
    /// <summary>
    /// 角色列表输出
    /// </summary>
    [SuppressSniffer]
    public class RoleListOutput : TreeModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 有效标记
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }

        /// <summary>
        /// 删除标记
        /// </summary>
        [JsonIgnore]
        public int? deleteMark { get; set; }
    }
}
