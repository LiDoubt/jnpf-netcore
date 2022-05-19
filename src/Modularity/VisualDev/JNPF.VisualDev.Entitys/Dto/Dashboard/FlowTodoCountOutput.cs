namespace JNPF.VisualDev.Entitys.Dto.Dashboard
{
    /// <summary>
    /// 我的待办输出实体类
    /// </summary>
    public class FlowTodoCountOutput
    {
        /// <summary>
        /// 待我审核
        /// </summary>
        public int toBeReviewed { get; set; }

        /// <summary>
        /// 流程委托
        /// </summary>
        public int entrust { get; set; }

        /// <summary>
        /// 已办事宜
        /// </summary>
        public int flowDone { get; set; }
    }
}
