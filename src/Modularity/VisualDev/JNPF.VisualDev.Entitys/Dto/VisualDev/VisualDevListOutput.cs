using JNPF.Common.Util;
using Newtonsoft.Json;
using System;

namespace JNPF.VisualDev.Entitys.Dto.VisualDev
{
    /// <summary>
    /// 可视化开发列表输出
    /// </summary>
    public class VisualDevListOutput : TreeModel
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
        /// 状态(0-禁用，1-开启)
        /// </summary>
        public int state { get; set; }

        /// <summary>
        /// 功能类型
        /// 1-Web设计,2-App设计,3-流程表单,4-Web表单,5-App表单
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 分类ID
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// 数据库表JSON
        /// </summary>
        public string tables { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string creatorUser { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? lastModifyTime { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string lastModifyUser { get; set; }

        /// <summary>
        /// 删除标记
        /// </summary>
        [JsonIgnore]
        public int? deleteMark { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
