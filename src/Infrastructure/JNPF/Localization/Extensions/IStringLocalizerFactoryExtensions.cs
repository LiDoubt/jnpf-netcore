using JNPF;
using JNPF.Dependency;
using JNPF.Localization;

namespace Microsoft.Extensions.Localization
{
    /// <summary>
    /// IStringLocalizerFactory 拓展类
    /// </summary>
    [SuppressSniffer]
    public static class IStringLocalizerFactoryExtensions
    {
        /// <summary>
        /// 创建默认多语言工厂
        /// </summary>
        /// <param name="stringLocalizerFactory"></param>
        /// <returns></returns>
        public static IStringLocalizer Create(this IStringLocalizerFactory stringLocalizerFactory)
        {
            var localizationSettings = App.GetOptions<LocalizationSettingsOptions>();
            return stringLocalizerFactory.Create(localizationSettings.LanguageFilePrefix, localizationSettings.AssemblyName);
        }
    }
}
