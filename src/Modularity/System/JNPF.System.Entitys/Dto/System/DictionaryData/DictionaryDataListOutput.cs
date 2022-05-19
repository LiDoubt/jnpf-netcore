using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.DictionaryData
{
    [SuppressSniffer]
    public class DictionaryDataListOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 上级id
        /// </summary>
        public string parentId { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 项目编码
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 状态(1-可用,0-禁用)
        /// </summary>
        public int enabledMark { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public long? sortCode { get; set; }

    }
}
