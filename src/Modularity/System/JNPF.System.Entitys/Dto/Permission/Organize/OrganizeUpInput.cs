using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.Permission.Organize
{
    /// <summary>
    /// 修改机构输入
    /// </summary>
    [SuppressSniffer]
    public class OrganizeUpInput : OrganizeCrInput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
    }
}
