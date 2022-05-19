using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.DictionaryType
{
    [SuppressSniffer]
    public class DictionaryTypeInfoOutput
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 判断字典列表展示方式(1-树形列表，0-普通列表)
        /// </summary>
        public int isTree { get; set; }
        /// <summary>
        /// 主键ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        ///上级分类ID
        /// </summary>
        public string parentId { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }
    }
}
