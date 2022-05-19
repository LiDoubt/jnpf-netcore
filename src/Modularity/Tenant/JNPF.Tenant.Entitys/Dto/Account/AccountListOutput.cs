using JNPF.Dependency;
using System;

namespace JNPF.Tenant.Entitys.Dto.Account
{
    [SuppressSniffer]
    public class AccountListOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string realName { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int? gender { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? enabledMark { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }
    }
}
