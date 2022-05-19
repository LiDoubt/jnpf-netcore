using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.Permission.Organize
{
    /// <summary>
    /// 机构信息输出
    /// </summary>
    [SuppressSniffer]
    public class OrganizeInfoOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 上级id
        /// </summary>
        public string parentId { get; set; }

        /// <summary>
        /// 集团名
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 集团编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 公司详情
        /// </summary>
        public string propertyJson { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
