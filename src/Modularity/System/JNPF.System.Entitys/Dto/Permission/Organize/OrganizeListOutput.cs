using JNPF.Common.Util;
using JNPF.Dependency;
using System;

namespace JNPF.System.Entitys.Dto.Permission.Organize
{
    /// <summary>
    /// 机构树列表输出
    /// </summary>
    [SuppressSniffer]
    public class OrganizeListOutput : TreeModel
    {
        /// <summary>
        /// 集团名
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 状态
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
        /// 分类
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; } = "icon-ym icon-ym-tree-department1";
    }
}