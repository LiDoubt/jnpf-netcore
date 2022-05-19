using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.DictionaryData
{
    [SuppressSniffer]
    public class DictionaryDataListQuery
    {
        /// <summary>
        /// 是否树形
        /// </summary>
        public string isTree { get; set; }
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string keyword { get; set; }
    }
}
