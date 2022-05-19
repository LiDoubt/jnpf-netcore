using JNPF.Dependency;
using JNPF.System.Entitys.Model.Permission.Organize;

namespace JNPF.System.Entitys.Dto.Permission.Organize
{
    /// <summary>
    /// 新增机构输入
    /// </summary>
    [SuppressSniffer]
    public class OrganizeCrInput
    {
        /// <summary>
        /// 上级ID
        /// </summary>
        public string parentId { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 公司编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        public OrganizePropertyModel propertyJson { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
