using JNPF.Dependency;

namespace JNPF.Apps.Entitys.Dto
{
    [SuppressSniffer]
    public class AppDataListOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 对象id
        /// </summary>
        public string objectId { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public string objectData { get; set; }
    }
}
