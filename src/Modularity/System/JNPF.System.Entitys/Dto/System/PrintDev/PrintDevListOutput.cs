using JNPF.Common.Util;
using JNPF.Dependency;
using System;
using System.Text.Json.Serialization;

namespace JNPF.System.Entitys.Dto.System.PrintDev
{
    [SuppressSniffer]
    public class PrintDevListOutput:TreeModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string creatorUser { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string lastModifyUser { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? lastModifyTime { get; set; }
        /// <summary>
        /// 状态(0-关闭，1-开启)
        /// </summary>
        public int? enabledMark { get; set; }
        /// <summary>
        /// 流程分类(数据字典-工作流-流程分类)
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// 流程分类(数据字典-工作流-流程分类)
        /// </summary>
        public long? sortCode { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int? type { get; set; }
        [JsonIgnore]
        public string description { get; set; }
        [JsonIgnore]
        public string dictionaryTypeId { get; set; }
        [JsonIgnore]
        public int? deleteMark { get; set; }
    }
}
