using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.Permission.Position
{
    /// <summary>
    /// 创建岗位输出
    /// </summary>
    [SuppressSniffer]
    public class PositionCrInput
    {
        /// <summary>
        /// 岗位编号
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        public string organizeId { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 岗位类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
