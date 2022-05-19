using JNPF.Common.Util;
using JNPF.Dependency;
using System;

namespace JNPF.System.Entitys.Dto.System.DbLink
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressSniffer]
    public class DbLinkListOutput : TreeModel
    {
        /// <summary>
        /// 连接名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string dbType { get; set; }

        /// <summary>
        /// 主机地址
        /// </summary>
        public string host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public string port { get; set; }

        /// <summary>
        /// 添加时间(时间戳)
        /// </summary>
        public DateTime? creatorTime { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public string creatorUser { get; set; }

        /// <summary>
        /// 修改时间(时间戳)
        /// </summary>
        public DateTime? lastModifyTime { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string lastModifyUser { get; set; }

        /// <summary>
        /// 是否可用(1-可用,0-禁用)
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
