using JNPF.Dependency;

namespace JNPF.System.Entitys.Model.System.DataInterFace
{
    [SuppressSniffer]
    public class DataInterfaceReqParameter
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string field { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string value { get; set; }
    }
}
