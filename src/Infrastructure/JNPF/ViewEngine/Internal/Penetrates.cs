using System;
using System.IO;

namespace JNPF.ViewEngine
{
    /// <summary>
    /// 常量、公共方法配置类
    /// </summary>
    internal static class Penetrates
    {
        /// <summary>
        /// 获取模板文件名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal static string GetTemplateFileName(string fileName)
        {
            var templateSaveDir = Path.Combine(AppContext.BaseDirectory, "templates");
            if (!Directory.Exists(templateSaveDir)) Directory.CreateDirectory(templateSaveDir);

            if (!fileName.EndsWith(".dll", System.StringComparison.OrdinalIgnoreCase)) fileName += ".dll";
            var templatePath = Path.Combine(templateSaveDir, "~" + fileName);

            return templatePath;
        }
    }
}