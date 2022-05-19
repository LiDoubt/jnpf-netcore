using JNPF.Dependency;
using System.ComponentModel.DataAnnotations;

namespace JNPF.System.Entitys.Dto.System.ModuleButton
{
    /// <summary>
    /// 功能按钮创建输入
    /// </summary>
    [SuppressSniffer]
    public class ModuleButtonCrInput
    {
        /// <summary>
        /// 功能主键
        /// </summary>
        public string moduleId { get; set; }

        /// <summary>
        /// 按钮名称
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 按钮图标	
        /// </summary>
        public string icon { get; set; }

        /// <summary>
        /// 按钮编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 按钮状态(1-可用,0-不可用)
        /// </summary>
        public int? enabledMark { get; set; }

        /// <summary>
        /// 按钮说明
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 上级菜单
        /// </summary>
        public string parentId { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public long? sortCode { get; set; }
    }
}
