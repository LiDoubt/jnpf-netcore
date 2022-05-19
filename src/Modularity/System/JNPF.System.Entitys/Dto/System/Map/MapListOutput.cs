using JNPF.Dependency;
using System;

namespace JNPF.System.Entitys.Dto.System.Map
{
    /// <summary>
    /// 地图列表
    /// </summary>
    [SuppressSniffer]
    public class MapListOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 地图名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 地图编码
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        public long? sortCode { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? enabledMark { get; set; }
        /// <summary>
        /// 添加者
        /// </summary>
        public string creatorUser { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime? creatorTime { get; set; }
    }
}
