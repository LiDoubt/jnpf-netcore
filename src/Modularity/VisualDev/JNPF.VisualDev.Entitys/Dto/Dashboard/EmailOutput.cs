using System;

namespace JNPF.VisualDev.Entitys.Dto.Dashboard
{
    /// <summary>
    /// 通知公告输出
    /// </summary>
    public class EmailOutput
    {
        /// <summary>
        /// ID
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }
    }
}
