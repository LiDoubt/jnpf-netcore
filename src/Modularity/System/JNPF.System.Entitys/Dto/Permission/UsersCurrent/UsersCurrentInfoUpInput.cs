using JNPF.Dependency;
using System;

namespace JNPF.System.Entitys.Dto.Permission.UsersCurrent
{
    /// <summary>
    /// 当前用户信息更新输入
    /// </summary>
    [SuppressSniffer]
    public class UsersCurrentInfoUpInput
    {
        /// <summary>
        /// 户名
        /// </summary>
        public string realName { get; set; }

        /// <summary>
        /// 自我介绍
        /// </summary>
        public string signature { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string gender { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        public string nation { get; set; }

        /// <summary>
        /// 籍贯
        /// </summary>
        public string nativePlace { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string certificatesType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string certificatesNumber { get; set; }

        /// <summary>
        /// 文化程度
        /// </summary>
        public string education { get; set; }

        /// <summary>
        /// 出生年月
        /// </summary>
        public DateTime? birthday { get; set; }

        /// <summary>
        /// 办公电话
        /// </summary>
        public string telePhone { get; set; }

        /// <summary>
        /// 办公座机
        /// </summary>
        public string landline { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string mobilePhone { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// 紧急联系
        /// </summary>
        public string urgentContacts { get; set; }

        /// <summary>
        /// 紧急电话
        /// </summary>
        public string urgentTelePhone { get; set; }

        /// <summary>
        /// 通讯地址
        /// </summary>
        public string postalAddress { get; set; }
    }
}
