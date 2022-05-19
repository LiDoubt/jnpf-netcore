using JNPF.Dependency;
using System;

namespace JNPF.Extend.Entitys.Model
{
    [SuppressSniffer]
    public class PostilModel
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }
    }
}
