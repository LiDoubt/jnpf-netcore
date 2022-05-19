namespace JNPF.VisualData.Entitys.Dto.ScreenCategory
{
    /// <summary>
    /// 大屏分类列表查询输入
    /// </summary>
    public class ScreenCategoryListQueryInput
    {
        /// <summary>
        /// 分类
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// 当前页码:pageIndex
        /// </summary>
        public virtual int current { get; set; } = 1;

        /// <summary>
        /// 每页行数
        /// </summary>
        public virtual int size { get; set; } = 50;
    }
}
