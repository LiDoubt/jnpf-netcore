using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.PrintDev
{
    [SuppressSniffer]
    public class PrintDevSqlDataQuery
    {
        /// <summary>
        /// 模板id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public string formId { get; set; }
    }
}
