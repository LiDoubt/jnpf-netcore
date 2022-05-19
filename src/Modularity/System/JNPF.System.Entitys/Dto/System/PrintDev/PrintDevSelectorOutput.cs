using JNPF.Common.Util;
using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.PrintDev
{
    [SuppressSniffer]
    public class PrintDevSelectorOutput:TreeModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 流程分类(数据字典-工作流-流程分类)
        /// </summary>
        public string category { get; set; }
    }
}
