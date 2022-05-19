using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.PrintDev
{
    [SuppressSniffer]
    public class PrintDevFieldsQuery
    {
        /// <summary>
        /// sql语句
        /// </summary>
        public string sqlTemplate { get; set; }

        /// <summary>
        /// 连接id
        /// </summary>
        public string dbLinkId { get; set; }
    }
}
