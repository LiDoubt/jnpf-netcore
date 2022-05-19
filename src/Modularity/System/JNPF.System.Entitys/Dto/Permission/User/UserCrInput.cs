using JNPF.Dependency;
using System;

namespace JNPF.System.Entitys.Dto.Permission.User
{
    /// <summary>
    /// 创建用户输入
    /// </summary>
    [SuppressSniffer]
    public class UserCrInput
    {
        /// <summary>
        /// 账户
        /// </summary>
        public string account { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string realName { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        public string organizeId { get; set; }

        /// <summary>
        /// 我的主管
        /// </summary>
        public string managerId { get; set; }

        /// <summary>
        /// 岗位主键
        /// </summary>
        public string positionId { get; set; }

        /// <summary>
        /// 角色主键
        /// </summary>
        public string roleId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int? gender { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        public int? enabledMark { get; set; }

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
        /// 生日
        /// </summary>
        public DateTime? birthday { get; set; }

        /// <summary>
        /// 入职日期
        /// </summary>
        public DateTime? entryDate { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string telePhone { get; set; }

        /// <summary>
        /// 固定电话
        /// </summary>
        public string landline { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string mobilePhone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// 紧急联系人
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

        /// <summary>
        /// 头像
        /// </summary>
        public string headIcon { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
