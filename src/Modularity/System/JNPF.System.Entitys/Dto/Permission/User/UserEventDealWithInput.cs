using JNPF.System.Entitys.Permission;

namespace JNPF.System.Entitys.Dto.Permission.User
{
    /// <summary>
    /// 用户登录信息事件处理输入
    /// </summary>
    public class UserEventDealWithInput
    {
        /// <summary>
        /// 租户ID
        /// </summary>
        public string tenantId { get; set; }

        /// <summary>
        /// 租户数据库名称
        /// </summary>
        public string tenantDbName { get; set; }

        /// <summary>
        /// 日记实体
        /// </summary>
        public UserEntity entity { get; set; }
    }
}
