using JNPF.Dependency;

namespace JNPF.Apps.Entitys.Dto
{
    [SuppressSniffer]
    public class AppUserInfoOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string realName { get; set; }
        /// <summary>
        /// 部门名
        /// </summary>
        public string organizeName { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 岗位名
        /// </summary>
        public string positionName { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string telePhone { get; set; }
        /// <summary>
        /// 座机号
        /// </summary>
        public string landline { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string mobilePhone { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string headIcon { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string email { get; set; }
    }
}
