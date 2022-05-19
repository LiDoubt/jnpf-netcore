using JNPF.Dependency;
using System.ComponentModel.DataAnnotations;

namespace JNPF.System.Entitys.Dto.System.ModuleColumn
{
    /// <summary>
    /// 功能列表创建输入
    /// </summary>
    [SuppressSniffer]
    public class ModuleFormCrInput
    {
        /// <summary>
        /// 菜单id
        /// </summary>
        public string moduleId { get; set; }

        /// <summary>
        /// 绑定表格描述
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
        /// 状态(1-可用，0-不可用)
        /// </summary>
        public int enabledMark { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 绑定表格
        /// </summary>
        public string bindTable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long? sortCode { get; set; }
    }
}
