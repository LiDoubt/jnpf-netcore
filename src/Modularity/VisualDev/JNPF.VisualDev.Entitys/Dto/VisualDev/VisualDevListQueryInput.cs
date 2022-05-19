using JNPF.Common.Filter;

namespace JNPF.VisualDev.Entitys.Dto.VisualDev
{
    /// <summary>
    /// 在线开发列表查询输入
    /// </summary>
    public class VisualDevListQueryInput : KeywordInput
    {
        /// <summary>
        /// 功能类型
        /// 1-Web设计,2-App设计,3-流程表单,4-Web表单,5-App表单
        /// </summary>
        public int type { get; set; } = 1;
    }
}
