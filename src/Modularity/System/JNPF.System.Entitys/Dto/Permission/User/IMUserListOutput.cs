using Newtonsoft.Json;

namespace JNPF.System.Entitys.Dto.Permission.User
{
    /// <summary>
    /// IM通讯录
    /// </summary>
    public class IMUserListOutput
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        public string account { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string realName { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string headIcon { get; set; }

        /// <summary>
        /// 用户部门
        /// </summary>
        public string department { get; set; }

        /// <summary>
        /// 用户排序
        /// </summary>
        [JsonIgnore]
        public long? sortCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int? deleteMark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int? enabledMark { get; set; }
    }
}
