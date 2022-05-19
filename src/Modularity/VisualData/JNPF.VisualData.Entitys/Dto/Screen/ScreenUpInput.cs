using JNPF.VisualData.Entitys.Dto.ScreenConfig;

namespace JNPF.VisualData.Entitys.Dto.Screen
{
    /// <summary>
    /// 大屏修改输入
    /// </summary>
    public class ScreenUpInput
    {
        /// <summary>
        /// 
        /// </summary>
        public ScreenConfigUpInput config { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ScreenEntityUpInput visual { get; set; }
    }

    /// <summary>
    /// 大屏实体修改输入
    /// </summary>
    public class ScreenEntityUpInput : ScreenEntityCrInput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 业务状态
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 背景图片
        /// </summary>
        public string backgroundUrl { get; set; }
    }
}
