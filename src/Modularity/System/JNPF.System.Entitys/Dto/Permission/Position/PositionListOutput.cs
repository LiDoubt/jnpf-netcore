using JNPF.Dependency;
using System;
using System.Text.Json.Serialization;

namespace JNPF.System.Entitys.Dto.Permission.Position
{
    /// <summary>
    /// 岗位列表输出
    /// </summary>
    [SuppressSniffer]
    public class PositionListOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 岗位编号
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 岗位类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string department { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int? deleteMark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public string organizeId { get; set; }
    }
}
