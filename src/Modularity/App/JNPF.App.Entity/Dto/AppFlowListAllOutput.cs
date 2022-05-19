using JNPF.Common.Util;
using JNPF.Dependency;

namespace JNPF.Apps.Entitys.Dto
{
    [SuppressSniffer]
    public class AppFlowListAllOutput:TreeModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 图标背景色
        /// </summary>
        public string iconBackground { get; set; }
        /// <summary>
        /// 表单类型 1-系统表单、2-动态表单
        /// </summary>
        public int formType { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 是否常用
        /// </summary>
        public bool isData { get; set; }
    }
}
