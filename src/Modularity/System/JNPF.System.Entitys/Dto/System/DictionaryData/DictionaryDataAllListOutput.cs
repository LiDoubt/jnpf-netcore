using JNPF.Dependency;
using System;

namespace JNPF.System.Entitys.Dto.System.DictionaryData
{
    [SuppressSniffer]
    public class DictionaryDataAllListOutput
    {
        /// <summary>
        /// 字典分类id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 是否树形
        /// </summary>
        public int isTree { get; set; }
        /// <summary>
        /// 字典分类编码
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        ///字典分类名称
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Object dictionaryList { get; set; }
    }
}
