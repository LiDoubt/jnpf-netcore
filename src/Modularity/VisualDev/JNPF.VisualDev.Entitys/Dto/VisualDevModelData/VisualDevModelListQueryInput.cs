using JNPF.Common.Filter;
using System.Collections.Generic;

namespace JNPF.VisualDev.Entitys.Dto.VisualDevModelData
{
    /// <summary>
    /// 在线开发功能模块列表查询输入
    /// </summary>
    public class VisualDevModelListQueryInput : PageInputBase
    {
        /// <summary>
        /// 动态搜索对像
        /// </summary>
        public string json { get; set; }

        /// <summary>
        /// 菜单ID
        /// </summary>
        public string menuId { get; set; }

        /// <summary>
        /// 选择导出数据key
        /// </summary>
        public List<string> selectKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string dataType { get; set; } = "0";
    }
}
