using JNPF.Common.Util;
using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.DictionaryData
{
    [SuppressSniffer]
    public class DictionaryDataSelectorOutput : TreeModel
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }

    }
}
