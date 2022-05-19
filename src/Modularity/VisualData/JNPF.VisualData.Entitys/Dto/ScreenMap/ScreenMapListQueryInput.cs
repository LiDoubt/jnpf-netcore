namespace JNPF.VisualData.Entitys.Dto.ScreenMap
{
    /// <summary>
    /// 大屏数据列表查询输入
    /// </summary>
    public class ScreenMapListQueryInput
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
