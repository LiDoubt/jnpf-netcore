using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.DictionaryType
{
    [SuppressSniffer]
    public class DictionaryTypeUpInput : DictionaryTypeCrInput
    {
        /// <summary>
        /// 字典id
        /// </summary>
        public string Id { get; set; }
    }
}
