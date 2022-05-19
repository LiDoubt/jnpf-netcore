using System;

namespace JNPF.Message.Entitys.Model.IM
{
    /// <summary>
    /// 在线用户模型
    /// </summary>
    public class UserOnlineModel
    {
        /// <summary>
        /// 连接ID
        /// </summary>
        public string connectionId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// 最后连接时间
        /// </summary>
        public DateTime lastTime { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        public string lastLoginIp { get; set; }

        /// <summary>
        /// 登录平台设备
        /// </summary>
        public string lastLoginPlatForm { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string account { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 租户id
        /// </summary>
        public string tenantId { get; set; }
    }
}
