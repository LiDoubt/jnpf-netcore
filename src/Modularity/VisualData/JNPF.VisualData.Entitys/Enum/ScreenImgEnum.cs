using System.ComponentModel;

namespace JNPF.VisualData.Entitys.Enum
{
    /// <summary>
    /// 大屏图片枚举
    /// </summary>
    public enum ScreenImgEnum
    {
        /// <summary>
        /// 背景图片
        /// </summary>
        [Description("bg")]
        BG = 0,

        /// <summary>
        /// 图片框
        /// </summary>
        [Description("border")]
        BORDER = 1,

        /// <summary>
        /// 图片
        /// </summary>
        [Description("source")]
        SOURCE = 1,

        /// <summary>
        /// banner
        /// </summary>
        [Description("banner")]
        BANNER = 3,

        /// <summary>
        /// 大屏截图
        /// </summary>
        [Description("screenShot")]
        SCREENSHOT = 4,
    }
}
