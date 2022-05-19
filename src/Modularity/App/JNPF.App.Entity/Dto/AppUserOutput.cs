using JNPF.Dependency;
using JNPF.System.Entitys.Model.Permission.User;
using System.Collections.Generic;

namespace JNPF.Apps.Entitys.Dto
{
    [SuppressSniffer]
    public class AppUserOutput
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// 用户账号
        /// </summary>
        public string userAccount { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string headIcon { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>
        public string departmentId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string departmentName { get; set; }
        /// <summary>
        /// 组织id
        /// </summary>
        public string organizeId { get; set; }
        /// <summary>
        /// 组织名称
        /// </summary>
        public string organizeName { get; set; }
        /// <summary>
        /// 角色id
        /// </summary>
        public string roleId { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string roleName { get; set; }
        /// <summary>
        /// 岗位
        /// </summary>
        public List<PositionInfo> positionIds { get; set; }
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
        /// 邮箱
        /// </summary>
        public string email { get; set; }
    }
}
