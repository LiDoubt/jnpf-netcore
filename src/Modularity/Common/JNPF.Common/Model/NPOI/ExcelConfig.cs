using System.Collections.Generic;
using System.Drawing;

namespace JNPF.Common.Model.NPOI
{
    /// <summary>
    /// Excel导出配置
    /// 版 本：V3.0.0
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2017.03.09
    /// </summary>
    public class ExcelConfig
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 标题字号
        /// </summary>
        public short TitlePoint { get; set; }
        /// <summary>
        /// 标题高度
        /// </summary>
        public short TitleHeight { get; set; }
        /// <summary>
        /// 标题字体
        /// </summary>
        public string TitleFont { get; set; }
        /// <summary>
        /// 字体景色
        /// </summary>
        public Color ForeColor { get; set; }
        /// <summary>
        /// 背景色
        /// </summary>
        public Color Background { get; set; }
        /// <summary>
        /// 列头字号
        /// </summary>
        public short HeadPoint { get; set; }
        /// <summary>
        /// 列标题高度
        /// </summary>
        public short HeadHeight { get; set; }
        /// <summary>
        /// 列头字体
        /// </summary>
        public string HeadFont { get; set; }
        /// <summary>
        /// 是否按内容长度来适应表格宽度
        /// </summary>
        public bool IsAllSizeColumn { get; set; }
        /// <summary>
        /// 列设置
        /// </summary>
        public List<ExcelColumnModel> ColumnModel { get; set; }
    }
}
