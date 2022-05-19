using JNPF.Dependency;

namespace JNPF.Apps.Entitys.Dto
{
    [SuppressSniffer]
    public class AppDataCrInput
    {
        /// <summary>
        /// 应用类型
        /// </summary>
        public string objectType { get; set; }

        /// <summary>
        /// 应用主键
        /// </summary>
        public string objectId { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public string objectData { get; set; }
    }
}
