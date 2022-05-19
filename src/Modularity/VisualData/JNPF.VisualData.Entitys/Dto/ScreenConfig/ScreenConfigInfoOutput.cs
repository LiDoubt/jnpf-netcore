namespace JNPF.VisualData.Entitys.Dto.ScreenConfig
{
    /// <summary>
    /// 大屏配置详情输出
    /// </summary>
    public class ScreenConfigInfoOutput
    {
        /// <summary>
        /// 组件json
        /// </summary>
        public string component { get; set; }

        /// <summary>
        /// 配置json
        /// </summary>
        public string detail { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 	可视化表主键
        /// </summary>
        public string visualId { get; set; }
    }
}
