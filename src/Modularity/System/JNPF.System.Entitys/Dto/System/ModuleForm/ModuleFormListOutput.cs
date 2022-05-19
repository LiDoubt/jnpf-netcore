using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.ModuleForm
{
    /// <summary>
    /// 表单权限列表输出
    /// </summary>
    [SuppressSniffer]
    public class ModuleFormListOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 字段注解	
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 菜单id
        /// </summary>
        public string moduleId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long? sortCode { get; set; }
    }
}
