using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.DictionaryData
{
    [SuppressSniffer]
    public class DictionaryDataCrInput
    {
        /// <summary>
        /// 项目代码
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 项目状态
        /// </summary>
        public int enabledMark { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 字典分类id
        /// </summary>
        public string dictionaryTypeId { get; set; }
        /// <summary>
        /// 上级id,没有传0
        /// </summary>
        public string parentId { get; set; }
        /// <summary>
        /// 项目说明
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
