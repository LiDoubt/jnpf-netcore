using JNPF.Dependency;

namespace JNPF.System.Entitys.Model.System.PrintDev
{
    [SuppressSniffer]
    public class PrintDevFieldModel
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string field { get; set; }

        /// <summary>
        /// 字段说明
        /// </summary>
        public string fieldName { get; set; }
    }
}
