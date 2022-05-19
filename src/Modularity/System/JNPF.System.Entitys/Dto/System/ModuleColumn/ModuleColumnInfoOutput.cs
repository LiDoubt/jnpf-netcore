using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.ModuleColumn
{
    /// <summary>
    /// 功能列表信息输出
    /// </summary>
    [SuppressSniffer]
    public class ModuleColumnInfoOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 功能ID
        /// </summary>
        public string moduleId { get; set; }

        /// <summary>
        /// 绑定表格
        /// </summary>
        public string bindTable { get; set; }

        /// <summary>
        /// 表格描述
        /// </summary>
        public string bindTableName { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 字段注解
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 字段状态
        /// </summary>
        public int enabledMark { get; set; }

        /// <summary>
        /// 字段说明
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long? sortCode { get; set; }
    }
}
