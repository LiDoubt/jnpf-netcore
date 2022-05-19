namespace JNPF.VisualData.Entitys.Dto.ScreenDataSource
{
    /// <summary>
    /// 大屏数据源
    /// </summary>
    public class ScreenDataSourceListQueryInput
    {
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
