using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.Permission.Role
{
    /// <summary>
    /// 角色信息输出
    /// </summary>
    [SuppressSniffer]
    public class RoleInfoOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 有效标记
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
