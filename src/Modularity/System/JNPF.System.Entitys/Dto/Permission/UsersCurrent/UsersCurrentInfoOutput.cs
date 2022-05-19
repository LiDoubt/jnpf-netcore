using JNPF.Dependency;
using Newtonsoft.Json;
using System;

namespace JNPF.System.Entitys.Dto.Permission.UsersCurrent
{
    /// <summary>
    /// 当前用户信息输出
    /// </summary>
    [SuppressSniffer]
    public class UsersCurrentInfoOutput
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string account { get; set; }

        /// <summary>
        /// 户名
        /// </summary>
        public string realName { get; set; }

        /// <summary>
        /// 组织
        /// </summary>
        public string organize { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>
        public string position { get; set; }

        /// <summary>
        /// 直属主管
        /// </summary>
        public string manager { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string roleId { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime? creatorTime { get; set; }

        /// <summary>
        /// 上次登录时间
        /// </summary>
        public DateTime? prevLogTime { get; set; }

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
        /// 入职日期
        /// </summary>
        public DateTime? entryDate { get; set; }

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

        /// <summary>
        /// 主题
        /// </summary>
        public string theme { get; set; }

        /// <summary>
        /// 语言
        /// </summary>
        public string language { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string avatar { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>
        [JsonIgnore]
        public string positionId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [JsonIgnore]
        public string roleIds { get; set; }
    }
}
