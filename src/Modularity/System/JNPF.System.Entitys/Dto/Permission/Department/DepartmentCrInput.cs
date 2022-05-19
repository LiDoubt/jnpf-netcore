using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.Permission.Department
{
    /// <summary>
    /// 部门创建输入
    /// </summary>
    [SuppressSniffer]
    public class DepartmentCrInput
    {
        /// <summary>
        /// 主管ID
        /// </summary>
        public string managerId { get; set; }

        /// <summary>
        /// 上级ID
        /// </summary>
        public string parentId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 部门编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public long? sortCode { get; set; }
    }
}
