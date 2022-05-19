namespace JNPF.VisualData.Entitys.Dto.ScreenCategory
{
    /// <summary>
    /// 大屏分类创建输入
    /// </summary>
    public class ScreenCategoryCrInput
    {
        /// <summary>
        /// 分类键值
        /// </summary>
        public string categoryKey { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string categoryValue { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public int isDeleted { get; set; }
    }
}
