using JNPF.Dependency;

namespace JNPF.Common.Const
{
    /// <summary>
    /// 公共常量
    /// </summary>
    [SuppressSniffer]
    public class CommonConst
    {
        /// <summary>
        /// 默认密码
        /// </summary>
        public const string DEFAULT_PASSWORD = "123456";

        /// <summary>
        /// 用户缓存
        /// </summary>
        public const string CACHE_KEY_USER = "user_";

        /// <summary>
        /// 菜单缓存
        /// </summary>
        public const string CACHE_KEY_MENU = "menu_";

        /// <summary>
        /// 权限缓存
        /// </summary>
        public const string CACHE_KEY_PERMISSION = "permission_";

        /// <summary>
        /// 数据范围缓存
        /// </summary>
        public const string CACHE_KEY_DATASCOPE = "datascope_";

        /// <summary>
        /// 验证码缓存
        /// </summary>
        public const string CACHE_KEY_CODE = "vercode_";

        /// <summary>
        /// 单据编码缓存
        /// </summary>
        public const string CACHE_KEY_BILLRULE = "billrule_";

        /// <summary>
        /// 在线用户缓存
        /// </summary>
        public const string CACHE_KEY_ONLINE_USER = "onlineuser_";

        /// <summary>
        /// 岗位缓存
        /// </summary>
        public const string CACHE_KEY_POSITION = "position_";

        /// <summary>
        /// 角色缓存
        /// </summary>
        public const string CACHE_KEY_ROLE = "role_";

        /// <summary>
        /// 在线开发缓存
        /// </summary>
        public const string VISUALDEV = "visualdev_";
    }
}
