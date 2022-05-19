namespace JNPF.Common.Helper
{
    /// <summary>
    /// 模板关键词帮助类
    /// </summary>
    public class TemplateKeywordsHelper
    {
        /// <summary>
        /// 转换关键词
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static string ReplaceKeywords(string template)
        {
            template = template.Replace("show-word-limit", "showwordlimit")
                .Replace("step-strictly", "stepstrictly")
                .Replace("controls-position", "controlsposition")
                .Replace("value-format", "valueformat")
                .Replace("prefix-icon", "prefixicon")
                .Replace("suffix-icon", "suffixicon")
                .Replace("picker-options", "pickeroptions")
                .Replace("range-separator", "rangeseparator")
                .Replace("start-placeholder", "startplaceholder")
                .Replace("end-placeholder", "endplaceholder")
                .Replace("allow-half", "allowhalf")
                .Replace("show-alpha", "showalpha")
                .Replace("color-format", "colorformat")
                .Replace("show-text", "showtext")
                .Replace("show-score", "showscore")
                .Replace("active-text", "activetext")
                .Replace("inactive-text", "inactivetext")
                .Replace("active-color", "activecolor")
                .Replace("inactive-color", "inactivecolor")
                .Replace("active-value", "activevalue")
                .Replace("inactive-value", "inactivevalue")
                .Replace("show-stops", "showstops")
                .Replace("content-position", "contentposition")
                .Replace("show-all-levels", "showalllevels")
                .Replace("tab-position", "tabPosition");
            return template;
        }
    }
}
