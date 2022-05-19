namespace JNPF.VisualData.Entitys.Dto.Screen
{
    /// <summary>
    /// 大屏列表查询输入
    /// </summary>
    public class ScreenListQueryInput
    {
        /// <summary>
        /// 大屏类型
        /// </summary>
        public int category { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int current { get; set; }

        /// <summary>
        /// 每页的数量
        /// </summary>
        public int size { get; set; }
    }
}
