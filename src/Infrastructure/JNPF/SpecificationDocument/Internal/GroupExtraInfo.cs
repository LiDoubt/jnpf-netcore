﻿using JNPF.Dependency;

namespace JNPF.SpecificationDocument
{
    /// <summary>
    /// 分组附加信息
    /// </summary>
    [SuppressSniffer]
    internal sealed class GroupExtraInfo
    {
        /// <summary>
        /// 分组名
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 分组排序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool Visible { get; set; }
    }
}