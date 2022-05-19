namespace JNPF.VisualData.Entitys.Dto.ScreenCategory
{
    /// <summary>
    /// 大屏分类详情输出
    /// </summary>
    public class ScreenCategoryInfoOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 分类键值
        /// </summary>
        public string categoryKey { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string categoryValue { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public int isDeleted { get; set; }
    }
}
