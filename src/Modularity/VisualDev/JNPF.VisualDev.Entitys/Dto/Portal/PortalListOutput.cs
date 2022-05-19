using JNPF.Common.Util;
using System;

namespace JNPF.VisualDev.Entitys.Dto.Portal
{
    /// <summary>
    /// 获取门户列表输出
    /// </summary>
    public class PortalListOutput : TreeModel
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
        /// 最后修改人
        /// </summary>
        public string lastModifyUser { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? lastModifyTime { get; set; }

        /// <summary>
        /// 是否可用
        /// 0-不可用，1-可用
        /// </summary>
        public int? enabledMark { get; set; } = 0;

        /// <summary>
        /// 排序
        /// </summary>
        public string sortCode { get; set; }

        /// <summary>
        /// 删除标记
        /// </summary>
        public string deleteMark { get; set; }
    }
}
