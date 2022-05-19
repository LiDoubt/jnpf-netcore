using JNPF.Dependency;
using Newtonsoft.Json;
using System;

namespace JNPF.System.Entitys.Dto.Permission.User
{
    /// <summary>
    /// 用户列表输出
    /// </summary>
    [SuppressSniffer]
    public class UserListOutput
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string account { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string realName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int? gender { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string mobilePhone { get; set; }

        /// <summary>
        /// 岗位ID
        /// </summary>
        [JsonIgnore]
        public string positionId { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>
        public string position { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string department { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [JsonIgnore]
        public string roleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string roleName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        [JsonIgnore]
        public string organizeId { get; set; }

        /// <summary>
        /// 删除标记
        /// </summary>
        [JsonIgnore]
        public int? deleteMark { get; set; }
    }
}
