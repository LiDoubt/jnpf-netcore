using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.DictionaryType
{
    [SuppressSniffer]
    public class DictionaryTypeCrInput
    {
        /// <summary>
        /// 说明
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 是否树形
        /// </summary>
        public int isTree { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 字典分类id
        /// </summary>
        public string parentId { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
