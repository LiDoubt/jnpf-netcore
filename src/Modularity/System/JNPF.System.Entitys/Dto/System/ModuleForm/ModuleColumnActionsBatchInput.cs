using JNPF.Dependency;
using System.Collections.Generic;

namespace JNPF.System.Entitys.Dto.System.ModuleColumn
{
    /// <summary>
    /// 功能列表行为处理输入
    /// </summary>
    [SuppressSniffer]
    public class ModuleFormActionsBatchInput
    {
        /// <summary>
        /// 功能主键
        /// </summary>
        public string moduleId { get; set; }

        /// <summary>
        /// 表格描述	
        /// </summary>
        public string bindTableName { get; set; }

        /// <summary>
        /// 字段数据
        /// </summary>
        public List<Form> formJson { get; set; }

        /// <summary>
        /// 绑定表格
        /// </summary>
        public string bindTable { get; set; }
    }

    /// <summary>
    /// 列字段数据
    /// </summary>
    [SuppressSniffer]
    public class Form
    {
        /// <summary>
        /// 字段名称
        /// </summary
        public string enCode { get; set; }

        /// <summary>
        /// 字段注解
        /// </summary>
        public string fullName { get; set; }
    }
}
