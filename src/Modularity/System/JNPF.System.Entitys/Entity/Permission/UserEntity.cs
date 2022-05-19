using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.System.Entitys.Permission
{
    /// <summary>
    /// 用户信息基类
    /// </summary>
    [SugarTable("BASE_USER")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class UserEntity : CLDEntityBase
    {
        /// <summary>
        /// 账户
        /// </summary>
        [SugarColumn(ColumnName = "F_ACCOUNT")]
        public string Account { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [SugarColumn(ColumnName = "F_REALNAME")]
        public string RealName { get; set; }

        /// <summary>
        /// 快速查询
        /// </summary>
        [SugarColumn(ColumnName = "F_QUICKQUERY")]
        public string QuickQuery { get; set; }

        /// <summary>
        /// 呢称
        /// </summary>
        [SugarColumn(ColumnName = "F_NICKNAME")]
        public string NickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [SugarColumn(ColumnName = "F_HEADICON")]
        public string HeadIcon { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [SugarColumn(ColumnName = "F_GENDER")]
        public int? Gender { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [SugarColumn(ColumnName = "F_BIRTHDAY")]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [SugarColumn(ColumnName = "F_MOBILEPHONE")]
        public string MobilePhone { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [SugarColumn(ColumnName = "F_TELEPHONE")]
        public string TelePhone { get; set; }

        /// <summary>
        /// 固定电话
        /// </summary>
        [SugarColumn(ColumnName = "F_LANDLINE")]
        public string Landline { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [SugarColumn(ColumnName = "F_EMAIL")]
        public string Email { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        [SugarColumn(ColumnName = "F_NATION")]
        public string Nation { get; set; }

        /// <summary>
        /// 籍贯
        /// </summary>
        [SugarColumn(ColumnName = "F_NATIVEPLACE")]
        public string NativePlace { get; set; }

        /// <summary>
        /// 入职日期
        /// </summary>
        [SugarColumn(ColumnName = "F_ENTRYDATE")]
        public DateTime? EntryDate { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        [SugarColumn(ColumnName = "F_CERTIFICATESTYPE")]
        public string CertificatesType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        [SugarColumn(ColumnName = "F_CERTIFICATESNUMBER")]
        public string CertificatesNumber { get; set; }

        /// <summary>
        /// 文化程度
        /// </summary>
        [SugarColumn(ColumnName = "F_EDUCATION")]
        public string Education { get; set; }

        /// <summary>
        /// 紧急联系人
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_URGENTCONTACTS")]
        public string UrgentContacts { get; set; }

        /// <summary>
        /// 紧急电话
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_URGENTTELEPHONE")]
        public string UrgentTelePhone { get; set; }

        /// <summary>
        /// 通讯地址
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_POSTALADDRESS")]
        public string PostalAddress { get; set; }

        /// <summary>
        /// 自我介绍
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_SIGNATURE")]
        public string Signature { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_PASSWORD")]
        public string Password { get; set; }

        /// <summary>
        /// 秘钥
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_SECRETKEY")]
        public string Secretkey { get; set; }

        /// <summary>
        /// 首次登录时间
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_FIRSTLOGTIME")]
        public DateTime? FirstLogTime { get; set; }

        /// <summary>
        /// 首次登录IP
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_FIRSTLOGIP")]
        public string FirstLogIP { get; set; }

        /// <summary>
        /// 前次登录时间
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_PREVLOGTIME")]
        public DateTime? PrevLogTime { get; set; }

        /// <summary>
        /// 前次登录IP
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_PREVLOGIP")]
        public string PrevLogIP { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_LASTLOGTIME")]
        public DateTime? LastLogTime { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_LASTLOGIP")]
        public string LastLogIP { get; set; }

        /// <summary>
        /// 登录成功次数
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_LOGSUCCESSCOUNT")]
        public int? LogSuccessCount { get; set; }

        /// <summary>
        /// 登录错误次数
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_LOGERRORCOUNT")]
        public int? LogErrorCount { get; set; }

        /// <summary>
        /// 最后修改密码时间
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_CHANGEPASSWORDDATE")]
        public DateTime? ChangePasswordDate { get; set; }

        /// <summary>
        /// 系统语言
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_LANGUAGE")]
        public string Language { get; set; }

        /// <summary>
        /// 系统样式
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_THEME")]
        public string Theme { get; set; }

        /// <summary>
        /// 常用菜单
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_COMMONMENU")]
        public string CommonMenu { get; set; }

        /// <summary>
        /// 是否管理员【0-普通、1-管理员】
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_ISADMINISTRATOR")]
        public int? IsAdministrator { get; set; } 

        /// <summary>
        /// 扩展属性
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_PROPERTYJSON")]
        public string PropertyJson { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }

        /// <summary>
        /// 主管主键
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_MANAGERID")]
        public string ManagerId { get; set; }

        /// <summary>
        /// 组织主键
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_ORGANIZEID")]
        public string OrganizeId { get; set; }

        /// <summary>
        /// 岗位主键
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_POSITIONID")]
        public string PositionId { get; set; }

        /// <summary>
        /// 角色主键
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_ROLEID")]
        public string RoleId { get; set; }

        /// <summary>
        /// 门户Id
        /// </summary>
        /// <returns></returns>
        [SugarColumn(ColumnName = "F_PORTALID")]
        public string PortalId { get; set; }
    }
}
